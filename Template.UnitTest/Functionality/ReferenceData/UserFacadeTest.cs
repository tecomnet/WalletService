using Template.UnitTest.Functionality.Configuration;

namespace Template.UnitTest.Functionality.ReferenceData;

public class UserFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<object>(setupConfig) //TODO: SE DEBE PASAR LA FACADE EN EL TIPO T
{
    /*[Theory]
    // Successfully case
    [InlineData("1. Successfully case, create user", true, "userTest", true, new string[] { })]
    // Wrong cases
    [InlineData("2. Wrong case, user already exists", false, "userTest", false, new string[] { ServiceErrorsBuilder.EmUserIsDuplicated })]
    public async Task AddUserTest(
        string caseName,
        bool success,
        string username,
        bool removeUser,
        string[] expectedErrors)
    {
        try
        {
            // Validate to exists user in context
            if (removeUser)
            {
                // Remove user in context
                Context.User.Remove(await Context.User.SingleAsync());
                // Save changes in context
                await Context.SaveChangesAsync();
            }
            // Call facade method
            var user = await Facade.AddAsync(
                creationUser: SetupConfig.UserId,
                username: username,
                testCase: SetupConfig.TestCaseId);
            // Assert user created
            Assert.NotNull(user);
            // Assert user properties
            Assert.True(user.Username == username &&
                        user.CreationUser == SetupConfig.UserId);
            // Get the user from context
            var userContext = await Context.User.AsNoTracking().SingleOrDefaultAsync();
            // Confirm user created in context
            Assert.NotNull(userContext);
            // Assert user properties
            Assert.True(userContext.Username == username &&
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
    }*/

  
}