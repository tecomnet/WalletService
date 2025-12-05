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
    /// <summary>
    /// Inicia el proceso de verificación por SMS enviando un código al número proporcionado.
    /// </summary>
    /// <param name="codigoPais">Código del país del número de teléfono.</param>
    /// <param name="telefono">Número de teléfono a verificar.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el resultado de la solicitud de verificación.</returns>
    Task<VerificacionResult> VerificacionSMS(string codigoPais, string telefono);

    /// <summary>
    /// Confirma el código de verificación enviado por SMS.
    /// </summary>
    /// <param name="codigoPais">Código del país del número de teléfono.</param>
    /// <param name="telefono">Número de teléfono a verificar.</param>
    /// <param name="codigo">Código de verificación recibido.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el resultado de la confirmación.</returns>
    Task<VerificacionResult> ConfirmarVerificacionSMS(string codigoPais, string telefono, string codigo);

    /// <summary>
    /// Inicia el proceso de verificación por correo electrónico enviando un código.
    /// </summary>
    /// <param name="correoElectronico">Dirección de correo electrónico a verificar.</param>
    /// <param name="nombreCliente">Nombre del cliente.</param>
    /// <param name="nombreEmpresa">Nombre de la empresa solicitante.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el resultado de la solicitud de verificación.</returns>
    Task<VerificacionResult> VerificacionEmail(string correoElectronico, string nombreCliente, string nombreEmpresa);

    /// <summary>
    /// Confirma el código de verificación enviado por correo electrónico.
    /// </summary>
    /// <param name="correoElectronico">Dirección de correo electrónico a verificar.</param>
    /// <param name="codigo">Código de verificación recibido.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el resultado de la confirmación.</returns>
    Task<VerificacionResult> ConfirmarVerificacionEmail(string correoElectronico, string codigo);
}

/// <summary>
/// Fachada para interactuar con el servicio de Twilio para verificaciones.
/// </summary>
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
        // Obtiene la clave API de las variables de entorno.
        var apiKey = Environment.GetEnvironmentVariable(variable: "API-Key");
        // Construye el cliente de servicio utilizando autenticación por API Key.
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
        // Construye el cliente de servicio utilizando autenticación Bearer.
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
        // Obtiene la URL base de la configuración.
        var baseUri = Environment.GetEnvironmentVariable(variable: TwilioSettingsData.RemoteServiceNameConfig);
        // Invoca BuildServiceClient para crear el cliente.
        var serviceClient = BuilServiceClient<TwilioService>(
            url: baseUri,
            // La función 'init' toma el cliente HTTP y la URL, y devuelve la instancia de TwilioService.
            init: (httpClient, baseUrl) => new TwilioService(httpClient: httpClient)
            {
                BaseUrl = baseUrl
            });
        return serviceClient;
    }

    protected override EMGeneralAggregateException? ExtractEMGeneralAggregateException(Exception exception)
    {
        // Si la excepción no es del tipo esperado (ApiException<Response>), retorna null.
        if (exception is not DOM.Comun.ApiException<Response> exception1) return null;
        // Obtiene los errores de la respuesta.
        var errors = exception1.Result.Errors;
        // Inicializa una lista de excepciones.
        List<EMGeneralException> exceptions = [];
        // Itera a través de los errores.
        foreach (var error in errors)
            // Agrega la excepción a la lista.
            exceptions.Add(item: new EMGeneralException(
                message: error.Detail,
                code: error.ErrorCode,
                title: error.Title,
                description: error.Detail,
                serviceName: TwilioSettingsData.ServiceName,
                module: this.GetType().Name,
                serviceInstance: "N/A",
                serviceLocation: "N/A"));
        // Retorna la excepción agregada.
        return new EMGeneralAggregateException(exceptions: exceptions);
    }

    #endregion


    /// <inheritdoc />
    public async Task<VerificacionResult> VerificacionSMS(string codigoPais, string telefono)
    {
        try
        {
            // Construye el cliente de servicio local con API Key.
            var serviceClient = BuildLocalServiceClientApiKey();
            // Crea el cuerpo de la solicitud de verificación SMS.
            var requestBody = new VerificacionSMSRequest()
            {
                CodigoPais = codigoPais,
                Telefono = telefono,
                NombreServicioCliente = DomCommon.ServiceName
            };
            // Realiza la petición POST para iniciar la verificación SMS.
            var response = await serviceClient.PostVerificacionSMSAsync(
                version: TwilioSettingsData.Version, body: requestBody);
            return response;
        }
        catch (Exception e)
        {
            // Maneja cualquier excepción que ocurra durante la llamada a la API.
            throw HandelAPIException(exception: e);
        }
    }

    /// <inheritdoc />
    public async Task<VerificacionResult> ConfirmarVerificacionSMS(string codigoPais, string telefono, string codigo)
    {
        try
        {
            // Construye el cliente de servicio local con API Key.
            var serviceClient = BuildLocalServiceClientApiKey();
            // Crea el cuerpo de la solicitud para confirmar la verificación SMS.
            var requestBody = new VerificacionSMSCheckRequest()
            {
                CodigoPais = codigoPais,
                Telefono = telefono,
                CodigoVerificacion = codigo,
                NombreServicioCliente = DomCommon.ServiceName
            };
            // Realiza la petición POST para confirmar el código SMS.
            var response = await serviceClient.PostVerificacionSMSCheckAsync(
                version: TwilioSettingsData.Version, body: requestBody);
            return response;
        }
        catch (Exception e)
        {
            // Maneja cualquier excepción que ocurra durante la llamada a la API.
            throw HandelAPIException(exception: e);
        }
    }

    /// <inheritdoc />
    public async Task<VerificacionResult> VerificacionEmail(string correoElectronico, string nombreCliente,
        string nombreEmpresa)
    {
        try
        {
            // Construye el cliente de servicio local con API Key.
            var serviceClient = BuildLocalServiceClientApiKey();
            // Crea el cuerpo de la solicitud de verificación por email.
            var requestBody = new VerificacionEmailRequest()
            {
                CorreoElectronico = correoElectronico,
                NombreCompleto = nombreCliente,
                TiempoExpiracion = 10,
                NombreEmpresa = nombreEmpresa,
                NombreServicioCliente = DomCommon.ServiceName
            };
            // Realiza la petición POST para iniciar la verificación por email.
            var response = await serviceClient.PostVerificacionEmailAsync(
                version: TwilioSettingsData.Version, body: requestBody);
            return response;
        }
        catch (Exception e)
        {
            // Maneja cualquier excepción que ocurra durante la llamada a la API.
            throw HandelAPIException(exception: e);
        }
    }

    /// <inheritdoc />
    public async Task<VerificacionResult> ConfirmarVerificacionEmail(string correoElectronico, string codigo)
    {
        try
        {
            // Construye el cliente de servicio local con API Key.
            var serviceClient = BuildLocalServiceClientApiKey();
            // Crea el cuerpo de la solicitud para confirmar la verificación por email.
            var requestBody = new VerificacionEmailCheckRequest()
            {
                CorreoElectronico = correoElectronico,
                CodigoVerificacion = codigo,
                NombreServicioCliente = DomCommon.ServiceName
            };
            // Realiza la petición POST para confirmar el código de email.
            var response = await serviceClient.PostVerificacionEmailCheckAsync(
                version: TwilioSettingsData.Version, body: requestBody);
            return response;
        }
        catch (Exception e)
        {
            // Maneja cualquier excepción que ocurra durante la llamada a la API.
            throw HandelAPIException(exception: e);
        }
    }
}