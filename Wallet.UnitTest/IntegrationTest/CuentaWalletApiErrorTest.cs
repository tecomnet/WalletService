using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Errors;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Enums;

namespace Wallet.UnitTest.IntegrationTest;

public class CuentaWalletApiErrorTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public CuentaWalletApiErrorTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }

    [Fact]
    public async Task PostTransacciones_TipoNoDisponible_ReturnsBadRequest()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        int clienteId;
        int cuentaId;
        int productoId;

        using (var context = CreateContext())
        {
            var commonSettings = new CommonSettings();

            // Seed common data if not present
            if (!context.Empresa.Any()) context.Empresa.AddRange(commonSettings.Empresas);
            if (!context.Estado.Any()) context.Estado.AddRange(commonSettings.Estados);
            if (!context.Broker.Any()) context.Broker.AddRange(commonSettings.Brokers);
            if (!context.Proveedor.Any()) context.Proveedor.AddRange(commonSettings.Proveedores);

            await context.SaveChangesAsync();

            // Link authenticated user to a client
            var dbUser = await context.Usuario.FindAsync(user.Id);
            var empresa = await context.Empresa.FirstOrDefaultAsync() ?? commonSettings.Empresas.First();
            // Refetch to attach to context
            empresa = await context.Empresa.FirstAsync(e => e.Id == empresa.Id);

            var cliente = new Wallet.DOM.Modelos.GestionCliente.Cliente(dbUser!, empresa, Guid.NewGuid());
            cliente.AgregarDatosPersonales(nombre: "Test",
                primerApellido: "User",
                segundoApellido: "Client",
                fechaNacimiento: new DateOnly(year: 1990,
                    month: 1,
                    day: 1),
                genero: Wallet.DOM.Enums.Genero.Masculino,
                modificationUser: Guid.NewGuid());
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();
            clienteId = cliente.Id;

            // Create Wallet
            var cuenta = new CuentaWallet(clienteId, "MXN", "123456789012345678", Guid.NewGuid());
            context.CuentaWallet.Add(cuenta);
            await context.SaveChangesAsync();
            cuentaId = cuenta.Id;

            // Get an existing product
            var producto = await context.Producto.FirstAsync();
            productoId = producto.Id;
        }

        // Request with unavailable transaction type 
        // We use MovilidadEnum as a type that should fail (is not RecargaTelefonicaEnum)
        var request = new TransaccionServicioRequest
        {
            Monto = 100,
            TipoTransaccion = TipoTransaccionEnum.MovilidadEnum,
            IdProducto = productoId,
            NumeroReferencia = "REF123",
            Concepto = "Test Transfer"
        };

        // Act
        var response =
            await client.PostAsync($"/{ApiVersion}/cuentaWallet/{cuentaId}/transaccion", CreateContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonConvert.DeserializeObject<InlineResponse400>(content);

        Assert.NotNull(errorResponse);
        Assert.NotNull(errorResponse.Errors);
        Assert.NotEmpty(errorResponse.Errors);

        // Verify the specific error code
        var error = errorResponse.Errors.FirstOrDefault();
        Assert.NotNull(error);
        Assert.Equal(RestAPIErrors.TransaccionNoDisponible, error.ErrorCode);
    }
}
