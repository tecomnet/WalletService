using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ProveedorFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ProveedorFacadeTest;

public class ProveedorFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IProveedorFacade>(setupConfig: setupConfig)
{
    [Theory]
    // Successfully case
    [InlineData(data: ["1. Successfully case, create proveedor", "Netflix", Categoria.Servicios, 1, true, new string[] { }])]
    // Wrong cases
    [InlineData(data:
    [
        "2. Wrong case, empty name", "", Categoria.Servicios, 1, false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    public async Task GuardarProveedorAsyncTest(
        string caseName,
        string nombre,
        Categoria categoria,
        int brokerId,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Call facade method
            var proveedor = await Facade.GuardarProveedorAsync(
                nombre: nombre,
                urlIcono: "https://example.com/icon.png",
                brokerId: brokerId,
                categoria: categoria,
                creationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId);

            // Assert proveedor created
            Assert.NotNull(@object: proveedor);
            // Assert properties
            Assert.True(condition: proveedor.Nombre == nombre &&
                                   proveedor.BrokerId == brokerId &&
                                   proveedor.Categoria == categoria &&
                                   proveedor.CreationUser == SetupConfig.UserId);

            // Get from context
            var proveedorContext = await Context.Proveedor.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == proveedor.Id);
            Assert.NotNull(@object: proveedorContext);
            Assert.True(condition: proveedorContext.Nombre == nombre &&
                                   proveedorContext.BrokerId == brokerId &&
                                   proveedorContext.Categoria == categoria &&
                                   proveedorContext.CreationUser == SetupConfig.UserId);

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
    [InlineData(data: ["1. Successfully case, update proveedor", 1, "CFE Updated", Categoria.Servicios, true, new string[] { }])]
    // Wrong cases
    [InlineData(data:
    [
        "2. Wrong case, not found", 99, "Name", Categoria.Servicios, false, new string[] { ServiceErrorsBuilder.ProveedorNoEncontrado }
    ])] // CHECK ERROR CODE
    public async Task ActualizarProveedorAsyncTest(
        string caseName,
        int idProveedor,
        string nombre,
        Categoria categoria,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Get existing token
            var existingProveedor =
                await Context.Proveedor.AsNoTracking().FirstOrDefaultAsync(predicate: p => p.Id == idProveedor);
            var token = Convert.ToBase64String(inArray: existingProveedor?.ConcurrencyToken ?? new byte[] { });

            var proveedor = await Facade.ActualizarProveedorAsync(
                idProveedor: idProveedor,
                nombre: nombre,
                categoria: categoria,
                urlIcono: "https://example.com/icon.png",
                concurrencyToken: token,
                modificationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId);

            Assert.NotNull(@object: proveedor);
            Assert.True(condition: proveedor.Nombre == nombre &&
                                   proveedor.ModificationUser == SetupConfig.UserId);

            var proveedorContext = await Context.Proveedor.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == proveedor.Id);
            Assert.NotNull(@object: proveedorContext);
            Assert.True(condition: proveedorContext.Nombre == nombre &&
                                   proveedorContext.ModificationUser == SetupConfig.UserId);

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
    [InlineData(data: ["1. Successfully case, delete proveedor", 1, true, new string[] { }])]
    [InlineData(data:
    [
        "2. Wrong case, not found", 99, false, new string[] { ServiceErrorsBuilder.ProveedorNoEncontrado }
    ])] // CHECK ERROR CODE
    public async Task EliminarProveedorAsyncTest(
        string caseName,
        int idProveedor,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var proveedor =
                await Facade.EliminarProveedorAsync(idProveedor: idProveedor, modificationUser: SetupConfig.UserId);
            Assert.NotNull(@object: proveedor);
            Assert.False(condition: proveedor.IsActive);

            var proveedorContext = await Context.Proveedor.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == proveedor.Id);
            Assert.NotNull(@object: proveedorContext);
            Assert.False(condition: proveedorContext.IsActive);

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
    [InlineData(data: ["1. Successfully case, activate proveedor", 1, true, new string[] { }])]
    [InlineData(data:
    [
        "2. Wrong case, not found", 99, false, new string[] { ServiceErrorsBuilder.ProveedorNoEncontrado }
    ])] // CHECK ERROR CODE
    public async Task ActivarProveedorAsyncTest(
        string caseName,
        int idProveedor,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var proveedor =
                await Facade.ActivarProveedorAsync(idProveedor: idProveedor, modificationUser: SetupConfig.UserId);
            Assert.NotNull(@object: proveedor);
            Assert.True(condition: proveedor.IsActive);

            var proveedorContext = await Context.Proveedor.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == proveedor.Id);
            Assert.NotNull(@object: proveedorContext);
            Assert.True(condition: proveedorContext.IsActive);

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
