using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ProveedorServicioFacadeTest;

public class ProveedorServicioFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IProveedorServicioFacade>(setupConfig: setupConfig)
{
    [Theory]
    // Successfully case
    [InlineData(data: ["1. Successfully case, create proveedor", "Netflix", ProductoCategoria.Servicios, "https://netflix.com/icon.png", true, new string[] { }])]
    // Wrong cases
    // Add validation error cases if any validation exists in DOM constructor
    [InlineData(data: ["2. Wrong case, empty name", "", ProductoCategoria.Servicios, "url", false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    public async Task GuardarProveedorServicioTest(
        string caseName,
        string nombre,
        ProductoCategoria categoria,
        string urlIcono,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Call facade method
            var proveedor = await Facade.GuardarProveedorServicioAsync(
                nombre: nombre,
                categoria: categoria,
                urlIcono: urlIcono,
                creationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId);

            // Assert proveedor created
            Assert.NotNull(proveedor);
            // Assert properties
            Assert.True(condition: proveedor.Nombre == nombre &&
                                   proveedor.Categoria == categoria &&
                                   proveedor.UrlIcono == urlIcono &&
                                   proveedor.CreationUser == SetupConfig.UserId);

            // Get from context
            var proveedorContext = await Context.ProveedorServicio.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == proveedor.Id);
            Assert.NotNull(proveedorContext);
            Assert.True(condition: proveedorContext.Nombre == nombre &&
                                   proveedorContext.Categoria == categoria &&
                                   proveedorContext.UrlIcono == urlIcono &&
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
    [InlineData(data: ["1. Successfully case, update proveedor", 1, "CFE Updated", ProductoCategoria.Movilidad, "new_url", true, new string[] { }])]
    // Wrong cases
    [InlineData(data: ["2. Wrong case, not found", 99, "Name", ProductoCategoria.Servicios, "url", false, new string[] { "PROVEEDOR-SERVICIO-NOT-FOUND" }])]
    public async Task ActualizarProveedorServicioTest(
        string caseName,
        int idProveedor,
        string nombre,
        ProductoCategoria categoria,
        string urlIcono,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var proveedor = await Facade.ActualizarProveedorServicioAsync(
                idProveedorServicio: idProveedor,
                nombre: nombre,
                categoria: categoria,
                urlIcono: urlIcono,
                modificationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId);

            Assert.NotNull(proveedor);
            Assert.True(condition: proveedor.Nombre == nombre &&
                                   proveedor.Categoria == categoria &&
                                   proveedor.UrlIcono == urlIcono &&
                                   proveedor.ModificationUser == SetupConfig.UserId);

            var proveedorContext = await Context.ProveedorServicio.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == proveedor.Id);
            Assert.NotNull(proveedorContext);
            Assert.True(condition: proveedorContext.Nombre == nombre &&
                                   proveedorContext.Categoria == categoria &&
                                   proveedorContext.UrlIcono == urlIcono &&
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
    [InlineData(data: ["2. Wrong case, not found", 99, false, new string[] { "PROVEEDOR-SERVICIO-NOT-FOUND" }])]
    public async Task EliminarProveedorServicioTest(
        string caseName,
        int idProveedor,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var proveedor = await Facade.EliminarProveedorServicioAsync(idProveedorServicio: idProveedor, modificationUser: SetupConfig.UserId);
            Assert.NotNull(proveedor);
            Assert.False(condition: proveedor.IsActive);

            var proveedorContext = await Context.ProveedorServicio.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == proveedor.Id);
            Assert.NotNull(proveedorContext);
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
    [InlineData(data: ["2. Wrong case, not found", 99, false, new string[] { "PROVEEDOR-SERVICIO-NOT-FOUND" }])]
    public async Task ActivarProveedorServicioTest(
        string caseName,
        int idProveedor,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // First deactivate it to ensure activation changes state (though it is active by default)
            // But we can just call Activate, it sets IsActive = true.

            var proveedor = await Facade.ActivarProveedorServicioAsync(idProveedorServicio: idProveedor, modificationUser: SetupConfig.UserId);
            Assert.NotNull(proveedor);
            Assert.True(condition: proveedor.IsActive);

            var proveedorContext = await Context.ProveedorServicio.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == proveedor.Id);
            Assert.NotNull(proveedorContext);
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
