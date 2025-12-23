using Wallet.DOM.Errors;

namespace Wallet.Funcionalidad.Functionality
{
    public static class GenericExceptionManager
    {
        /// <summary>
        /// Retorna una lista de excepciones genéricas (excepciones que no son EMGeneralAggregateException).
        /// </summary>
        /// <param name="serviceName">Nombre del servicio que genera el error.</param>
        /// <param name="module">Módulo que generó el error.</param>
        /// <param name="exception">Excepción original lanzada.</param>
        /// <returns>Una excepción agregada que contiene la lista de errores.</returns>
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