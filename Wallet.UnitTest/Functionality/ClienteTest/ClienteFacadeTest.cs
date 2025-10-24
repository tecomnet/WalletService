using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ClienteTest;

public class ClienteFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IClienteFacade>(setupConfig) 
{
    [Theory]
    // Successfully case
    [InlineData("1. Successfully case, create cliente", "+52", "9818523698",true, new string[] { })]
    // Wrong cases
    [InlineData("2. Wrong case, cliente already exists", "+52", "9812078573", false, new string[] { ServiceErrorsBuilder.ClienteDuplicado })]
    public async Task GuardarPreRegistroClienteTest(
        string caseName,
        string codigoPais,
        string telefono,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Call facade method
            var cliente = await Facade.GuardarClientePreRegistroAsync(
                creationUser: SetupConfig.UserId,
                codigoPais: codigoPais,
                telefono: telefono,
                testCase: SetupConfig.TestCaseId);
            // Assert user created
            Assert.NotNull(cliente);
            // Assert user properties
            Assert.True(cliente.CodigoPais == codigoPais &&
                        cliente.Telefono == telefono &&
                        cliente.CreationUser == SetupConfig.UserId);
            // Get the user from context
            var clienteContext = await Context.Cliente.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cliente.Id);
            // Confirm user created in context
            Assert.NotNull(clienteContext);
            // Assert user properties
            Assert.True(clienteContext.CodigoPais == codigoPais &&
                        clienteContext.Telefono == telefono &&
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
    [InlineData("1. Successfully case, se actualiza el cliente", 1, "Edilberto", "Martinez", "Diaz", "1991-01-01", Genero.Masculino, "correo@correo.com",
        true, new string[] { })]
    // Wrong cases
    [InlineData("2. Caso de error, el cliente no existe por id", 25, "Nombre", "Apellido", "Apellido", "2000-01-01", Genero.Masculino, "correo@correo.com",
        false, new string[] { ServiceErrorsBuilder.ClienteNoEncontrado})]
    [InlineData("3. Caso de error, el correo electronico ya lo tiene otro cliente", 1, "Nombre", "Apellido", "Apellido", "2000-01-01", Genero.Masculino, "cliente@cliente.com",
        false, new string[] { ServiceErrorsBuilder.ClienteDuplicadoPorCorreoElectronico})]
    public async Task ActualizarDatosPersonalesTest(
        string caseName,
        int idCliente,
        string nombre,
        string primerApellido, 
        string segundoApellido,
        string fechaNacimiento,
        Genero genero,
        string correoElectronico,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Convierte la fecha nacimiento a date only
            DateOnly fechaNacimientoDateOnly = DateOnly.Parse(fechaNacimiento);
            // Call facade method
            var cliente = await Facade.ActualizarClienteDatosPersonalesAsync(
                idCliente: idCliente,
                nombre: nombre,
                primerApellido: primerApellido,
                segundoApellido: segundoApellido,
                fechaNacimiento: fechaNacimientoDateOnly,
                genero: genero,
                correoElectronico: correoElectronico,
                modificationUser: SetupConfig.UserId);
            // Assert user created
            Assert.NotNull(cliente);
            // Assert user properties
            Assert.True(cliente.Nombre == nombre &&
                        cliente.PrimerApellido == primerApellido &&
                        cliente.SegundoApellido == segundoApellido &&
                        cliente.FechaNacimiento == fechaNacimientoDateOnly &&
                        cliente.Genero == genero &&
                        cliente.CorreoElectronico == correoElectronico &&
                        cliente.ModificationUser == SetupConfig.UserId);
            // Get the user from context
            var clienteContext = await Context.Cliente.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cliente.Id);
            // Confirm user created in context
            Assert.NotNull(clienteContext);
            // Assert user properties
            Assert.True(clienteContext.Nombre == nombre &&
                        clienteContext.PrimerApellido == primerApellido &&
                        clienteContext.SegundoApellido == segundoApellido &&
                        clienteContext.FechaNacimiento == fechaNacimientoDateOnly &&
                        clienteContext.Genero == genero &&
                        clienteContext.CorreoElectronico == correoElectronico &&
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
    [InlineData("1. Successfully case, create cliente", 1,"micontrasena", false, "nuevacontrasena", "nuevacontrasena",
        true, new string[] { })]
    // Wrong cases
    [InlineData("2. Caso error, cliente no encontrado", 25,"micontrasena", false, "nuevacontrasena", "nuevacontrasena",
        false, new string[] { ServiceErrorsBuilder.ClienteNoEncontrado})]
    [InlineData("3. Caso error, la nueva contrasena con coincide con la confirmacion", 2,"micontrasena", true, "nuevacontrasena", "nocoincide",
        false, new string[] { ServiceErrorsBuilder.ContrasenasNoCoinciden})]
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
                        cliente.Contrasena == contrasena &&
                        cliente.ModificationUser == SetupConfig.UserId);
            // Get the user from context
            var clienteContext = await Context.Cliente.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cliente.Id);
            // Confirm user created in context
            Assert.NotNull(clienteContext);
            // Assert user properties
            Assert.True(clienteContext.Id == idCliente &&
                        clienteContext.Contrasena == contrasena &&
                        clienteContext.ModificationUser == SetupConfig.UserId);

            if (aplicaActualizacion)
            {
                // Call facade method
                cliente = await Facade.ActualizarContrasenaAsync(
                    idCliente: idCliente,
                    contrasenaActual: cliente.Contrasena,
                    contrasenaNueva: contrasenaNueva,
                    confirmacionContrasenaNueva: contrasenaConfirmacion,                    
                    modificationUser: SetupConfig.UserId);
                // Assert user created
                Assert.NotNull(cliente);
                // Assert user properties
                Assert.True(cliente.Id == idCliente &&
                            cliente.Contrasena == contrasenaNueva &&
                            cliente.ModificationUser == SetupConfig.UserId);
                // Get the user from context
                clienteContext = await Context.Cliente.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cliente.Id);
                // Confirm user created in context
                Assert.NotNull(clienteContext);
                // Assert user properties
                Assert.True(clienteContext.Id == idCliente &&
                            clienteContext.Contrasena == contrasenaNueva &&
                            clienteContext.ModificationUser == SetupConfig.UserId);
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

}