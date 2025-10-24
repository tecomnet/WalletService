using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ClienteTest;

public class ClienteFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IClienteFacade>(setupConfig) 
{
    [Theory]
    // Successfully case
    [InlineData("1. Successfully case, create user", "+52", "9818523698",true, new string[] { })]
    // Wrong cases
    [InlineData("2. Wrong case, user already exists", "+52", "9812078573", false, new string[] { ServiceErrorsBuilder.ClienteDuplicado })]
    public async Task GuardarPreRegistroClienteTest(
        string caseName,
        string codigoPais,
        string telefono,
        bool success,
        string[] expectedErrors)
    {
        try
        {
            // Call facade method
            var user = await Facade.GuardarClientePreRegistroAsync(
                creationUser: SetupConfig.UserId,
                codigoPais: codigoPais,
                telefono: telefono,
                testCase: SetupConfig.TestCaseId);
            // Assert user created
            Assert.NotNull(user);
            // Assert user properties
            Assert.True(user.CodigoPais == codigoPais &&
                        user.Telefono == telefono &&
                        user.CreationUser == SetupConfig.UserId);
            // Get the user from context
            var userContext = await Context.Cliente.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
            // Confirm user created in context
            Assert.NotNull(userContext);
            // Assert user properties
            Assert.True(userContext.CodigoPais == codigoPais &&
                        userContext.Telefono == telefono &&
                        userContext.CreationUser == SetupConfig.UserId);
            // Assert successful test
            Assert.True(success);
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