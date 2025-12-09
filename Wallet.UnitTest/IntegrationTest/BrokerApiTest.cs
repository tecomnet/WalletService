using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create Broker
        var request = new BrokerRequest { Nombre = "Broker Integration Test" };
        var response = await client.PostAsync($"/{ApiVersion}/broker", CreateContent(request));

        var content = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK,
            $"Expected Created or OK. Got {response.StatusCode}. Content: {content}");

        var result = JsonConvert.DeserializeObject<BrokerResult>(content, _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(request.Nombre, result.Nombre);
        Assert.True(result.Id > 0);
        _output.WriteLine($"Broker Created: {result.Nombre} (ID: {result.Id})");
    }

    [Fact]
    public async Task Test_Get_Brokers_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create a Broker first
        var request = new BrokerRequest { Nombre = "Broker List Test" };
        await client.PostAsync($"/{ApiVersion}/broker", CreateContent(request));

        // 3. Get List
        var response = await client.GetAsync($"/{ApiVersion}/brokers");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonConvert.DeserializeObject<List<BrokerResult>>(content, _jsonSettings);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, b => b.Nombre == "Broker List Test");
    }

    [Fact]
    public async Task Test_Get_BrokerById_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create Broker
        var createRequest = new BrokerRequest { Nombre = "Broker GetById Test" };
        var createResponse = await client.PostAsync($"/{ApiVersion}/broker", CreateContent(createRequest));
        var createResult =
            JsonConvert.DeserializeObject<BrokerResult>(await createResponse.Content.ReadAsStringAsync(),
                _jsonSettings);

        // 3. Get By Id
        var response = await client.GetAsync($"/{ApiVersion}/broker/{createResult.Id}");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonConvert.DeserializeObject<BrokerResult>(content, _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(createResult.Id, result.Id);
        Assert.Equal(createResult.Nombre, result.Nombre);
    }

    [Fact]
    public async Task Test_Update_Broker_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create Broker
        var createRequest = new BrokerRequest { Nombre = "Broker Update Test Original" };
        var createResponse = await client.PostAsync($"/{ApiVersion}/broker", CreateContent(createRequest));
        var createResult =
            JsonConvert.DeserializeObject<BrokerResult>(await createResponse.Content.ReadAsStringAsync(),
                _jsonSettings);

        // 3. Update Broker (Using POST as per Swagger/Controller spec for update usually or maybe PUT? Controller says HttpPost for Update!??)
        // Checking Controller Implementation: 
        // [HttpPost] Route("/{version:apiVersion}/broker/{idBroker}") public abstract Task<IActionResult> ActualizarBroker...

        var updateRequest = new BrokerRequest { Nombre = "Broker Update Test Updated" };
        var response = await client.PostAsync($"/{ApiVersion}/broker/{createResult.Id}", CreateContent(updateRequest));
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = JsonConvert.DeserializeObject<BrokerResult>(content, _jsonSettings);

        Assert.NotNull(result);
        Assert.Equal(createResult.Id, result.Id);
        Assert.Equal("Broker Update Test Updated", result.Nombre);
    }

    [Fact]
    public async Task Test_Delete_Broker_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create Broker
        var createRequest = new BrokerRequest { Nombre = "Broker Delete Test" };
        var createResponse = await client.PostAsync($"/{ApiVersion}/broker", CreateContent(createRequest));
        var createResult =
            JsonConvert.DeserializeObject<BrokerResult>(await createResponse.Content.ReadAsStringAsync(),
                _jsonSettings);

        // 3. Delete Broker
        var response = await client.DeleteAsync($"/{ApiVersion}/broker/{createResult.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // 4. Verify Not Found (or deleted)
        var getResponse = await client.GetAsync($"/{ApiVersion}/broker/{createResult.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Test_Get_Broker_Proveedores_Ok()
    {
        // 1. Auths
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Setup Data: Broker & Provider
        // Using direct context for speed and relationship setup
        int brokerId;
        string providerName = "Provider for Broker Test " + Guid.NewGuid();

        using (var context = CreateContext())
        {
            var broker = new Wallet.DOM.Modelos.Broker("Broker with Providers " + Guid.NewGuid(), Guid.NewGuid());
            context.Broker.Add(broker);
            await context.SaveChangesAsync();
            brokerId = broker.Id;

            var proveedor = new Wallet.DOM.Modelos.Proveedor(providerName, "https://example.com/icon.png", broker, Guid.NewGuid());
            context.Proveedor.Add(proveedor);
            await context.SaveChangesAsync();
        }

        // 3. Test Get Providers by Broker
        var response = await client.GetAsync($"/{ApiVersion}/broker/{brokerId}/proveedores");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = JsonConvert.DeserializeObject<List<ProveedorResult>>(content, _jsonSettings);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.Nombre == providerName);
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(body, _jsonSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
