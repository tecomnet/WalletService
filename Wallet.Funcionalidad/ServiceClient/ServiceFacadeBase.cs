using Wallet.DOM;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Helper;

namespace Wallet.Funcionalidad.ServiceClient
{
    public abstract class ServiceFacadeBase(
        UrlBuilder urlBuilder,
        string runningServiceName,
        string runningModuleName,
        string remoteServiceNameConfig,
        string version)
    {
        private readonly string _unmanagedServiceErrorCode = "EM-UNMANAGED-SERVICE-CLIENT-ERROR";
        protected readonly Guid User = new Guid(g: "75BAF9A7-BBBC-4BCF-B65B-2AAE35F31050");
        protected T BuildServiceClient<T>(
            AuthorizationType authorizationType,
            string? authorization,
            string serviceErrorCode,
            Func<HttpClient, string, T> init,
            string? user = null,
            string? satellite = null) where T : class
        {
            ServiceClient<T> serviceClient;
            var baseUrl = urlBuilder.BuildUrl(serviceName: remoteServiceNameConfig);

            switch (authorizationType)
            {
                case AuthorizationType.BEARER:
                    serviceClient = ServiceClient<T>
                        .CreateBearerServiceClientFacade(
                            baseUrl: baseUrl, version: version, bearerToken: authorization);
                    break;
                case AuthorizationType.POSTMAN:
                    serviceClient = ServiceClient<T>
                        .CreatePostmanServiceClientFacade(
                            baseUrl: baseUrl, version: version, postmanApiKey: authorization);
                    break;
                case AuthorizationType.API_KEY when user != null && satellite != null:
                    serviceClient = ServiceClient<T>
                        .CreateApiKeyServiceClientFacade(
                            baseUrl: baseUrl, version: version, apiKey: authorization, userGuid: user, satellite: satellite);
                    break;
                case AuthorizationType.API_KEY when user != null:
                    serviceClient = ServiceClient<T>
                        .CreateApiKeyServiceClientFacade(
                            baseUrl: baseUrl, version: version, apiKey: authorization, userGuid: user);
                    break;
                default:
                    {
                        throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                            errorCode: serviceErrorCode,
                            dynamicContent: [authorizationType, user ?? "N/A"],
                            module: runningModuleName));
                    }
            }
            // Instantiate the service client with the corresponding HTTP client
            serviceClient.Client = init.Invoke(arg1: serviceClient.HttpClient, arg2: baseUrl);
            return serviceClient.Client;
        }
        
        protected T BuildXDistribuidorServiceClient<T>(
            string url,
            string authorization,
            string xIdDistribuidor,
            Func<HttpClient, string, T> init) where T : class
        {
            var serviceClient = ServiceClient<T>.CreateXDistribuidorClientFacade(
                baseUrl: url,
                bearerToken: authorization,
                xIdDistribuidor: xIdDistribuidor);

            // Instantiate the service client with the corresponding HTTP client
            serviceClient.Client = init.Invoke(arg1: serviceClient.HttpClient, arg2: url);
            return serviceClient.Client;
        }
        
        protected T BuildXAccesoServiceClient<T>(
            string url,
            string authorization,
            string xAcceso,
            Func<HttpClient, string, T> init) where T : class
        {
            var serviceClient = ServiceClient<T>.CreateXAccesoClientFacade(
                baseUrl: url,
                bearerToken: authorization,
                xIdAcceso: xAcceso);

            // Instantiate the service client with the corresponding HTTP client
            serviceClient.Client = init.Invoke(arg1: serviceClient.HttpClient, arg2: url);
            return serviceClient.Client;
        }

        
        protected T BuilServiceClient<T>(
            string url,
            string authorization,
            Func<HttpClient, string, T> init) where T : class
        {
            var serviceClient = ServiceClient<T>.CreateServiceClientFacade(
                baseUrl: url,
                bearerToken: authorization);

            // Instantiate the service client with the corresponding HTTP client
            serviceClient.Client = init.Invoke(arg1: serviceClient.HttpClient, arg2: url);
            return serviceClient.Client;
        }
        
           
        protected T BuilServiceClient<T>(
            string url,
            Func<HttpClient, string, T> init) where T : class
        {
            var serviceClient = ServiceClient<T>.CreateServiceClientFacadeFreeAccess(baseUrl: url, version: version);

            // Instantiate the service client with the corresponding HTTP client
            serviceClient.Client = init.Invoke(arg1: serviceClient.HttpClient, arg2: url);
            return serviceClient.Client;
        }


        protected virtual EMGeneralAggregateException HandelAPIException(Exception exception)
        {
            // If the exception has inner exceptions
            var itaGeneralAggregateException = ExtractEMGeneralAggregateException(exception: exception);
            if (itaGeneralAggregateException == null)
            {
                // Create a generic ITA general exception
                return new EMGeneralAggregateException(exception: new EMGeneralException(
                    message: exception.Message,
                    code: _unmanagedServiceErrorCode,
                    title: "Not managed service client error",
                    description: exception.Message,
                    serviceName: runningServiceName,
                    module: runningModuleName,
                    serviceInstance: "N/A",
                    serviceLocation: "N/A"));
            }

            return itaGeneralAggregateException;
        }

        protected abstract EMGeneralAggregateException? ExtractEMGeneralAggregateException(Exception exception);
    }
}
