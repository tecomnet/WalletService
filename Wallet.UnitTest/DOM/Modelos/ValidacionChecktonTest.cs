using System;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class ValidacionChecktonTest : UnitTestTemplate
{
    [Theory]
    // PARÁMETROS: CaseName, TipoCheckton (enum), Resultado (bool), Success, ExpectedErrors
    
    // === CASOS DE ÉXITO ===
    [InlineData("1. OK: ListaNegra, Resultado True", 
        TipoCheckton.ListaNegra, true, 
        true, new string[] { })]
    [InlineData("2. OK: ListaRestrictivaUSA, Resultado False", 
        TipoCheckton.ListaRestrictivaUSA, false, 
        true, new string[] { })]
    [InlineData("3. OK: Curp, Resultado True", 
        TipoCheckton.Curp, true, 
        true, new string[] { })]
    // === CASO DE ERROR (Simulación de TipoCheckton null/requerido) ===
    /*[InlineData("4. ERROR: TipoCheckton null (Simulación)", 
        null, true, 
        false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]*/
    public void ValidacionCheckton_ConstructorTest(
        string caseName,
        TipoCheckton? tipoCheckton,
        bool resultado,
        bool success,
        string[]? expectedErrors = null)
    {
        try
        {
            // Act: Crear la instancia de ValidacionCheckton
            var validacion = new ValidacionCheckton(
                tipoCheckton: tipoCheckton.GetValueOrDefault(), 
                resultado: resultado,
                creationUser: Guid.NewGuid(),
                testCase: caseName);            
            // Comprobar la asignación de propiedades (solo si hay éxito)
            Assert.Equal(tipoCheckton, validacion.TipoCheckton);
            Assert.Equal(resultado, validacion.Resultado);
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
