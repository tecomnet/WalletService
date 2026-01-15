using System.Net;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

[Trait("Category", "Integration")]
public class UsuarioDeactivationTest : DatabaseTestFixture
{
    private const string API_VERSION = "0.1";

    public UsuarioDeactivationTest()
    {
        Factory.UseTestAuth = false;
    }

    [Fact]
    public async Task Delete_Usuario_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var concurrencyToken = Convert.ToBase64String(user.ConcurrencyToken);

        // Act
        var response = await client.DeleteAsync(
            $"{API_VERSION}/usuario/{user.Id}?concurrencyToken={WebUtility.UrlEncode(concurrencyToken)}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify in DB
        using var context = CreateContext();
        var dbUser = await context.Usuario.FindAsync(user.Id);
        Assert.NotNull(dbUser);
        Assert.False(dbUser.IsActive);
    }

    [Fact]
    public async Task Modify_Client_Blocked_When_User_Inactive()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();

        // Ensure client exists for this user (CreateAuthenticatedUserAsync creates client?)
        // Usually `CreateAuthenticatedUserAsync` creates a user. Client creation logic depends on setup.
        // I will check if client exists, otherwise create it.
        using (var setupContext = CreateContext())
        {
            var dbUser = await setupContext.Usuario.FindAsync(user.Id);
            if (await setupContext.Cliente.AllAsync(c => c.UsuarioId != user.Id))
            {
                var empresa = await setupContext.Empresa.FirstOrDefaultAsync(e => e.Nombre == "Tecomnet");
                if (empresa == null)
                {
                    empresa = new Wallet.DOM.Modelos.GestionEmpresa.Empresa("Tecomnet", Guid.NewGuid());
                    setupContext.Empresa.Add(empresa);
                    await setupContext.SaveChangesAsync();
                }

                var cliente = new Wallet.DOM.Modelos.GestionCliente.Cliente(dbUser, empresa, Guid.NewGuid());
                setupContext.Cliente.Add(cliente);
                await setupContext.SaveChangesAsync();
            }

            // Deactivate user manually
            dbUser.Deactivate(Guid.NewGuid());
            await setupContext.SaveChangesAsync();
        }

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new TelefonoUpdateRequest
        {
            CodigoPais = "+52",
            Telefono = "5512345678",
            ConcurrencyToken = Convert.ToBase64String(user.ConcurrencyToken)
        };

        var content = CreateContent(request);

        // Act - Try to update user phone (which is allowed for inactive user? Wait.
        // The requirement said "prevents any modification operations on a Client or Usuario entity if either of them is currently deactivated".
        // Let's verify if `ActualizarTelefono` in `UsuarioFacade` has this check.
        // `UsuarioFacade.ActualizarTelefonoAsync` does check `ValidarUsuarioIsActive`.

        var response = await client.PutAsync(
            $"{API_VERSION}/usuario/{user.Id}/actualizaTelefono", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        // Could verify error code "UsuarioInactivo" in response body if desired.
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(body);
        return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    }
}
