using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Moq;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class ProductoProveedorTest : UnitTestTemplate
{
    [Theory]
    // --- Casos de éxito ---
    [InlineData("OK: Datos válidos", 1, "SKU001", "Producto A", 150.75, "Descripción del producto A", true,
        new string[] { })]
    [InlineData("OK: Sin descripción", 2, "SKU002", "Producto B", 99.99, null, true, new string[] { })]
    [InlineData("OK: Monto sin decimales", 3, "SKU003", "Producto C", 200, "", true, new string[] { })]
    [InlineData("OK: Límites de longitud", 4, "SKU con 50 caracteres para probar el límite máximo",
        "Nombre de producto con 100 caracteres para probar el límite máximo establecido en la base de datos..", 0.01,
        "Descripción al límite.", true, new string[] { })]

    // --- Casos de error de validación ---
    [InlineData("ERROR: Sku nulo", 1, null, "Producto X", 10.0, null, false,
        new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("ERROR: Sku vacío", 1, "", "Producto X", 10.0, null, false,
        new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("ERROR: Sku excede límite", 1, "Este SKU es demasiado largo y excede el límite de 50 caracteres",
        "Producto X", 10.0, null, false, new[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    [InlineData("ERROR: Nombre nulo", 1, "SKU-VALID", null, 10.0, null, false,
        new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("ERROR: Nombre vacío", 1, "SKU-VALID", "", 10.0, null, false,
        new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("ERROR: Nombre excede límite", 1, "SKU-VALID",
        "Este nombre es excesivamente largo y supera los 100 caracteres permitidos para el nombre de un producto...",
        10.0, null, false, new[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    [InlineData("ERROR: Monto es cero", 1, "SKU-VALID", "Producto Y", 0.00, null, false,
        new[] { "PROPERTY-VALIDATION-ZERO-INVALID" })]
    [InlineData("ERROR: Monto es negativo", 1, "SKU-VALID", "Producto Y", -50.0, null, false,
        new[] { "PROPERTY-VALIDATION-NEGATIVE-INVALID" })]
    [InlineData("ERROR: Monto con muchos decimales", 1, "SKU-VALID", "Producto Y", 10.123, null, false,
        new[] { "PROPERTY-VALIDATION-DECIMALS-INVALID" })]

    // --- Casos de error múltiples ---
    [InlineData("ERROR: Múltiples errores", 1, "", "", -10, null, false,
        new[]
        {
            "PROPERTY-VALIDATION-REQUIRED-ERROR", "PROPERTY-VALIDATION-REQUIRED-ERROR",
            "PROPERTY-VALIDATION-NEGATIVE-INVALID"
        })]
    public void ConstructorValidation_Test(
        string caseName,
        int proveedorServicioId,
        string? sku,
        string? nombre,
        decimal monto,
        string? descripcion,
        bool success,
        string[]? expectedErrors = null)
    {
        try
        {
            // Arrange
            var mockProveedor = new Mock<ProveedorServicio>();
            mockProveedor.SetupGet(p => p.Id).Returns(proveedorServicioId);
            
            // Act
#pragma warning disable CS8604 // Possible null reference argument
            var producto = new ProductoProveedor(
                proveedorServicio: mockProveedor.Object,
                sku: sku,
                nombre: nombre,
                monto: monto,
                descripcion: descripcion,
                creationUser: Guid.NewGuid()
            );

            // Assert
            Assert.True(success, $"El caso '{caseName}' debería haber tenido éxito, pero falló.");
            Assert.Equal(proveedorServicioId, producto.ProveedorServicioId);
            Assert.Equal(sku, producto.Sku);
            Assert.Equal(nombre, producto.Nombre);
            Assert.Equal(monto, producto.Monto);
            Assert.Equal(descripcion, producto.Descripcion);
        }
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName, success, expectedErrors, exception);
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            Assert.Fail($"Excepción no gestionada en '{caseName}': {exception.Message}");
        }
    }
}
