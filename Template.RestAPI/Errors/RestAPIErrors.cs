using System.Collections.Generic;
using Template.DOM;

namespace Template.RestAPI.Errors
{
    /// <summary>
    /// Rest API Error class that holds the different errors available for the API
    /// </summary>
    public class RestAPIErrors
    {
        #region Backing fields
        // Private dictionary with the errors
        private Dictionary<string, IRestAPIError> _restAPIErrors = new();
        #endregion

        #region Constructors
        /// <summary>
        /// Simple constructor that adds all the errors
        /// </summary>
        public RestAPIErrors()
        {
            // Add the GeneralErrors
            GeneralErrors();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets an error by code, if not found, the default error
        /// </summary>
        /// <param name="errorCode">Code of the error</param>
        /// <param name="dynamicContent">Dynamic contents in the form of a list of stings to create the detail</param>
        /// <returns>RestAPIError when found or DefaultRestAPIError when not</returns>
#nullable enable
        public IRestAPIError GetRestAPIError(string errorCode, List<string>? dynamicContent = null)
        {
            // Check the existence of the error code in the dictionary
            if (_restAPIErrors.ContainsKey(errorCode))
            {
                // Get the error instance
                var error = _restAPIErrors[errorCode];
                // Update the dynamic contents
                if (dynamicContent is not null)
                    ((RestAPIError)error).UpdateDynamicContent(dynamicContent: dynamicContent);
                // Return the error
                return error;
            }

            // If not, return a default error
            return new DefaultRestAPIError();
        }
        #endregion

        #region Errors
        /// <summary>
        /// Method that adds all general errors
        /// </summary>
        private void GeneralErrors()
        {
            // Bad version error
            _restAPIErrors.Add(
                "REST-API-BAD-VERSION",
                new RestAPIError(
                type: null,
                status: 400,
                errorCode: "REST-API-BAD-VERSION",
                title: "Bad version used for the REST API",
                detail: "The current supported versions for the REST API are 0.1, 0.2 and 0.4",
                instance: "DEFAULT",
                module: "REST-API",
                serviceName: DomCommon.ServiceName,
                serviceLocation: "NA"));

        }
        #endregion

    }
}
