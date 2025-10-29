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
        // 3. Errores específicos de empresa
        EmpresaErrors();
        // 4. Errores específicos de TipoDocumentos
        TipoDocumentosErrors();
        // 5. Errores específicos de Estado
        EstadoErrors();
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
    #endregion

    #region Cliente
    public const string DispositivoMovilAutorizadoRequerido = "DISPOSITIVO-MOVIL-AUTORIZADO-REQUERIDO";
    public const string UbicacionGeolocalizacionRequerido = "UBICACION-GEOLOCALIZACION-REQUERIDO";
    public const string ContrasenasNoCoinciden = "CONTRASEÑAS-NO-COINCIDEN";
    public const string ContrasenaActualIncorrecta = "CONTRASEÑA-ACTUAL-INCORRECTA";
    public const string DireccionRequerida = "DIRECCION-REQUERIDA";
    public const string DireccionNoConfigurada = "DIRECCION-NO-CONFIGURADA";
    public const string EmpresaRequerida = "EMPRESA-REQUERIDA";
    public const string EstadoRequerido = "ESTADO-REQUERIDO";
    public const string Verificacion2FARequerida = "VERIFICACION-2FA-REQUERIDA";
    public const string CodigoVerificacionInactivo = "CODIGO-VERIFICACION-INACTIVO";
    public const string CodigoVerificacionConfirmado = "CODIGO-VERIFICACION-CONFIRMADO";
    public const string CodigoVerificacionVencido = "CODIGO-VERIFICACION-VENCIDO";
    public const string CodigoVerificacionNoEncontrado = "CODIGO-VERIFICACION-NO-ENCONTRADO";
    public const string DocumentacionAdjuntaRequerida = "DOCUMENTACION-ADJUNTA-REQUERIDA";
    public const string TipoPersonaNoConfigurada = "TIPO-PERSONA-NO-CONFIGURADA";
    public const string DocumentacionAdjuntaYaExiste = "DOCUMENTACION-ADJUNTA-YA-EXISTE";
    public const string ClienteDuplicado = "CLIENTE-DUPLICADO";
    public const string ClienteDuplicadoPorCorreoElectronico = "CLIENTE-DUPLICADO-POR-CORREO-ELECTRONICO";
    public const string ClienteNoEncontrado = "CLIENTE-NO-ENCONTRADO";
    public const string DispositivoMovilAutorizadoDuplicado = "DISPOSITIVO-MOVIL-AUTORIZADO-DUPLICADO";
    

    private void ClienteErrors()
    {
        // Error de dispositivo móvil autorizado requerido
        AddServiceError(
            errorCode: DispositivoMovilAutorizadoRequerido,
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
        // Error contrasena actual incorrecta
        AddServiceError(
            errorCode: ContrasenaActualIncorrecta,
            message: "La contraseña actual es incorrecta.",
            description: "La contraseña actual no coincide con la contraseña del cliente.");
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
        // Error de código de verificación inactivo
        AddServiceError(
            errorCode: CodigoVerificacionInactivo,
            message: "El código de verificación esta inactivo.",
            description: "El código de verificación proporcionado esta inactivo.");
        // Error de código de verificación confirmado
        AddServiceError(
            errorCode: CodigoVerificacionConfirmado,
            message: "El código de verificación ya fue confirmado.",
            description: "El código de verificación proporcionado ya fue confirmado anteriormente.");
        // Error de código de verificación vencido
        AddServiceError(
            errorCode: CodigoVerificacionVencido,
            message: "El código de verificación ha vencido.",
            description: "El código de verificación proporcionado ha expirado.");
        // Error de código de verificación no encontrado
        AddServiceError(
            errorCode: CodigoVerificacionNoEncontrado,
            message: "El código de verificación no fue encontrado.",
            description: "El código de verificación proporcionado del tipo {1} no existe.");
        // Error de documentación adjunta requerida
        AddServiceError(
            errorCode: DocumentacionAdjuntaRequerida,
            message: "La documentación adjunta es requerida.",
            description: "La documentación adjunta no puede ser nula o vacía.");
        // Error de tipo de persona no configurada
        AddServiceError(
            errorCode: TipoPersonaNoConfigurada,
            message: "El tipo de persona no fue configurado.",
            description: "El tipo de persona no fue configurado. Primero debe configurar el tipo de persona.");
        // Error de documentación adjunta ya existe
        AddServiceError(
            errorCode: DocumentacionAdjuntaYaExiste,
            message: "La documentación adjunta ya existe.",
            description: "La documentación adjunta proporcionada {0} ya está asociada al cliente.");
        // Error cliente duplicado
        AddServiceError(
            errorCode: ClienteDuplicado,
            message: "El cliente ya existe.",
            description: "El cliente con codigo pais {0} y telefono {1} ya existe.");
        // Error cliente duplicado por correo electrónico
        AddServiceError(
            errorCode: ClienteDuplicadoPorCorreoElectronico,
            message: "El cliente ya existe.",
            description: "El cliente con correo electrónico {0} ya existe.");
        // Error cliente no encontrado
        AddServiceError(
            errorCode: ClienteNoEncontrado,
            message: "El cliente no fue encontrado.",
            description: "El cliente no existe.");
        // Error direccion aun no configurada
        AddServiceError(
            errorCode: DireccionNoConfigurada,
            message: "La direccion aun no esta configurada.",
            description: "La direccion aun no esta configurada.");

        // Error dispositivo movil autorizado duplicado
        AddServiceError(
            errorCode: DispositivoMovilAutorizadoDuplicado,
            message: "El dispositivo movil ya existe como autorizado.",
            description: "El dispositivo movil {0} ya existe como autorizado.");
    }


    #endregion

    #region Empresa
    public const string EmpresaNoEncontrada = "EMPRESA-NO-ENCONTRADA";
    public const string EmpresaDuplicada = "EMPRESA-DUPLICADA";
    public const string EmpresaInactiva = "EMPRESA-INACTIVA";
    private void EmpresaErrors()
    {
        // Error de empresa no encontrada
        AddServiceError(
            errorCode: EmpresaNoEncontrada,
            message: "La empresa no fue encontrada.",
            description: "La empresa {0} no existe.");
        // Error de empresa duplicada
        AddServiceError(
            errorCode: EmpresaDuplicada,
            message: "La empresa ya existe.",
            description: "La empresa {0} ya existe.");
        // Error de empresa inactiva
        AddServiceError(
            errorCode: EmpresaInactiva,
            message: "La empresa no esta activa.",
            description: "La empresa {0} no esta activa. Primero debe activarla.");
    }
    #endregion

    #region TipoDocumentos
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


    #region  Estado
    public const string EstadoDuplicado = "ESTADO-DUPLICADO";
    public const string EstadoNoEncontrado = "ESTADO-NO-ENCONTRADO";
    public const string EstadoInactivo = "ESTADO-INACTIVO";
    private void EstadoErrors()
    {
        // Error de estado duplicado
        AddServiceError(
            errorCode: EstadoDuplicado,
            message: "El estado ya existe.",
            description: "El estado {0} ya existe.");
        // Error estado no encontrado
        AddServiceError(
            errorCode: EstadoNoEncontrado,
            message: "El estado no fue encontrado.",
            description: "El estado {0} no existe.");
        // Error de estado inactivo
        AddServiceError(
            errorCode: EstadoInactivo,
            message: "El estado no esta activo.",
            description: "El estado {0} no esta activo. Primero debe activarlo.");
    }
    #endregion
}