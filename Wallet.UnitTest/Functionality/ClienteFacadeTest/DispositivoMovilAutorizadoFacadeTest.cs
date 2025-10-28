using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ClienteFacadeTest;

public class DispositivoMovilAutorizadoFacadeFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IDispositivoMovilAutorizadoFacade>(setupConfig) 
{
    [Theory]
    // Casos Ok
    [InlineData("1. Caso Ok: Se agrega dispositivo movil autorizado", 1, "token", "id dispositivo", "nombre dispositivo", "caracteristica dispositivo", true,
        true, new string[] { })]
    // Casos Error
    [InlineData("2. Caso Error: El cliente no existe", 245, "token", "id dispositivo", "nombre dispositivo", "caracteristica dispositivo", true,
        false, new string[] { ServiceErrorsBuilder.ClienteNoEncontrado })]
    [InlineData("3. Caso Error: El dispositivo movil autorizado ya existe", 3, "32414", "32414", "nombre dispositivo", "caracteristica dispositivo", true,
        false, new string[] { ServiceErrorsBuilder.DispositivoMovilAutorizadoDuplicado })]
    public async Task GuardarDispositivoAutorizadoAsync(
        string caseName,
        int idCliente,
        string token,
        string idDispositivo,
        string nombre,
        string caracteristicas,
        bool validarDispositivo,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var dispositivo = await Facade.GuardarDispositivoAutorizadoAsync(
                idCliente: idCliente,
                token: token,
                idDispositivo: idDispositivo,
                nombre: nombre,
                caracteristicas: caracteristicas,
                creationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId
            );
            // Assert
            Assert.NotNull(dispositivo);
            // Checa las propiedades
            Assert.Equal(token, dispositivo.Token);
            Assert.Equal(idDispositivo, dispositivo.IdDispositivo);
            Assert.Equal(nombre, dispositivo.Nombre);
            Assert.Equal(caracteristicas, dispositivo.Caracteristicas);
            Assert.True(dispositivo.Actual);
            // Obtener directamente de la bd
            var dispositivoContext = await Context.DispositivoMovilAutorizado.FirstOrDefaultAsync(x => x.Id == dispositivo.Id);
            // Valida que existe en bd
            Assert.NotNull(dispositivoContext);
            // Compara las propiedades
            Assert.Equal(token, dispositivoContext.Token);
            Assert.Equal(idDispositivo, dispositivoContext.IdDispositivo);
            Assert.Equal(nombre, dispositivoContext.Nombre);
            Assert.Equal(caracteristicas, dispositivoContext.Caracteristicas);
            Assert.True(dispositivoContext.Actual);
            // Checa si es el dispositivo movil autroizado
            if (validarDispositivo)
            {
                // Es dispositivo movil autorizado
                var esDispositivo = await Facade.EsDispositivoAutorizadoAsync(
                    idCliente: idCliente, idDispositivo: idDispositivo, token: token);
                // Valida que es el dispositivo moveil autorizado
                Assert.True(esDispositivo);
                // Valida cuando no es dispositivo movil autorizado
                esDispositivo = await Facade.EsDispositivoAutorizadoAsync(
                    idCliente: idCliente, idDispositivo: idDispositivo, token: "token invalido");
                // No es el dispositivo movil autorizado
                Assert.False(esDispositivo);
            }
            // Assert the success
            Assert.True(condition:success, userMessage: "Should not reach on failures.");
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