using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wallet.DOM.Modelos;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class KeyValueConfigApiTest : DatabaseTestFixture
{
    private const string ApiVersion = "0.1";
    private const string ApiUri = "configurations";

    public KeyValueConfigApiTest()
    {
        // Ensure context is created if needed, though DatabaseTestFixture should handle distinct DBs per test class or shared.
        // We can seed data here if we want isolated data, or use shared if appropriate.
        // For KeyValueConfig, let's insert some data.
        var context = CreateContext();
        if (!context.KeyValueConfig.AnyAsync(predicate: x => x.Key == "IntegrationTestKey").Result)
        {
            context.KeyValueConfig.Add(entity: new KeyValueConfig(key: "IntegrationTestKey", value: "IntegrationTestValue",
                creationUser: System.Guid.NewGuid()));
            context.SaveChanges();
        }
    }

    [Fact]
    public async Task GetAllKeyValueConfigs_ReturnsOk()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync(requestUri: $"{ApiVersion}/{ApiUri}");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var results = JsonConvert.DeserializeObject<List<KeyValueConfigResult>>(value: content);

        Assert.NotNull(@object: results);
        Assert.Contains(collection: results, filter: x => x.Key == "IntegrationTestKey" && x.Value == "IntegrationTestValue");
    }

    [Fact]
    public async Task CreateKeyValueConfig_ReturnsCreated()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var key = "NewKey_" + System.Guid.NewGuid();
        var payload = new KeyValueConfigRequest { Key = key, Value = "NewValue" };

        // Act
        var response = await client.PostAsync(requestUri: $"{ApiVersion}/{ApiUri}",
            content: new System.Net.Http.StringContent(content: JsonConvert.SerializeObject(value: payload), encoding: System.Text.Encoding.UTF8,
                mediaType: "application/json"));

        // Assert
        Assert.Equal(expected: HttpStatusCode.Created, actual: response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<KeyValueConfigResult>(value: content);
        Assert.NotNull(@object: result);
        Assert.Equal(expected: key, actual: result.Key);
        Assert.Equal(expected: "NewValue", actual: result.Value);
    }

    [Fact]
    public async Task GetKeyValueConfigByKey_ReturnsOk()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var context = CreateContext();
        var key = "GetKey_" + System.Guid.NewGuid();
        context.KeyValueConfig.Add(entity: new KeyValueConfig(key: key, value: "GetValue", creationUser: System.Guid.NewGuid()));
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync(requestUri: $"{ApiVersion}/{ApiUri}/{key}");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<KeyValueConfigResult>(value: content);
        Assert.NotNull(@object: result);
        Assert.Equal(expected: key, actual: result.Key);
        Assert.Equal(expected: "GetValue", actual: result.Value);
    }

    [Fact]
    public async Task UpdateKeyValueConfig_ReturnsOk()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var context = CreateContext();
        var key = "UpdateKey_" + System.Guid.NewGuid();
        var entity = new KeyValueConfig(key: key, value: "OriginalValue", creationUser: System.Guid.NewGuid());
        context.KeyValueConfig.Add(entity: entity);
        await context.SaveChangesAsync();

        var payload = new KeyValueConfigUpdateRequest
        {
            Value = "UpdatedValue",
            ConcurrencyToken = Convert.ToBase64String(inArray: entity.ConcurrencyToken)
        };

        // Act
        var response = await client.PutAsync(requestUri: $"{ApiVersion}/{ApiUri}/{key}",
            content: new System.Net.Http.StringContent(content: JsonConvert.SerializeObject(value: payload), encoding: System.Text.Encoding.UTF8,
                mediaType: "application/json"));

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<KeyValueConfigResult>(value: content);
        Assert.NotNull(@object: result);
        Assert.Equal(expected: key, actual: result.Key);
        Assert.Equal(expected: "UpdatedValue", actual: result.Value);
    }

    [Fact]
    public async Task DeleteKeyValueConfig_ReturnsOk()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var context = CreateContext();
        var key = "DeleteKey_" + System.Guid.NewGuid();
        context.KeyValueConfig.Add(entity: new KeyValueConfig(key: key, value: "DeleteValue", creationUser: System.Guid.NewGuid()));
        await context.SaveChangesAsync();

        // Act
        var response = await client.DeleteAsync(requestUri: $"{ApiVersion}/{ApiUri}/{key}");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);

        // Verify deletion
        context.ChangeTracker.Clear();
        var deletedEntity = await context.KeyValueConfig.IgnoreQueryFilters()
            .FirstOrDefaultAsync(predicate: x => x.Key == key);
        Assert.NotNull(@object: deletedEntity);
        Assert.False(condition: deletedEntity.IsActive, userMessage: "Entity should be logically deleted (IsActive=false)");
    }
}
