using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;
using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.UnitTest.IntegrationTest;

public class CuentaWalletApiTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public CuentaWalletApiTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }

    [Fact]
    public async Task GetCuentaWallet_PorCliente_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        int clienteId;
        int cuentaId;

        using (var context = CreateContext())
        {
            var commonSettings = new CommonSettings();

            // Use seeded data from CommonSettings, but we need to persist it to this test's context
            context.Empresa.AddRange(commonSettings.Empresas);
            context.Estado.AddRange(commonSettings.Estados);
            context.Usuario.AddRange(commonSettings.Usuarios);
            context.Cliente.AddRange(commonSettings.Clientes);

            await context.SaveChangesAsync();

            // Setup a specific user/client for this test if not using the authenticated one
            // Ideally we should link the authenticated user to a client, but CreateAuthenticatedUserAsync creates a NEW user.
            // Let's attach a client to the authenticated user.

            var dbUser = await context.Usuario.FindAsync(user.Id);
            var empresa = await context.Empresa.FirstAsync(e => e.Nombre == "Tecomnet");

            var cliente = new Wallet.DOM.Modelos.GestionCliente.Cliente(dbUser!, empresa, Guid.NewGuid());
            cliente.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "Client",
                fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Wallet.DOM.Enums.Genero.Masculino,
                modificationUser: Guid.NewGuid());
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();
            clienteId = cliente.Id;

            // Create Wallet
            var cuenta = new CuentaWallet(clienteId, "MXN", "123456789012345678", Guid.NewGuid());
            context.CuentaWallet.Add(cuenta);
            await context.SaveChangesAsync();
            cuentaId = cuenta.Id;
        }

        // Act
        var response = await client.GetAsync($"/{ApiVersion}/cuentaWallet/{clienteId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CuentaWalletResult>(content);

        Assert.NotNull(result);
        Assert.Equal("MXN", result.Moneda);
        Assert.Equal(cuentaId, result.Id);
        Assert.Equal(clienteId, result.ClienteId);
    }

    [Fact]
    public async Task GetTarjetasVinculadas_PorWallet_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        int cuentaId;
        int tarjetaId;

        using (var context = CreateContext())
        {
            var commonSettings = new CommonSettings();
            context.Empresa.AddRange(commonSettings.Empresas);
            context.Estado.AddRange(commonSettings.Estados);
            await context.SaveChangesAsync();

            var dbUser = await context.Usuario.FindAsync(user.Id);
            var empresa = await context.Empresa.FirstAsync(e => e.Nombre == "Tecomnet");
            var cliente = new Wallet.DOM.Modelos.GestionCliente.Cliente(dbUser!, empresa, Guid.NewGuid());
            cliente.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "Client",
                fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Wallet.DOM.Enums.Genero.Masculino,
                modificationUser: Guid.NewGuid());
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();

            var cuenta = new CuentaWallet(cliente.Id, "MXN", "123456789012345678", Guid.NewGuid());
            context.CuentaWallet.Add(cuenta);
            await context.SaveChangesAsync();
            cuentaId = cuenta.Id;

            var tarjeta = new TarjetaVinculada(cuenta.Id, "4111222233334444", "My Card",
                Wallet.DOM.Enums.MarcaTarjeta.Visa, DateTime.Now.AddYears(2), Guid.NewGuid());
            context.TarjetaVinculada.Add(tarjeta);
            await context.SaveChangesAsync();
            tarjetaId = tarjeta.Id;
        }

        // Act
        var response = await client.GetAsync($"/{ApiVersion}/cuentaWallet/{cuentaId}/tarjetasVinculadas");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<TarjetaVinculadaResult>>(content, _jsonSettings);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(tarjetaId, result[0].Id);
    }
}
