using Wallet.DOM;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Helper;

namespace Wallet.Funcionalidad.ServiceClient
{
    /// <summary>
    /// Clase base abstracta para las fachadas de servicios.
    /// Proporciona métodos para construir clientes de servicio con diferentes tipos de autorización y manejo de excepciones.
    /// </summary>
    /// <param name="urlBuilder">Constructor de URLs para los servicios.</param>
    /// <param name="runningServiceName">Nombre del servicio que se está ejecutando.</param>
    /// <param name="runningModuleName">Nombre del módulo que se está ejecutando.</param>
    /// <param name="remoteServiceNameConfig">Nombre de configuración del servicio remoto.</param>
    /// <param name="version">Versión del servicio.</param>
    public abstract class ServiceFacadeBase(
        UrlBuilder urlBuilder,
        string runningServiceName,
        string runningModuleName,
        string remoteServiceNameConfig,
        string version)
    {
        private readonly string _unmanagedServiceErrorCode = "EM-UNMANAGED-SERVICE-CLIENT-ERROR";
        protected readonly Guid User = new Guid(g: "75BAF9A7-BBBC-4BCF-B65B-2AAE35F31050");

        /// <summary>
        /// Construye un cliente de servicio genérico.
        /// </summary>
        /// <typeparam name="T">El tipo del cliente de servicio.</typeparam>
        /// <param name="authorizationType">El tipo de autorización a utilizar.</param>
        /// <param name="authorization">El token o clave de autorización.</param>
        /// <param name="serviceErrorCode">El código de error a lanzar si falla la creación del cliente.</param>
        /// <param name="init">Función de inicialización del cliente.</param>
        /// <param name="user">Opcional. Identificador del usuario (para API Key).</param>
        /// <param name="satellite">Opcional. Identificador del satélite (para API Key).</param>
        /// <returns>Una instancia del cliente de servicio configurado.</returns>
        protected T BuildServiceClient<T>(
            AuthorizationType authorizationType,
            string? authorization,
            string serviceErrorCode,
            Func<HttpClient, string, T> init,
            string? user = null,
            string? satellite = null) where T : class
        {
            ServiceClient<T> serviceClient;
            // Construye la URL base del servicio remoto.
            var baseUrl = urlBuilder.BuildUrl(serviceName: remoteServiceNameConfig);

            switch (authorizationType)
            {
                case AuthorizationType.BEARER:
                    // Crea un cliente con autenticación Bearer.
                    serviceClient = ServiceClient<T>
                        .CreateBearerServiceClientFacade(
                            baseUrl: baseUrl, version: version, bearerToken: authorization);
                    break;
                case AuthorizationType.POSTMAN:
                    // Crea un cliente con autenticación Postman.
                    serviceClient = ServiceClient<T>
                        .CreatePostmanServiceClientFacade(
                            baseUrl: baseUrl, version: version, postmanApiKey: authorization);
                    break;
                case AuthorizationType.API_KEY when user != null && satellite != null:
                    // Crea un cliente con autenticación API Key, usuario y satélite.
                    serviceClient = ServiceClient<T>
                        .CreateApiKeyServiceClientFacade(
                            baseUrl: baseUrl, version: version, apiKey: authorization, userGuid: user,
                            satellite: satellite);
                    break;
                case AuthorizationType.API_KEY when user != null:
                    // Crea un cliente con autenticación API Key y usuario.
                    serviceClient = ServiceClient<T>
                        .CreateApiKeyServiceClientFacade(
                            baseUrl: baseUrl, version: version, apiKey: authorization, userGuid: user);
                    break;
                default:
                {
                    // Lanza una excepción si el tipo de autorización no es soportado o faltan parámetros.
                    throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                        errorCode: serviceErrorCode,
                        dynamicContent: [authorizationType, user ?? "N/A"],
                        module: runningModuleName));
                }
            }

