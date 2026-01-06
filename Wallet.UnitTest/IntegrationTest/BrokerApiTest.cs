using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;

namespace Wallet.UnitTest.IntegrationTest;

public class BrokerApiTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public BrokerApiTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Test_Create_Broker_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create Broker
        var request = new BrokerRequest { Nombre = "Broker Integration Test" };
        var response = await client.PostAsync(requestUri: $"/{ApiVersion}/broker", content: CreateContent(body: request));

        var content = await response.Content.ReadAsStringAsync();
        Assert.True(condition: response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK,
            userMessage: $"Expected Created or OK. Got {response.StatusCode}. Content: {content}");

        var result = JsonConvert.DeserializeObject<BrokerResult>(value: content, settings: _jsonSettings);
        Assert.NotNull(@object: result);
        Assert.Equal(expected: request.Nombre, actual: result.Nombre);
        Assert.True(condition: result.Id > 0);
        _output.WriteLine(message: $"Broker Created: {result.Nombre} (ID: {result.Id})");
    }

    [Fact]
    public async Task Test_Get_Brokers_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create a Broker first
        var request = new BrokerRequest { Nombre = "Broker List Test" };
        await client.PostAsync(requestUri: $"/{ApiVersion}/broker", content: CreateContent(body: request));

        // 3. Get List
        var response = await client.GetAsync(requestUri: $"/{ApiVersion}/broker");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);

        var result = JsonConvert.DeserializeObject<List<BrokerResult>>(value: content, settings: _jsonSettings);
        Assert.NotNull(@object: result);
        Assert.NotEmpty(collection: result);
        Assert.Contains(collection: result, filter: b => b.Nombre == "Broker List Test");
    }

    [Fact]
    public async Task Test_Get_BrokerById_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create Broker
        var createRequest = new BrokerRequest { Nombre = "Broker GetById Test" };
        var createResponse = await client.PostAsync(requestUri: $"/{ApiVersion}/broker", content: CreateContent(body: createRequest));
        var createResult =
            JsonConvert.DeserializeObject<BrokerResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Get By Id
        var response = await client.GetAsync(requestUri: $"/{ApiVersion}/broker/{createResult.Id}");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);

        var result = JsonConvert.DeserializeObject<BrokerResult>(value: content, settings: _jsonSettings);
        Assert.NotNull(@object: result);
        Assert.Equal(expected: createResult.Id, actual: result.Id);
        Assert.Equal(expected: createResult.Nombre, actual: result.Nombre);
    }

    [Fact]
    public async Task Test_Update_Broker_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create Broker
        var createRequest = new BrokerRequest { Nombre = "Broker Update Test Original" };
        var createResponse = await client.PostAsync(requestUri: $"/{ApiVersion}/broker", content: CreateContent(body: createRequest));
        var createResult =
            JsonConvert.DeserializeObject<BrokerResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Update Broker
        var updateRequest = new BrokerUpdateRequest
        {
            Nombre = "Broker Update Test Updated",
            ConcurrencyToken = createResult.ConcurrencyToken
        };
        var response = await client.PutAsync(requestUri: $"/{ApiVersion}/broker/{createResult.Id}", content: CreateContent(body: updateRequest));
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result = JsonConvert.DeserializeObject<BrokerResult>(value: content, settings: _jsonSettings);

        Assert.NotNull(@object: result);
        Assert.Equal(expected: createResult.Id, actual: result.Id);
        Assert.Equal(expected: "Broker Update Test Updated", actual: result.Nombre);
    }

    [Fact]
    public async Task Test_Delete_Broker_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create Broker
        var createRequest = new BrokerRequest { Nombre = "Broker Delete Test" };
        var createResponse = await client.PostAsync(requestUri: $"/{ApiVersion}/broker", content: CreateContent(body: createRequest));
        var createResult =
            JsonConvert.DeserializeObject<BrokerResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Delete Broker
        var response = await client.DeleteAsync(requestUri: $"/{ApiVersion}/broker/{createResult.Id}");
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);

        // 4. Verify Not Found (or deleted)
        var getResponse = await client.GetAsync(requestUri: $"/{ApiVersion}/broker/{createResult.Id}");

        var getContent = await getResponse.Content.ReadAsStringAsync();
        var getResult = JsonConvert.DeserializeObject<BrokerResult>(value: getContent, settings: _jsonSettings);
        // If API returns logically deleted object, verify IsActive is false
        Assert.False(condition: getResult.IsActive, userMessage: "Broker should be inactive after delete");
    }

    [Fact]
    public async Task Test_Get_Broker_Proveedores_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Setup Data: Broker & Provider
        int brokerId;
        string providerName = "Provider for Broker Test " + Guid.NewGuid();

        using (var context = CreateContext())
        {
            var broker = new Broker(nombre: "Broker with Providers " + Guid.NewGuid(), creationUser: Guid.NewGuid());
            context.Broker.Add(entity: broker);
            await context.SaveChangesAsync();
            brokerId = broker.Id;

            var proveedor =
                new Proveedor(nombre: providerName, urlIcono: "https://example.com/icon.png", categoria: Categoria.Servicios, broker: broker, creationUser: Guid.NewGuid());
            context.Proveedor.Add(entity: proveedor);
            await context.SaveChangesAsync();
        }

        // 3. Test Get Providers by Broker
        var response = await client.GetAsync(requestUri: $"/{ApiVersion}/broker/{brokerId}/proveedores");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result = JsonConvert.DeserializeObject<List<ProveedorResult>>(value: content, settings: _jsonSettings);

        Assert.NotNull(@object: result);
        Assert.NotEmpty(collection: result);
        Assert.Contains(collection: result, filter: p => p.Nombre == providerName);
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }
}
