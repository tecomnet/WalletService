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
        "1234", "2025-12-31T23:59:59", Tipo2FA.Sms, true, 
        true, new string[] { })]
    [InlineData("2. OK: Full Valid (EMAIL, False)", 
        "9876", "2026-01-01T00:00:00", Tipo2FA.Email, false, 
        true, new string[] { })]

    // === 2. ERRORES DE CÓDIGO (string, required, length=4) ===
    [InlineData("3. ERROR: Codigo null", 
        null, "2025-12-31T23:59:59", Tipo2FA.Sms, true, 
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("4. ERROR: Codigo empty", 
        "", "2025-12-31T23:59:59", Tipo2FA.Email, false, 
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("5. ERROR: Codigo too short (3)", 
        "123", "2025-12-31T23:59:59", Tipo2FA.Sms, true, 
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    [InlineData("6. ERROR: Codigo too long (5)", 
        "12345", "2025-12-31T23:59:59", Tipo2FA.Email, false, 
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]

    // === 3. ERRORES DE TIPOS REQUERIDOS (FechaVencimiento, Tipo, Verificado) ===
    // NOTA: Como DateTime, Enum y bool son tipos 'struct' (value types) y no son nullable en la firma 
    // del constructor, es **imposible** que fallen la validación `isRequired: true` con el tipo de constructor dado.
    // Solo un `DateTime?`, `Tipo2FA?` o `bool?` podrían ser nulos y fallar.
    // No se incluyen tests de error forzado para estos, ya que el compilador previene el paso de 'null'.

    // === 4. ERROR MÚLTIPLE ===
    [InlineData("7. ERROR: Multiple (Codigo Required + Length invalid)", 
        "", "2025-12-31T23:59:59", Tipo2FA.Sms, true, 
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })] 
    // Nota: Si el código tiene más de 4, el 'Required' (empty/null) no aplica, solo el Length.
    [InlineData("8. ERROR: Multiple (Codigo Length invalid)", 
        "123456", "2025-12-31T23:59:59", Tipo2FA.Email, false, 
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    public void Verificacion2FAConsructorTest(
        string caseName,
        string? codigo,
        string fechaVencimientoStr, // Usamos string para InlineData
        Tipo2FA tipo,
        bool verificado,
        bool success,
        string[]? expectedErrors = null)
    {
        // Preparación de datos: Convertir strings a tipos reales
        DateTime fechaVencimiento = DateTime.Parse(fechaVencimientoStr);
        
        try
        {
            // Act: Crear la instancia de Verificacion2FA
            var verificacion = new Verificacion2FA(
                codigo: codigo!,
                fechaVencimiento: fechaVencimiento,
                tipo: tipo,
                verificado: verificado,
                creationUser: Guid.NewGuid(),
                testCase: caseName);

            // Assert Success
            Assert.True(success, $"El caso '{caseName}' falló cuando se esperaba éxito.");
            
            // Comprobar la asignación de propiedades (solo si hay éxito)
            Assert.Equal(codigo, verificacion.Codigo);
            Assert.Equal(fechaVencimiento, verificacion.FechaVencimiento);
            Assert.Equal(tipo, verificacion.Tipo);
            Assert.Equal(verificado, verificacion.Verificado);
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
