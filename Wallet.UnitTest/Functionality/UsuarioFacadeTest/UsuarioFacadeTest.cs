using Microsoft.EntityFrameworkCore;
using Moq;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

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
            // Setup mocks
            TokenServiceMock.Setup(expression: x =>
                    x.GenerateAccessToken(It.IsAny<IEnumerable<System.Security.Claims.Claim>>()))
                .Returns(value: "generated_token");

            // Call facade method
            var token = await Facade.GuardarContrasenaAsync(
                idUsuario: idUsuario,
                contrasena: contrasena,
                modificationUser: SetupConfig.UserId);

            // Assert token returned
            Assert.NotNull(token);
            Assert.Equal(expected: "generated_token", actual: token);

            // Get the user from context
            var usuarioContext = await Context.Usuario.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == idUsuario);
            // Confirm user updated in context
            Assert.NotNull(usuarioContext);
            Assert.NotNull(usuarioContext.Contrasena);

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

            // Fetch real concurrency token
            var userDb = await Context.Usuario.AsNoTracking().FirstOrDefaultAsync(u => u.Id == idUsuario);
            var token = userDb?.ConcurrencyToken ?? [];

            // Call facade method
            var usuario = await Facade.ActualizarCorreoElectronicoAsync(
                idUsuario: idUsuario,
                correoElectronico: correoElectronico,
                concurrencyToken: Convert.ToBase64String(token),
                modificationUser: SetupConfig.UserId);

            // Assert user updated
            Assert.NotNull(usuario);
            Assert.Equal(expected: correoElectronico, actual: usuario.CorreoElectronico);

            // Get the user from context
            var usuarioContext = await Context.Usuario.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == idUsuario);
            // Confirm user updated in context
            Assert.NotNull(usuarioContext);
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
        new string[] { ServiceErrorsBuilder.ClienteYaRegistrado }
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
                testCase: SetupConfig.TestCaseId);

            // Assert user created
            Assert.NotNull(usuario);
            // Assert user properties
            Assert.True(condition: usuario.CodigoPais == codigoPais &&
                                   usuario.Telefono == telefono);

            // Get the user from context
            var usuarioContext = await Context.Usuario
                .FirstOrDefaultAsync(predicate: x => x.Id == usuario.Id);

            // Confirm user created in context
            Assert.NotNull(usuarioContext);
            // Assert user properties
            Assert.True(condition: usuarioContext.CodigoPais == codigoPais &&
                                   usuarioContext.Telefono == telefono);
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
    [InlineData(data: ["1. Successfully case, confirm verification", 1, "1234", true, new string[] { }])]
    // Wrong cases
    [InlineData(data:
    [
        "2. Wrong case, user not found", 99, "1234", false,
        new string[] { ServiceErrorsBuilder.UsuarioNoEncontrado }
    ])]
    public async Task ConfirmarCodigoVerificacion2FATest(
        string caseName,
        int idUsuario,
        string codigoVerificacion,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Setup mocks
            TwilioServiceFacadeMock.Setup(expression: x =>
                    x.ConfirmarVerificacionSMS(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(value: new VerificacionResult { Sid = "SID123", IsVerified = true });

            TokenServiceMock.Setup(expression: x =>
                    x.GenerateAccessToken(It.IsAny<IEnumerable<System.Security.Claims.Claim>>()))
                .Returns(value: "generated_token");

            // Setup data for success case
            if (success)
            {
                var usuario = await Context.Usuario
                    .Include(navigationPropertyPath: u => u.Verificaciones2Fa)
                    .FirstOrDefaultAsync(predicate: u => u.Id == idUsuario);

                if (usuario != null)
                {
                    var verificacion = new Verificacion2FA(
                        twilioSid: "SID123",
                        fechaVencimiento: DateTime.Now.AddMinutes(value: 10),
                        tipo: Tipo2FA.Sms,
                        creationUser: SetupConfig.UserId,
                        testCase: caseName);

                    usuario.AgregarVerificacion2Fa(verificacion: verificacion, modificationUser: SetupConfig.UserId);
                    await Context.SaveChangesAsync();
                }
            }

            // Call facade method
            var result = await Facade.ConfirmarCodigoVerificacion2FAAsync(
                idUsuario: idUsuario,
                tipo2FA: Tipo2FA.Sms,
                codigoVerificacion: codigoVerificacion,
                modificationUser: SetupConfig.UserId);

            if (success)
            {
                // Assert result is true
                Assert.True(result);

                // Get the user from context
                var usuarioContext = await Context.Usuario.Include(navigationPropertyPath: x => x.Verificaciones2Fa)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(predicate: x => x.Id == idUsuario);

                // Confirm user updated in context
                Assert.NotNull(usuarioContext);
                Assert.Contains(collection: usuarioContext.Verificaciones2Fa,
                    filter: v => v.Verificado && v.Codigo == codigoVerificacion);
            }

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
}
