using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.UnitTest.DOM.Modelos;

public class EmpresaTest:UnitTestTemplate
{
// Definición de una cadena de 101 caracteres para probar el límite máximo
    private const string NombreTooLong = "Este nombre es demasiado largo y tiene más de 100 caracteres. Superamos el límite de cien caracteres por poco. X"; // 101 caracteres

    [Theory]
    // ----------------------------------------------------------------------------------------------------------------
    // CASOS DE ÉXITO (Nombre: string, min 1, max 100)
    // ----------------------------------------------------------------------------------------------------------------
    [InlineData(data: ["1. OK: Nombre válido (mínimo)", "A", true, new string[] { }])]
    [InlineData(data: ["2. OK: Nombre válido (completo)", "Empresa de Servicios Globales S.A. de C.V.", true, new string[] { }])]
    [InlineData(data: ["3. OK: Nombre válido (máximo 100)", "Nombre muy largo que tiene exactamente 100 caracteres. Un total de cien caracteres para el nombre.", true, new string[] { }])]

    // ----------------------------------------------------------------------------------------------------------------
    // CASOS DE ERROR (Nombre: string, min 1, max 100)
    // ----------------------------------------------------------------------------------------------------------------
    [InlineData(data: ["4. ERROR: Nombre null (Required)", null, false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["5. ERROR: Nombre vacío (Required)", "", false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["6. ERROR: Nombre muy largo (> 100)", NombreTooLong, false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }])]
    public void BasicEmpresaTest(
        // Case name
        string caseName,
        // Test data
        string? nombre,
        // Result
        bool success,
        string[]? expectedErrors = null
    )
    {
        try
        {
            // Crea la Empresa (ejecuta el constructor y la validación)
            var empresa = new Empresa(
                nombre: nombre!, // Nota: El constructor recibe string (no nullable), pero el validador maneja null.
                creationUser: Guid.NewGuid(),
                testCase: caseName);

            // Check the properties (solo si es éxito)
            Assert.Equal(expected: nombre, actual: empresa.Nombre);

            // Assert success
            Assert.True(condition: success, userMessage: $"El caso '{caseName}' falló cuando se esperaba éxito.");
        }
        // Catch the managed errors and check them with the expected ones in the case of failures
        catch (EMGeneralAggregateException exception)
        {
            // Treat the raised error
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Catch any non managed errors and display them to understand the root cause
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                             exception is not Xunit.Sdk.TrueException && exception is not Xunit.Sdk.FalseException)
        {
            // Should not reach for unmanaged errors
            Assert.Fail(message: $"Excepción no gestionada en '{caseName}': {exception.GetType().Name} - {exception.Message}");
        }
    }
}
