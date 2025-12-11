using Wallet.DOM;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Helper;
using Wallet.Funcionalidad.Remoting.REST.ChecktonPldManagement;
using Response = Wallet.Funcionalidad.Remoting.REST.ChecktonPldManagement.Response;

namespace Wallet.Funcionalidad.ServiceClient;

internal static class ChecktonPldSettingsData
{
    internal const string ServiceName = "ChecktonPldService";
    internal const string Version = "0.1";
    internal const string RemoteServiceNameConfig = "checkton-pld-service";
    internal const string ServiceErrorCode = ServiceErrorsBuilder.EmIncorrectAuthorizationType;
}

public interface IChecktonPldServiceFacade
{
    /// <summary>
    /// Valida la información de una persona (CURP) contra el servicio Checkton PLD.
    /// </summary>
    /// <param name="nombre">Nombre(s) de la persona.</param>
    /// <param name="primerApellido">Primer apellido de la persona.</param>
    /// <param name="segundoApellido">Segundo apellido de la persona.</param>
    /// <param name="genero">Género de la persona.</param>
    /// <param name="fechaNacimiento">Fecha de nacimiento de la persona.</param>
    /// <param name="estado">Estado de nacimiento o residencia.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el resultado de la validación <see cref="ValidacionCurpResult"/>.</returns>
    Task<ValidacionCurpResult> ValidarChecktonPld(string nombre, string primerApellido, string segundoApellido,
        Genero genero, DateTime fechaNacimiento, string estado);
}

/// <summary>
/// Fachada para interactuar con el servicio Checkton PLD.
/// </summary>
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
        // Obtiene la clave API de las variables de entorno.
        var apiKey = Environment.GetEnvironmentVariable(variable: "API-Key");
        // Construye el cliente de servicio utilizando autenticación por API Key.
        return BuildServiceClient(
            authorizationType: AuthorizationType.API_KEY,
            authorization: apiKey,
            serviceErrorCode: ChecktonPldSettingsData.ServiceErrorCode,
            init: (client, baseUrl) => new ChecktonPldService(httpClient: client)
            {
                BaseUrl = baseUrl
            },
            user: User.ToString());
    }

    private ChecktonPldService BuildLocalServiceClientBearer(string token)
    {
        // Construye el cliente de servicio utilizando autenticación Bearer.
        return BuildServiceClient(
            authorizationType: AuthorizationType.BEARER,
            authorization: token,
            serviceErrorCode: ChecktonPldSettingsData.ServiceErrorCode,
            init: (client, baseUrl) => new ChecktonPldService(httpClient: client)
            {
                BaseUrl = baseUrl
            });
    }

    private ChecktonPldService BuildLocalServiceClient()
    {
        // Obtiene la URL base de la configuración.
        var baseUri = Environment.GetEnvironmentVariable(variable: ChecktonPldSettingsData.RemoteServiceNameConfig);
        // Invoca BuildServiceClient para crear el cliente.
        var serviceClient = BuilServiceClient<ChecktonPldService>(
            url: baseUri,
            // La función 'init' toma el cliente HTTP y la URL, y devuelve la instancia de ChecktonPldService.
            init: (httpClient, baseUrl) => new ChecktonPldService(httpClient: httpClient)
            {
                BaseUrl = baseUrl
            });
        return serviceClient;
    }

    protected override EMGeneralAggregateException? ExtractEMGeneralAggregateException(Exception exception)
    {
        // Si la excepción no es del tipo esperado (ApiException<Response>), retorna null.
        if (exception is not ApiException<Response> exception1) return null;
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
                serviceName: ChecktonPldSettingsData.ServiceName,
                module: this.GetType().Name,
                serviceInstance: "N/A",
                serviceLocation: "N/A"));
        // Retorna la excepción agregada.
        return new EMGeneralAggregateException(exceptions: exceptions);
    }

    #endregion


    /// <inheritdoc />
    public async Task<ValidacionCurpResult> ValidarChecktonPld(string nombre, string primerApellido,
        string segundoApellido, Genero genero, DateTime fechaNacimiento, string estado)
    {
        try
        {
            // Construye el cliente de servicio local con API Key.
            var serviceClient = BuildLocalServiceClientApiKey();
            // Crea el cuerpo de la solicitud de validación.
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
            // Realiza la petición POST para validar con RENAPO.
            var response = await serviceClient.PostValidaConRenapoAsync(
                version: ChecktonPldSettingsData.Version, body: requestBody);
            return response;
        }
        catch (Exception e)
        {
            // Maneja cualquier excepción que ocurra durante la llamada a la API.
            throw HandelAPIException(exception: e);
        }
    }
}