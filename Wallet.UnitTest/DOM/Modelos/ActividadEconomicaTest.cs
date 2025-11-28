using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class ActividadEconomicaTest : UnitTestTemplate
{
    [Theory]
    // CASOS DE ÉXITO
    [InlineData(data: ["1. OK: Full Valid", "Mi Actividad Principal", 1500.75, "Ventas Online", "s3://ruta/archivo_valido.pdf", true, new string[] { }])]
    [InlineData(data: ["2. OK: Min length", "A", 0.01, "O", "A", true, new string[] { }])]
    [InlineData(data: ["3. OK: Max length", "Nombre muy largo que tiene exactamente 100 caracteres. Un total de cien caracteres para el nombre.", 9999999999999999.99, "Origen largo que tiene 100 caracteres, justo el máximo permitido.", "https://ruta-muy-larga-para-el-archivo-aws.com/ejemplo/nombre-archivo-aws-500-caracteres-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-ejemplo-de-una-ruta-muy.pdf", true, new string[] { }])]

    // CASOS DE ERROR PARA NOMBRE (string, min 1, max 100)
    [InlineData(data: ["4. ERROR: Nombre null", null, 100.00, "Ventas", "s3://ruta/archivo.pdf", false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["5. ERROR: Nombre empty", "", 100.00, "Ventas", "s3://ruta/archivo.pdf", false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])] // Ojo: Si mínimo 1, string.Empty falla como requerido.
    [InlineData(data: ["6. ERROR: Nombre too long", "Este nombre es demasiado largo y tiene más de 100 caracteres. Superamos el límite de cien caracteres por poco.", 100.00, "Ventas", "s3://ruta/archivo.pdf", false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }])]

    // CASOS DE ERROR PARA INGRESO (decimal, requerido, > 0, máx 2 decimales)
    [InlineData(data: ["7. ERROR: Ingreso cero", "Ventas", 0.00, "Ventas", "s3://ruta/archivo.pdf", false, new string[] { "PROPERTY-VALIDATION-ZERO-INVALID" }])] // allowZero: false
    [InlineData(data: ["8. ERROR: Ingreso negativo", "Ventas", -10.50, "Ventas", "s3://ruta/archivo.pdf", false, new string[] { "PROPERTY-VALIDATION-NEGATIVE-INVALID" }])] // allowNegative: false
    [InlineData(data: ["9. ERROR: Ingreso con 3 decimales", "Ventas", 100.123, "Ventas", "s3://ruta/archivo.pdf", false, new string[] { "PROPERTY-VALIDATION-DECIMALS-INVALID" }])] // allowedDecimals: 2

    // CASOS DE ERROR PARA ORIGEN RECURSO (string, min 1, max 100)
    [InlineData(data: ["10. ERROR: OrigenRecurso null", "Ventas", 100.00, null, "s3://ruta/archivo.pdf", false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["11. ERROR: OrigenRecurso too long", "Este origen de recurso es demasiado largo y tiene más de 100 caracteres, lo cual es inválido lo cual es inválido.", 100.00, "Un origen muy muy muy muy largo que supera los cien caracteres establecidos en la restricción.", "s3://ruta/archivo.pdf", false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }])]

    // CASOS DE ERROR PARA ARCHIVO AWS (string, min 1, max 500)
    [InlineData(data: ["12. ERROR: ArchivoAWS null", "Ventas", 100.00, "Ventas", null, false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data: ["13. ERROR: ArchivoAWS empty", "Ventas", 100.00, "Ventas", "", false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" }])]
    [InlineData(data:
    ["14. ERROR: ArchivoAWS too long", "Ventas", 100.00, "Ventas", // 501 caracteres (ejemplo de una cadena que excede por poco los 500)
            "https://ejemplo-de-ruta-muy-larga-para-el-archivo-aws.com/ejemplo/ruta-archivo-aws-500-caracteres-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo-ejemplo-de-una-ruta-muy-larga-para-el-archivo-aws-ejemplo.pdf", false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" }
    ])]

    // CASO DE ERROR MÚLTIPLE
    [InlineData(data:
    ["15. ERROR: Multiple errors", "", 0.00, null, "", false, new string[]
        {
            "PROPERTY-VALIDATION-REQUIRED-ERROR", // Nombre
            "PROPERTY-VALIDATION-ZERO-INVALID", // Ingreso (cero)
            "PROPERTY-VALIDATION-REQUIRED-ERROR", // OrigenRecurso
            "PROPERTY-VALIDATION-REQUIRED-ERROR" // ArchivoAWS
        }
    ])]
    public void BasicActividadEconomicaTest(
        // Case name
        string caseName,
        // Test data
        string? nombre,
        decimal ingreso,
        string? origenRecurso,
        string? archivoAWS,
        // Result
        bool success,
        string[]? expectedErrors = null
    )
    {
        try
        {
            // Crea la Actividad Económica
#pragma warning disable CS8604 // Possible null reference argument
            var actividad = new ActividadEconomica(
                nombre: nombre,
                ingreso: ingreso,
                origenRecurso: origenRecurso,
                archivoAWS: archivoAWS,
                creationUser: Guid.NewGuid(),
                testCase: caseName);

            // Check the properties (solo si es éxito)
            Assert.True(condition: actividad.Nombre == nombre,
                userMessage: $"Nombre no es correcto. Actual: {actividad.Nombre}");
            Assert.True(condition: actividad.Ingreso == ingreso,
                userMessage: $"Ingreso no es correcto. Actual: {actividad.Ingreso}");
            Assert.True(condition: actividad.OrigenRecurso == origenRecurso,
                userMessage: $"OrigenRecurso no es correcto. Actual: {actividad.OrigenRecurso}");
            Assert.True(condition: actividad.ArchivoAWS == archivoAWS,
                userMessage: $"ArchivoAWS no es correcto. Actual: {actividad.ArchivoAWS}");

            // Assert success
            Assert.True(condition: success, userMessage: "Should not reach on failures.");
        }
        // Catch the managed errors and check them with the expected ones in the case of failures
        catch (EMGeneralAggregateException exception)
        {
            // Treat the raised error
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Catch any non managed errors and display them to understand the root cause
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            // Should not reach for unmanaged errors
            Assert.Fail(message: $"Uncaught exception. {exception.Message}");
        }
    }
}
