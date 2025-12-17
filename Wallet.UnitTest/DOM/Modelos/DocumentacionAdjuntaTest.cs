using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionCliente;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class DocumentacionAdjuntaTest : UnitTestTemplate
{
    private const string ValidArchivoAWS = "s3://ruta/archivo_valido.pdf";

    private const string Max500Chars = "https://ejemplo-ruta-larga.com/" +
                                       "01234567890123456789012345678901234567890123456789" + // 50
                                       "01234567890123456789012345678901234567890123456789" + // 100
                                       "01234567890123456789012345678901234567890123456789" + // 150
                                       "01234567890123456789012345678901234567890123456789" + // 200
                                       "01234567890123456789012345678901234567890123456789" + // 250
                                       "01234567890123456789012345678901234567890123456789" + // 300
                                       "01234567890123456789012345678901234567890123456789" + // 350
                                       "01234567890123456789012345678901234567890123456789" + // 400
                                       "01234567890123456789012345678901234567890123456789" + // 450
                                       "0123456789012345678"; // 500

    private const string Over500Chars = Max500Chars + "X"; // 501 caracteres


    // Usaremos un string especial para indicar que el objeto Documento debe ser NULL.
    private const string DOCUMENTO_NULL_SIMULATION = "NULL_DOCUMENTO";

    [Theory]
    // PARÁMETROS: CaseName, Documento_Key (string), ArchivoAWS (string), Success, ExpectedErrors

    // === 1. CASOS DE ÉXITO ===
    [InlineData(data:
        ["1. OK: Full Valid (Fisica)", TipoPersona.Fisica, "INE", ValidArchivoAWS, true, new string[] { }])]
    [InlineData(data:
    [
        "2. OK: Full Valid (Moral)", TipoPersona.Moral, "Passaporte", "s3://ruta/otro_doc.jpg", true, new string[] { }
    ])]
    [InlineData(data:
        ["3. OK: ArchivoAWS Min Length (1)", TipoPersona.Extranjero, "VISA", "A", true, new string[] { }])]
    [InlineData(data:
    [
        "4. OK: ArchivoAWS Max Length (500)", TipoPersona.Fisica, "Acta de nacimiento", Max500Chars, true,
        new string[] { }
    ])]

    // === 2. ERRORES DE ArchivoAWS (string, required, min 1, max 500) ===
    [InlineData(data:
    [
        "5. ERROR: ArchivoAWS null", TipoPersona.Fisica, "Documento test", null, false,
        new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "6. ERROR: ArchivoAWS empty", TipoPersona.Moral, "Documento test", "", false,
        new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "7. ERROR: ArchivoAWS too long (501)", TipoPersona.Extranjero, "Documento test", Over500Chars, false,
        new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]

    // === 3. ERRORES DE Documento (object, required) ===
    [InlineData(data:
    [
        "8. ERROR: Documento null", TipoPersona.Fisica, DOCUMENTO_NULL_SIMULATION, ValidArchivoAWS, false,
        new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]

    // === 4. ERRORES MÚLTIPLES ===
    [InlineData(data:
    [
        "9. ERROR: Multiple (Documento null + ArchivoAWS empty)", TipoPersona.Fisica, DOCUMENTO_NULL_SIMULATION, "",
        false,
        new string[]
        {
            ServiceErrorsBuilder.PropertyValidationRequiredError, ServiceErrorsBuilder.PropertyValidationRequiredError
        }
    ])]
    [InlineData(data:
    [
        "10. ERROR: Multiple (Documento null + ArchivoAWS too long)", TipoPersona.Fisica, DOCUMENTO_NULL_SIMULATION,
        Over500Chars, false,
        new string[]
        {
            ServiceErrorsBuilder.PropertyValidationRequiredError, ServiceErrorsBuilder.PropertyValidationLengthInvalid
        }
    ])]
    public void DocumentacionAdjunta_ConstructorTest(
        string caseName,
        TipoPersona tipoPersona,
        string? documentoKey, // Usamos para la simulación de null
        string? archivoAWS,
        bool success,
        string[]? expectedErrors = null)
    {
        // Preparación de datos: Crear el objeto Documento si no es el caso de simulación de null.
        Documento? documento = null;
        if (documentoKey != DOCUMENTO_NULL_SIMULATION)
        {
#pragma warning disable CS8604 // Possible null reference argument
            documento = new Documento(nombre: documentoKey, tipoPersona: tipoPersona, creationUser: Guid.NewGuid());
        }

        try
        {
            // Act: Crear la instancia de DocumentacionAdjunta
#pragma warning disable CS8604 // Possible null reference argument
            var documentacion = new DocumentacionAdjunta(
                documento: documento!, // Usamos '!' para permitir el paso de null en la prueba
                archivoAWS: archivoAWS!,
                creationUser: Guid.NewGuid(),
                testCase: caseName);

            // Assert Success
            Assert.True(condition: success, userMessage: $"El caso '{caseName}' falló cuando se esperaba éxito.");

            // Comprobar la asignación de propiedades (solo si hay éxito)
            Assert.Equal(expected: documento, actual: documentacion.Documento);
            Assert.Equal(expected: archivoAWS, actual: documentacion.ArchivoAWS);
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
