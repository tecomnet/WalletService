using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Wallet.DOM.Modelos;
using Wallet.DOM.ApplicationDbContext;

namespace Wallet.UnitTest.IntegrationTest;

public class ServicioFavoritoApiTest : DatabaseTestFixture
{
    private const string API_URI = "servicioFavorito";
    private const string PROVEEDOR_API_URI = "proveedorServicio";
    private const string CLIENTE_API_URI = "cliente";
    private const string API_VERSION = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    protected ServiceDbContext Context;

    public ServicioFavoritoApiTest()
    {
        Context = CreateContext();
        SetupDataAsync(setupDataAction: async context => { await context.SaveChangesAsync(); }).GetAwaiter()
            .GetResult();
    }

    [Fact]
    public async Task Post_ServicioFavorito_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var cliente = await CreateCliente(client: client);
        var proveedor = await CreateProveedor(client: client);
        // Product is not strictly needed for favorite service, but we create it for completeness if needed elsewhere
        // var producto = await CreateProducto(client, proveedor.Id!.Value);

        var request = new ServicioFavoritoRequest
        {
            ClienteId = cliente.Id,
            ProveedorServicioId = proveedor.Id!.Value,
            Alias = "My Netflix",
            NumeroReferencia = "REF123"
        };
        var content = CreateContent(body: request);

        // Act
        var response = await client.PostAsync(requestUri: $"{API_VERSION}/{API_URI}", content: content);

        // Assert
        Assert.Equal(expected: HttpStatusCode.Created, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ServicioFavoritoResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: request.Alias, actual: result.Alias);
        Assert.True(condition: result.Id > 0);
    }

    [Fact]
    public async Task Get_ServiciosFavoritos_ByCliente_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var cliente = await CreateCliente(client: client);
        var servicioFavorito = await CreateServicioFavorito(client: client, clienteId: cliente.Id.GetValueOrDefault());

        // Act
        var response = await client.GetAsync(requestUri: $"0.1/servicioFavorito/cliente/{cliente.Id}");

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Assert.Fail(message: $"Get failed with {response.StatusCode}: {error}");
        }

        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<List<ServicioFavoritoResult>>(
                value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Contains(collection: result, filter: s => s.Id == servicioFavorito.Id);
    }

    private async Task<ProveedorServicioResult> CreateProveedor(HttpClient client)
    {
        var request = new ProveedorServicioRequest
        {
            Nombre = "Test Provider",
            Categoria = "Servicios",
            UrlIcono = "https://test.com/icon.png"
        };
        var response =
            await client.PostAsync(requestUri: "0.1/proveedorServicio", content: CreateContent(body: request));
        Assert.True(condition: response.IsSuccessStatusCode);
        return JsonConvert.DeserializeObject<ProveedorServicioResult>(value: await response.Content.ReadAsStringAsync(),
            settings: _jsonSettings)!;
    }

    private async Task<ClienteResult> CreateCliente(HttpClient client)
    {
        var usuario = new Usuario("+52", "5512345678", "juan@example.com", null,
            Wallet.DOM.Enums.EstatusRegistroEnum.RegistroCompletado,
            Guid.NewGuid());
        Context.Usuario.Add(usuario);
        await Context.SaveChangesAsync();

        var cliente = new Cliente(usuario, Guid.NewGuid());
        cliente.AgregarDatosPersonales("Juan", "Perez", "Lopez", new DateOnly(1990, 1, 1),
            Wallet.DOM.Enums.Genero.Masculino, Guid.NewGuid());
        cliente.AgregarTipoPersona(Wallet.DOM.Enums.TipoPersona.Fisica, Guid.NewGuid());
        cliente.AgregarRfc("ABC1234567890", Guid.NewGuid());
        cliente.AgregarCurp("ABCD123456EFGHIJ01", Guid.NewGuid());
        cliente.AgregarFotoAWS("foto.jpg", Guid.NewGuid());

        Context.Cliente.Add(cliente);
        await Context.SaveChangesAsync();

        return new ClienteResult
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            PrimerApellido = cliente.PrimerApellido,
            SegundoApellido = cliente.SegundoApellido,
            CorreoElectronico = usuario.CorreoElectronico,
            Telefono = usuario.Telefono
        };
    }

    private async Task<ServicioFavoritoResult> CreateServicioFavorito(HttpClient client, int clienteId)
    {
        var proveedor = await CreateProveedor(client: client);
        var request = new ServicioFavoritoRequest
        {
            ClienteId = clienteId,
            ProveedorServicioId = proveedor.Id!.Value,
            Alias = "Test Favorite",
            NumeroReferencia = "REFTEST"
        };
        var response =
            await client.PostAsync(requestUri: $"{API_VERSION}/{API_URI}", content: CreateContent(body: request));
        Assert.True(condition: response.IsSuccessStatusCode);
        return JsonConvert.DeserializeObject<ServicioFavoritoResult>(value: await response.Content.ReadAsStringAsync(),
            settings: _jsonSettings)!;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }
}
