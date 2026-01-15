using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class ConcurrencyTest : DatabaseTestFixture
{
    private const string API_VERSION = "0.1";

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }

    [Fact]
    public async Task Put_CorreoElectronico_With_Correct_Token_Returns_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // Get current user to fetch ConcurrencyToken
        var getResponse = await client.GetAsync(requestUri: $"{API_VERSION}/usuario/{user.Id}");
        getResponse.EnsureSuccessStatusCode();
        var currentUsuario =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await getResponse.Content.ReadAsStringAsync());
        Assert.NotNull(@object: currentUsuario);
        Assert.NotNull(@object: currentUsuario.ConcurrencyToken);

        var request = new EmailUpdateRequest
        {
            CorreoElectronico = $"newemail{Guid.NewGuid()}@test.com",
            ConcurrencyToken = currentUsuario.ConcurrencyToken
        };

        // Act
        var response = await client.PutAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/actualizaEmail", content: CreateContent(body: request));

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result = JsonConvert.DeserializeObject<UsuarioResult>(value: await response.Content.ReadAsStringAsync());
        Assert.NotNull(@object: result);
        Assert.Equal(expected: request.CorreoElectronico, actual: result.CorreoElectronico);
    }

    [Fact]
    public async Task Put_CorreoElectronico_With_Stale_Token_Returns_Conflict()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 1. Get current token (Version 1)
        var getResponse = await client.GetAsync(requestUri: $"{API_VERSION}/usuario/{user.Id}");
        getResponse.EnsureSuccessStatusCode();
        var currentUsuario =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await getResponse.Content.ReadAsStringAsync());
        Assert.NotNull(@object: currentUsuario);
        var staleToken = currentUsuario.ConcurrencyToken;

        // 2. Simulate concurrent update (Backdoor)
        await using var context = CreateContext();
        var userDb = await context.Usuario.FindAsync(keyValues: user.Id);
        userDb.ActualizarTelefono(codigoPais: userDb.CodigoPais, telefono: "5555555555",
            modificationUser: Guid.NewGuid()); // Modify something to change version
        await context.SaveChangesAsync(); // DB token is now Version 2

        var request = new EmailUpdateRequest
        {
            CorreoElectronico = $"concurrent{Guid.NewGuid()}@test.com",
            ConcurrencyToken = staleToken // Version 1
        };

        // Act
        var response = await client.PutAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/actualizaEmail", content: CreateContent(body: request));

        // Assert
        Assert.Equal(expected: HttpStatusCode.Conflict, actual: response.StatusCode);
    }
}
