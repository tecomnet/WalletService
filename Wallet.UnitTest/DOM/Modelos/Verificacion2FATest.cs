using System;
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
    [InlineData("1. OK: Full Valid (SMS, True)", 
        "VE5342e31ef0ac80b9b7ae9f49da6bfc21", "2025-12-31T23:59:59", Tipo2FA.Sms, true, "1234", 
        true, new string[] { })]
    [InlineData("2. OK: Full Valid (EMAIL, False)", 
        "VE5342e31ef0ac80b9b7ae9f49da6bfc22", "2026-01-01T00:00:00", Tipo2FA.Email,  true, "5678",
        true, new string[] { })]

    // === 2. ERRORES DE twilio sid (string, required, length=4) ===
    [InlineData("3. ERROR: twilio sid null",
        null, "2025-12-31T23:59:59", Tipo2FA.Sms,  false, null,
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("4. ERROR: twilio sid empty",
        "", "2025-12-31T23:59:59", Tipo2FA.Email, false, null,
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("5. ERROR: twilio sid tiene mas de 100 caracteres",
        "VE5342e31ef0ac80b9b7ae9f49da6bfc2222222222222222222222222222222222222222222222222222222222222222222222222222222222222222", "2025-12-31T23:59:59", Tipo2FA.Email, false, null,
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    // === 2. ERRORES DE CÓDIGO (string, required, length=4) ===
    [InlineData("6. ERROR: Codigo es empty",
        "VE5342e31ef0ac80b9b7ae9f49da6bfc22", "2025-12-31T23:59:59", Tipo2FA.Email,  true, "",
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("7. ERROR: Codigo too short (3)",
        "VE5342e31ef0ac80b9b7ae9f49da6bfc23", "2025-12-31T23:59:59", Tipo2FA.Sms, true, "123",  
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    [InlineData("8. ERROR: Codigo too long (5)", 
        "VE5342e31ef0ac80b9b7ae9f49da6bfc22", "2025-12-31T23:59:59", Tipo2FA.Email,  true, "12345",
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]

    // === 3. ERRORES DE TIPOS REQUERIDOS (FechaVencimiento, Tipo, Verificado) ===
    // NOTA: Como DateTime, Enum y bool son tipos 'struct' (value types) y no son nullable en la firma 
    // del constructor, es **imposible** que fallen la validación `isRequired: true` con el tipo de constructor dado.
    // Solo un `DateTime?`, `Tipo2FA?` o `bool?` podrían ser nulos y fallar.
    // No se incluyen tests de error forzado para estos, ya que el compilador previene el paso de 'null'.

    // === 4. ERROR MÚLTIPLE ===
    [InlineData("9. ERROR: Multiple (Codigo Required + Length invalid)", 
        "", "2025-12-31T23:59:59", Tipo2FA.Sms,  false, null,
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })] 
    public void Verificacion2FAConsructorTest(
        string caseName,
        string twilioSid,
        string fechaVencimientoStr, // Usamos string para InlineData
        Tipo2FA tipo,
        bool confirmacion,
        string codigo,
        bool success,
        string[]? expectedErrors = null)
    {
        // Preparación de datos: Convertir strings a tipos reales
        DateTime fechaVencimiento = DateTime.Parse(fechaVencimientoStr);
        
        try
        {
            // Act: Crear la instancia de Verificacion2FA
            var verificacion = new Verificacion2FA(
                twilioSid: twilioSid!,
                fechaVencimiento: fechaVencimiento,
                tipo: tipo,
                creationUser: Guid.NewGuid(),
                testCase: caseName);
            // Comprobar la asignación de propiedades (solo si hay éxito)
            Assert.Equal(twilioSid, verificacion.TwilioSid);
            Assert.Equal(fechaVencimiento, verificacion.FechaVencimiento);
            Assert.Equal(tipo, verificacion.Tipo);
            // Confirmación de verificación
            if (confirmacion)
            {
                // Marcar como verificada
                verificacion.MarcarComoVerificado(codigo: codigo, modificationUser: Guid.NewGuid());
                // Comprobar la asignación de propiedades (solo si hay éxito)
                Assert.Equal(codigo, verificacion.Codigo);
                Assert.True(verificacion.Verificado);
            }
            else
            {
                Assert.False(verificacion.Verificado);
            }
             // Assert Success
            Assert.True(success, $"El caso '{caseName}' falló cuando se esperaba éxito.");
        }
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException && exception is not TrueException && exception is not FalseException)
        {
            Assert.Fail($"Excepción no gestionada en '{caseName}': {exception.GetType().Name} - {exception.Message}");
        }
    }
}
