using Template.DOM.Errors;

namespace Template.UnitTest
{
    public abstract class UnitTestTemplate
    {
        /// <summary>
        /// Method to handle the expected errors
        /// </summary>
        /// <param name="caseName">Name of the case being tested</param>
        /// <param name="success">Success flag expected</param>
        /// <param name="expectedErrors">List of expected errors</param>
        /// <param name="exception">Raised aggregate exception</param>
        private protected static void CatchErrors(
            string caseName,
            bool success,
            string[]? expectedErrors,
            EMGeneralAggregateException exception)
        {
            // Assert not null errors
            if (exception.InnerException is not null)
                Assert.True(expectedErrors is not null,
                    $"No errors defined. {exception.InnerException.Description}");
            else
                Assert.True(expectedErrors is not null, $"No errors defined.");
            // Only process when the expected errors exists
            if (expectedErrors is not null)
            {
                // Initialize the errors
                List<string> errorCodes = new();
                // Get the list of error codes
                if (exception.InnerExceptions is not null)
                    errorCodes = exception.InnerExceptions.Select(e => e.Code).ToList();
                // Match the errors
                var allErrors = true;
                // Iterate the excpetions
                foreach (var innerException in errorCodes)
                    // Check the expected errors
                    allErrors &= expectedErrors.Contains(innerException);
                // Convert the expected errors as a string
                var stringExpectedErrors = string.Empty;
                // Iterate the expected errors and concatenate them
                foreach (var expectedError in expectedErrors)
                    // Concatenate the errors
                    stringExpectedErrors += $"{expectedError} | ";
                // Convert the exception errors as a string
                var stringExceptionErrors = string.Empty;
                // Iterate the expected errors and concatenate them
                foreach (var errorCode in errorCodes)
                    // Concatenate the errors
                    stringExceptionErrors += $"{errorCode} | ";
                // Assert the expected result and errors
                Assert.True(
                    !success &&
                    allErrors,
                    $"Missing errors. Case: {caseName}: " +
                    $"Expected are {stringExpectedErrors} and got {stringExceptionErrors}");
            }
        }
    }
}

