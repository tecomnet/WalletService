using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;
using Moq;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;
using Wallet.DOM.Modelos;

namespace Wallet.UnitTest.Functionality.UsuarioFacadeTest;

public class UsuarioFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IUsuarioFacade>(setupConfig)
{
    [Theory]
    // Successfully case
    [InlineData("1. Successfully case, create password", 1, "Password123!", true, new string[] { })]
    // Wrong cases
    [InlineData("2. Wrong case, user not found", 99, "Password123!", false,
        new string[] { ServiceErrorsBuilder.UsuarioNoEncontrado })]
    public async Task GuardarContrasenaTest(
        string caseName,
        int idUsuario,
        string contrasena,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Call facade method
            var usuario = await Facade.GuardarContrasenaAsync(
                idUsuario: idUsuario,
                contrasena: contrasena,
                modificationUser: SetupConfig.UserId);

            // Assert user updated
            Assert.NotNull(usuario);
            // Assert password set (we can't check the hash directly easily, but we can check it's not null)
            Assert.NotNull(usuario.Contrasena);
            
            // Get the user from context
            var usuarioContext = await Context.Usuario.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idUsuario);
            // Confirm user updated in context
            Assert.NotNull(usuarioContext);
            Assert.NotNull(usuarioContext.Contrasena);

            // Assert successful test
            Assert.True(success);
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

    [Theory]
    // Successfully case
    [InlineData("1. Successfully case, update email", 1, "newemail@test.com", true, new string[] { })]
    // Wrong cases
    [InlineData("2. Wrong case, user not found", 99, "newemail@test.com", false,
        new string[] { ServiceErrorsBuilder.UsuarioNoEncontrado })]
    public async Task ActualizarCorreoElectronicoTest(
        string caseName,
        int idUsuario,
        string correoElectronico,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Setup mocks
            TwilioServiceFacadeMock.Setup(x => x.VerificacionEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new VerificacionResult { Sid = "SID123", IsVerified = true });

            // Call facade method
            var usuario = await Facade.ActualizarCorreoElectronicoAsync(
                idUsuario: idUsuario,
                correoElectronico: correoElectronico,
                modificationUser: SetupConfig.UserId);

            // Assert user updated
            Assert.NotNull(usuario);
            Assert.Equal(correoElectronico, usuario.CorreoElectronico);
            
            // Get the user from context
            var usuarioContext = await Context.Usuario.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == idUsuario);
            // Confirm user updated in context
            Assert.NotNull(usuarioContext);
            Assert.Equal(correoElectronico, usuarioContext.CorreoElectronico);

            // Assert successful test
            Assert.True(success);
        }
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            Assert.Fail($"Uncaught exception. {exception.Message}");
        }
    }
}
