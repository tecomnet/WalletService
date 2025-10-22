namespace Wallet.DOM.Errors;


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
        // 2. Errores específicos de cliente
        ClienteErrors();
        // 3. Errores específicos de TipoDocumentos
        TipoDocumentosErrors();
    }

    // Método privado para añadir un error al diccionario
    public void AddServiceError(string errorCode, string message, string description)
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

    public const string ApiErrorNoManejado = "API-ERROR-NO-MANEJADO";
    private void GeneralErrors()
    {
        // Error interno no manejado
        AddServiceError(
            errorCode: ApiErrorNoManejado, // Usa la constante pública
            message: "Error Interno del Servidor",
            description: "Ocurrió un error inesperado que ha sido registrado. Inténtelo de nuevo más tarde.");


    }

    public const string DiscpositorioMovilAutorizadoRequerido = "DISPOSITIVO-MOVIL-AUTORIZADO-REQUERIDO";
    public const string UbicacionGeolocalizacionRequerido = "UBICACION-GEOLOCALIZACION-REQUERIDO";
    public const string ContrasenasNoCoinciden = "CONTRASEÑAS-NO-COINCIDEN";
    public const string DireccionRequerida = "DIRECCION-REQUERIDA";

    public const string EmpresaRequerida = "EMPRESA-REQUERIDA";
    public const string EstadoRequerido = "ESTADO-REQUERIDO";

    public const string Verificacion2FARequerida = "VERIFICACION-2FA-REQUERIDA";

    private void ClienteErrors()
    {
        // Error de dispositivo móvil autorizado requerido
        AddServiceError(
            errorCode: DiscpositorioMovilAutorizadoRequerido,
            message: "El dispositivo móvil autorizado es requerido.",
            description: "El dispositivo móvil autorizado no puede ser nulo o vacío.");
        // Error de ubicaciones de geolocalización requerido
        AddServiceError(
            errorCode: UbicacionGeolocalizacionRequerido,
            message: "Las ubicacione de geolocalización es requerida.",
            description: "La ubicación de geolocalización no puede ser nulo o vacío.");
        // Error de contraseñas no coinciden
        AddServiceError(
            errorCode: ContrasenasNoCoinciden,
            message: "Las contraseñas no coinciden.",
            description: "La contraseña y la confirmación de la contraseña deben ser iguales.");
        // Error de dirección requerida
        AddServiceError(
            errorCode: DireccionRequerida,
            message: "La dirección es requerida.",
            description: "La dirección no puede ser nula o vacía.");
        // Error de empresa requerida
        AddServiceError(
            errorCode: EmpresaRequerida,
            message: "La empresa es requerida.",
            description: "La empresa no puede ser nula.");
        // Error de estado requerido
        AddServiceError(
            errorCode: EstadoRequerido,
            message: "El estado es requerido.",
            description: "El estado no puede ser nulo.");
        // Error de verificación 2FA requerida
        AddServiceError(
            errorCode: Verificacion2FARequerida,
            message: "La verificación 2FA es requerida.",
            description: "La verificación 2FA no puede ser nula.");
    }

    // Errores específicos de TipoDocumentos
    public const string DocumentoRequerido = "DOCUMENTO-REQUERIDO";
    public const string DocumentoYaExisteEnTipoDocumento = "DOCUMENTO-YA-EXISTE-EN-TIPO-DOCUMENTO";
    private void TipoDocumentosErrors()
    {
        // Aquí se pueden agregar errores específicos para TipoDocumentos
        // Error de documento requeridp
        AddServiceError(
            errorCode: DocumentoRequerido,
            message: "El documento es requerido.",
            description: "El documento no puede ser nulo o vacío.");
        // Error de documento ya existe en tipo documento
        AddServiceError(
            errorCode: DocumentoYaExisteEnTipoDocumento,
            message: "El documento ya existe en el tipo de documento.",
            description: "El documento proporcionado {0} de tipo persona {1} ya está asociado con otro tipo de documento.");
    }

    #endregion
}