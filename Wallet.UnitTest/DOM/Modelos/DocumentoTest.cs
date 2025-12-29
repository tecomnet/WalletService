using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionCliente;

namespace Wallet.UnitTest.DOM.Modelos;

public class DocumentoTest : UnitTestTemplate
{
    // Cadena de 100 caracteres exactos
    private const string Max100Chars =
        "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789";

    // Cadena de 101 caracteres
    private const string Over100Chars = Max100Chars + "X";

    [Theory]
    // PARÁMETROS: CaseName, Nombre (string), TipoPersona (TipoPersona), Success, ExpectedErrors

    // === 1. CASOS DE ÉXITO (Nombre y TipoPersona válidos) ===
    [InlineData(data: ["1. OK: Nombre válido (Fisica)", "Pasaporte", TipoPersona.Fisica, true, new string[] { }])]
    [InlineData(data: ["2. OK: Nombre Min Length (Moral)", "A", TipoPersona.Moral, true, new string[] { }])]
    [InlineData(data:
        ["3. OK: Nombre Max Length (Extrajero)", Max100Chars, TipoPersona.Extranjero, true, new string[] { }])]

    // === 2. ERRORES DE NOMBRE (string, required, min 1, max 100) ===
    [InlineData(data:
    [
        "4. ERROR: Nombre null", null, TipoPersona.Fisica, false,
        new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "5. ERROR: Nombre empty", "", TipoPersona.Moral, false,
        new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "6. ERROR: Nombre too long (101)", Over100Chars, TipoPersona.Extranjero, false,
        new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]

    // === 3. ERRORES DE TIPO PERSONA (Requerido) ===
    // NOTA: Como 'TipoPersona' es un 'enum' no-nullable en el constructor, no puede ser null. 
    // No se pueden simular fallos de REQUIRED o NULL sin usar un 'TipoPersona?' o 'object' en el constructor, 
    // lo cual violaría el diseño del dominio.

    // === 4. ERRORES MÚLTIPLES ===
    // En este caso, solo el Nombre tiene validaciones de múltiples fallos posibles (Required y Length), pero son mutuamente exclusivos (null/empty vs too long).
    // Por lo tanto, no hay caso de error múltiple, ya que TipoPersona no puede fallar y Nombre solo falla con 1 error a la vez.
    public void DocumentoConstructorTest(
        string caseName,
        string? nombre,
        TipoPersona tipoPersona, // Xunit permite la conversión directa de constantes si el tipo lo permite.
        bool success,
        string[]? expectedErrors = null)
    {
        try
        {
            // Act: Crear la instancia de Documento
            var documento = new Documento(
                nombre: nombre!, // Usamos '!' para permitir el paso de null/empty en la prueba
                tipoPersona: tipoPersona,
                creationUser: Guid.NewGuid(),
                testCase: caseName);

            // Assert Success
            Assert.True(condition: success, userMessage: $"El caso '{caseName}' falló cuando se esperaba éxito.");

            // Comprobar la asignación de propiedades (solo si hay éxito)
            Assert.Equal(expected: nombre, actual: documento.Nombre);
            Assert.Equal(expected: tipoPersona, actual: documento.TipoPersona);
        }
        // Catch the managed errors and check them with the expected ones in the case of failures
        catch (EMGeneralAggregateException exception)
        {
            // Treat the raised error
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Catch any non managed errors and display them to understand the root cause
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not Xunit.Sdk.TrueException &&
                                          exception is not Xunit.Sdk.FalseException)
        {
            // Should not reach for unmanaged errors
            Assert.Fail(
                message: $"Excepción no gestionada en '{caseName}': {exception.GetType().Name} - {exception.Message}");
        }
    }
}
