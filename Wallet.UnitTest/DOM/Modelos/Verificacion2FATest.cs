using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class Verificacion2FATest : UnitTestTemplate
{
    [Theory]
    // PARÁMETROS: CaseName, Codigo (string), FechaVencimiento (DateTime), Tipo (string), Verificado (bool), Success, ExpectedErrors

    // === 1. CASOS DE ÉXITO ===
    [InlineData(data:
    [
        "1. OK: Full Valid (SMS, True)", "VE5342e31ef0ac80b9b7ae9f49da6bfc21", "2025-12-31T23:59:59", Tipo2FA.Sms, true,
        "1234", true, new string[] { }
    ])]
    [InlineData(data:
    [
        "2. OK: Full Valid (EMAIL, False)", "VE5342e31ef0ac80b9b7ae9f49da6bfc22", "2026-01-01T00:00:00", Tipo2FA.Email,
        true, "5678", true, new string[] { }
    ])]

    // === 2. ERRORES DE twilio sid (string, required, length=4) ===
    [InlineData(data:
    [
        "3. ERROR: twilio sid null", null, "2025-12-31T23:59:59", Tipo2FA.Sms, false, null, false,
        new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "4. ERROR: twilio sid empty", "", "2025-12-31T23:59:59", Tipo2FA.Email, false, null, false,
        new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "5. ERROR: twilio sid tiene mas de 100 caracteres",
        "VE5342e31ef0ac80b9b7ae9f49da6bfc2222222222222222222222222222222222222222222222222222222222222222222222222222222222222222",
        "2025-12-31T23:59:59", Tipo2FA.Email, false, null, false,
        new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]
    // === 2. ERRORES DE CÓDIGO (string, required, length=4) ===
    [InlineData(data:
    [
        "6. ERROR: Codigo es empty", "VE5342e31ef0ac80b9b7ae9f49da6bfc22", "2025-12-31T23:59:59", Tipo2FA.Email, true,
        "", false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "7. ERROR: Codigo too short (3)", "VE5342e31ef0ac80b9b7ae9f49da6bfc23", "2025-12-31T23:59:59", Tipo2FA.Sms,
        true, "123", false, new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]
    [InlineData(data:
    [
        "8. ERROR: Codigo too long (5)", "VE5342e31ef0ac80b9b7ae9f49da6bfc22", "2025-12-31T23:59:59", Tipo2FA.Email,
        true, "12345", false, new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]

    // === 3. ERRORES DE TIPOS REQUERIDOS (FechaVencimiento, Tipo, Verificado) ===
    // NOTA: Como DateTime, Enum y bool son tipos 'struct' (value types) y no son nullable en la firma 
    // del constructor, es **imposible** que fallen la validación `isRequired: true` con el tipo de constructor dado.
    // Solo un `DateTime?`, `Tipo2FA?` o `bool?` podrían ser nulos y fallar.
    // No se incluyen tests de error forzado para estos, ya que el compilador previene el paso de 'null'.

    // === 4. ERROR MÚLTIPLE ===
    [InlineData(data:
    [
        "9. ERROR: Multiple (Codigo Required + Length invalid)", "", "2025-12-31T23:59:59", Tipo2FA.Sms, false, null,
        false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    public void Verificacion2FAConsructorTest(
        string caseName,
        string? twilioSid,
        string fechaVencimientoStr, // Usamos string para InlineData
        Tipo2FA tipo,
        bool confirmacion,
        string? codigo,
        bool success,
        string[]? expectedErrors = null)
    {
        // Preparación de datos: Convertir strings a tipos reales
        DateTime fechaVencimiento = DateTime.Parse(s: fechaVencimientoStr);

        try
        {
            // Act: Crear la instancia de Verificacion2FA
#pragma warning disable CS8604 // Possible null reference argument
            var verificacion = new Verificacion2FA(
                twilioSid: twilioSid!,
                fechaVencimiento: fechaVencimiento,
                tipo: tipo,
                creationUser: Guid.NewGuid(),
                testCase: caseName);
            // Comprobar la asignación de propiedades (solo si hay éxito)
            Assert.Equal(expected: twilioSid, actual: verificacion.TwilioSid);
            Assert.Equal(expected: fechaVencimiento, actual: verificacion.FechaVencimiento);
            Assert.Equal(expected: tipo, actual: verificacion.Tipo);
            // Confirmación de verificación
            if (confirmacion)
            {
                // Marcar como verificada
#pragma warning disable CS8604 // Possible null reference argument
                verificacion.MarcarComoVerificado(codigo: codigo, modificationUser: Guid.NewGuid());
                // Comprobar la asignación de propiedades (solo si hay éxito)
                Assert.Equal(expected: codigo, actual: verificacion.Codigo);
                Assert.True(condition: verificacion.Verificado);
            }
            else
            {
                Assert.False(condition: verificacion.Verificado);
            }

            // Assert Success
            Assert.True(condition: success, userMessage: $"El caso '{caseName}' falló cuando se esperaba éxito.");
        }
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            Assert.Fail(
                message: $"Excepción no gestionada en '{caseName}': {exception.GetType().Name} - {exception.Message}");
        }
    }
}
