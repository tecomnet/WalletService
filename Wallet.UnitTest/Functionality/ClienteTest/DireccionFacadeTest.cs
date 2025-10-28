using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ClienteTest;

public class DireccionFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IDireccionFacade>(setupConfig) 
{
     [Theory]
    // Successfully case
    [InlineData("1. Caso ok, guarda direccion preregistro cliente", 1,"Mexico", "Campeche", false, null, null, null, null, null, null, null,
        true, new string[] { })]
    [InlineData("2. Caso ok, guarda direccion preregistro cliente y la actualiza por los datos completos", 1,"Mexico", "Campeche", true, "24000", "Carmen", "San Roman", "Calle 1", "123", "456", "Referencia 1", 
        true, new string[] { })]
    // Wrong cases
    [InlineData("3. Caso error, guarda direccion preregistro cliente, pero el estado no existe", 1,"Mexico", "Quintana Roo", false, "24000", "Carmen", "San Roman", "Calle 1", "123", "456", "Referencia 1", 
        false, new string[] { ServiceErrorsBuilder.EstadoNoEncontrado})]
    [InlineData("4. Caso error, cliente no encontrado", 20,"Mexico", "Quintana Roo", false, "24000", "Carmen", "San Roman", "Calle 1", "123", "456", "Referencia 1", 
        false, new string[] { ServiceErrorsBuilder.ClienteNoEncontrado})]
    [InlineData("5. Caso error, guarda direccion preregistro cliente, pero el estado no existe", 1,"Mexico", "Aguascalientes", true, "24000", "Carmen", "San Roman", "Calle 1", "123", "456", "Referencia 1", 
        false, new string[] { ServiceErrorsBuilder.DireccionNoConfigurada})]
    public async Task GuardarYActualizarDireccionClienteTest(
        string caseName,
        int idCliente,
        string pais,
        string estado,
        bool aplicaActualizacion,
        string codigoPostal,
        string municipio,
        string colonia,
        string calle,
        string numeroExterior,
        string numeroInterior,
        string referencia,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            if (!expectedErrors.Contains(ServiceErrorsBuilder.DireccionNoConfigurada))
            {
                // Call facade method
                var direccion = await Facade.AgregarDireccionClientePreRegistro(
                    idCliente: idCliente,
                    pais: pais,
                    estado: estado,
                    creationUser: SetupConfig.UserId,
                    testCase: SetupConfig.TestCaseId);
                // Assert user created
                Assert.NotNull(direccion);
                // Assert user properties
                Assert.True(direccion.Cliente.Id == idCliente &&
                            direccion.Pais == pais &&
                            direccion.Estado == estado &&
                            direccion.ModificationUser == SetupConfig.UserId);
                // Get the user from context
                var direccionContext = await Context.Direccion.Include(x => x.Cliente).AsNoTracking().FirstOrDefaultAsync(x => x.Id == direccion.Id);
                // Confirm user created in context
                Assert.NotNull(direccionContext);
                // Assert user properties
                Assert.True(direccionContext.Cliente.Id == idCliente &&
                            direccionContext.Pais == pais &&
                            direccionContext.Estado == estado &&
                            direccionContext.ModificationUser == SetupConfig.UserId);
            }
            if (aplicaActualizacion)
            {
                // Call facade method
                var direccion = await Facade.ActualizarDireccionCliente(
                    idCliente: idCliente,
                    codigoPostal: codigoPostal,
                    municipio: municipio,
                    colonia: colonia,
                    calle: calle,
                    numeroExterior: numeroExterior,
                    numeroInterior: numeroInterior,
                    referencia: referencia,
                    modificationUser: SetupConfig.UserId);
                // Assert user created
                Assert.NotNull(direccion);
                // Assert user properties
                Assert.True(direccion.Id == idCliente &&
                            direccion.CodigoPostal == codigoPostal &&
                            direccion.Municipio == municipio &&
                            direccion.Colonia == colonia &&
                            direccion.Calle == calle &&
                            direccion.NumeroExterior == numeroExterior &&
                            direccion.NumeroInterior == numeroInterior &&
                            direccion.Referencia == referencia &&
                            direccion.ModificationUser == SetupConfig.UserId);
                // Get the user from context
                var direccionContext = await Context.Direccion.Include(x => x.Cliente).AsNoTracking().FirstOrDefaultAsync(x => x.Id == direccion.Id);
                // Confirm user created in context
                Assert.NotNull(direccionContext);
                // Assert user properties
                Assert.True(direccionContext.Id == idCliente &&
                            direccionContext.CodigoPostal == codigoPostal &&
                            direccionContext.Municipio == municipio &&
                            direccionContext.Colonia == colonia &&
                            direccionContext.Calle == calle &&
                            direccionContext.NumeroExterior == numeroExterior &&
                            direccionContext.NumeroInterior == numeroInterior &&
                            direccionContext.Referencia == referencia &&
                            direccionContext.ModificationUser == SetupConfig.UserId);
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