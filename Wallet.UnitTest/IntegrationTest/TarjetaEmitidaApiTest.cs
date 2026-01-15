using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;

namespace Wallet.UnitTest.IntegrationTest;

public class TarjetaEmitidaApiTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public TarjetaEmitidaApiTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }

    [Fact]
    public async Task ActualizarConfiguracionTarjeta_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        int tarjetaId;
        string concurrencyToken;

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

            var tarjeta = new TarjetaEmitida(cuenta.Id, "1234567890123456", "1111", Wallet.DOM.Enums.TipoTarjeta.Fisica,
                DateTime.UtcNow.AddYears(1), Guid.NewGuid());
            context.TarjetaEmitida.Add(tarjeta);
            await context.SaveChangesAsync();

            tarjetaId = tarjeta.Id;
            concurrencyToken = Convert.ToBase64String(tarjeta.ConcurrencyToken);
        }

        var request = new ConfiguracionTarjetaRequest
        {
            LimiteDiario = 5000,
            ComprasEnLinea = true,
            Retiros = true,
            ConcurrencyToken = concurrencyToken
        };

        // Act
        // Act
        var response = await client.PutAsync($"/{ApiVersion}/tarjetasemitidas/{tarjetaId}/configuracion",
            CreateContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var context = CreateContext())
        {
            var updatedTarjeta = await context.TarjetaEmitida.FindAsync(tarjetaId);
            Assert.NotNull(updatedTarjeta);
            Assert.Equal(5000, updatedTarjeta.LimiteDiario);
            Assert.True(updatedTarjeta.ComprasEnLineaHabilitadas);
            Assert.True(updatedTarjeta.RetirosCajeroHabilitados);
        }
    }

    [Fact]
    public async Task CambiarBloqueoTarjeta_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        int tarjetaId;
        string concurrencyToken;

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

            var tarjeta = new TarjetaEmitida(cuenta.Id, "1234567890123456", "1111", Wallet.DOM.Enums.TipoTarjeta.Fisica,
                DateTime.UtcNow.AddYears(1), Guid.NewGuid());
            context.TarjetaEmitida.Add(tarjeta);
            await context.SaveChangesAsync();

            tarjetaId = tarjeta.Id;
            concurrencyToken = Convert.ToBase64String(tarjeta.ConcurrencyToken);
        }

        var request = new CambioBloqueoRequest
        {
            Bloquear = true,
            ConcurrencyToken = concurrencyToken
        };

        // Act
        var response =
            await client.PutAsync($"/{ApiVersion}/tarjetasemitidas/{tarjetaId}/bloqueo", CreateContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using (var context = CreateContext())
        {
            var updatedTarjeta = await context.TarjetaEmitida.FindAsync(tarjetaId);
            Assert.NotNull(updatedTarjeta);
            Assert.True(updatedTarjeta.BloqueoTemporal);
        }
    }
}
