using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Enums;
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
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create Active Config
        var key = $"ConfigTest_{Guid.NewGuid()}";
        var createRequest = new KeyValueConfigRequest { Key = key, Value = "Initial" };
        var createRes = await client.PostAsync(requestUri: $"/{ApiVersion}/configurations", content: CreateContent(body: createRequest));
        Assert.Equal(expected: HttpStatusCode.Created, actual: createRes.StatusCode);
        var config =
            JsonConvert.DeserializeObject<KeyValueConfigResult>(value: await createRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Deactivate (Delete)
        var deleteRes =
            await client.DeleteAsync(
                requestUri: $"/{ApiVersion}/configurations/{key}?concurrencyToken={WebUtility.UrlEncode(value: config.ConcurrencyToken)}");
        Assert.Equal(expected: HttpStatusCode.OK, actual: deleteRes.StatusCode);
        var deactivatedConfig =
            JsonConvert.DeserializeObject<KeyValueConfigResult>(value: await deleteRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 4. Try Update
        var updateRequest = new KeyValueConfigUpdateRequest
            { Value = "Updated", ConcurrencyToken = deactivatedConfig.ConcurrencyToken };
        var updateRes = await client.PutAsync(requestUri: $"/{ApiVersion}/configurations/{key}", content: CreateContent(body: updateRequest));

        // 5. Assert BadRequest (due to mapped exception) or 500/409 depending on how EMGeneralAggregateException is handled.
        // Usually, ServiceErrors map to 400 or 409 or 422.
        // Assuming Standard behavior for business rule violation.
        var content = await updateRes.Content.ReadAsStringAsync();
        _output.WriteLine(message: $"Response: {updateRes.StatusCode}, Content: {content}");
        Assert.False(condition: updateRes.IsSuccessStatusCode, userMessage: "Should not update inactive config");
    }

    [Fact]
    public async Task Test_Update_Inactive_Usuario_Telefono_ThrowsException()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create User manually (to control status) or via API
        // For simplicity, let's use the DB Context to create and deactivate a user.
        int userId;
        string base64Token;
        using (var context = CreateContext())
        {
            var u = new Usuario(codigoPais: "+52", telefono: "5550009999", correoElectronico: "testinactive@test.com", contrasena: "Pass123!",
                estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: Guid.NewGuid());
            context.Usuario.Add(entity: u);
            await context.SaveChangesAsync();
            userId = u.Id;

            // Deactivate
            u.Deactivate(modificationUser: Guid.NewGuid());
            await context.SaveChangesAsync();
            base64Token = Convert.ToBase64String(inArray: u.ConcurrencyToken);
        }

        // 3. Try Update Telefono
        var updateReq = new TelefonoUpdateRequest
            { CodigoPais = "+52", Telefono = "5551112222", ConcurrencyToken = base64Token };

        var updateRes =
            await client.PutAsync(requestUri: $"/{ApiVersion}/usuario/{userId}/actualizaTelefono", content: CreateContent(body: updateReq));

        Assert.False(condition: updateRes.IsSuccessStatusCode, userMessage: "Should not update inactive user phone");
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }
}
