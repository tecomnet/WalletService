using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;
using Moq;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;
using Wallet.Funcionalidad.Remoting.REST.ChecktonPldManagement;
using Wallet.DOM.Modelos;

namespace Wallet.UnitTest.Functionality.ClienteFacadeTest;

public class ClienteFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IClienteFacade>(setupConfig: setupConfig)
{
    [Theory]
    // Successfully case
    [InlineData(data:
    [
        "1. Successfully case, se actualiza el cliente", 1, "Edilberto", "Martinez", "Diaz", "Campeche", "1991-01-01",
        Genero.Masculino, true, new string[] { }
    ])]
    // Wrong cases
    [InlineData(data:
    [
        "2. Caso de error, el cliente no existe por id", 25, "Nombre", "Apellido", "Apellido", "Aguascalientes",
        "2000-01-01", Genero.Masculino, false, new string[] { ServiceErrorsBuilder.ClienteNoEncontrado }
    ])]
    [InlineData(data:
    [
        "3. Caso de error, el estado no existe", 1, "Nombre", "Apellido", "Apellido", "Merida", "2000-01-01",
        Genero.Masculino, false, new string[] { ServiceErrorsBuilder.EstadoNoEncontrado }
    ])]
    public async Task ActualizarDatosPersonalesTest(
        string caseName,
        int idCliente,
        string nombre,
        string primerApellido,
        string segundoApellido,
        string nombreEstado,
        string fechaNacimiento,
        Genero genero,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Setup mocks
            ChecktonPldServiceFacadeMock.Setup(expression: x => x.ValidarChecktonPld(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<Genero>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(value: new ValidacionCurpResult { Success = true, CurpGenerada = "AAAA000000HDFXXX00" });

            // Pre-requisite: Client must have verified SMS for this test
            var clientToUpdate =
                await Context.Cliente.Include(navigationPropertyPath: x => x.Usuario)
                    .FirstOrDefaultAsync(predicate: x => x.Id == idCliente);
            if (clientToUpdate != null)
            {
                var verificacion = new Verificacion2FA(twilioSid: "SID_PRE_VERIFIED",
                    fechaVencimiento: DateTime.Now.AddMinutes(value: 10), tipo: Tipo2FA.Sms,
                    creationUser: SetupConfig.UserId);
                clientToUpdate.Usuario.AgregarVerificacion2Fa(verificacion: verificacion,
                    modificationUser: SetupConfig.UserId);
                clientToUpdate.Usuario.ConfirmarVerificacion2Fa(tipo: Tipo2FA.Sms, codigo: "0000",
                    modificationUser: SetupConfig.UserId);
                await Context.SaveChangesAsync();
                // Detach to ensure Facade fetches fresh data if needed, though same context should track changes.
                Context.Entry(entity: clientToUpdate).State = EntityState.Detached;
            }

            // Convierte la fecha nacimiento a date only
            DateOnly fechaNacimientoDateOnly = DateOnly.Parse(s: fechaNacimiento);
            // Call facade method
            var cliente = await Facade.ActualizarClienteDatosPersonalesAsync(
                idCliente: idCliente,
                nombre: nombre,
                primerApellido: primerApellido,
                segundoApellido: segundoApellido,
                nombreEstado: nombreEstado,
                fechaNacimiento: fechaNacimientoDateOnly,
                genero: genero,
                modificationUser: SetupConfig.UserId);
            // Assert user created
            Assert.NotNull(cliente);
            // Assert user properties
            Assert.True(condition: cliente.Nombre == nombre &&
                                   cliente.PrimerApellido == primerApellido &&
                                   cliente.SegundoApellido == segundoApellido &&
                                   cliente.Estado!.Nombre == nombreEstado &&
                                   cliente.FechaNacimiento == fechaNacimientoDateOnly &&
                                   cliente.Genero == genero &&
                                   cliente.ModificationUser == SetupConfig.UserId);
            // Get the user from context
            var clienteContext = await Context.Cliente.Include(navigationPropertyPath: x => x.Estado).AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == cliente.Id);
            // Confirm user created in context
            Assert.NotNull(clienteContext);
            // Assert user properties
            Assert.True(condition: clienteContext.Nombre == nombre &&
                                   clienteContext.PrimerApellido == primerApellido &&
                                   clienteContext.SegundoApellido == segundoApellido &&
                                   clienteContext.Estado!.Nombre == nombreEstado &&
                                   clienteContext.FechaNacimiento == fechaNacimientoDateOnly &&
                                   clienteContext.Genero == genero &&
                                   clienteContext.ModificationUser == SetupConfig.UserId);
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
    [InlineData(data: ["1. Successfully case, elimina cliente", 1, true, new string[] { }])]
    // Wrong cases
    [InlineData(data:
    [
        "2. Caso error, el cliente no existe por id", 25, false,
        new string[] { ServiceErrorsBuilder.ClienteNoEncontrado }
    ])]
    public async Task EliminarClienteTest(
        string caseName,
        int idCliente,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Call facade method
            var cliente =
                await Facade.EliminarClienteAsync(idCliente: idCliente, modificationUser: SetupConfig.UserId);
            // Assert cliente returned
            Assert.NotNull(cliente);
            // Assert cliente properties
            Assert.False(condition: cliente.IsActive);
            Assert.True(condition: cliente.ModificationUser == SetupConfig.UserId);
            // Get the cliente from context
            var clienteContext =
                await Context.Cliente.AsNoTracking().FirstOrDefaultAsync(predicate: x => x.Id == cliente.Id);
            // Confirm cliente updated in context
            Assert.NotNull(clienteContext);
            // Assert cliente persisted as deactivated
            Assert.False(condition: clienteContext.IsActive);
            Assert.True(condition: clienteContext.ModificationUser == SetupConfig.UserId);
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
    [InlineData(data: ["1. Successfully case, activa cliente", 1, true, new string[] { }])]
    // Wrong cases
    [InlineData(data:
    [
        "2. Caso error, el cliente no existe por id", 25, false,
        new string[] { ServiceErrorsBuilder.ClienteNoEncontrado }
    ])]
    public async Task ActivarClienteTest(
        string caseName,
        int idCliente,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Call facade method
            var cliente =
                await Facade.ActivarClienteAsync(idCliente: idCliente, modificationUser: SetupConfig.UserId);
            // Assert cliente returned
            Assert.NotNull(cliente);
            // Assert cliente properties
            Assert.True(condition: cliente.IsActive);
            Assert.True(condition: cliente.ModificationUser == SetupConfig.UserId);
            // Get the cliente from context
            var clienteContext =
                await Context.Cliente.AsNoTracking().FirstOrDefaultAsync(predicate: x => x.Id == cliente.Id);
            // Confirm cliente updated in context
            Assert.NotNull(clienteContext);
            // Assert cliente persisted as activated
            Assert.True(condition: clienteContext.IsActive);
            Assert.True(condition: clienteContext.ModificationUser == SetupConfig.UserId);
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


