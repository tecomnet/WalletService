namespace Template.DOM.Errors;


using System.Collections.Generic;

public class ServiceErrorsBuilder
{
    // Almacena todos los errores de servicio por su código
    private readonly Dictionary<string, IServiceError> _errors = new();
    private static readonly Lazy<ServiceErrorsBuilder> _instance = new(() => new ServiceErrorsBuilder());

    public static ServiceErrorsBuilder Instance() => _instance.Value;
    public ServiceErrorsBuilder()
    {
        // 1. Carga inicial de todos los catálogos de errores
        GeneralErrors();
    }

    // Método privado para añadir un error al diccionario
    private void AddServiceError(string errorCode, string message, string description)
    {
        _errors[errorCode] = new ServiceError(errorCode, message, description);
    }

    /// <summary>
    /// Método público utilizado por el Middleware (capa .api) para obtener los detalles de un error.
    /// </summary>
    /// <param name="errorCode">El código de error constante (ej: "EM-MONITOR-DB-ERROR").</param>
    /// <returns>El objeto ServiceError con todos los detalles.</returns>
    public IServiceError GetError(string errorCode)
    {
        if (_errors.TryGetValue(errorCode, out var error))
        {
            return error;
        }
        
        // Retorna un error de configuración si el código no fue definido (Error 500)
        return new ServiceError(
            errorCode: "ERROR-CODE-NO-DEFINIDO", 
            message: "Error de Configuración Interna", 
            description: $"El código de error '{errorCode}' no fue definido en el catálogo.");
    }

    #region Constantes y Carga de Errores
    
    // Errores del servicio de monitoreo
    public const string EjemploDeError = "EJEMPLO-DE-ERROR";
    public const string ApiErrorNoManejado = "API-ERROR-NO-MANEJADO"; 
    private void GeneralErrors()
    {
        AddServiceError(
            errorCode: ApiErrorNoManejado, // Usa la constante pública
            message: "Error Interno del Servidor",
            description: "Ocurrió un error inesperado que ha sido registrado. Inténtelo de nuevo más tarde.");
        // Error de base de datos
        AddServiceError(
            errorCode: EjemploDeError,
            message: "Error de Conexión a la Base de Datos",
            description: "La base de datos del servicio de monitoreo no está disponible o no respondió.");
        
    }
    #endregion
}