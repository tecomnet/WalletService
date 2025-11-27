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
        SetupDataAsync(async context => { await context.SaveChangesAsync(); }).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Post_ServicioFavorito_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var cliente = await CreateCliente(client);
        var proveedor = await CreateProveedor(client);
        // Product is not strictly needed for favorite service, but we create it for completeness if needed elsewhere
        // var producto = await CreateProducto(client, proveedor.Id!.Value);

        var request = new ServicioFavoritoRequest
        {
            ClienteId = cliente.Id,
            ProveedorServicioId = proveedor.Id!.Value,
            Alias = "My Netflix",
            NumeroReferencia = "REF123"
        };
        var content = CreateContent(request);

        // Act
        var response = await client.PostAsync($"{API_VERSION}/{API_URI}", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ServicioFavoritoResult>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(request.Alias, result.Alias);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task Get_ServiciosFavoritos_ByCliente_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var cliente = await CreateCliente(client);
        var servicioFavorito = await CreateServicioFavorito(client, cliente.Id.GetValueOrDefault());

        // Act
        var response = await client.GetAsync($"0.1/servicioFavorito/cliente/{cliente.Id}");

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Assert.Fail($"Get failed with {response.StatusCode}: {error}");
        }

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<List<ServicioFavoritoResult>>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.Contains(result, s => s.Id == servicioFavorito.Id);
    }

    private async Task<ProveedorServicioResult> CreateProveedor(HttpClient client)
    {
        var request = new ProveedorServicioRequest
        {
            Nombre = "Test Provider",
            Categoria = "Servicios",
            UrlIcono = "https://test.com/icon.png"
        };
        var response = await client.PostAsync("0.1/proveedorServicio", CreateContent(request));
        Assert.True(response.IsSuccessStatusCode);
        return JsonConvert.DeserializeObject<ProveedorServicioResult>(await response.Content.ReadAsStringAsync(),
            _jsonSettings)!;
    }

    private async Task<ClienteResult> CreateCliente(HttpClient client)
    {
        var request = new ClienteRequest
        {
            CodigoPais = "052",
            Telefono = "5512345678"
        };
        var response = await client.PostAsync("0.1/cliente", CreateContent(request));
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Assert.Fail($"CreateCliente failed with {response.StatusCode}: {error}");
        }

        return JsonConvert.DeserializeObject<ClienteResult>(await response.Content.ReadAsStringAsync(), _jsonSettings)!;
    }

    private async Task<ServicioFavoritoResult> CreateServicioFavorito(HttpClient client, int clienteId)
    {
        var proveedor = await CreateProveedor(client);
        var request = new ServicioFavoritoRequest
        {
            ClienteId = clienteId,
            ProveedorServicioId = proveedor.Id!.Value,
            Alias = "Test Favorite",
            NumeroReferencia = "REFTEST"
        };
        var response = await client.PostAsync($"{API_VERSION}/{API_URI}", CreateContent(request));
        Assert.True(response.IsSuccessStatusCode);
        return JsonConvert.DeserializeObject<ServicioFavoritoResult>(await response.Content.ReadAsStringAsync(),
            _jsonSettings)!;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(body, _jsonSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
