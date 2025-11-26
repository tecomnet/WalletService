using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ProveedorServicioFacadeTest;

public class ProveedorServicioFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IProveedorServicioFacade>(setupConfig)
{
    [Theory]
    // Successfully case
    [InlineData("1. Successfully case, create proveedor", "Netflix", ProductoCategoria.Servicios,
        "https://netflix.com/icon.png", true, new string[] { })]
    // Wrong cases
    // Add validation error cases if any validation exists in DOM constructor
    [InlineData("2. Wrong case, empty name", "", ProductoCategoria.Servicios, "url", false,
        new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
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
            Assert.True(proveedor.Nombre == nombre &&
                        proveedor.Categoria == categoria &&
                        proveedor.UrlIcono == urlIcono &&
                        proveedor.CreationUser == SetupConfig.UserId);

            // Get from context
            var proveedorContext = await Context.ProveedorServicio.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == proveedor.Id);
            Assert.NotNull(proveedorContext);
            Assert.True(proveedorContext.Nombre == nombre &&
                        proveedorContext.Categoria == categoria &&
                        proveedorContext.UrlIcono == urlIcono &&
                        proveedorContext.CreationUser == SetupConfig.UserId);

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
    // Successfully case
    [InlineData("1. Successfully case, update proveedor", 1, "CFE Updated", ProductoCategoria.Movilidad, "new_url",
        true, new string[] { })]
    // Wrong cases
    [InlineData("2. Wrong case, not found", 99, "Name", ProductoCategoria.Servicios, "url", false,
        new string[] { "PROVEEDOR-SERVICIO-NOT-FOUND" })]
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
            Assert.True(proveedor.Nombre == nombre &&
                        proveedor.Categoria == categoria &&
                        proveedor.UrlIcono == urlIcono &&
                        proveedor.ModificationUser == SetupConfig.UserId);

            var proveedorContext = await Context.ProveedorServicio.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == proveedor.Id);
            Assert.NotNull(proveedorContext);
            Assert.True(proveedorContext.Nombre == nombre &&
                        proveedorContext.Categoria == categoria &&
                        proveedorContext.UrlIcono == urlIcono &&
                        proveedorContext.ModificationUser == SetupConfig.UserId);

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
    [InlineData("1. Successfully case, delete proveedor", 1, true, new string[] { })]
    [InlineData("2. Wrong case, not found", 99, false, new string[] { "PROVEEDOR-SERVICIO-NOT-FOUND" })]
    public async Task EliminarProveedorServicioTest(
        string caseName,
        int idProveedor,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var proveedor = await Facade.EliminarProveedorServicioAsync(idProveedor, SetupConfig.UserId);
            Assert.NotNull(proveedor);
            Assert.False(proveedor.IsActive);

            var proveedorContext = await Context.ProveedorServicio.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == proveedor.Id);
            Assert.NotNull(proveedorContext);
            Assert.False(proveedorContext.IsActive);

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
    [InlineData("1. Successfully case, activate proveedor", 1, true, new string[] { })]
    [InlineData("2. Wrong case, not found", 99, false, new string[] { "PROVEEDOR-SERVICIO-NOT-FOUND" })]
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

            var proveedor = await Facade.ActivarProveedorServicioAsync(idProveedor, SetupConfig.UserId);
            Assert.NotNull(proveedor);
            Assert.True(proveedor.IsActive);

            var proveedorContext = await Context.ProveedorServicio.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == proveedor.Id);
            Assert.NotNull(proveedorContext);
            Assert.True(proveedorContext.IsActive);

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
