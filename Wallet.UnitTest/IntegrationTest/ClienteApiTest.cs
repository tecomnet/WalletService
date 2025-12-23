using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
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
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Setup Data (Empresa, Broker, Proveedor, Cliente, ServicioFavorito)
        // Need to seed complex graph. Using Context directly for speed/simplicity as in other tests.
        using (var context = CreateContext())
        {
            var guid = Guid.NewGuid();
            
            // Create required dependencies
            var empresa = new Empresa("Tecomnet " + guid, guid);
            var broker = new Wallet.DOM.Modelos.Broker("Broker " + guid, guid);
            
            context.Empresa.Add(empresa);
            context.Broker.Add(broker);
            await context.SaveChangesAsync(); // IDs generated

            var proveedor = new Proveedor("Provider " + guid, "https://example.com/icon.png", broker, guid);
            context.Proveedor.Add(proveedor);
            
            var usuario = new Usuario("+52", "55" + new Random().Next(10000000, 99999999), "test" + guid + "@test.com", "Pass123!", EstatusRegistroEnum.RegistroCompletado, guid);
            context.Usuario.Add(usuario);
            
            var clienteEntity = new Cliente(usuario, empresa, guid);
            clienteEntity.AgregarDatosPersonales("Test", "User", "Client", new DateOnly(1990,1,1), Genero.Masculino, guid);
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
}
