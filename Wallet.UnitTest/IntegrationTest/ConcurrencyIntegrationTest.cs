using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit;

namespace Wallet.UnitTest.IntegrationTest;

public class ConcurrencyIntegrationTest : DatabaseTestFixture
{
    private const string ApiVersion = "0.1";
    private const string ApiUri = "configurations";

    [Fact]
    public async Task UpdateKeyValueConfig_OptimisticConcurrency_ShouldFailOnSecondUpdate()
    {
        // 1. Arrange
        var client1 = Factory.CreateAuthenticatedClient();
        var client2 = Factory.CreateAuthenticatedClient();
        var context = CreateContext();

        var key = "ConcurrencyKey_" + Guid.NewGuid();
        var initialConfig = new KeyValueConfig(key, "InitialValue", Guid.NewGuid());
        context.KeyValueConfig.Add(initialConfig);
        await context.SaveChangesAsync();

        // 2. Act - Both users get the resource
        // User 1 gets the resource
        var response1 = await client1.GetAsync($"{ApiVersion}/{ApiUri}/{key}");
        var config1 = JsonConvert.DeserializeObject<KeyValueConfigResult>(await response1.Content.ReadAsStringAsync());

        // User 2 gets the resource
        var response2 = await client2.GetAsync($"{ApiVersion}/{ApiUri}/{key}");
        var config2 = JsonConvert.DeserializeObject<KeyValueConfigResult>(await response2.Content.ReadAsStringAsync());

        // 3. Act - User 1 updates the resource successfully
        var updatePayload1 = new KeyValueConfigUpdateRequest
        {
            Value = "ValueUpdatedByUser1",
            ConcurrencyToken = config1.ConcurrencyToken
        };

        var updateResponse1 = await client1.PutAsync($"{ApiVersion}/{ApiUri}/{key}",
            new StringContent(JsonConvert.SerializeObject(updatePayload1), System.Text.Encoding.UTF8,
                "application/json"));

        var content1 = await updateResponse1.Content.ReadAsStringAsync();
        Assert.True(updateResponse1.StatusCode == HttpStatusCode.OK,
            $"User 1 update failed. Status: {updateResponse1.StatusCode}, Body: {content1}");

        // 4. Act - User 2 tries to update with the OLD token
        var updatePayload2 = new KeyValueConfigUpdateRequest
        {
            Value = "ValueUpdatedByUser2",
            ConcurrencyToken = config2.ConcurrencyToken // This token is now stale
        };

        var updateResponse2 = await client2.PutAsync($"{ApiVersion}/{ApiUri}/{key}",
            new StringContent(JsonConvert.SerializeObject(updatePayload2), System.Text.Encoding.UTF8,
                "application/json"));

        // 5. Assert - Should fail with 500 or specific error, depending on how global error handler maps the exception
        // The facade throws EMGeneralAggregateException with ConcurrencyError
        var content2 = await updateResponse2.Content.ReadAsStringAsync();

        // Since the exception handling middleware might wrap it in 500 or 409 depending on configuration.
        // But ServiceErrorsBuilder doesn't automatically map to 409 Conflict unless middleware does.
        // Assuming it returns 500 or 400 with the error code in the body.

        // Let's inspect the response. Ideally it should be 409 Conflict if mapped, or 500/400 with our error code.
        // Based on ServiceErrorsBuilder usage, it usually returns a JSON with error details.

        if (updateResponse2.StatusCode == HttpStatusCode.InternalServerError ||
            updateResponse2.StatusCode == HttpStatusCode.BadRequest ||
            updateResponse2.StatusCode == HttpStatusCode.Conflict)
        {
            // Check if body contains the error code
            Assert.Contains(ServiceErrorsBuilder.ConcurrencyError, content2);
        }
        else
        {
            // Fail if it's 200 OK
            Assert.NotEqual(HttpStatusCode.OK, updateResponse2.StatusCode);
        }
    }
}
