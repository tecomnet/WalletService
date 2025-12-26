using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class ProveedorApiTest : DatabaseTestFixture
{
    private const string API_URI = "proveedor";
    private const string API_VERSION = "0.1";

    public ProveedorApiTest()
    {
        SetupDataAsync(setupDataAction: async context =>
        {
            var broker1 = new Broker(nombre: "Broker Test 1", creationUser: Guid.NewGuid());
            var broker2 = new Broker(nombre: "Broker Test 2", creationUser: Guid.NewGuid());
            await context.Broker.AddRangeAsync(broker1, broker2);
            await context.SaveChangesAsync();
        }).GetAwaiter().GetResult();
    }

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    [Fact]
    public async Task Post_Proveedor_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var request = new ProveedorRequest
        {
            Nombre = "Netflix",
            UrlIcono = "https://netflix.com/icon.png",
            BrokerId = 1
        };
        var content = CreateContent(body: request);

        // Act
        var response = await client.PostAsync(requestUri: $"{API_VERSION}/{API_URI}", content: content);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Assert.Fail(message: $"Post failed with {response.StatusCode}: {error}");
        }

        Assert.Equal(expected: HttpStatusCode.Created, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProveedorResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: request.Nombre, actual: result.Nombre);
        Assert.True(condition: result.Id > 0);
    }

    [Fact]
    public async Task Get_Proveedores_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync(requestUri: $"{API_VERSION}/{API_URI}");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result =
            JsonConvert.DeserializeObject<List<ProveedorResult>>(value: responseContent, settings: _jsonSettings);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Get_Proveedor_ById_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        // Create a provider first
        var request = new ProveedorRequest
        {
            Nombre = "Spotify",
            UrlIcono = "https://spotify.com/icon.png",
            BrokerId = 1
        };
        var createResponse =
            await client.PostAsync(requestUri: $"{API_VERSION}/{API_URI}", content: CreateContent(body: request));
        var createResult =
            JsonConvert.DeserializeObject<ProveedorResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(createResult);

        // Act
        var response = await client.GetAsync(requestUri: $"{API_VERSION}/{API_URI}/{createResult.Id}");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProveedorResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: createResult.Id, actual: result.Id);
    }

    [Fact]
    public async Task Put_Proveedor_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        // Create a provider first
        var createRequest = new ProveedorRequest
        {
            Nombre = "Amazon",
            UrlIcono = "https://amazon.com/icon.png",
            BrokerId = 1
        };
        var createResponse = await client.PostAsync(requestUri: $"{API_VERSION}/{API_URI}",
            content: CreateContent(body: createRequest));
        var createResult =
            JsonConvert.DeserializeObject<ProveedorResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(createResult);

        // Act
        var updateRequest = new ProveedorUpdateRequest
        {
            Nombre = "Amazon Prime",
            UrlIcono = "https://amazon.com/icon.png",
            BrokerId = 2,
            ConcurrencyToken = createResult.ConcurrencyToken
        };
        var response =
            await client.PutAsync(requestUri: $"{API_VERSION}/{API_URI}/{createResult.Id}",
                content: CreateContent(body: updateRequest));

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProveedorResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: updateRequest.Nombre, actual: result.Nombre);
        // Assert.Equal(expected: updateRequest.Categoria, actual: result.Categoria); // Categoria removed
    }

    [Fact]
    public async Task Delete_Proveedor_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        // Create a provider first
        var createRequest = new ProveedorRequest
        {
            Nombre = "Hulu",
            UrlIcono = "https://hulu.com/icon.png",
            BrokerId = 1
        };
        var createResponse = await client.PostAsync(requestUri: $"{API_VERSION}/{API_URI}",
            content: CreateContent(body: createRequest));
        if (!createResponse.IsSuccessStatusCode)
        {
            var error = await createResponse.Content.ReadAsStringAsync();
            Assert.Fail(message: $"Create provider failed with {createResponse.StatusCode}: {error}");
        }

        var createResult =
            JsonConvert.DeserializeObject<ProveedorResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(createResult);
        Assert.True(condition: createResult.Id > 0, userMessage: "Created provider ID should be > 0");

        // Act
        var response = await client.DeleteAsync(requestUri: $"{API_VERSION}/{API_URI}/{createResult.Id}");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProveedorResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.False(condition: result.IsActive);
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }
}
