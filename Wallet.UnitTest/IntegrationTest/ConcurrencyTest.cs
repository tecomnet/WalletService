using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class ConcurrencyTest : DatabaseTestFixture
{
    private const string API_VERSION = "0.1";

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(body);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    [Fact]
    public async Task Put_CorreoElectronico_With_Correct_Token_Returns_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Get current user to fetch ConcurrencyToken
        var getResponse = await client.GetAsync($"{API_VERSION}/usuario/{user.Id}");
        getResponse.EnsureSuccessStatusCode();
        var currentUsuario =
            JsonConvert.DeserializeObject<UsuarioResult>(await getResponse.Content.ReadAsStringAsync());
        Assert.NotNull(currentUsuario);
        Assert.NotNull(currentUsuario.ConcurrencyToken);

        var request = new EmailUpdateRequest
        {
            CorreoElectronico = $"newemail{Guid.NewGuid()}@test.com",
            ConcurrencyToken = currentUsuario.ConcurrencyToken
        };

        // Act
        var response = await client.PutAsync(
            $"{API_VERSION}/usuario/{user.Id}/actualizaEmail", CreateContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = JsonConvert.DeserializeObject<UsuarioResult>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(result);
        Assert.Equal(request.CorreoElectronico, result.CorreoElectronico);
    }

    [Fact]
    public async Task Put_CorreoElectronico_With_Stale_Token_Returns_Conflict()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 1. Get current token (Version 1)
        var getResponse = await client.GetAsync($"{API_VERSION}/usuario/{user.Id}");
        getResponse.EnsureSuccessStatusCode();
        var currentUsuario =
            JsonConvert.DeserializeObject<UsuarioResult>(await getResponse.Content.ReadAsStringAsync());
        Assert.NotNull(currentUsuario);
        var staleToken = currentUsuario.ConcurrencyToken;

        // 2. Simulate concurrent update (Backdoor)
        await using var context = CreateContext();
        var userDb = await context.Usuario.FindAsync(user.Id);
        userDb.ActualizarTelefono(userDb.CodigoPais, "5555555555",
            Guid.NewGuid()); // Modify something to change version
        await context.SaveChangesAsync(); // DB token is now Version 2

        var request = new EmailUpdateRequest
        {
            CorreoElectronico = $"concurrent{Guid.NewGuid()}@test.com",
            ConcurrencyToken = staleToken // Version 1
        };

        // Act
        var response = await client.PutAsync(
            $"{API_VERSION}/usuario/{user.Id}/actualizaEmail", CreateContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
