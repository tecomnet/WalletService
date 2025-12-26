using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionCliente;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class DireccionTest : UnitTestTemplate
{
    [Theory]
    // CASOS DE ÉXITO
    [InlineData(data:
    [
        "1. OK: Full Valid", "12345", "México", "Jalisco", "Guadalajara", "Centro", "Av. Principal", "123", "Apto 1",
        "Cerca del parque", true, new string[] { }
    ])]
    [InlineData(data: ["2. OK: Min length", "12345", "A", "B", "C", "D", "E", "F", "1", "G", true, new string[] { }])]
    [InlineData(data:
    [
        "3. OK: Max length", "99999",
        "Nombre de país con exactamente cien caracteres. Un total de cien caracteres para el nombre del país.",
        "Nombre de estado con exactamente cien caracteres.Total de cien caracteres para el nombre del estado",
        "Nombre de municipio con exactamente cien caracteres.De cien caracteres para el nombre del municipio",
        "Nombre de colonia con exactamente cien caracteres. De cien caracteres para el nombre de la colonia",
        "Nombre de calle con exactamente cien caracteres.Total de cien caracteres para el nombre de la calle", "123456",
        "123456",
        "Referencia con exactamente doscientos cincuenta caracteres. Un total de doscientos cincuenta caracteres para la referencia.",
        true, new string[] { }
    ])]

    // CASOS DE ERROR PARA CÓDIGO POSTAL (string, min 5, max 5)
    [InlineData(data:
    [
        "4. ERROR: Codigo Postal null", null, "México", "Jalisco", "Guadalajara", "Centro", "Av. Principal", "123",
        "Apto 1", "Cerca del parque", false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "5. ERROR: Codigo Postal empty", "", "México", "Jalisco", "Guadalajara", "Centro", "Av. Principal", "123",
        "Apto 1", "Cerca del parque", false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "6. ERROR: Codigo Postal too short", "1234", "México", "Jalisco", "Guadalajara", "Centro", "Av. Principal",
        "123", "Apto 1", "Cerca del parque", false,
        new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]
    [InlineData(data:
    [
        "7. ERROR: Codigo Postal too long", "123456", "México", "Jalisco", "Guadalajara", "Centro", "Av. Principal",
        "123", "Apto 1", "Cerca del parque", false,
        new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]
    // CASOS DE ERROR PARA PAÍS (string, min 1, max 100)
    [InlineData(data:
    [
        "8. ERROR: País null", "12345", null, "Jalisco", "Guadalajara", "Centro", "Av. Principal", "123", "Apto 1",
        "Cerca del parque", false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "9. ERROR: País empty", "12345", "", "Jalisco", "Guadalajara", "Centro", "Av. Principal", "123", "Apto 1",
        "Cerca del parque", false, new string[] { ServiceErrorsBuilder.PropertyValidationRequiredError }
    ])]
    [InlineData(data:
    [
        "10. ERROR: País too long", "12345",
        "Nombre de país con exactamente cien caracteres. Un total de cien caracteres para el nombre del país. Mas de cien",
        "Nombre de estado con exactamente cien caracteres.Total de cien caracteres para el nombre del estado",
        "Nombre de municipio con exactamente cien caracteres.De cien caracteres para el nombre del municipio",
        "Nombre de colonia con exactamente cien caracteres. De cien caracteres para el nombre de la colonia",
        "Nombre de calle con exactamente cien caracteres.Total de cien caracteres para el nombre de la calle", "123456",
        "123456",
        "Referencia con exactamente doscientos cincuenta caracteres. Un total de doscientos cincuenta caracteres para la referencia.",
        false, new string[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]
    // MÁS CASOS DE ERROR PARA OTRAS PROPIEDADES SE PUEDEN AGREGAR AQUÍ...
    public void Direccion_Constructor_Tests(
        string caseName,
        string? codigoPostal,
        string? pais,
        string estado,
        string municipio,
        string colonia,
        string calle,
        string numeroExterior,
        string numeroInterior,
        string referencia,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Crea direccion con país y estado
#pragma warning disable CS8604 // Possible null reference argument
            Direccion direccion = new Direccion(
                pais: pais,
                estado: estado,
                creationUser: Guid.NewGuid());

            Assert.NotNull(direccion);
            Assert.Equal(expected: pais, actual: direccion.Pais);
            Assert.Equal(expected: estado, actual: direccion.Estado);

            // Actualiza el resto de la dirección
#pragma warning disable CS8604 // Possible null reference argument
            direccion.ActualizarDireccion(
                codigoPostal: codigoPostal,
                municipio: municipio,
                colonia: colonia,
                calle: calle,
                numeroExterior: numeroExterior,
                numeroInterior: numeroInterior,
                referencia: referencia,
                modificationUser: Guid.NewGuid());
            // Validamos los valores actualizados
            Assert.NotNull(direccion);
            Assert.Equal(expected: codigoPostal, actual: direccion.CodigoPostal);
            Assert.Equal(expected: municipio, actual: direccion.Municipio);
            Assert.Equal(expected: colonia, actual: direccion.Colonia);
            Assert.Equal(expected: calle, actual: direccion.Calle);
            Assert.Equal(expected: numeroExterior, actual: direccion.NumeroExterior);
            Assert.Equal(expected: numeroInterior, actual: direccion.NumeroInterior);
            Assert.Equal(expected: referencia, actual: direccion.Referencia);

            // 3. Verificación Final de Éxito
            Assert.True(condition: success, userMessage: $"El caso '{caseName}' falló cuando se esperaba éxito.");
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
