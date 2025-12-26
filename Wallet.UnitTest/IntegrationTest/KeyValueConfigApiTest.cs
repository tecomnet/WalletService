using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wallet.DOM.Modelos;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit;

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
        if (!context.KeyValueConfig.AnyAsync(x => x.Key == "IntegrationTestKey").Result)
        {
            context.KeyValueConfig.Add(new KeyValueConfig("IntegrationTestKey", "IntegrationTestValue",
                System.Guid.NewGuid()));
            context.SaveChanges();
        }
    }

    [Fact]
    public async Task GetAllKeyValueConfigs_ReturnsOk()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync($"{ApiVersion}/{ApiUri}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var results = JsonConvert.DeserializeObject<List<KeyValueConfigResult>>(content);

        Assert.NotNull(results);
        Assert.Contains(results, x => x.Key == "IntegrationTestKey" && x.Value == "IntegrationTestValue");
    }

    [Fact]
    public async Task CreateKeyValueConfig_ReturnsCreated()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var key = "NewKey_" + System.Guid.NewGuid();
        var payload = new KeyValueConfigRequest { Key = key, Value = "NewValue" };

        // Act
        var response = await client.PostAsync($"{ApiVersion}/{ApiUri}",
            new System.Net.Http.StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<KeyValueConfigResult>(content);
        Assert.NotNull(result);
        Assert.Equal(key, result.Key);
        Assert.Equal("NewValue", result.Value);
    }

    [Fact]
    public async Task GetKeyValueConfigByKey_ReturnsOk()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var context = CreateContext();
        var key = "GetKey_" + System.Guid.NewGuid();
        context.KeyValueConfig.Add(new KeyValueConfig(key, "GetValue", System.Guid.NewGuid()));
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"{ApiVersion}/{ApiUri}/{key}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<KeyValueConfigResult>(content);
        Assert.NotNull(result);
        Assert.Equal(key, result.Key);
        Assert.Equal("GetValue", result.Value);
    }

    [Fact]
    public async Task UpdateKeyValueConfig_ReturnsOk()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var context = CreateContext();
        var key = "UpdateKey_" + System.Guid.NewGuid();
        var entity = new KeyValueConfig(key, "OriginalValue", System.Guid.NewGuid());
        context.KeyValueConfig.Add(entity);
        await context.SaveChangesAsync();

        var payload = new KeyValueConfigUpdateRequest
        {
            Value = "UpdatedValue",
            ConcurrencyToken = Convert.ToBase64String(entity.ConcurrencyToken)
        };

        // Act
        var response = await client.PutAsync($"{ApiVersion}/{ApiUri}/{key}",
            new System.Net.Http.StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8,
                "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<KeyValueConfigResult>(content);
        Assert.NotNull(result);
        Assert.Equal(key, result.Key);
        Assert.Equal("UpdatedValue", result.Value);
    }

    [Fact]
    public async Task DeleteKeyValueConfig_ReturnsOk()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var context = CreateContext();
        var key = "DeleteKey_" + System.Guid.NewGuid();
        context.KeyValueConfig.Add(new KeyValueConfig(key, "DeleteValue", System.Guid.NewGuid()));
        await context.SaveChangesAsync();

        // Act
        var response = await client.DeleteAsync($"{ApiVersion}/{ApiUri}/{key}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify deletion
        context.ChangeTracker.Clear();
        var deletedEntity = await context.KeyValueConfig.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Key == key);
        Assert.NotNull(deletedEntity);
        Assert.False(deletedEntity.IsActive, "Entity should be logically deleted (IsActive=false)");
    }
}
