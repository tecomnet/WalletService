using Wallet.DOM.Errors;
using System.Collections.Generic;

namespace Wallet.DOM
{
    /// <summary>
    /// Proporciona utilidades y constantes comunes relacionadas con el dominio de la billetera.
    /// </summary>
    public static class DomCommon
    {
        /// <summary>
        /// Define el nombre del servicio para el dominio de la billetera.
        /// </summary>
        public const string ServiceName = "WalletService";

        /// <summary>
        /// Define el nombre del módulo para el dominio de la billetera.
        /// </summary>
        private const string ModuleName = "DOM";

        /// <summary>
        /// Instancia del constructor de errores de servicio para obtener definiciones de errores.
        /// </summary>
        private static readonly ServiceErrorsBuilder ServiceError = ServiceErrorsBuilder.Instance();

        /// <summary>
        /// Construye una excepción general de tipo <see cref="EMGeneralException"/> utilizando un objeto <see cref="IServiceError"/>.
        /// Este método es privado y sirve como base para otros constructores de excepciones.
        /// </summary>
        /// <param name="serviceError">El objeto IServiceError que contiene los detalles del error.</param>
        /// <param name="dynamicContent">Una lista de objetos que representan contenido dinámico para incluir en la descripción del error.</param>
        /// <param name="module">El nombre del módulo donde se originó la excepción. Por defecto es <see cref="ModuleName"/>.</param>
        /// <returns>Una nueva instancia de <see cref="EMGeneralException"/>.</returns>
        private static EMGeneralException BuildEmGeneralException(IServiceError serviceError, List<object> dynamicContent, string module = ModuleName)
        {
            return new EMGeneralException(
                serviceError: serviceError,
                serviceName: ServiceName,
                module: module,
                descriptionDynamicContents: dynamicContent);
        }

        /// <summary>
        /// Construye una excepción general de tipo <see cref="EMGeneralException"/> a partir de un código de error.
        /// Este método busca el error correspondiente usando el <see cref="ServiceErrorsBuilder"/>.
        /// </summary>
        /// <param name="errorCode">El código de error único que identifica el tipo de error.</param>
        /// <param name="dynamicContent">Una lista de objetos que representan contenido dinámico para incluir en la descripción del error.</param>
        /// <param name="module">El nombre del módulo donde se originó la excepción. Por defecto es <see cref="ModuleName"/>.</param>
        /// <returns>Una nueva instancia de <see cref="EMGeneralException"/>.</returns>
        public static EMGeneralException BuildEmGeneralException(string errorCode, List<object> dynamicContent, string module = ModuleName)
        {
            // Obtiene el objeto de error de servicio a partir del código de error.
            var serviceError = ServiceError.GetError(errorCode: errorCode);
            // Construye la excepción general utilizando el objeto de error de servicio.
            var itaGeneralException = BuildEmGeneralException(serviceError: serviceError, dynamicContent: dynamicContent, module: module);
            return itaGeneralException;
        }
    }
}
