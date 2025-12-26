using Moq;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionEmpresa;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.Modelos;

public class ProveedorTest : UnitTestTemplate
{
    [Theory]
    // --- Casos de éxito ---
    [InlineData(data: ["OK: Datos válidos", 1, "Proveedor A", true, new string[] { }])]
    [InlineData(data:
    [
        "OK: Nombre en el límite", 2,
        "Nombre de proveedor con exactamente cien caracteres para probar el límite máximo establecido en la..", true,
        new string[] { }
    ])]

    // --- Casos de error de validación ---
    [InlineData(data:
        ["ERROR: Nombre nulo", 1, null, false, new[] { ServiceErrorsBuilder.PropertyValidationRequiredError }])]
    [InlineData(data:
        ["ERROR: Nombre vacío", 1, "", false, new[] { ServiceErrorsBuilder.PropertyValidationRequiredError }])]
    [InlineData(data:
    [
        "ERROR: Nombre excede límite", 1,
        "Este es un nombre de proveedor que excede el límite de los cien caracteres permitidos para el campo de nombre.",
        false, new[] { ServiceErrorsBuilder.PropertyValidationLengthInvalid }
    ])]
    public void ConstructorValidation_Test(
        string caseName,
        int brokerId,
        string? nombre,
        bool success,
        string[]? expectedErrors = null)
    {
        try
        {
            // Arrange
            var mockBroker = new Mock<Broker>();
            mockBroker.SetupGet(b => b.Id).Returns(brokerId);

            // Act
            var proveedor = new Proveedor(
                broker: mockBroker.Object,
                urlIcono: "https://cfe.mx/logo.png",
                nombre: nombre,
                creationUser: Guid.NewGuid()
            );

            // Assert
            Assert.True(condition: success,
                userMessage: $"El caso '{caseName}' debería haber tenido éxito, pero falló.");
            Assert.Equal(expected: brokerId, actual: proveedor.BrokerId);
            Assert.Equal(expected: nombre, actual: proveedor.Nombre);
            Assert.NotNull(proveedor.Productos);
        }
        catch (EMGeneralAggregateException exception)
        {
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            Assert.Fail(message: $"Excepción no gestionada en '{caseName}': {exception.Message}");
        }
    }
}
