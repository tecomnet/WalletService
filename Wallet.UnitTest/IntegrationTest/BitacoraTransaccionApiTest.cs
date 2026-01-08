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

public class BitacoraTransaccionApiTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public BitacoraTransaccionApiTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task GetDetallesPorTransaccion_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        Guid transaccionId = Guid.NewGuid();
        string idTransaccionString = transaccionId.ToString();

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

            // Create Transaction
            var bitacora = new BitacoraTransaccion(
                idBilletera: cuenta.Id,
                monto: 100.00m,
                tipo: "PagoServicio",
                direccion: "Cargo",
                estatus: "Completada",
                creationUser: Guid.NewGuid()
            );

            context.BitacoraTransaccion.Add(bitacora);
            await context.SaveChangesAsync();

            // Create DetallesPagoServicio
            var detalles = new DetallesPagoServicio(
                idTransaccion: bitacora.Id,
                idProveedor: 1, // Mock provider ID
                numeroReferencia: "REF123",
                creationUser: Guid.NewGuid()
            );
            context.DetallesPagoServicio.Add(detalles);
            await context.SaveChangesAsync();

            idTransaccionString = bitacora.Id.ToString();
        }

        // Act
        var response = await client.GetAsync($"/{ApiVersion}/transacciones/{idTransaccionString}/PagoServicioDetalles");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<DetallesPagoServicioResult>(content, _jsonSettings);

        Assert.NotNull(result);
        Assert.Equal("REF123", result.NumeroReferencia);
    }
}
