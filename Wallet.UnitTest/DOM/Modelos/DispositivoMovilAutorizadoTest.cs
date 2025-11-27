using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.UnitTest.DOM.Modelos;

public class DispositivoMovilAutorizadoTest : UnitTestTemplate
{
// Constantes de datos
    private const string MinLength = "A";
    private const string Max100Chars = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"; // 100 caracteres
    private const string Max101Chars = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890X"; // 101 caracteres

    [Theory]
    // PARÁMETROS: CaseName, Token, IdDispositivo, Nombre, Caracteristicas, Success, ExpectedErrors

    // === 1. CASOS DE ÉXITO ===
    [InlineData("1. OK: Full Valid",
        "Token-ABC-12345", "UUID-XYZ-9876", "iPhone 15 Pro", "iOS 17.0, Screen 6.1, RAM 8GB", 
        true, new string[] { })]
    [InlineData("2. OK: Min length",
        MinLength, MinLength, MinLength, MinLength, 
        true, new string[] { })]
    [InlineData("3. OK: Max length (100)",
        Max100Chars, Max100Chars, Max100Chars, Max100Chars, 
        true, new string[] { })]

    // === 2. ERRORES DE REQUERIMIENTO (null o empty) ===
    [InlineData("4. ERROR: Token null",
        null, "id_valido", "Nombre", "Caracteristicas", 
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("5. ERROR: IdDispositivo empty",
        "Token", "", "Nombre", "Caracteristicas", 
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("6. ERROR: Nombre null",
        "Token", "id_valido", null, "Caracteristicas", 
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("7. ERROR: Caracteristicas empty",
        "Token", "id_valido", "Nombre", "", 
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]

    // === 3. ERRORES DE LONGITUD (> 100 caracteres) ===
    [InlineData("8. ERROR: Token too long",
        Max101Chars, "id_valido", "Nombre", "Caracteristicas", 
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    [InlineData("9. ERROR: IdDispositivo too long",
        "Token", Max101Chars, "Nombre", "Caracteristicas", 
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    [InlineData("10. ERROR: Nombre too long",
        "Token", "id_valido", Max101Chars, "Caracteristicas", 
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    [InlineData("11. ERROR: Caracteristicas too long",
        "Token", "id_valido", "Nombre", Max101Chars, 
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]

    // === 4. CASO DE ERRORES MÚLTIPLES ===
    [InlineData("12. ERROR: Multiple errors (Required & Length)",
        "", Max101Chars, null, Max101Chars,
        false, new string[] { 
            "PROPERTY-VALIDATION-REQUIRED-ERROR", // Token (empty)
            "PROPERTY-VALIDATION-LENGTH-INVALID", // IdDispositivo (>100)
            "PROPERTY-VALIDATION-REQUIRED-ERROR", // Nombre (null)
            "PROPERTY-VALIDATION-LENGTH-INVALID"  // Caracteristicas (>100)
        })]
    public void DispositivoMovilAutorizadoConstructorTest(
        // Case name
        string caseName,
        // Test data
        string? token,
        string? idDispositivo,
        string? nombre,
        string? caracteristicas,
        // Result
        bool success,
        string[]? expectedErrors = null
    )
    {
        try
        {
            // 1. Ejecutar el constructor (que realiza la validación)
            var dispositivoAutorizado = new DispositivoMovilAutorizado(
                token: token!,
                idDispositivo: idDispositivo!,
                nombre: nombre!,
                caracteristicas: caracteristicas!,
                creationUser: Guid.NewGuid(),
                testCase: caseName);

            // 2. Verificación de éxito y asignación de valores
            Assert.True(condition: success, userMessage: $"El caso '{caseName}' falló cuando se esperaba éxito.");
            
            // Comprobar la asignación de propiedades (solo si hay éxito)
            Assert.Equal(token, dispositivoAutorizado.Token);
            Assert.Equal(idDispositivo, dispositivoAutorizado.IdDispositivo);
            Assert.Equal(nombre, dispositivoAutorizado.Nombre);
            Assert.Equal(caracteristicas, dispositivoAutorizado.Caracteristicas);
            // Comprobar la propiedad Actual que se setea internamente
            Assert.True(dispositivoAutorizado.Actual, "La propiedad 'Actual' debería ser true al crearse.");
        }
        // 3. Capturar y verificar errores gestionados
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // 4. Capturar errores no gestionados
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                             exception is not Xunit.Sdk.TrueException && exception is not Xunit.Sdk.FalseException)
        {
            Assert.Fail($"Excepción no gestionada en '{caseName}': {exception.GetType().Name} - {exception.Message}");
        }
    }
}
