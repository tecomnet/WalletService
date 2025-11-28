using Wallet.DOM.Errors;

namespace Wallet.Funcionalidad.Functionality
{
    public static class GenericExceptionManager
    {
        /// <summary>
        /// Return list of generic exceptions, (Not EMGeneralAggregateException exceptions)
        /// </summary>
        /// <param name="serviceName">Name of the service generating the error</param>
        /// <param name="module">Module that generated the error</param>
        /// <param name="exception">original exception raised</param>
        /// <returns></returns>
        public static EMGeneralAggregateException GetAggregateException(string serviceName
            , string module, Exception exception)
        {
            // Initialize the list of exceptions
            List<EMGeneralException> exceptions = [];
            // Make a local copy of the exception
            var localException = exception;

            while (localException != null)
            {
                exceptions.Add(item: new EMGeneralException(
                    message: localException.Message,
                    code: localException.Message,
                    title: localException.Message,
                    description: localException.Message,
                    serviceName: serviceName,
                    serviceInstance: null,
                    serviceLocation: null,
                    module: module,
                    descriptionDynamicContents: null));
                localException = localException.InnerException;
            }

            // return list of exceptions

            return new EMGeneralAggregateException(exceptions: exceptions);
        }
    }
}