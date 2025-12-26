using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;

namespace Wallet.UnitTest.IntegrationTest;

public class ClienteApiTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public ClienteApiTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(body, _jsonSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    [Fact]
    public async Task Test_Get_Cliente_ServiciosFavoritos_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Setup Data (Empresa, Broker, Proveedor, Cliente, ServicioFavorito)
        // Need to seed complex graph. Using Context directly for speed/simplicity as in other tests.
        using (var context = CreateContext())
        {
            var guid = Guid.NewGuid();

            // Create required dependencies
            var empresa = new Empresa("Tecomnet " + guid, guid);
            var broker = new Broker("Broker " + guid, guid);

            context.Empresa.Add(empresa);
            context.Broker.Add(broker);
            await context.SaveChangesAsync(); // IDs generated

            var proveedor = new Proveedor("Provider " + guid, "https://example.com/icon.png", broker, guid);
            context.Proveedor.Add(proveedor);

            var usuario = new Usuario("+52", "55" + new Random().Next(10000000, 99999999), "test" + guid + "@test.com",
                "Pass123!", EstatusRegistroEnum.RegistroCompletado, guid);
            context.Usuario.Add(usuario);

            var clienteEntity = new Cliente(usuario, empresa, guid);
            clienteEntity.AgregarDatosPersonales("Test", "User", "Client", new DateOnly(1990, 1, 1), Genero.Masculino,
                guid);
            context.Cliente.Add(clienteEntity);

            await context.SaveChangesAsync();

            // Link Servicio Favorito
            var servicioFav = new ServicioFavorito(clienteEntity, proveedor, "My Internet", "REF-001", guid);
            context.ServicioFavorito.Add(servicioFav);
            await context.SaveChangesAsync();

            // 3. Test Get Favorite Services by Client
            var response = await client.GetAsync($"/{ApiVersion}/cliente/{clienteEntity.Id}/servicios-favoritos");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = JsonConvert.DeserializeObject<List<ServicioFavoritoResult>>(content, _jsonSettings);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(result, s => s.Alias == "My Internet" && s.NumeroReferencia == "REF-001");
        }
    }

    [Fact]
    public async Task Test_Delete_Cliente_Returns_Conflict_With_Stale_Token()
    {
        // 1. Auth & Setup
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        Cliente clienteEntity;
        using (var context = CreateContext())
        {
            var guid = Guid.NewGuid();
            var empresa = new Empresa("Tecomnet " + guid, guid);
            context.Empresa.Add(empresa);
            await context.SaveChangesAsync();

            var usuario = new Usuario("+52", "55" + new Random().Next(10000000, 99999999),
                "testDel" + guid + "@test.com", "Pass123!", EstatusRegistroEnum.RegistroCompletado, guid);
            context.Usuario.Add(usuario);

            clienteEntity = new Cliente(usuario, empresa, guid);
            clienteEntity.AgregarDatosPersonales("Test", "User", "Client", new DateOnly(1990, 1, 1), Genero.Masculino,
                guid);
            context.Cliente.Add(clienteEntity);
            await context.SaveChangesAsync();
        }

        // 2. Fetch Client to get Token
        var getRes = await client.GetAsync($"/{ApiVersion}/cliente/{clienteEntity.Id}");
        var clienteResult =
            JsonConvert.DeserializeObject<ClienteResult>(await getRes.Content.ReadAsStringAsync(), _jsonSettings);

        // 3. Use an invalid/stale token (random bytes)
        var staleToken = Convert.ToBase64String(Encoding.UTF8.GetBytes("OldToken"));
        var encodedToken = System.Web.HttpUtility.UrlEncode(staleToken);

        // 4. Delete with Stale Token
        var response =
            await client.DeleteAsync($"/{ApiVersion}/cliente/{clienteEntity.Id}?concurrencyToken={encodedToken}");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Test_Activate_Cliente_Returns_Conflict_With_Stale_Token()
    {
        // 1. Auth & Setup
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        Cliente clienteEntity;
        using (var context = CreateContext())
        {
            var guid = Guid.NewGuid();
            var empresa = new Empresa("Tecomnet " + guid, guid);
            context.Empresa.Add(empresa);
            await context.SaveChangesAsync();

            var usuario = new Usuario("+52", "55" + new Random().Next(10000000, 99999999),
                "testAct" + guid + "@test.com", "Pass123!", EstatusRegistroEnum.RegistroCompletado, guid);
            context.Usuario.Add(usuario);

            clienteEntity = new Cliente(usuario, empresa, guid);
            clienteEntity.AgregarDatosPersonales("Test", "User", "Client", new DateOnly(1990, 1, 1), Genero.Masculino,
                guid);
            // Create as inactive or deactivate it?
            // Since we are testing CONCURRENCY, it doesn't matter if it's already active or not, the concurrency check happens first.
            // But let's follow logic. 
            clienteEntity.Deactivate(guid);

            context.Cliente.Add(clienteEntity);
            await context.SaveChangesAsync();
        }

        // 2. Use an invalid/stale token
        var staleToken = Convert.ToBase64String(Encoding.UTF8.GetBytes("OldToken"));
        var statusChange = new StatusChangeRequest { ConcurrencyToken = staleToken };

        // 3. Activate with Stale Token
        var response = await client.PutAsync($"/{ApiVersion}/cliente/{clienteEntity.Id}/activar",
            CreateContent(statusChange));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
