using Wallet.DOM;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Helper;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;

namespace Wallet.Funcionalidad.ServiceClient;

internal static class TwilioSettingsData
{
    internal const string ServiceName = "TwilioService";
    internal const string Version = "0.1";
    internal const string RemoteServiceNameConfig = "twilio-service";
    internal const string ServiceErrorCode = "EM-INCORRECT-AUTHORIZATION-TYPE";
}

public interface ITwilioServiceFacade
{
    Task<VerificacionResult> VerificacionSMS(string codigoPais, string telefono);
    Task<VerificacionResult> ConfirmarVerificacionSMS(string codigoPais, string telefono, string codigo);
    Task<VerificacionResult> VerificacionEmail(string correoElectronico, string nombreCliente, string nombreEmpresa);
    Task<VerificacionResult> ConfirmarVerificacionEmail(string correoElectronico, string codigo);
}

public class TwilioServiceFacade(
    IServiceProvider serviceProvider,
    UrlBuilder urlBuilder)
    : ServiceFacadeBase(urlBuilder: urlBuilder,
        runningServiceName: TwilioSettingsData.ServiceName,
        runningModuleName: nameof(TwilioServiceFacade),
        remoteServiceNameConfig: TwilioSettingsData.RemoteServiceNameConfig,
        version: TwilioSettingsData.Version), ITwilioServiceFacade
{
    #region private methods

    private TwilioService BuildLocalServiceClientApiKey()
    {
        // Get api key
        var apiKey = Environment.GetEnvironmentVariable(variable: "API-Key");
        // Build service client
        return BuildServiceClient(
            authorizationType: AuthorizationType.API_KEY,
            authorization: apiKey,
            serviceErrorCode: TwilioSettingsData.ServiceErrorCode,
            init: (client, baseUrl) => new TwilioService(httpClient: client)
            {
                BaseUrl = baseUrl
            },
            user: User.ToString());
    }

    private TwilioService BuildLocalServiceClientBearer(string token)
    {
        // Build service client
        return BuildServiceClient(
            authorizationType: AuthorizationType.BEARER,
            authorization: token,
            serviceErrorCode: TwilioSettingsData.ServiceErrorCode,
            init: (client, baseUrl) => new TwilioService(httpClient: client)
            {
                BaseUrl = baseUrl
            });
    }

    private TwilioService BuildLocalServiceClient()
    {
        // Get url 
        var baseUri = Environment.GetEnvironmentVariable(variable: TwilioSettingsData.RemoteServiceNameConfig);
        // 2. Invoca BuildServiceClient
        var serviceClient = BuilServiceClient<TwilioService>(
            url: baseUri,
            // La función 'init' toma el cliente HTTP y la URL, y devuelve la instancia de TwilioService
            init: (httpClient, baseUrl) => new TwilioService(httpClient: httpClient)
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
            exceptions.Add(item: new EMGeneralException(
                message: error.Detail,
                code: error.ErrorCode,
                title: error.Title,
                description: error.Detail,
                serviceName: TwilioSettingsData.ServiceName,
                module: this.GetType().Name,
                serviceInstance: "N/A",
                serviceLocation: "N/A"));
        // Throw the aggregate exception
        return new EMGeneralAggregateException(exceptions: exceptions);
    }

    #endregion


    public async Task<VerificacionResult> VerificacionSMS(string codigoPais, string telefono)
    {
        try
        {
            var serviceClient = BuildLocalServiceClientApiKey();
            var requestBody = new VerificacionSMSRequest()
            {
                CodigoPais = codigoPais,
                Telefono = telefono,
                NombreServicioCliente = DomCommon.ServiceName
            };
            var response = await serviceClient.PostVerificacionSMSAsync(
                version: TwilioSettingsData.Version, body: requestBody);
            return response;
        }
        catch (Exception e)
        {
            throw HandelAPIException(exception: e);
        }
    }

    public async Task<VerificacionResult> ConfirmarVerificacionSMS(string codigoPais, string telefono, string codigo)
    {
        try
        {
            var serviceClient = BuildLocalServiceClientApiKey();
            var requestBody = new VerificacionSMSCheckRequest()
            {
                CodigoPais = codigoPais,
                Telefono = telefono,
                CodigoVerificacion = codigo,
                NombreServicioCliente = DomCommon.ServiceName
            };
            var response = await serviceClient.PostVerificacionSMSCheckAsync(
                version: TwilioSettingsData.Version, body: requestBody);
            return response;
        }
        catch (Exception e)
        {
            throw HandelAPIException(exception: e);
        }
    }

    public async Task<VerificacionResult> VerificacionEmail(string correoElectronico, string nombreCliente,
        string nombreEmpresa)
    {
        try
        {
            var serviceClient = BuildLocalServiceClientApiKey();
            var requestBody = new VerificacionEmailRequest()
            {
                CorreoElectronico = correoElectronico,
                NombreCompleto = nombreCliente,
                TiempoExpiracion = 10,
                NombreEmpresa = nombreEmpresa,
                NombreServicioCliente = DomCommon.ServiceName
            };
            var response = await serviceClient.PostVerificacionEmailAsync(
                version: TwilioSettingsData.Version, body: requestBody);
            return response;
        }
        catch (Exception e)
        {
            throw HandelAPIException(exception: e);
        }
    }

    public async Task<VerificacionResult> ConfirmarVerificacionEmail(string correoElectronico, string codigo)
    {
        try
        {
            var serviceClient = BuildLocalServiceClientApiKey();
            var requestBody = new VerificacionEmailCheckRequest()
            {
                CorreoElectronico = correoElectronico,
                CodigoVerificacion = codigo,
                NombreServicioCliente = DomCommon.ServiceName
            };
            var response = await serviceClient.PostVerificacionEmailCheckAsync(
                version: TwilioSettingsData.Version, body: requestBody);
            return response;
        }
        catch (Exception e)
        {
            throw HandelAPIException(exception: e);
        }
    }
}