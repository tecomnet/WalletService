using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionUsuario;

namespace Wallet.UnitTest.DOM.Modelos;

public class UbicacionesGeolocalizacionTest : UnitTestTemplate
{
// Constantes de datos para casos límite y errores
    private const string LatLongValid = "19.4326";

    private const string Max100Chars =
        "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"; // 100 caracteres

    private const string Max45Chars = "123456789012345678901234567890123456789012345"; // 45 caracteres
    private const string IpV4Valid = "192.168.1.1";
    private const string IpV6Valid = "2001:db8:3333:4444:5555:6666:7777:8888"; // 39 caracteres

    [Theory]
    // ----------------------------------------------------------------------------------------------------------------
    // 1. CASOS DE ÉXITO (Datos mínimos y máximos válidos)
    // ----------------------------------------------------------------------------------------------------------------
    [InlineData(data:
    [
        "1. OK: Datos mínimos válidos", "1", "-1", Dispositivo.App, "Login", "Mobile", "Chrome", "1.1.1.1", true,
        new string[] { }
    ])]
    [InlineData(data:
    [
        "2. OK: Latitud/Longitud compleja y Máx 8 decimales", "89.99999999", "-179.99999999", Dispositivo.Web,
        Max100Chars, Max100Chars, Max100Chars, IpV6Valid, // Max 100 para los 3 strings
        true, new string[] { }
    ])]
    [InlineData(data:
    [
        "3. OK: IP Máxima (45 chars)", LatLongValid, LatLongValid, Dispositivo.App, "Logout", "Desktop", "Firefox",
        Max45Chars, true, new string[] { }
    ])]
    [InlineData(data:
    [
        "4. OK: IP IPv4 Válida", LatLongValid, LatLongValid, Dispositivo.App, "View", "Tablet", "Safari", IpV4Valid,
        true, new string[] { }
    ])]

    // ----------------------------------------------------------------------------------------------------------------
    // 2. ERRORES DE REQUERIMIENTO (PROPERTY-VALIDATION-REQUIRED-ERROR)
    // ----------------------------------------------------------------------------------------------------------------
    // [InlineData("5. ERROR: Latitud null",
    //     null, LatLongValid, Dispositivo.App, "Login", "Mobile", "Chrome", IpV4Valid, 
    //     false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    // [InlineData("6. ERROR: Longitud empty",
    //     LatLongValid, "", Dispositivo.App, "Login", "Mobile", "Chrome", IpV4Valid, 
    //     false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData(data:
    [
        "7. ERROR: TipoEvento null", LatLongValid, LatLongValid, Dispositivo.App, null, "Mobile", "Chrome", IpV4Valid,
        false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "8. ERROR: DireccionIp null", LatLongValid, LatLongValid, Dispositivo.App, "Login", "Mobile", "Chrome", null,
        false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]

    // ----------------------------------------------------------------------------------------------------------------
    // 3. ERRORES DE LONGITUD (PROPERTY-VALIDATION-LENGTH-INVALID)
    // ----------------------------------------------------------------------------------------------------------------
    [InlineData(data:
    [
        "9. ERROR: Latitud > 8 decimales", "19.1234567891", LatLongValid, Dispositivo.App, "Login", "Mobile", "Chrome",
        IpV4Valid, false, new string[] { ServiceErrorsBuilder.PropertyValidationDecimalsInvalid }
    ])]
    [InlineData(data:
    [
        "10. ERROR: Agente > 100 caracteres", LatLongValid, LatLongValid, Dispositivo.App, "Login", "Mobile",
        Max100Chars + "X", IpV4Valid, false, new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]
    [InlineData(data:
    [
        "11. ERROR: DireccionIp > 45 caracteres", LatLongValid, LatLongValid, Dispositivo.App, "Login", "Mobile",
        "Chrome", Max45Chars + "X", // 46 chars
        false, new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]

    // ----------------------------------------------------------------------------------------------------------------
    // 4. CASO DE ERRORES MÚLTIPLES 
    // ----------------------------------------------------------------------------------------------------------------
    [InlineData(data:
    [
        "12. ERROR: Múltiples fallos críticos", "1", "1", Dispositivo.App, null, "", Max100Chars + "X", "", false,
        new string[]
        {
            ServiceErrorsBuilder.PropertyValidationRequiredError, // Latitud
            ServiceErrorsBuilder.PropertyValidationRequiredError, // Longitud
            ServiceErrorsBuilder.PropertyValidationRequiredError, // TipoEvento
            ServiceErrorsBuilder.PropertyValidationRequiredError, // TipoDispositivo
            ServiceErrorsBuilder.PropertyValidationLengthInvalid, // Agente
            ServiceErrorsBuilder.PropertyValidationRequiredError // DireccionIp
        }
    ])]
    public void UbicacionesGeolocalizacionConstructorTest(
        // Case name
        string caseName,
        // Test data
        string? latitud,
        string? longitud,
        Dispositivo dispositivo,
        string? tipoEvento,
        string? tipoDispositivo,
        string? agente,
        string? direccionIp,
        // Result
        bool success,
        string[]? expectedErrors = null
    )
    {
        decimal? latitudConverted = latitud == null ? null : decimal.Parse(s: latitud);
        decimal? longitudConverted = longitud == null ? null : decimal.Parse(s: longitud);
        try
        {
            // Ejecutar el constructor (que realiza la validación)
            var ubicacion = new UbicacionesGeolocalizacion(
                latitud: latitudConverted.GetValueOrDefault(),
                longitud: longitudConverted.GetValueOrDefault(),
                dispositivo: dispositivo,
                tipoEvento: tipoEvento!,
                tipoDispositivo: tipoDispositivo!,
                agente: agente!,
                direccionIp: direccionIp!,
                creationUser: Guid.NewGuid(),
                testCase: caseName);
            // Verificación de asignación de valores (si success es true)
            Assert.Equal(expected: latitudConverted, actual: ubicacion.Latitud);
            Assert.Equal(expected: longitudConverted, actual: ubicacion.Longitud);
            Assert.Equal(expected: tipoEvento, actual: ubicacion.TipoEvento);
            Assert.Equal(expected: tipoDispositivo, actual: ubicacion.TipoDispositivo);
            Assert.Equal(expected: agente, actual: ubicacion.Agente);
            Assert.Equal(expected: direccionIp, actual: ubicacion.DireccionIp);
            // Verificación de éxito
            Assert.True(condition: success, userMessage: $"El caso '{caseName}' falló cuando se esperaba éxito.");
        }
        // Capturar y verificar errores gestionados
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Capturar errores no gestionados
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not Xunit.Sdk.TrueException &&
                                          exception is not Xunit.Sdk.FalseException)
        {
            Assert.Fail(
                message: $"Excepción no gestionada en '{caseName}': {exception.GetType().Name} - {exception.Message}");
        }
    }
}
