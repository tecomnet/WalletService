using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Xunit.Sdk;

namespace Wallet.UnitTest.DOM.ReferenceData;

public class UserTest : UnitTestTemplate
{
    [Theory]
    [InlineData("OK: New user", "+52","9815263699", true, new string[] { })]
    [InlineData("ERROR: User null", null, null, false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("ERROR: User empty", "", "", false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("ERROR: User long string", "ThisisexampleofastringthatcontainsmorethanfiftycharactersokThisisexampleofastringthatcontainsmorethanfiftycharactersok1",
        "5959595959595959595959595",false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    public void BasicClienteTest(
        // Case name
        string caseName,
        // Test data
        string codigoPais,
        string telefono,
        // Result
        bool success,
        string[]? expectedErrors = null
    )
    {
        try
        {
            // Crea un cliente
            var user = new Cliente(
                codigoPais: codigoPais,
                telefono: telefono,
                creationUser: Guid.NewGuid(),
                testCase: caseName);
            // Check the properties
            Assert.True(condition: user.Telefono == telefono,
                userMessage: $"CodigoPais is not correct. Expected: {codigoPais}. " +
                             $"Actual: {user.CodigoPais}");
            Assert.True(condition: user.Telefono == telefono,
                userMessage: $"Telefono is not correct. Expected: {telefono}. " +
                             $"Actual: {user.Telefono}");
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
            Assert.Fail($"Uncaught exception. {exception.Message}");
        }
    }
}