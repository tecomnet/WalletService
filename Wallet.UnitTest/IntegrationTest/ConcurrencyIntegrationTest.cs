using System.Net;
using Newtonsoft.Json;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

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
        var initialConfig = new KeyValueConfig(key: key, value: "InitialValue", creationUser: Guid.NewGuid());
        context.KeyValueConfig.Add(entity: initialConfig);
        await context.SaveChangesAsync();

        // 2. Act - Both users get the resource
        // User 1 gets the resource
        var response1 = await client1.GetAsync(requestUri: $"{ApiVersion}/{ApiUri}/{key}");
        var config1 = JsonConvert.DeserializeObject<KeyValueConfigResult>(value: await response1.Content.ReadAsStringAsync());

        // User 2 gets the resource
        var response2 = await client2.GetAsync(requestUri: $"{ApiVersion}/{ApiUri}/{key}");
        var config2 = JsonConvert.DeserializeObject<KeyValueConfigResult>(value: await response2.Content.ReadAsStringAsync());

        // 3. Act - User 1 updates the resource successfully
        var updatePayload1 = new KeyValueConfigUpdateRequest
        {
            Value = "ValueUpdatedByUser1",
            ConcurrencyToken = config1.ConcurrencyToken
        };

        var updateResponse1 = await client1.PutAsync(requestUri: $"{ApiVersion}/{ApiUri}/{key}",
            content: new StringContent(content: JsonConvert.SerializeObject(value: updatePayload1), encoding: System.Text.Encoding.UTF8,
                mediaType: "application/json"));

        var content1 = await updateResponse1.Content.ReadAsStringAsync();
        Assert.True(condition: updateResponse1.StatusCode == HttpStatusCode.OK,
            userMessage: $"User 1 update failed. Status: {updateResponse1.StatusCode}, Body: {content1}");

        // 4. Act - User 2 tries to update with the OLD token
        var updatePayload2 = new KeyValueConfigUpdateRequest
        {
            Value = "ValueUpdatedByUser2",
            ConcurrencyToken = config2.ConcurrencyToken // This token is now stale
        };

        var updateResponse2 = await client2.PutAsync(requestUri: $"{ApiVersion}/{ApiUri}/{key}",
            content: new StringContent(content: JsonConvert.SerializeObject(value: updatePayload2), encoding: System.Text.Encoding.UTF8,
                mediaType: "application/json"));

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
            Assert.Contains(expectedSubstring: ServiceErrorsBuilder.ConcurrencyError, actualString: content2);
        }
        else
        {
            // Fail if it's 200 OK
            Assert.NotEqual(expected: HttpStatusCode.OK, actual: updateResponse2.StatusCode);
        }
    }
}
