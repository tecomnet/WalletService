using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Enums;
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
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }

    [Fact]
    public async Task Test_Get_Cliente_ServiciosFavoritos_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Setup Data (Empresa, Broker, Proveedor, Cliente, ServicioFavorito)
        // Need to seed complex graph. Using Context directly for speed/simplicity as in other tests.
        using (var context = CreateContext())
        {
            var guid = Guid.NewGuid();

            // Create required dependencies
            var empresa = new Empresa(nombre: "Tecomnet " + guid, creationUser: guid);
            var broker = new Broker(nombre: "Broker " + guid, creationUser: guid);

            context.Empresa.Add(entity: empresa);
            context.Broker.Add(entity: broker);
            await context.SaveChangesAsync(); // IDs generated

            var proveedor = new Proveedor(nombre: "Provider " + guid, urlIcono: "https://example.com/icon.png", broker: broker, creationUser: guid);
            context.Proveedor.Add(entity: proveedor);

            var usuario = new Usuario(codigoPais: "+52", telefono: "55" + new Random().Next(minValue: 10000000, maxValue: 99999999), correoElectronico: "test" + guid + "@test.com",
                contrasena: "Pass123!", estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: guid);
            context.Usuario.Add(entity: usuario);

            var clienteEntity = new Cliente(usuario: usuario, empresa: empresa, creationUser: guid);
            clienteEntity.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "Client", fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Genero.Masculino,
                modificationUser: guid);
            context.Cliente.Add(entity: clienteEntity);

            await context.SaveChangesAsync();

            // Link Servicio Favorito
            var servicioFav = new ServicioFavorito(cliente: clienteEntity, proveedor: proveedor, alias: "My Internet", numeroReferencia: "REF-001", creationUser: guid);
            context.ServicioFavorito.Add(entity: servicioFav);
            await context.SaveChangesAsync();

            // 3. Get Servicios Favoritos
            var response = await client.GetAsync(requestUri: $"/{ApiVersion}/cliente/{clienteEntity.Id}/serviciosFavoritos");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
            var result = JsonConvert.DeserializeObject<List<ServicioFavoritoResult>>(value: content, settings: _jsonSettings);

            Assert.NotNull(@object: result);
            Assert.NotEmpty(collection: result);
            Assert.Contains(collection: result, filter: s => s.Alias == "My Internet" && s.NumeroReferencia == "REF-001");
        }
    }

    [Fact]
    public async Task Test_Delete_Cliente_Returns_Conflict_With_Stale_Token()
    {
        // 1. Auth & Setup
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        Cliente clienteEntity;
        using (var context = CreateContext())
        {
            var guid = Guid.NewGuid();
            var empresa = new Empresa(nombre: "Tecomnet " + guid, creationUser: guid);
            context.Empresa.Add(entity: empresa);
            await context.SaveChangesAsync();

            var usuario = new Usuario(codigoPais: "+52", telefono: "55" + new Random().Next(minValue: 10000000, maxValue: 99999999),
                correoElectronico: "testDel" + guid + "@test.com", contrasena: "Pass123!", estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: guid);
            context.Usuario.Add(entity: usuario);

            clienteEntity = new Cliente(usuario: usuario, empresa: empresa, creationUser: guid);
            clienteEntity.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "Client", fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Genero.Masculino,
                modificationUser: guid);
            context.Cliente.Add(entity: clienteEntity);
            await context.SaveChangesAsync();
        }

        // 2. Fetch Client to get Token
        var getRes = await client.GetAsync(requestUri: $"/{ApiVersion}/cliente/{clienteEntity.Id}");
        var clienteResult =
            JsonConvert.DeserializeObject<ClienteResult>(value: await getRes.Content.ReadAsStringAsync(), settings: _jsonSettings);

        // 3. Use an invalid/stale token (random bytes)
        var staleToken = Convert.ToBase64String(inArray: Encoding.UTF8.GetBytes(s: "OldToken"));
        var encodedToken = System.Web.HttpUtility.UrlEncode(str: staleToken);

        // 4. Delete with Stale Token
        var response =
            await client.DeleteAsync(requestUri: $"/{ApiVersion}/cliente/{clienteEntity.Id}?concurrencyToken={encodedToken}");

        Assert.Equal(expected: HttpStatusCode.Conflict, actual: response.StatusCode);
    }

    [Fact]
    public async Task Test_Activate_Cliente_Returns_Conflict_With_Stale_Token()
    {
        // 1. Auth & Setup
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        Cliente clienteEntity;
        using (var context = CreateContext())
        {
            var guid = Guid.NewGuid();
            var empresa = new Empresa(nombre: "Tecomnet " + guid, creationUser: guid);
            context.Empresa.Add(entity: empresa);
            await context.SaveChangesAsync();

            var usuario = new Usuario(codigoPais: "+52", telefono: "55" + new Random().Next(minValue: 10000000, maxValue: 99999999),
                correoElectronico: "testAct" + guid + "@test.com", contrasena: "Pass123!", estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: guid);
            context.Usuario.Add(entity: usuario);

            clienteEntity = new Cliente(usuario: usuario, empresa: empresa, creationUser: guid);
            clienteEntity.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "Client", fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Genero.Masculino,
                modificationUser: guid);
            // Create as inactive or deactivate it?
            // Since we are testing CONCURRENCY, it doesn't matter if it's already active or not, the concurrency check happens first.
            // But let's follow logic. 
            clienteEntity.Deactivate(modificationUser: guid);

            context.Cliente.Add(entity: clienteEntity);
            await context.SaveChangesAsync();
        }

        // 2. Use an invalid/stale token
        var staleToken = Convert.ToBase64String(inArray: Encoding.UTF8.GetBytes(s: "OldToken"));
        var statusChange = new StatusChangeRequest { ConcurrencyToken = staleToken };

        // 3. Activate with Stale Token
        var response = await client.PutAsync(requestUri: $"/{ApiVersion}/cliente/{clienteEntity.Id}/activar",
            content: CreateContent(body: statusChange));

        Assert.Equal(expected: HttpStatusCode.Conflict, actual: response.StatusCode);
    }
}
