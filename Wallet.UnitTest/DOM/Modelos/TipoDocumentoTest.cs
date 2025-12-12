using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class TipoDocumentoTest : UnitTestTemplate
{
    private const string Max100Chars =
        "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"; // 100 caracteres

    private const string Max101Chars = Max100Chars + "X"; // 101 caracteres

    // ----------------------------------------------------------------------------------------------------------------
    // 1. TESTS DEL CONSTRUCTOR (Validación de Nombre)
    // ----------------------------------------------------------------------------------------------------------------

    [Theory]
    // PARÁMETROS: CaseName, Nombre, Success, ExpectedErrors
    [InlineData(data: ["1. OK: Nombre Válido (Full)", "Cédula de Identidad", true, new string[] { }])]
    [InlineData(data: ["2. OK: Nombre Mínimo (1)", "A", true, new string[] { }])]
    [InlineData(data: ["3. OK: Nombre Máximo (100)", Max100Chars, true, new string[] { }])]
    [InlineData(data:
        ["4. ERROR: Nombre null", null, false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }])]
    [InlineData(data:
        ["5. ERROR: Nombre empty", "", false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }])]
    [InlineData(data:
    [
        "6. ERROR: Nombre too long (101)", Max101Chars, false,
        new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]
    public void TipoDocumento_ConstructorTest(
        string caseName,
        string? nombre,
        bool success,
        string[]? expectedErrors = null)
    {
        try
        {
            // Act: Crear la instancia de TipoDocumento
            var tipoDocumento = new TipoDocumento(
                nombre: nombre!,
                creationUser: Guid.NewGuid(),
                testCase: caseName);
            // Assert properties
            Assert.Equal(expected: nombre, actual: tipoDocumento.Nombre);
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

    // ----------------------------------------------------------------------------------------------------------------
    // 2. TESTS DEL MÉTODO AgregarDocumento
    // ----------------------------------------------------------------------------------------------------------------

    [Theory]
    // PARÁMETROS: CaseName, DocumentoNombre, DocumentoTipoPersona, Intentos, 
    // Intentos: 1 = Agregar con éxito. 2 = Agregar dos veces (espera error).
    [InlineData(data: ["7. OK: Agregar el primer documento", "Cedula", TipoPersona.Fisica, 1, true, new string[] { }])]
    [InlineData(data:
        ["8. OK: Agregar dos documentos diferentes", "Pasaporte", TipoPersona.Moral, 1, true, new string[] { }])]
    [InlineData(data:
    [
        "9. ERROR: Agregar documento null", "Licencia", TipoPersona.Extranjero, 1, false,
        new string[] { ServiceErrorsBuilder.DocumentoRequerido }
    ])]
    [InlineData(data:
    [
        "10. ERROR: Documento ya existe (duplicado por Nombre y TipoPersona)", "RUC", TipoPersona.Moral, 2, false,
        new string[] { ServiceErrorsBuilder.DocumentoYaExisteEnTipoDocumento }
    ])]
    public void TipoDocumento_AgregarDocumentoTest(
        string caseName,
        string docNombre,
        TipoPersona docTipoPersona,
        int intentos, // 1 para OK, 2 para Duplicado
        bool success, // Controla si el último intento debe ser exitoso
        string[]? expectedErrors = null)
    {
        // Setup: Crear 3 documentos 
        var tipoDocumento = new TipoDocumento(nombre: "Identificaciones", creationUser: Guid.NewGuid());
        var documentoToAdd =
            new Documento(nombre: docNombre, tipoPersona: docTipoPersona, creationUser: Guid.NewGuid());

        try
        {
            if (caseName == "9. ERROR: Agregar documento null")
            {
                tipoDocumento.AgregarDocumento(documento: null!,
                    modificationUser: Guid.NewGuid()); // Simular un documento null
                Assert.Fail(message: $"El caso '{caseName}' falló al no lanzar la excepción requerida.");
            }

            // Primer intento: Agregar el documento inicial (siempre debe ser exitoso para la prueba de duplicidad)
            tipoDocumento.AgregarDocumento(documento: documentoToAdd, modificationUser: Guid.NewGuid());

            // Si el test espera solo 1 intento (OK o caso especial), terminar aquí.
            if (intentos == 1)
            {
                Assert.Single(collection: tipoDocumento.Documentos!);
                Assert.True(condition: success, userMessage: "Se esperaba éxito en el primer intento.");
            }

            // Segundo intento (si intentos = 2): Probar la duplicidad
            if (intentos == 2)
            {
                tipoDocumento.AgregarDocumento(documento: documentoToAdd,
                    modificationUser: Guid.NewGuid()); // Intentar agregar el mismo (duplicado)
                Assert.False(condition: success,
                    userMessage: "El segundo intento no lanzó la excepción de duplicidad.");
            }
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
