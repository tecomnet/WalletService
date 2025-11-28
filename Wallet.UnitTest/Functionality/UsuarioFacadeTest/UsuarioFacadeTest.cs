using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;
using Moq;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;

namespace Wallet.UnitTest.Functionality.UsuarioFacadeTest;

public class UsuarioFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IUsuarioFacade>(setupConfig: setupConfig)
{
    [Theory]
    // Successfully case
    [InlineData(data: ["1. Successfully case, create password", 1, "Password123!", true, new string[] { }])]
    // Wrong cases
    [InlineData(data:
    [
        "2. Wrong case, user not found", 99, "Password123!", false,
        new string[] { ServiceErrorsBuilder.UsuarioNoEncontrado }
    ])]
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
            Assert.NotNull(@object: usuario);
            // Assert password set (we can't check the hash directly easily, but we can check it's not null)
            Assert.NotNull(@object: usuario.Contrasena);

            // Get the user from context
            var usuarioContext = await Context.Usuario.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == idUsuario);
            // Confirm user updated in context
            Assert.NotNull(@object: usuarioContext);
            Assert.NotNull(@object: usuarioContext.Contrasena);

            // Assert successful test
            Assert.True(condition: success);
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
            Assert.Fail(message: $"Uncaught exception. {exception.Message}");
        }
    }

    [Theory]
    // Successfully case
    [InlineData(data: ["1. Successfully case, update email", 1, "newemail@test.com", true, new string[] { }])]
    // Wrong cases
    [InlineData(data:
    [
        "2. Wrong case, user not found", 99, "newemail@test.com", false,
        new string[] { ServiceErrorsBuilder.UsuarioNoEncontrado }
    ])]
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
            TwilioServiceFacadeMock.Setup(expression: x =>
                    x.VerificacionEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(value: new VerificacionResult { Sid = "SID123", IsVerified = true });

            // Call facade method
            var usuario = await Facade.ActualizarCorreoElectronicoAsync(
                idUsuario: idUsuario,
                correoElectronico: correoElectronico,
                modificationUser: SetupConfig.UserId);

            // Assert user updated
            Assert.NotNull(@object: usuario);
            Assert.Equal(expected: correoElectronico, actual: usuario.CorreoElectronico);

            // Get the user from context
            var usuarioContext = await Context.Usuario.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == idUsuario);
            // Confirm user updated in context
            Assert.NotNull(@object: usuarioContext);
            Assert.Equal(expected: correoElectronico, actual: usuarioContext.CorreoElectronico);

            // Assert successful test
            Assert.True(condition: success);
        }
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            Assert.Fail(message: $"Uncaught exception. {exception.Message}");
        }
    }

    [Theory]
    // Successfully case
    [InlineData(data:
        ["1. Successfully case, create usuario preregistro", "+52", "5525212560", true, new string[] { }])]
    [InlineData(data:
        ["1. Successfully case, create usuario preregistro", "+52", "9818523698", true, new string[] { }])]
    // Wrong cases
    [InlineData(data:
    [
        "2. Wrong case, usuario already exists", "+52", "9812078573", false,
        new string[] { ServiceErrorsBuilder.ClienteDuplicado }
    ])]
    public async Task GuardarUsuarioPreRegistroTest(
        string caseName,
        string codigoPais,
        string telefono,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Setup mocks
            TwilioServiceFacadeMock.Setup(expression: x => x.VerificacionSMS(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(value: new VerificacionResult { Sid = "SID123", IsVerified = true });

            // Call facade method
            var usuario = await Facade.GuardarUsuarioPreRegistroAsync(
                codigoPais: codigoPais,
                telefono: telefono,
                creationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId);

            // Assert user created
            Assert.NotNull(@object: usuario);
            // Assert user properties
            Assert.True(condition: usuario.CodigoPais == codigoPais &&
                                   usuario.Telefono == telefono &&
                                   usuario.CreationUser == SetupConfig.UserId);

            // Get the user from context
            var usuarioContext = await Context.Usuario.Include(navigationPropertyPath: x => x.Cliente).AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == usuario.Id);

            // Confirm user created in context
            Assert.NotNull(@object: usuarioContext);
            // Assert user properties
            Assert.True(condition: usuarioContext.CodigoPais == codigoPais &&
                                   usuarioContext.Telefono == telefono &&
                                   usuarioContext.CreationUser == SetupConfig.UserId);

            // Assert associated cliente created
            Assert.NotNull(@object: usuarioContext.Cliente);
            Assert.True(condition: usuarioContext.Cliente.CreationUser == SetupConfig.UserId);

            // Assert successful test
            Assert.True(condition: success);
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
            Assert.Fail(message: $"Uncaught exception. {exception.Message}");
        }
    }
}
