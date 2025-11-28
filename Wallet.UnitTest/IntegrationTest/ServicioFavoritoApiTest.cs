using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

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

    public ServicioFavoritoApiTest()
    {
        SetupDataAsync(setupDataAction: async context => { await context.SaveChangesAsync(); }).GetAwaiter().GetResult();
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
            JsonConvert.DeserializeObject<List<ServicioFavoritoResult>>(value: await response.Content.ReadAsStringAsync(),
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
        var response = await client.PostAsync(requestUri: "0.1/proveedorServicio", content: CreateContent(body: request));
        Assert.True(condition: response.IsSuccessStatusCode);
        return JsonConvert.DeserializeObject<ProveedorServicioResult>(value: await response.Content.ReadAsStringAsync(),
            settings: _jsonSettings)!;
    }

    private async Task<ClienteResult> CreateCliente(HttpClient client)
    {
        var request = new ClienteRequest
        {
            CodigoPais = "052",
            Telefono = "5512345678"
        };
        var response = await client.PostAsync(requestUri: "0.1/cliente", content: CreateContent(body: request));
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Assert.Fail(message: $"CreateCliente failed with {response.StatusCode}: {error}");
        }

        return JsonConvert.DeserializeObject<ClienteResult>(value: await response.Content.ReadAsStringAsync(), settings: _jsonSettings)!;
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
        var response = await client.PostAsync(requestUri: $"{API_VERSION}/{API_URI}", content: CreateContent(body: request));
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
