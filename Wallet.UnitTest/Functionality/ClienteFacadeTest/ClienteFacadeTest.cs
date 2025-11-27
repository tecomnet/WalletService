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
    : BaseFacadeTest<IClienteFacade>(setupConfig)
{
    [Theory]
    // Successfully case
    [InlineData("1. Successfully case, create cliente", "+52", "5525212560", true, new string[] { })]
    [InlineData("1. Successfully case, create cliente", "+52", "9818523698", true, new string[] { })]
    // Wrong cases
    [InlineData("2. Wrong case, cliente already exists", "+52", "9812078573", false,
        new string[] { ServiceErrorsBuilder.ClienteDuplicado })]
    public async Task GuardarPreRegistroClienteTest(
        string caseName,
        string codigoPais,
        string telefono,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Setup mocks
            TwilioServiceFacadeMock.Setup(x => x.VerificacionSMS(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new VerificacionResult { Sid = "SID123", IsVerified = true });

            // Call facade method
            var cliente = await Facade.GuardarClientePreRegistroAsync(
                creationUser: SetupConfig.UserId,
                codigoPais: codigoPais,
                telefono: telefono,
                testCase: SetupConfig.TestCaseId);
            // Assert user created
            Assert.NotNull(cliente);
            // Assert user properties
            Assert.True(cliente.Usuario.CodigoPais == codigoPais &&
                        cliente.Usuario.Telefono == telefono &&
                        cliente.CreationUser == SetupConfig.UserId);
            // Get the user from context
            var clienteContext = await Context.Cliente.Include(x => x.Usuario).AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == cliente.Id);
            // Confirm user created in context
            Assert.NotNull(clienteContext);
            // Assert user properties
            Assert.True(clienteContext.Usuario.CodigoPais == codigoPais &&
                        clienteContext.Usuario.Telefono == telefono &&
                        clienteContext.CreationUser == SetupConfig.UserId);
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
    [InlineData("1. Successfully case, se actualiza el cliente", 1, "Edilberto", "Martinez", "Diaz", "Campeche",
        "1991-01-01", Genero.Masculino,
        true, new string[] { })]
    // Wrong cases
    [InlineData("2. Caso de error, el cliente no existe por id", 25, "Nombre", "Apellido", "Apellido", "Aguascalientes",
        "2000-01-01", Genero.Masculino,
        false, new string[] { ServiceErrorsBuilder.ClienteNoEncontrado })]
    [InlineData("3. Caso de error, el estado no existe", 1, "Nombre", "Apellido", "Apellido", "Merida", "2000-01-01",
        Genero.Masculino,
        false, new string[] { ServiceErrorsBuilder.EstadoNoEncontrado })]
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
            ChecktonPldServiceFacadeMock.Setup(x => x.ValidarChecktonPld(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<Genero>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(new ValidacionCurpResult { Success = true, CurpGenerada = "AAAA000000HDFXXX00" });

            // Pre-requisite: Client must have verified SMS for this test
            var clientToUpdate =
                await Context.Cliente.Include(x => x.Usuario).FirstOrDefaultAsync(x => x.Id == idCliente);
            if (clientToUpdate != null)
            {
                var verificacion = new Verificacion2FA("SID_PRE_VERIFIED", DateTime.Now.AddMinutes(10), Tipo2FA.Sms,
                    SetupConfig.UserId);
                clientToUpdate.Usuario.AgregarVerificacion2FA(verificacion, SetupConfig.UserId);
                clientToUpdate.Usuario.ConfirmarVerificacion2FA(Tipo2FA.Sms, "0000", SetupConfig.UserId);
                await Context.SaveChangesAsync();
                // Detach to ensure Facade fetches fresh data if needed, though same context should track changes.
                Context.Entry(clientToUpdate).State = EntityState.Detached;
            }

            // Convierte la fecha nacimiento a date only
            DateOnly fechaNacimientoDateOnly = DateOnly.Parse(fechaNacimiento);
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
            Assert.True(cliente.Nombre == nombre &&
                        cliente.PrimerApellido == primerApellido &&
                        cliente.SegundoApellido == segundoApellido &&
                        cliente.Estado!.Nombre == nombreEstado &&
                        cliente.FechaNacimiento == fechaNacimientoDateOnly &&
                        cliente.Genero == genero &&
                        cliente.ModificationUser == SetupConfig.UserId);
            // Get the user from context
            var clienteContext = await Context.Cliente.Include(x => x.Estado).AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == cliente.Id);
            // Confirm user created in context
            Assert.NotNull(clienteContext);
            // Assert user properties
            Assert.True(clienteContext.Nombre == nombre &&
                        clienteContext.PrimerApellido == primerApellido &&
                        clienteContext.SegundoApellido == segundoApellido &&
                        clienteContext.Estado!.Nombre == nombreEstado &&
                        clienteContext.FechaNacimiento == fechaNacimientoDateOnly &&
                        clienteContext.Genero == genero &&
                        clienteContext.ModificationUser == SetupConfig.UserId);
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
    [InlineData("1. Successfully case, guarda contrasena cliente", 1, "micontrasena", false, "nuevacontrasena",
        "nuevacontrasena",
        true, new string[] { })]
    [InlineData("1.1. Successfully case, guarda y actualiza contrasena cliente", 1, "micontrasena", true,
        "nuevacontrasena", "nuevacontrasena",
        true, new string[] { })]
    // Wrong cases
    [InlineData("2. Caso error, cliente no encontrado", 25, "micontrasena", false, "nuevacontrasena",
        "nuevacontrasena",
        false, new string[] { ServiceErrorsBuilder.ClienteNoEncontrado })]
    [InlineData("3. Caso error, la nueva contrasena con coincide con la confirmacion", 2, "micontrasena", true,
        "nuevacontrasena", "nocoincide",
        false, new string[] { ServiceErrorsBuilder.ContrasenasNoCoinciden })]
    public async Task GuardarYActualizarContrasenaClienteTest(
        string caseName,
        int idCliente,
        string contrasena,
        bool aplicaActualizacion,
        string contrasenaNueva,
        string contrasenaConfirmacion,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Call facade method
            var cliente = await Facade.GuardarContrasenaAsync(
                idCliente: idCliente,
                contrasena: contrasena,
                modificationUser: SetupConfig.UserId);
            // Assert user created
            Assert.NotNull(cliente);
            // Assert user properties
            Assert.True(cliente.Id == idCliente &&
                        cliente.Usuario.Contrasena == contrasena &&
                        cliente.Usuario.ModificationUser == SetupConfig.UserId);
            // Get the user from context
            var clienteContext = await Context.Cliente.Include(x => x.Usuario).AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == cliente.Id);
            // Confirm user created in context
            Assert.NotNull(clienteContext);
            // Assert user properties
            Assert.True(clienteContext.Id == idCliente &&
                        clienteContext.Usuario.Contrasena == contrasena &&
                        clienteContext.Usuario.ModificationUser == SetupConfig.UserId);

            if (aplicaActualizacion)
            {
                // Call facade method
                cliente = await Facade.ActualizarContrasenaAsync(
                    idCliente: idCliente,
                    contrasenaActual: cliente.Usuario.Contrasena,
                    contrasenaNueva: contrasenaNueva,
                    confirmacionContrasenaNueva: contrasenaConfirmacion,
                    modificationUser: SetupConfig.UserId);
                // Assert user created
                Assert.NotNull(cliente);
                // Assert user properties
                Assert.True(cliente.Id == idCliente &&
                            cliente.Usuario.Contrasena == contrasenaNueva &&
                            cliente.Usuario.ModificationUser == SetupConfig.UserId);
                // Get the user from context
                clienteContext = await Context.Cliente.Include(x => x.Usuario).AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == cliente.Id);
                // Confirm user created in context
                Assert.NotNull(clienteContext);
                // Assert user properties
                Assert.True(clienteContext.Id == idCliente &&
                            clienteContext.Usuario.Contrasena == contrasenaNueva &&
                            clienteContext.Usuario.ModificationUser == SetupConfig.UserId);
            }

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
    [InlineData("1. Successfully case, actualiza telefono cliente", 1, "+55", "5528631488",
        true, new string[] { })]
    // Wrong cases
    [InlineData("2. Wrong case, otro cliente con el mismo telefono", 2, "+52", "9812078573",
        false, new string[] { ServiceErrorsBuilder.ClienteDuplicado })]
    public async Task ActualizaTelefonoClienteTest(
        string caseName,
        int idCliente,
        string codigoPais,
        string telefono,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Setup mocks
            TwilioServiceFacadeMock.Setup(x => x.VerificacionSMS(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new VerificacionResult { Sid = "SID123", IsVerified = true });

            // Call facade method
            var cliente = await Facade.ActualizarTelefonoAsync(idCliente: idCliente, codigoPais: codigoPais,
                telefono: telefono, modificationUser: SetupConfig.UserId);
            // Assert user created
            Assert.NotNull(cliente);
            // Assert user properties
            Assert.True(cliente.Usuario.CodigoPais == codigoPais &&
                        cliente.Usuario.Telefono == telefono &&
                        cliente.Usuario.ModificationUser == SetupConfig.UserId);
            // Get the user from context
            var clienteContext = await Context.Cliente.Include(x => x.Usuario).AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == cliente.Id);
            // Confirm user created in context
            Assert.NotNull(clienteContext);
            // Assert user properties
            Assert.True(clienteContext.Usuario.CodigoPais == codigoPais &&
                        clienteContext.Usuario.Telefono == telefono &&
                        clienteContext.Usuario.ModificationUser == SetupConfig.UserId);
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
    [InlineData("1. Successfully case, actualiza correo electronico cliente", 2, "hola@hotmail.com",
        true, new string[] { })]
    [InlineData("2. Caso ok, actualiza correo electronico cliente de cliente pre registro", 1, "hola@hotmail.com",
        true, new string[] { })]
    // Wrong cases
    [InlineData("3. Wrong case, otro cliente con el mismo correo electronico", 1, "cliente@cliente.com",
        false, new string[] { ServiceErrorsBuilder.ClienteDuplicadoPorCorreoElectronico })]
    public async Task ActualizaCorreoElectronicoClienteTest(
        string caseName,
        int idCliente,
        string correoElectronico,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Pre-requisite: Inject duplicate client if needed
            if (expectedErrors.Contains(ServiceErrorsBuilder.ClienteDuplicadoPorCorreoElectronico))
            {
                var usuario = new Usuario("+52", "9999999999", null, null, "Activo", SetupConfig.UserId);
                var duplicateClient = new Cliente(usuario, SetupConfig.UserId);
                duplicateClient.Usuario.ActualizarCorreoElectronico(correoElectronico, SetupConfig.UserId);
                Context.Usuario.Add(usuario);
                Context.Cliente.Add(duplicateClient);
                await Context.SaveChangesAsync();
            }

            // Setup mocks
            TwilioServiceFacadeMock.Setup(x =>
                    x.VerificacionEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new VerificacionResult { Sid = "SID123", IsVerified = true });

            // Call facade method
            var cliente = await Facade.ActualizarCorreoElectronicoAsync(idCliente: idCliente,
                correoElectronico: correoElectronico, modificationUser: SetupConfig.UserId);
            // Assert user created
            Assert.NotNull(cliente);
            // Assert user properties
            Assert.True(cliente.Usuario.CorreoElectronico == correoElectronico &&
                        cliente.Usuario.ModificationUser == SetupConfig.UserId);
            // Get the user from context
            var clienteContext = await Context.Cliente.Include(x => x.Usuario).AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == cliente.Id);
            // Confirm user created in context
            Assert.NotNull(clienteContext);
            // Assert user properties
            Assert.True(clienteContext.Usuario.CorreoElectronico == correoElectronico &&
                        clienteContext.Usuario.ModificationUser == SetupConfig.UserId);
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
    [InlineData("1. Successfully case, elimina cliente", 1, true, new string[] { })]
    // Wrong cases
    [InlineData("2. Caso error, el cliente no existe por id", 25, false,
        new string[] { ServiceErrorsBuilder.ClienteNoEncontrado })]
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
            Assert.False(cliente.IsActive);
            Assert.True(cliente.ModificationUser == SetupConfig.UserId);
            // Get the cliente from context
            var clienteContext = await Context.Cliente.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cliente.Id);
            // Confirm cliente updated in context
            Assert.NotNull(clienteContext);
            // Assert cliente persisted as deactivated
            Assert.False(clienteContext.IsActive);
            Assert.True(clienteContext.ModificationUser == SetupConfig.UserId);
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
    [InlineData("1. Successfully case, activa cliente", 1, true, new string[] { })]
    // Wrong cases
    [InlineData("2. Caso error, el cliente no existe por id", 25, false,
        new string[] { ServiceErrorsBuilder.ClienteNoEncontrado })]
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
            Assert.True(cliente.IsActive);
            Assert.True(cliente.ModificationUser == SetupConfig.UserId);
            // Get the cliente from context
            var clienteContext = await Context.Cliente.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cliente.Id);
            // Confirm cliente updated in context
            Assert.NotNull(clienteContext);
            // Assert cliente persisted as activated
            Assert.True(clienteContext.IsActive);
            Assert.True(clienteContext.ModificationUser == SetupConfig.UserId);
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
}


