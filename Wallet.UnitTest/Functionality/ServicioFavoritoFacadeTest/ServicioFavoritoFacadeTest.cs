using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ServicioFavoritoFacadeTest;

public class ServicioFavoritoFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IServicioFavoritoFacade>(setupConfig)
{
    [Theory]
    // Successfully case
    [InlineData("1. Successfully case, create servicio favorito", 1, 1, "Mi Internet", "987654321", true,
        new string[] { })]
    // Wrong cases
    [InlineData("2. Wrong case, empty alias", 1, 1, "", "987654321", false,
        new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("3. Wrong case, Cliente not found", 99, 1, "Mi Internet", "987654321", false,
        new string[] { "CLIENTE-NO-ENCONTRADO" })]
    [InlineData("4. Wrong case, ProveedorServicio not found", 1, 99, "Mi Internet", "987654321", false,
        new string[] { "PROVEEDOR-SERVICIO-NOT-FOUND" })]
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
                proveedorServicioId: proveedorId,
                alias: alias,
                numeroReferencia: numeroReferencia,
                creationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId);

            Assert.NotNull(servicio);
            Assert.True(servicio.ClienteId == clienteId &&
                        servicio.ProveedorServicioId == proveedorId &&
                        servicio.Alias == alias &&
                        servicio.NumeroReferencia == numeroReferencia &&
                        servicio.CreationUser == SetupConfig.UserId);

            var servicioContext =
                await Context.ServicioFavorito.AsNoTracking().FirstOrDefaultAsync(x => x.Id == servicio.Id);
            Assert.NotNull(servicioContext);
            Assert.True(servicioContext.Alias == alias);

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

    [Theory]
    [InlineData("1. Successfully case, update servicio favorito", 1, "Mi Luz Updated", "111111", true,
        new string[] { })]
    [InlineData("2. Wrong case, not found", 99, "Alias", "Ref", false, new string[] { "SERVICIO-FAVORITO-NOT-FOUND" })]
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
                modificationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId);

            Assert.NotNull(servicio);
            Assert.True(servicio.Alias == alias &&
                        servicio.NumeroReferencia == numeroReferencia &&
                        servicio.ModificationUser == SetupConfig.UserId);

            var servicioContext =
                await Context.ServicioFavorito.AsNoTracking().FirstOrDefaultAsync(x => x.Id == servicio.Id);
            Assert.NotNull(servicioContext);
            Assert.True(servicioContext.Alias == alias);

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

    [Theory]
    [InlineData("1. Successfully case, delete servicio favorito", 1, true, new string[] { })]
    [InlineData("2. Wrong case, not found", 99, false, new string[] { "SERVICIO-FAVORITO-NOT-FOUND" })]
    public async Task EliminarServicioFavoritoTest(
        string caseName,
        int idServicio,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var servicio = await Facade.EliminarServicioFavoritoAsync(idServicio, SetupConfig.UserId);
            Assert.NotNull(servicio);
            Assert.False(servicio.IsActive);

            var servicioContext =
                await Context.ServicioFavorito.AsNoTracking().FirstOrDefaultAsync(x => x.Id == servicio.Id);
            Assert.NotNull(servicioContext);
            Assert.False(servicioContext.IsActive);

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
