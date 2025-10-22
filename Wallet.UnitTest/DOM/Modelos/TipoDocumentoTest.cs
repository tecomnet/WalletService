using System;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class TipoDocumentoTest : UnitTestTemplate
{
    private const string Max100Chars = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"; // 100 caracteres
    private const string Max101Chars = Max100Chars + "X"; // 101 caracteres

    // ----------------------------------------------------------------------------------------------------------------
    // 1. TESTS DEL CONSTRUCTOR (Validación de Nombre)
    // ----------------------------------------------------------------------------------------------------------------

    [Theory]
    // PARÁMETROS: CaseName, Nombre, Success, ExpectedErrors
    [InlineData("1. OK: Nombre Válido (Full)", 
        "Cédula de Identidad", 
        true, new string[] { })]
    [InlineData("2. OK: Nombre Mínimo (1)", 
        "A", 
        true, new string[] { })]
    [InlineData("3. OK: Nombre Máximo (100)", 
        Max100Chars, 
        true, new string[] { })]
    [InlineData("4. ERROR: Nombre null",
        null, 
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("5. ERROR: Nombre empty",
        "", 
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("6. ERROR: Nombre too long (101)",
        Max101Chars, 
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
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
            Assert.Equal(nombre, tipoDocumento.Nombre);
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
    
    // ----------------------------------------------------------------------------------------------------------------
    // 2. TESTS DEL MÉTODO AgregarDocumento
    // ----------------------------------------------------------------------------------------------------------------

    [Theory]
    // PARÁMETROS: CaseName, DocumentoNombre, DocumentoTipoPersona, Intentos, 
    // Intentos: 1 = Agregar con éxito. 2 = Agregar dos veces (espera error).
    [InlineData("7. OK: Agregar el primer documento", 
        "Cedula", TipoPersona.Fisica, 1, true, new string[] { })]
    [InlineData("8. OK: Agregar dos documentos diferentes", 
        "Pasaporte", TipoPersona.Moral, 1, true, new string[] { })]
    [InlineData("9. ERROR: Agregar documento null", 
        "Licencia", TipoPersona.Extranjero, 1, false, new string[] { ServiceErrorsBuilder.DocumentoRequerido })]
    [InlineData("10. ERROR: Documento ya existe (duplicado por Nombre y TipoPersona)", 
        "RUC", TipoPersona.Moral, 2, false, new string[] { ServiceErrorsBuilder.DocumentoYaExisteEnTipoDocumento })]
    public void TipoDocumento_AgregarDocumentoTest(
        string caseName,
        string docNombre,
        TipoPersona docTipoPersona,
        int intentos, // 1 para OK, 2 para Duplicado
        bool success, // Controla si el último intento debe ser exitoso
        string[]? expectedErrors = null)
    {
        // Setup: Crear 3 documentos 
        var tipoDocumento = new TipoDocumento("Identificaciones", Guid.NewGuid());
        var documentoToAdd = new Documento(docNombre, docTipoPersona, Guid.NewGuid());
        
        try
        {
            if (caseName == "9. ERROR: Agregar documento null")
            {
                tipoDocumento.AgregarDocumento(null!); // Simular un documento null
                Assert.Fail($"El caso '{caseName}' falló al no lanzar la excepción requerida.");
            }
            
            // Primer intento: Agregar el documento inicial (siempre debe ser exitoso para la prueba de duplicidad)
            tipoDocumento.AgregarDocumento(documentoToAdd);
            
            // Si el test espera solo 1 intento (OK o caso especial), terminar aquí.
            if (intentos == 1)
            {
                Assert.Equal(1, tipoDocumento.Documentos!.Count);
                Assert.True(success, "Se esperaba éxito en el primer intento.");
            }

            // Segundo intento (si intentos = 2): Probar la duplicidad
            if (intentos == 2)
            {
                tipoDocumento.AgregarDocumento(documentoToAdd); // Intentar agregar el mismo (duplicado)
                Assert.False(success, "El segundo intento no lanzó la excepción de duplicidad.");
            }
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
