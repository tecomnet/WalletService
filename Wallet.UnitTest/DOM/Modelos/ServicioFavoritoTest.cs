using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class ServicioFavoritoTest : UnitTestTemplate
{
    [Theory]
    // --- Casos de éxito ---
    [InlineData(data: ["OK: Datos válidos", 1, 10, "Luz Casa", "1234567890", true, new string[] { }])]
    [InlineData(data: ["OK: Límites de longitud", 2, 20, "Alias con cincuenta caracteres para probar el lim.", "Ref con cincuenta caracteres para probar el lim...", true, new string[] { }])]

    // --- Casos de error de validación para Alias ---
    [InlineData(data: ["ERROR: Alias nulo", 1, 10, null, "1234567890", false, new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["ERROR: Alias vacío", 1, 10, "", "1234567890", false, new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["ERROR: Alias excede límite", 1, 10, "Este alias es demasiado largo, excede los 50 caracteres", "1234567890", false, new[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }])]

    // --- Casos de error de validación para NumeroReferencia ---
    [InlineData(data: ["ERROR: NumeroReferencia nulo", 1, 10, "Luz Casa", null, false, new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["ERROR: NumeroReferencia vacío", 1, 10, "Luz Casa", "", false, new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["ERROR: NumeroReferencia excede límite", 1, 10, "Luz Casa", "Este número de referencia es demasiado largo y excede el límite", false, new[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }])]

    // --- Caso de error múltiple ---
    [InlineData(data: ["ERROR: Múltiples errores", 1, 10, "", "", false, new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR", "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    public void ConstructorValidation_Test(
        string caseName,
        int clienteId,
        int proveedorServicioId,
        string? alias,
        string? numeroReferencia,
        bool success,
        string[]? expectedErrors = null)
    {
        try
        {
            // Act
#pragma warning disable CS8604 // Possible null reference argument
            var servicioFavorito = new ServicioFavorito(
                clienteId: clienteId,
                proveedorServicioId: proveedorServicioId,
                alias: alias,
                numeroReferencia: numeroReferencia,
                creationUser: Guid.NewGuid()
            );

            // Assert
            Assert.True(condition: success, userMessage: $"El caso '{caseName}' debería haber tenido éxito, pero falló.");
            Assert.Equal(expected: clienteId, actual: servicioFavorito.ClienteId);
            Assert.Equal(expected: proveedorServicioId, actual: servicioFavorito.ProveedorServicioId);
            Assert.Equal(expected: alias, actual: servicioFavorito.Alias);
            Assert.Equal(expected: numeroReferencia, actual: servicioFavorito.NumeroReferencia);
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
