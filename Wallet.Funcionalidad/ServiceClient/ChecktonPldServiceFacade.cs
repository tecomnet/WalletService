using Wallet.DOM;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Helper;
using Wallet.Funcionalidad.Remoting.REST.ChecktonPldManagement;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;
using Response = Wallet.Funcionalidad.Remoting.REST.ChecktonPldManagement.Response;

namespace Wallet.Funcionalidad.ServiceClient;

internal static class ChecktonPldSettingsData
{
    internal const string ServiceName = "ChecktonPldService"; 
    internal const string Version = "0.1";
    internal const string RemoteServiceNameConfig = "checkton-pld-service";
    internal const string ServiceErrorCode = "EM-INCORRECT-AUTHORIZATION-TYPE";
}

public interface IChecktonPldServiceFacade
{
    Task<ValidacionCurpResult> ValidarChecktonPld(string nombre, string primerApellido, string segundoApellido, Genero genero, DateTime fechaNacimiento, string estado);
}

public class ChecktonPldServiceFacade(
    IServiceProvider serviceProvider,
    UrlBuilder urlBuilder)
    : ServiceFacadeBase(urlBuilder: urlBuilder,
        runningServiceName: ChecktonPldSettingsData.ServiceName,
        runningModuleName: nameof(ChecktonPldServiceFacade),
        remoteServiceNameConfig: ChecktonPldSettingsData.RemoteServiceNameConfig,
        version: ChecktonPldSettingsData.Version), IChecktonPldServiceFacade
{
    #region private methods
    private ChecktonPldService BuildLocalServiceClientApiKey()
    {
        // Get api key
        var apiKey = Environment.GetEnvironmentVariable("API-Key");
        // Build service client
        return BuildServiceClient(
            authorizationType: AuthorizationType.API_KEY,
            authorization: apiKey,
            serviceErrorCode: ChecktonPldSettingsData.ServiceErrorCode,
            init: (client, baseUrl) => new ChecktonPldService(client)
            {
                BaseUrl = baseUrl
            },
            user: User.ToString());
    }
    private ChecktonPldService BuildLocalServiceClientBearer(string token)
    {
        // Build service client
        return BuildServiceClient(
            authorizationType: AuthorizationType.BEARER,
            authorization: token,
            serviceErrorCode: ChecktonPldSettingsData.ServiceErrorCode,
            init: (client, baseUrl) => new ChecktonPldService(client)
            {
                BaseUrl = baseUrl
            });
    }
    private ChecktonPldService BuildLocalServiceClient()
    {
        // Get url 
        var baseUri = Environment.GetEnvironmentVariable(ChecktonPldSettingsData.RemoteServiceNameConfig);
        // 2. Invoca BuildServiceClient
        var serviceClient = BuilServiceClient<ChecktonPldService>(
            url: baseUri, 
            // La función 'init' toma el cliente HTTP y la URL, y devuelve la instancia de ChecktonPldService
            init: (httpClient, baseUrl) => new ChecktonPldService(httpClient)
            {
                BaseUrl = baseUrl
            });
        return serviceClient;
    }

    protected override EMGeneralAggregateException? ExtractEMGeneralAggregateException(Exception exception)
    {
        // If the exception has inner exceptions
        if (exception is not ApiException<Response> exception1) return null;
        // Get the errors
        var errors = exception1.Result.Errors;
        // Initialize a list of exceptions
        List<EMGeneralException> exceptions = [];
        // Iterate through the errors
        foreach (var error in errors)
            // Add the exception
            exceptions.Add(new EMGeneralException(
                message: error.Detail,
                code: error.ErrorCode,
                title: error.Title,
                description: error.Detail,
                serviceName: ChecktonPldSettingsData.ServiceName,
                module: this.GetType().Name,
                serviceInstance: "N/A",
                serviceLocation: "N/A"));
        // Throw the aggregate exception
        return new EMGeneralAggregateException(exceptions: exceptions);
    }

    #endregion


    public async Task<ValidacionCurpResult> ValidarChecktonPld(string nombre, string primerApellido, string segundoApellido, Genero genero, DateTime fechaNacimiento, string estado)
    {
        try
        {
            var serviceClient = BuildLocalServiceClientApiKey();
            var requestBody = new ValidacionCurpRequest()
            {
                Nombre = nombre,
                PrimerApellido = primerApellido,
                SegundoApellido = segundoApellido,
                Genero = (GeneroEnum)genero,
                FechaNacimiento = fechaNacimiento,
                Estado = estado,
                NombreServicioCliente = DomCommon.ServiceName
            };
            var response = await serviceClient.PostValidaConRenapoAsync(
                version: ChecktonPldSettingsData.Version, body: requestBody);
            return response;
        }
        catch (Exception e)
        {
            throw HandelAPIException(e);
        }
    }
}