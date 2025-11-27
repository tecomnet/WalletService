using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class ProveedorServicioTest : UnitTestTemplate
{
    [Theory]
    // --- Casos de éxito ---
    [InlineData("OK: Datos válidos", "Proveedor A", ProductoCategoria.Servicios, "http://example.com/icon.png", true,
        new string[] { })]
    [InlineData("OK: Sin URL de ícono", "Proveedor B", ProductoCategoria.Recargas, null, true, new string[] { })]
    [InlineData("OK: Nombre en el límite",
        "Nombre de proveedor con exactamente cien caracteres para probar el límite máximo establecido en la..",
        ProductoCategoria.Movilidad, null, true, new string[] { })]

    // --- Casos de error de validación ---
    [InlineData("ERROR: Nombre nulo", null, ProductoCategoria.Servicios, null, false,
        new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("ERROR: Nombre vacío", "", ProductoCategoria.Recargas, null, false,
        new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("ERROR: Nombre excede límite",
        "Este es un nombre de proveedor que excede el límite de los cien caracteres permitidos para el campo de nombre.",
        ProductoCategoria.Movilidad, null, false, new[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    // El enum 'Categoria' no puede ser nulo si no es nullable, por lo que no se prueba el caso nulo directamente en el constructor.
    // El compilador lo previene. Si fuera `ProductoCategoria?`, se necesitaría un test.

    // --- Casos de error múltiples ---
    [InlineData("ERROR: Múltiples errores", "", ProductoCategoria.Servicios,
        "url_icono_demasiado_larga_para_ser_almacenada_en_la_base_de_datos_generalmente_mas_de_255_caracteres_lo_cual_es_una_muy_mala_practica_y_debe_evitarse_a_toda_costa_porque_no_es_necesario_y_es_muy_largo_y_no_es_buena_idea_y_deberia_ser_mas_corto",
        false, new[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    public void ConstructorValidation_Test(
        string caseName,
        string? nombre,
        ProductoCategoria categoria,
        string? urlIcono,
        bool success,
        string[]? expectedErrors = null)
    {
        try
        {
            // Act
#pragma warning disable CS8604 // Possible null reference argument
            var proveedor = new ProveedorServicio(
                nombre: nombre,
                categoria: categoria,
                urlIcono: urlIcono,
                creationUser: Guid.NewGuid()
            );

            // Assert
            Assert.True(success, $"El caso '{caseName}' debería haber tenido éxito, pero falló.");
            Assert.Equal(nombre, proveedor.Nombre);
            Assert.Equal(categoria, proveedor.Categoria);
            Assert.Equal(urlIcono, proveedor.UrlIcono);
            Assert.NotNull(proveedor.Productos);
            Assert.NotNull(proveedor.ServiciosFavoritos);
        }
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName, success, expectedErrors, exception);
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            Assert.Fail($"Excepción no gestionada en '{caseName}': {exception.Message}");
        }
    }
}
