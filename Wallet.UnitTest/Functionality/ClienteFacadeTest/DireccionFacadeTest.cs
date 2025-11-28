using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ClienteFacadeTest;

public class DireccionFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IDireccionFacade>(setupConfig: setupConfig) 
{
     [Theory]
    // Successfully case
    [InlineData(data: ["1. Caso ok, actualiza direccion por los datos completos", 2, "24000", "Carmen", "San Roman", "Calle 1", "123", "456", "Referencia 1", true, new string[] { }])]
    // Wrong cases
    [InlineData(data: ["2. Caso error, cliente no encontrado", 20, "24000", "Carmen", "San Roman", "Calle 1", "123", "456", "Referencia 1", false, new string[] { ServiceErrorsBuilder.ClienteNoEncontrado}])]
    [InlineData(data: ["3. Caso error, guarda direccion preregistro cliente, pero el estado no existe", 1, "24000", "Carmen", "San Roman", "Calle 1", "123", "456", "Referencia 1", false, new string[] { ServiceErrorsBuilder.DireccionNoConfigurada}])]
    public async Task ActualizarDireccionClienteTest(
        string caseName,
        int idCliente,
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
            Assert.NotNull(@object: direccion);
            // Assert user properties
            Assert.True(condition: direccion.ClienteId == idCliente &&
                                   direccion.CodigoPostal == codigoPostal &&
                                   direccion.Municipio == municipio &&
                                   direccion.Colonia == colonia &&
                                   direccion.Calle == calle &&
                                   direccion.NumeroExterior == numeroExterior &&
                                   direccion.NumeroInterior == numeroInterior &&
                                   direccion.Referencia == referencia &&
                                   direccion.ModificationUser == SetupConfig.UserId);
            // Get the user from context
            var direccionContext = await Context.Direccion.Include(navigationPropertyPath: x => x.Cliente).AsNoTracking().FirstOrDefaultAsync(predicate: x => x.Id == direccion.Id);
            // Confirm user created in context
            Assert.NotNull(@object: direccionContext);
            // Assert user properties
            Assert.True(condition: direccionContext.ClienteId == idCliente &&
                                   direccionContext.CodigoPostal == codigoPostal &&
                                   direccionContext.Municipio == municipio &&
                                   direccionContext.Colonia == colonia &&
                                   direccionContext.Calle == calle &&
                                   direccionContext.NumeroExterior == numeroExterior &&
                                   direccionContext.NumeroInterior == numeroInterior &&
                                   direccionContext.Referencia == referencia &&
                                   direccionContext.ModificationUser == SetupConfig.UserId);

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