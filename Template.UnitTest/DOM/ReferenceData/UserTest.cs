namespace Template.UnitTest.DOM.ReferenceData;

public class UserTest : UnitTestTemplate
{
    /*[Theory]
    [InlineData("OK: New user", "user", true, new string[] { })]
    [InlineData("ERROR: User null", null, false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("ERROR: User empty", "", false, new string[] { "PROPERTY-VALIDATION-REQUIRED-ERROR" })]
    [InlineData("ERROR: User long string", "ThisisexampleofastringthatcontainsmorethanfiftycharactersokThisisexampleofastringthatcontainsmorethanfiftycharactersok1",
        false, new string[] { "PROPERTY-VALIDATION-LENGTH-INVALID" })]
    public void BasicUserTest(
        // Case name
        string caseName,
        // Test data
        string username,
        // Result
        bool success,
        string[]? expectedErrors = null
    )
    {
        try
        {
            // Instantiate the user
            var user = new User(
                username: username,
                creationUser: Guid.NewGuid(),
                testCase: caseName);
            // Check the properties
            Assert.True(condition: user.Username == username,
                userMessage: $"Username is not correct. Expected: {username}. " +
                             $"Actual: {user.Username}");
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
    }*/
}