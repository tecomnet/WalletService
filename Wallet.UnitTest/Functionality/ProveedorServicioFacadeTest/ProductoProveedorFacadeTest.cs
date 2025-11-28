using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ProveedorServicioFacadeTest;

public class ProductoProveedorFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IProveedorServicioFacade>(setupConfig: setupConfig)
{
    [Theory]
    // Successfully case
    [InlineData(data: ["1. Successfully case, create producto", 1, "SKU123", "Netflix Premium", 15.99, "Premium subscription", true, new string[] { }])]
    // Wrong cases
    [InlineData(data: ["2. Wrong case, empty sku", 1, "", "Netflix Premium", 15.99, "Premium subscription", false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["3. Wrong case, negative amount", 1, "SKU123", "Netflix Premium", -1.0, "Premium subscription", false, new string[] { "PROPERTY-VALIDATION-NEGATIVE-INVALID" }])]
    public async Task GuardarProductoTest(
        string caseName,
        int proveedorId,
        string sku,
        string nombre,
        double monto,
        string descripcion,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Call facade method
            var producto = await Facade.GuardarProductoAsync(
                proveedorServicioId: proveedorId,
                sku: sku,
                nombre: nombre,
                monto: (decimal)monto,
                descripcion: descripcion,
                creationUser: SetupConfig.UserId);

            // Assert producto created
            Assert.NotNull(producto);
            // Assert properties
            Assert.True(condition: producto.Sku == sku &&
                                   producto.Nombre == nombre &&
                                   producto.Monto == (decimal)monto &&
                                   producto.Descripcion == descripcion &&
                                   producto.CreationUser == SetupConfig.UserId);

            // Get from context
            var productoContext = await Context.ProductoProveedor.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == producto.Id);
            Assert.NotNull(productoContext);
            Assert.True(condition: productoContext.Sku == sku &&
                                   productoContext.Nombre == nombre &&
                                   productoContext.Monto == (decimal)monto &&
                                   productoContext.Descripcion == descripcion &&
                                   productoContext.CreationUser == SetupConfig.UserId);

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
    [InlineData(data: ["1. Successfully case, update producto", 1, "SKU123-UPD", "Netflix Standard", 10.99, "Standard subscription", true, new string[] { }])]
    // Wrong cases
    [InlineData(data: ["2. Wrong case, not found", 99, "SKU123", "Netflix Premium", 15.99, "Premium subscription", false, new string[] { "PRODUCTO-PROVEEDOR-NOT-FOUND" }])]
    public async Task ActualizarProductoTest(
        string caseName,
        int idProducto,
        string sku,
        string nombre,
        double monto,
        string descripcion,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var producto = await Facade.ActualizarProductoAsync(
                idProducto: idProducto,
                sku: sku,
                nombre: nombre,
                monto: (decimal)monto,
                descripcion: descripcion,
                modificationUser: SetupConfig.UserId);

            Assert.NotNull(producto);
            Assert.True(condition: producto.Sku == sku &&
                                   producto.Nombre == nombre &&
                                   producto.Monto == (decimal)monto &&
                                   producto.Descripcion == descripcion &&
                                   producto.ModificationUser == SetupConfig.UserId);

            var productoContext = await Context.ProductoProveedor.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == producto.Id);
            Assert.NotNull(productoContext);
            Assert.True(condition: productoContext.Sku == sku &&
                                   productoContext.Nombre == nombre &&
                                   productoContext.Monto == (decimal)monto &&
                                   productoContext.Descripcion == descripcion &&
                                   productoContext.ModificationUser == SetupConfig.UserId);

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
    [InlineData(data: ["1. Successfully case, delete producto", 1, true, new string[] { }])]
    [InlineData(data: ["2. Wrong case, not found", 99, false, new string[] { "PRODUCTO-PROVEEDOR-NOT-FOUND" }])]
    public async Task EliminarProductoTest(
        string caseName,
        int idProducto,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var producto = await Facade.EliminarProductoAsync(idProducto: idProducto, modificationUser: SetupConfig.UserId);
            Assert.NotNull(producto);
            Assert.False(condition: producto.IsActive);

            var productoContext = await Context.ProductoProveedor.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == producto.Id);
            Assert.NotNull(productoContext);
            Assert.False(condition: productoContext.IsActive);

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
    [InlineData(data: ["1. Successfully case, activate producto", 1, true, new string[] { }])]
    [InlineData(data: ["2. Wrong case, not found", 99, false, new string[] { "PRODUCTO-PROVEEDOR-NOT-FOUND" }])]
    public async Task ActivarProductoTest(
        string caseName,
        int idProducto,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            var producto = await Facade.ActivarProductoAsync(idProducto: idProducto, modificationUser: SetupConfig.UserId);
            Assert.NotNull(producto);
            Assert.True(condition: producto.IsActive);

            var productoContext = await Context.ProductoProveedor.AsNoTracking()
                .FirstOrDefaultAsync(predicate: x => x.Id == producto.Id);
            Assert.NotNull(productoContext);
            Assert.True(condition: productoContext.IsActive);

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
