using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ServicioFavoritoFacadeTest;

public class ServicioFavoritoFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IServicioFavoritoFacade>(setupConfig: setupConfig)
{
    [Theory]
    // Successfully case
    [InlineData(data:
        ["1. Successfully case, create servicio favorito", 1, 1, "Mi Internet", "987654321", true, new string[] { }])]
    // Wrong cases
    [InlineData(data:
    [
        "2. Wrong case, empty alias", 1, 1, "", "987654321", false,
        new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "3. Wrong case, Cliente not found", 99, 1, "Mi Internet", "987654321", false,
        new string[] { ServiceErrorsBuilder.ClienteNoEncontrado }
    ])]
    [InlineData(data:
    [
        "4. Wrong case, Proveedor not found", 1, 99, "Mi Internet", "987654321", false,
        new string[] { ServiceErrorsBuilder.ProveedorNoEncontrado }
    ])]
    public async Task GuardarServicioFavoritoTest(
        string caseName,
        int clienteId,
        int proveedorId,
        string alias,
        string numeroReferencia,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var servicio = await Facade.GuardarServicioFavoritoAsync(
                clienteId: clienteId,
                proveedorId: proveedorId,
                alias: alias,
                numeroReferencia: numeroReferencia,
                creationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId);

            Assert.NotNull(@object: servicio);
            Assert.True(condition: servicio.ClienteId == clienteId &&
                                   servicio.ProveedorId == proveedorId &&
                                   servicio.Alias == alias &&
                                   servicio.NumeroReferencia == numeroReferencia &&
                                   servicio.CreationUser == SetupConfig.UserId);

            var servicioContext =
                await Context.ServicioFavorito.AsNoTracking().FirstOrDefaultAsync(predicate: x => x.Id == servicio.Id);
            Assert.NotNull(@object: servicioContext);
            Assert.True(condition: servicioContext.Alias == alias);

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
    [InlineData(data:
        ["1. Successfully case, update servicio favorito", 1, "Mi Luz Updated", "111111", true, new string[] { }])]
    [InlineData(data:
    [
        "2. Wrong case, not found", 99, "Alias", "Ref", false,
        new string[] { ServiceErrorsBuilder.ServicioFavoritoNoEncontrado }
    ])]
    public async Task ActualizarServicioFavoritoTest(
        string caseName,
        int idServicio,
        string alias,
        string numeroReferencia,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var servicio = await Facade.ActualizarServicioFavoritoAsync(
                idServicioFavorito: idServicio,
                alias: alias,
                numeroReferencia: numeroReferencia,
                concurrencyToken: null,
                modificationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId);

            Assert.NotNull(@object: servicio);
            Assert.True(condition: servicio.Alias == alias &&
                                   servicio.NumeroReferencia == numeroReferencia &&
                                   servicio.ModificationUser == SetupConfig.UserId);

            var servicioContext =
                await Context.ServicioFavorito.AsNoTracking().FirstOrDefaultAsync(predicate: x => x.Id == servicio.Id);
            Assert.NotNull(@object: servicioContext);
            Assert.True(condition: servicioContext.Alias == alias);

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
    [InlineData(data: ["1. Successfully case, delete servicio favorito", 1, true, new string[] { }])]
    [InlineData(data:
        ["2. Wrong case, not found", 99, false, new string[] { ServiceErrorsBuilder.ServicioFavoritoNoEncontrado }])]
    public async Task EliminarServicioFavoritoTest(
        string caseName,
        int idServicio,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var servicio = await Facade.EliminarServicioFavoritoAsync(idServicioFavorito: idServicio,
                modificationUser: SetupConfig.UserId);
            Assert.NotNull(@object: servicio);
            Assert.False(condition: servicio.IsActive);

            var servicioContext =
                await Context.ServicioFavorito.AsNoTracking().FirstOrDefaultAsync(predicate: x => x.Id == servicio.Id);
            Assert.NotNull(@object: servicioContext);
            Assert.False(condition: servicioContext.IsActive);

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
