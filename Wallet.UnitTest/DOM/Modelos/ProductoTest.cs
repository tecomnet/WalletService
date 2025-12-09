using Moq;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class ProductoTest : UnitTestTemplate
{
    [Theory]
    // --- Casos de éxito ---
    [InlineData(data:
        ["OK: Datos válidos", 1, "SKU001", "Producto A", 150.75, "icon.png", "Servicios", true, new string[] { }])]
    [InlineData(data:
        ["OK: Sin urlIcono/categoria", 2, "SKU002", "Producto B", 99.99, null, null, true, new string[] { }])]
    [InlineData(data:
        ["OK: Precio sin decimales", 3, "SKU003", "Producto C", 200, "icon.png", "Servicios", true, new string[] { }])]
    [InlineData(data:
    [
        "OK: Límites de longitud", 4, "SKU con 50 caracteres para probar el límite máximo",
        "Nombre de producto con 100 caracteres para probar el límite máximo establecido en la base de datos..", 0.01,
        null, null, true, new string[] { }
    ])]

    // --- Casos de error de validación ---
    [InlineData(data:
    [
        "ERROR: Sku nulo", 1, null, "Producto X", 10.0, null, null, false,
        new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }
    ])]
    [InlineData(data:
    [
        "ERROR: Sku vacío", 1, "", "Producto X", 10.0, null, null, false,
        new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }
    ])]
    [InlineData(data:
    [
        "ERROR: Sku excede límite", 1, "Este SKU es demasiado largo y excede el límite de 50 caracteres", "Producto X",
        10.0, null, null, false, new[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }
    ])]
    [InlineData(data:
    [
        "ERROR: Nombre nulo", 1, "SKU-VALID", null, 10.0, null, null, false,
        new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }
    ])]
    [InlineData(data:
    [
        "ERROR: Nombre vacío", 1, "SKU-VALID", "", 10.0, null, null, false,
        new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }
    ])]
    [InlineData(data:
    [
        "ERROR: Nombre excede límite", 1, "SKU-VALID",
        "Este nombre es excesivamente largo y supera los 100 caracteres permitidos para el nombre de un producto...",
        10.0, null, null, false, new[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }
    ])]
    [InlineData(data:
    [
        "ERROR: Monto es cero", 1, "SKU-VALID", "Producto Y", 0.00, null, null, false,
        new[] { "PROPERTY-VALIDATION-ZERO-INVALID" }
    ])]
    [InlineData(data:
    [
        "ERROR: Monto es negativo", 1, "SKU-VALID", "Producto Y", -50.0, null, null, false,
        new[] { "PROPERTY-VALIDATION-NEGATIVE-INVALID" }
    ])]
    [InlineData(data:
    [
        "ERROR: Monto con muchos decimales", 1, "SKU-VALID", "Producto Y", 10.123, null, null, false,
        new[] { "PROPERTY-VALIDATION-DECIMALS-INVALID" }
    ])]

    // --- Casos de error múltiples ---
    [InlineData(data:
    [
        "ERROR: Múltiples errores", 1, "", "", -10, null, null, false, new[]
        {
            "PROPERTY-VALIDATION-REQUIRED-ERROR", "PROPERTY-VALIDATION-REQUIRED-ERROR",
            "PROPERTY-VALIDATION-NEGATIVE-INVALID"
        }
    ])]
    public void ConstructorValidation_Test(
        string caseName,
        int proveedorServicioId,
        string? sku,
        string? nombre,
        double precio,
        string? icono,
        string? categoria,
        bool success,
        string[]? expectedErrors = null)
    {
        try
        {
            // Arrange
            var mockProveedor = new Mock<Proveedor>();
            mockProveedor.SetupGet(expression: p => p.Id).Returns(value: proveedorServicioId);

            // Act
#pragma warning disable CS8604 // Possible null reference argument
            var producto = new Producto(
                proveedor: mockProveedor.Object,
                sku: sku,
                nombre: nombre,
                precio: (decimal)precio,
                urlIcono: icono,
                categoria: categoria,
                creationUser: Guid.NewGuid()
            );

            // Assert
            Assert.True(condition: success,
                userMessage: $"El caso '{caseName}' debería haber tenido éxito, pero falló.");
            Assert.Equal(expected: proveedorServicioId, actual: producto.ProveedorId);
            Assert.Equal(expected: sku, actual: producto.Sku);
            Assert.Equal(expected: nombre, actual: producto.Nombre);
            Assert.Equal(expected: (decimal)precio, actual: producto.Precio);
            Assert.Equal(expected: icono, actual: producto.UrlIcono);
            Assert.Equal(expected: categoria, actual: producto.Categoria);
        }
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            Assert.Fail(message: $"Excepción no gestionada en '{caseName}': {exception.Message}");
        }
    }
}
