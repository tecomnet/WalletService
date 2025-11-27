using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ClienteFacadeTest;

public class UbicacionGeolocalizacionFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IUbicacionGeolocalizacionFacade>(setupConfig)
{
    [Theory]
    // Casos ok
    [InlineData("1. OK: Nueva ubicacion", 1, 1, 1, Dispositivo.Web, "Inicio session", "Tablet", "Chrome", "127.0.0.1",
        true, new string[] { })]
    [InlineData("1. OK: Nueva ubicacion", 2, 1.2515, 2.18956, Dispositivo.App, "Nuevo usuario", "Smartphone",
        "App movil", "127.0.0.1", true, new string[] { })]
    // Casos error
    [InlineData("2. ERROR: Cliente no encontrado", 25, 1, 1, Dispositivo.App, "Nueva cuenta", "Celular", "App Wallet",
        "127.0.0.1", false, new string[] { ServiceErrorsBuilder.ClienteNoEncontrado })]
    public async Task GuardarUbicacionGeolocalizacionTest(
        string caseName,
        int idCliente,
        decimal latitud,
        decimal longitud,
        Dispositivo dispositivo,
        string tipoEvento,
        string tipoDispositivo,
        string agente,
        string direccionIp,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Ejecuta el método
            var ubicacion = await Facade.GuardarUbicacionGeolocalizacionAsync(
                idCliente: idCliente,
                latitud: latitud,
                longitud: longitud,
                dispositivo: dispositivo,
                tipoEvento: tipoEvento,
                tipoDispositivo: tipoDispositivo,
                agente: agente,
                direccionIp: direccionIp,
                creationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId);
            // No nullo
            Assert.NotNull(ubicacion);
            // Assert properties 
            Assert.Equal(expected: idCliente, actual: ubicacion.Usuario!.Cliente!.Id);
            Assert.Equal(expected: latitud, actual: ubicacion.Latitud);
            Assert.Equal(expected: longitud, actual: ubicacion.Longitud);
            Assert.Equal(expected: dispositivo, actual: ubicacion.Dispositivo);
            Assert.Equal(expected: tipoEvento, actual: ubicacion.TipoEvento);
            Assert.Equal(expected: tipoDispositivo, actual: ubicacion.TipoDispositivo);
            Assert.Equal(expected: agente, actual: ubicacion.Agente);
            Assert.Equal(expected: direccionIp, actual: ubicacion.DireccionIp);
            // Obtener de la bd
            // Get the user from context
            var ubicacionContext = await Context.UbicacionGeolocalizacion.Include(x => x.Usuario)
                .ThenInclude(u => u.Cliente).AsNoTracking().FirstOrDefaultAsync(x => x.Id == ubicacion.Id);
            // Confirm user created in context
            Assert.NotNull(ubicacionContext);
            // Assert properties 
            Assert.Equal(expected: idCliente, actual: ubicacionContext.Usuario!.Cliente!.Id);
            Assert.Equal(expected: latitud, actual: ubicacionContext.Latitud);
            Assert.Equal(expected: longitud, actual: ubicacionContext.Longitud);
            Assert.Equal(expected: dispositivo, actual: ubicacionContext.Dispositivo);
            Assert.Equal(expected: tipoEvento, actual: ubicacionContext.TipoEvento);
            Assert.Equal(expected: tipoDispositivo, actual: ubicacionContext.TipoDispositivo);
            Assert.Equal(expected: agente, actual: ubicacionContext.Agente);
            Assert.Equal(expected: direccionIp, actual: ubicacionContext.DireccionIp);
            // Assert success
            Assert.True(condition: success, userMessage: "Should not reach on failures.");
        }
        // Catch the managed errors and check them with the expected ones in the case of failures
        catch (EMGeneralAggregateException exception)
        {
            // Treat the raised error
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Catch any non managed errors and display them to understand the root cause
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            // Should not reach for unmanaged errors
            Assert.Fail($"Uncaught exception. {exception.Message}");
        }
    }
}