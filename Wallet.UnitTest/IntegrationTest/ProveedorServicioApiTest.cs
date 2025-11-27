using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class ProveedorServicioApiTest : DatabaseTestFixture
{
    private const string PROVEEDOR_API_URI = "proveedorServicio";
    private const string API_VERSION = "0.1";

    public ProveedorServicioApiTest()
    {
        SetupDataAsync(async context => { await context.SaveChangesAsync(); }).GetAwaiter().GetResult();
    }

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    [Fact]
    public async Task Post_ProveedorServicio_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var request = new ProveedorServicioRequest
        {
            Nombre = "Netflix",
            Categoria = "Servicios",
            UrlIcono = "https://test.com/icon.png"
        };
        var content = CreateContent(request);

        // Act
        var response = await client.PostAsync($"{API_VERSION}/{PROVEEDOR_API_URI}", content);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Assert.Fail($"Post failed with {response.StatusCode}: {error}");
        }

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProveedorServicioResult>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(request.Nombre, result.Nombre);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task Get_ProveedoresServicio_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync($"{API_VERSION}/{PROVEEDOR_API_URI}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<ProveedorServicioResult>>(responseContent, _jsonSettings);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Get_ProveedorServicio_ById_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        // Create a provider first
        var request = new ProveedorServicioRequest
        {
            Nombre = "Spotify",
            Categoria = "Servicios",
            UrlIcono = "https://spotify.com/icon.png"
        };
        var createResponse = await client.PostAsync($"{API_VERSION}/{PROVEEDOR_API_URI}", CreateContent(request));
        var createResult =
            JsonConvert.DeserializeObject<ProveedorServicioResult>(await createResponse.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(createResult);

        // Act
        var response = await client.GetAsync($"{API_VERSION}/{PROVEEDOR_API_URI}/{createResult.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProveedorServicioResult>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(createResult.Id, result.Id);
    }

    [Fact]
    public async Task Put_ProveedorServicio_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        // Create a provider first
        var createRequest = new ProveedorServicioRequest
        {
            Nombre = "Amazon",
            Categoria = "Recargas",
            UrlIcono = "https://amazon.com/icon.png"
        };
        var createResponse = await client.PostAsync($"{API_VERSION}/{PROVEEDOR_API_URI}", CreateContent(createRequest));
        var createResult =
            JsonConvert.DeserializeObject<ProveedorServicioResult>(await createResponse.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(createResult);

        // Act
        var updateRequest = new ProveedorServicioRequest
        {
            Nombre = "Amazon Prime",
            Categoria = "Movilidad",
            UrlIcono = "https://amazon.com/prime.png"
        };
        var response =
            await client.PutAsync($"{API_VERSION}/{PROVEEDOR_API_URI}/{createResult.Id}", CreateContent(updateRequest));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProveedorServicioResult>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(updateRequest.Nombre, result.Nombre);
        Assert.Equal(updateRequest.Categoria, result.Categoria);
    }

    [Fact]
    public async Task Delete_ProveedorServicio_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        // Create a provider first
        var createRequest = new ProveedorServicioRequest
        {
            Nombre = "Hulu",
            Categoria = "Servicios",
            UrlIcono = "https://hulu.com/icon.png"
        };
        var createResponse = await client.PostAsync($"{API_VERSION}/{PROVEEDOR_API_URI}", CreateContent(createRequest));
        if (!createResponse.IsSuccessStatusCode)
        {
            var error = await createResponse.Content.ReadAsStringAsync();
            Assert.Fail($"Create provider failed with {createResponse.StatusCode}: {error}");
        }

        var createResult =
            JsonConvert.DeserializeObject<ProveedorServicioResult>(await createResponse.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(createResult);
        Assert.True(createResult.Id > 0, "Created provider ID should be > 0");

        // Act
        var response = await client.DeleteAsync($"{API_VERSION}/{PROVEEDOR_API_URI}/{createResult.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProveedorServicioResult>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.False(result.IsActive);
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(body, _jsonSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
