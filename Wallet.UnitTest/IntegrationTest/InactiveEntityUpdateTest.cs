using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;

namespace Wallet.UnitTest.IntegrationTest;

public class InactiveEntityUpdateTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public InactiveEntityUpdateTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Test_Update_Inactive_KeyValueConfig_ThrowsException()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create Active Config
        var key = $"ConfigTest_{Guid.NewGuid()}";
        var createRequest = new KeyValueConfigRequest { Key = key, Value = "Initial" };
        var createRes = await client.PostAsync($"/{ApiVersion}/configurations", CreateContent(createRequest));
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        var config =
            JsonConvert.DeserializeObject<KeyValueConfigResult>(await createRes.Content.ReadAsStringAsync(),
                _jsonSettings);

        // 3. Deactivate (Delete)
        var deleteRes =
            await client.DeleteAsync(
                $"/{ApiVersion}/configurations/{key}?concurrencyToken={WebUtility.UrlEncode(config.ConcurrencyToken)}");
        Assert.Equal(HttpStatusCode.OK, deleteRes.StatusCode);
        var deactivatedConfig =
            JsonConvert.DeserializeObject<KeyValueConfigResult>(await deleteRes.Content.ReadAsStringAsync(),
                _jsonSettings);

        // 4. Try Update
        var updateRequest = new KeyValueConfigUpdateRequest
            { Value = "Updated", ConcurrencyToken = deactivatedConfig.ConcurrencyToken };
        var updateRes = await client.PutAsync($"/{ApiVersion}/configurations/{key}", CreateContent(updateRequest));

        // 5. Assert BadRequest (due to mapped exception) or 500/409 depending on how EMGeneralAggregateException is handled.
        // Usually, ServiceErrors map to 400 or 409 or 422.
        // Assuming Standard behavior for business rule violation.
        var content = await updateRes.Content.ReadAsStringAsync();
        _output.WriteLine($"Response: {updateRes.StatusCode}, Content: {content}");
        Assert.False(updateRes.IsSuccessStatusCode, "Should not update inactive config");
    }

    [Fact]
    public async Task Test_Update_Inactive_Usuario_Telefono_ThrowsException()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create User manually (to control status) or via API
        // For simplicity, let's use the DB Context to create and deactivate a user.
        int userId;
        string base64Token;
        using (var context = CreateContext())
        {
            var u = new Usuario("+52", "5550009999", "testinactive@test.com", "Pass123!",
                EstatusRegistroEnum.RegistroCompletado, Guid.NewGuid());
            context.Usuario.Add(u);
            await context.SaveChangesAsync();
            userId = u.Id;

            // Deactivate
            u.Deactivate(Guid.NewGuid());
            await context.SaveChangesAsync();
            base64Token = Convert.ToBase64String(u.ConcurrencyToken);
        }

        // 3. Try Update Telefono
        var updateReq = new TelefonoUpdateRequest
            { CodigoPais = "+52", Telefono = "5551112222", ConcurrencyToken = base64Token };

        var updateRes =
            await client.PutAsync($"/{ApiVersion}/usuario/{userId}/actualizaTelefono", CreateContent(updateReq));

        Assert.False(updateRes.IsSuccessStatusCode, "Should not update inactive user phone");
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(body, _jsonSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
