using System;
using System.Collections.Generic;
using System.Linq;
using Wallet.DOM;
using AutoMapper;
using Wallet.DOM.Enums;
using Wallet.RestAPI.Helpers;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Errors
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
            if (_restAPIErrors.ContainsKey(key: errorCode))
            {
                // Get the error instance
                var error = _restAPIErrors[key: errorCode];
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

        // Constants
        /// <summary>
        /// Error code for bad API version
        /// </summary>
        public const string RestApiBadVersion = "REST-API-BAD-VERSION";

        /// <summary>
        /// Error code for resource not found
        /// </summary>
        public const string ResourceNotFound = "RESOURCE-NOT-FOUND";

        /// <summary>
        /// Error code for generic 500 exception
        /// </summary>
        public const string EmGeneric500Exception = "EM-GENERIC-500-EXCEPTION";

        /// <summary>
        /// Error code for AutoMapper mapping exception
        /// </summary>
        public const string AutomapperMappingException = "AUTOMAPPER-MAPPING-EXCEPTION";

        /// <summary>
        /// Error code for default REST API error
        /// </summary>
        public const string RestApiDefaultError = "REST-API-DEFAULT-ERROR";

        /// <summary>
        /// Error code for general problem processing error
        /// </summary>
        public const string EmGeneralErrorProblemProcessingError = "EM-GENERAL-ERROR-PROBLEM-PROCESSING-ERROR";
        
        public const string CategoriaInvalida = "CATEGORIA-INVALIDA";

        /// <summary>
        /// Method that adds all general errors
        /// </summary>
        private void GeneralErrors()
        {
            // Bad version error
            _restAPIErrors.Add(
                key: RestApiBadVersion,
                value: new RestAPIError(
                    type: null,
                    status: 400,
                    errorCode: RestApiBadVersion,
                    title: "Versión incorrecta utilizada para la API REST",
                    detail: "Las versiones actuales soportadas para la API REST son 0.1, 0.2 y 0.4",
                    instance: "DEFAULT",
                    module: "REST-API",
                    serviceName: DomCommon.ServiceName,
                    serviceLocation: "NA"));

            // Resource not found
            _restAPIErrors.Add(
                key: ResourceNotFound,
                value: new RestAPIError(
                    type: null,
                    status: 404,
                    errorCode: ResourceNotFound,
                    title: "Recurso no encontrado",
                    detail: "El recurso solicitado no fue encontrado.",
                    instance: "DEFAULT",
                    module: "REST-API",
                    serviceName: DomCommon.ServiceName,
                    serviceLocation: "NA"));

            // Generic 500 Exception
            _restAPIErrors.Add(
                key: EmGeneric500Exception,
                value: new RestAPIError(
                    type: null,
                    status: 500,
                    errorCode: EmGeneric500Exception,
                    title: "Excepción Genérica 500",
                    detail: "Ocurrió una excepción no controlada.",
                    instance: "DEFAULT",
                    module: "REST-API",
                    serviceName: DomCommon.ServiceName,
                    serviceLocation: "NA"));

            // Automapper Mapping Exception
            _restAPIErrors.Add(
                key: AutomapperMappingException,
                value: new RestAPIError(
                    type: nameof(AutoMapperMappingException),
                    status: 500,
                    errorCode: AutomapperMappingException,
                    title: "Excepción de Mapeo de Automapper",
                    detail: "Error al mapear objetos.",
                    instance: "DEFAULT",
                    module: "REST-API",
                    serviceName: DomCommon.ServiceName,
                    serviceLocation: "NA"));

            // Default Rest API Error
            _restAPIErrors.Add(
                key: RestApiDefaultError,
                value: new RestAPIError(
                    type: null,
                    status: 500,
                    errorCode: RestApiDefaultError,
                    title: "Error Predeterminado de API REST",
                    detail: "Error predeterminado.",
                    instance: "DEFAULT",
                    module: "REST-API",
                    serviceName: DomCommon.ServiceName,
                    serviceLocation: "NA"));

            // General Problem Processing Error
            _restAPIErrors.Add(
                key: EmGeneralErrorProblemProcessingError,
                value: new RestAPIError(
                    type: null,
                    status: 400,
                    errorCode: EmGeneralErrorProblemProcessingError,
                    title: "Problema al Procesar Error",
                    detail: "Error al procesar la solicitud.",
                    instance: "DEFAULT",
                    module: "REST-API",
                    serviceName: DomCommon.ServiceName,
                    serviceLocation: "NA"));
            // Categoria invalida
            _restAPIErrors.Add(
                key: CategoriaInvalida,
                value: new RestAPIError(
                    type: null,
                    status: 400,
                    errorCode: CategoriaInvalida,
                    title: "Categoria invalida",
                    detail: $"El valor de la categoria es invalido. Valores validos son: {string.Join(", ",  EnumExtensions.GetEnumMemberValues<CategoriaEnum>())}",
                    instance: "DEFAULT",
                    module: "REST-API",
                    serviceName: DomCommon.ServiceName,
                    serviceLocation: "NA"));
        }

        #endregion
    }
}