            // Instancia el cliente de servicio con el cliente HTTP correspondiente.
            serviceClient.Client = init.Invoke(arg1: serviceClient.HttpClient, arg2: baseUrl);
            return serviceClient.Client;
        }

        /// <summary>
        /// Construye un cliente de servicio con cabecera X-Distribuidor.
        /// </summary>
        /// <typeparam name="T">El tipo del cliente de servicio.</typeparam>
        /// <param name="url">La URL base del servicio.</param>
        /// <param name="authorization">El token de autorización.</param>
        /// <param name="xIdDistribuidor">El ID del distribuidor para la cabecera.</param>
        /// <param name="init">Función de inicialización del cliente.</param>
        /// <returns>Una instancia del cliente de servicio configurado.</returns>
        protected T BuildXDistribuidorServiceClient<T>(
            string url,
            string authorization,
            string xIdDistribuidor,
            Func<HttpClient, string, T> init) where T : class
        {
            // Crea el cliente con la configuración específica para X-Distribuidor.
            var serviceClient = ServiceClient<T>.CreateXDistribuidorClientFacade(
                baseUrl: url,
                bearerToken: authorization,
                xIdDistribuidor: xIdDistribuidor);

            // Instancia el cliente de servicio con el cliente HTTP correspondiente.
            serviceClient.Client = init.Invoke(arg1: serviceClient.HttpClient, arg2: url);
            return serviceClient.Client;
        }

        /// <summary>
        /// Construye un cliente de servicio con cabecera X-Acceso.
        /// </summary>
        /// <typeparam name="T">El tipo del cliente de servicio.</typeparam>
        /// <param name="url">La URL base del servicio.</param>
        /// <param name="authorization">El token de autorización.</param>
        /// <param name="xAcceso">El valor de acceso para la cabecera.</param>
        /// <param name="init">Función de inicialización del cliente.</param>
        /// <returns>Una instancia del cliente de servicio configurado.</returns>
        protected T BuildXAccesoServiceClient<T>(
            string url,
            string authorization,
            string xAcceso,
            Func<HttpClient, string, T> init) where T : class
        {
            // Crea el cliente con la configuración específica para X-Acceso.
            var serviceClient = ServiceClient<T>.CreateXAccesoClientFacade(
                baseUrl: url,
                bearerToken: authorization,
                xIdAcceso: xAcceso);

            // Instancia el cliente de servicio con el cliente HTTP correspondiente.
            serviceClient.Client = init.Invoke(arg1: serviceClient.HttpClient, arg2: url);
            return serviceClient.Client;
        }


        /// <summary>
        /// Construye un cliente de servicio básico con autorización Bearer.
        /// </summary>
        /// <typeparam name="T">El tipo del cliente de servicio.</typeparam>
        /// <param name="url">La URL base del servicio.</param>
        /// <param name="authorization">El token de autorización.</param>
        /// <param name="init">Función de inicialización del cliente.</param>
        /// <returns>Una instancia del cliente de servicio configurado.</returns>
        protected T BuilServiceClient<T>(
            string url,
            string authorization,
            Func<HttpClient, string, T> init) where T : class
        {
            // Crea el cliente de servicio básico.
            var serviceClient = ServiceClient<T>.CreateServiceClientFacade(
                baseUrl: url,
                bearerToken: authorization);

            // Instancia el cliente de servicio con el cliente HTTP correspondiente.
            serviceClient.Client = init.Invoke(arg1: serviceClient.HttpClient, arg2: url);
            return serviceClient.Client;
        }


        /// <summary>
        /// Construye un cliente de servicio de acceso libre (sin autorización).
        /// </summary>
        /// <typeparam name="T">El tipo del cliente de servicio.</typeparam>
        /// <param name="url">La URL base del servicio.</param>
        /// <param name="init">Función de inicialización del cliente.</param>
        /// <returns>Una instancia del cliente de servicio configurado.</returns>
        protected T BuilServiceClient<T>(
            string url,
            Func<HttpClient, string, T> init) where T : class
        {
            // Crea el cliente de servicio de acceso libre.
            var serviceClient = ServiceClient<T>.CreateServiceClientFacadeFreeAccess(baseUrl: url, version: version);

            // Instancia el cliente de servicio con el cliente HTTP correspondiente.
            serviceClient.Client = init.Invoke(arg1: serviceClient.HttpClient, arg2: url);
            return serviceClient.Client;
        }


        /// <summary>
        /// Maneja las excepciones de la API, envolviéndolas en una excepción agregada estándar.
        /// </summary>
        /// <param name="exception">La excepción original capturada.</param>
        /// <returns>Una excepción <see cref="EMGeneralAggregateException"/> procesada.</returns>
        protected virtual EMGeneralAggregateException HandelAPIException(Exception exception)
        {
            // Intenta extraer una EMGeneralAggregateException de la excepción original.
            var itaGeneralAggregateException = ExtractEMGeneralAggregateException(exception: exception);
            if (itaGeneralAggregateException == null)
            {
                // Si no se puede extraer, crea una excepción genérica no gestionada.
                return new EMGeneralAggregateException(exception: new EMGeneralException(
                    message: exception.Message,
                    code: _unmanagedServiceErrorCode,
                    title: "Error de cliente de servicio no gestionado",
                    description: exception.Message,
                    serviceName: runningServiceName,
                    module: runningModuleName,
                    serviceInstance: "N/A",
                    serviceLocation: "N/A"));
            }

            return itaGeneralAggregateException;
        }

        /// <summary>
        /// Método abstracto para extraer una excepción <see cref="EMGeneralAggregateException"/> de una excepción dada.
        /// Debe ser implementado por las clases derivadas.
        /// </summary>
        /// <param name="exception">La excepción a procesar.</param>
        /// <returns>Una <see cref="EMGeneralAggregateException"/> si se puede extraer, o null.</returns>
        protected abstract EMGeneralAggregateException? ExtractEMGeneralAggregateException(Exception exception);
    }
}
