namespace Wallet.DOM.Errors;

using System.Collections.Generic;

/// <summary>
/// Clase constructora para gestionar y registrar los errores de servicio en la aplicación.
/// Utiliza un patrón Singleton para mantener un catálogo centralizado de errores.
/// </summary>
public class ServiceErrorsBuilder
{
    // Almacena todos los errores de servicio por su código
    private readonly Dictionary<string, IServiceError> _errors = new();

    // Instancia Lazy para el patrón Singleton
    private static readonly Lazy<ServiceErrorsBuilder> _instance = new(valueFactory: () => new ServiceErrorsBuilder());

    /// <summary>
    /// Obtiene la instancia única de la clase <see cref="ServiceErrorsBuilder"/>.
    /// </summary>
    /// <returns>La instancia singleton de <see cref="ServiceErrorsBuilder"/>.</returns>
    public static ServiceErrorsBuilder Instance() => _instance.Value;

    /// <summary>
    /// Constructor privado para inicializar el catálogo de errores.
    /// Carga todos los errores definidos en las diferentes regiones.
    /// </summary>
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
        // 6. Errores específicos de autenticación
        AuthenticationErrors();
        // 7. Errores específicos de Proveedor
        ProveedorErrors();
        // 8. Errores específicos de Broker
        BrokerErrors();
        // 8. Errores de validación de propiedades
        PropertyValidationErrors();
        // 9. Errores de ServicioFavorito
        ServicioFavoritoErrors();
        // 10. Errores de Registro
        RegistroErrors();
        // 11. Errores de KeyValueConfig
        KeyValueConfigErrors();
        // 12. Errores de GestionWallet
        GestionWalletErrors();
        // 13. Errores de BitacoraTransaccion
        BitacoraTransaccionErrors();
    }

    /// <summary>
    /// Método privado para añadir un nuevo error al diccionario de errores.
    /// </summary>
    /// <param name="errorCode">El código único del error.</param>
    /// <param name="message">El mensaje descriptivo del error.</param>
    /// <param name="description">La descripción detallada del error.</param>
    public void AddServiceError(string errorCode, string message, string description)
    {
        _errors[key: errorCode] = new ServiceError(errorCode: errorCode, message: message, description: description);
    }

    /// <summary>
    /// Método público utilizado por el Middleware (capa .api) para obtener los detalles de un error.
    /// </summary>
    /// <param name="errorCode">El código de error constante (ej: "EM-MONITOR-DB-ERROR").</param>
    /// <returns>El objeto ServiceError con todos los detalles.</returns>
    public IServiceError GetError(string errorCode)
    {
        if (_errors.TryGetValue(key: errorCode, value: out var error))
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

    /// <summary>
    /// Código de error para excepciones no manejadas por la API.
    /// </summary>
    public const string ApiErrorNoManejado = "API-ERROR-NO-CONTROLADO";

    /// <summary>
    /// Carga los errores generales del sistema.
    /// </summary>
    private void GeneralErrors()
    {
        // Error interno no manejado
        AddServiceError(
            errorCode: ApiErrorNoManejado, // Usa la constante pública
            message: "Error Interno del Servidor",
            description: "Ocurrió un error inesperado que ha sido registrado. Inténtelo de nuevo más tarde.");

        // Error de concurrencia optimista
        AddServiceError(
            errorCode: ConcurrencyError,
            message: "El registro ha sido modificado por otro usuario.",
            description:
            "El registro que intenta actualizar ha sido modificado por otro usuario. Por favor, recargue los datos e intente nuevamente.");

        // Error de token de concurrencia inválido
        AddServiceError(
            errorCode: ConcurrencyTokenInvalido,
            message: "El token de concurrencia es inválido.",
            description: "El token de concurrencia proporcionado '{0}' no tiene un formato válido.");

        // Error de token de concurrencia requerido
        AddServiceError(
            errorCode: ConcurrencyTokenRequerido,
            message: "El token de concurrencia es requerido.",
            description: "El token de concurrencia es obligatorio para esta operación.");
    }

    /// <summary>Error: El registro fue modificado por otro usuario.</summary>
    public const string ConcurrencyError = "ERROR-CONCURRENCIA-OPTIMISTA";

    /// <summary>Error: El token de concurrencia es inválido.</summary>
    public const string ConcurrencyTokenInvalido = "ERROR-TOKEN-CONCURRENCIA-INVALIDO";

    /// <summary>Error: El token de concurrencia es requerido.</summary>
    public const string ConcurrencyTokenRequerido = "ERROR-TOKEN-CONCURRENCIA-REQUERIDO";

    /// <summary>Error: El tipo de autorización es incorrecto.</summary>
    public const string EmIncorrectAuthorizationType = "EM-TIPO-AUTORIZACION-INCORRECTO";

    /// <summary>Error: Error no gestionado del cliente de servicio.</summary>
    public const string EmUnmanagedServiceClientError = "EM-ERROR-CLIENTE-SERVICIO-NO-CONTROLADO";

    #endregion

    #region Cliente

    /// <summary>Error: El usuario está inactivo.</summary>
    public const string UsuarioInactivo = "USUARIO-INACTIVO";

    /// <summary>Error: El dispositivo móvil autorizado es requerido.</summary>
    public const string DispositivoMovilAutorizadoRequerido = "DISPOSITIVO-MOVIL-AUTORIZADO-REQUERIDO";

    /// <summary>Error: La ubicación de geolocalización es requerida.</summary>
    public const string UbicacionGeolocalizacionRequerido = "UBICACION-GEOLOCALIZACION-REQUERIDO";

    /// <summary>Error: Las contraseñas no coinciden.</summary>
    public const string ContrasenasNoCoinciden = "CONTRASEÑAS-NO-COINCIDEN";

    /// <summary>Error: La contraseña actual es incorrecta.</summary>
    public const string ContrasenaActualIncorrecta = "CONTRASEÑA-ACTUAL-INCORRECTA";

    /// <summary>Error: La dirección es requerida.</summary>
    public const string DireccionRequerida = "DIRECCION-REQUERIDA";

    /// <summary>Error: La dirección no está configurada.</summary>
    public const string DireccionNoConfigurada = "DIRECCION-NO-CONFIGURADA";

    /// <summary>Error: La empresa es requerida.</summary>
    public const string EmpresaRequerida = "EMPRESA-REQUERIDA";

    /// <summary>Error: El estado es requerido.</summary>
    public const string EstadoRequerido = "ESTADO-REQUERIDO";

    /// <summary>Error: La verificación 2FA es requerida.</summary>
    public const string Verificacion2FARequerida = "VERIFICACION-2FA-REQUERIDA";

    /// <summary>Error: El código de verificación está inactivo.</summary>
    public const string CodigoVerificacionInactivo = "CODIGO-VERIFICACION-INACTIVO";

    /// <summary>Error: El código de verificación ya fue confirmado.</summary>
    public const string CodigoVerificacionConfirmado = "CODIGO-VERIFICACION-CONFIRMADO";

    /// <summary>Error: El código de verificación ha vencido.</summary>
    public const string CodigoVerificacionVencido = "CODIGO-VERIFICACION-VENCIDO";

    /// <summary>Error: El código de verificación no fue encontrado.</summary>
    public const string CodigoVerificacionNoEncontrado = "CODIGO-VERIFICACION-NO-ENCONTRADO";

    /// <summary>Error: La documentación adjunta es requerida.</summary>
    public const string DocumentacionAdjuntaRequerida = "DOCUMENTACION-ADJUNTA-REQUERIDA";

    /// <summary>Error: El tipo de persona no está configurado.</summary>
    public const string TipoPersonaNoConfigurada = "TIPO-PERSONA-NO-CONFIGURADA";

    /// <summary>Error: La documentación adjunta ya existe.</summary>
    public const string DocumentacionAdjuntaYaExiste = "DOCUMENTACION-ADJUNTA-YA-EXISTE";

    /// <summary>Error: El cliente está duplicado.</summary>
    public const string ClienteDuplicado = "CLIENTE-DUPLICADO";

    /// <summary>Error: El cliente está duplicado por correo electrónico.</summary>
    public const string ClienteDuplicadoPorCorreoElectronico = "CLIENTE-DUPLICADO-POR-CORREO-ELECTRONICO";

    /// <summary>Error: El cliente no fue encontrado.</summary>
    public const string ClienteNoEncontrado = "CLIENTE-NO-ENCONTRADO";

    /// <summary>Error: El dispositivo móvil autorizado está duplicado.</summary>
    public const string DispositivoMovilAutorizadoDuplicado = "DISPOSITIVO-MOVIL-AUTORIZADO-DUPLICADO";

    /// <summary>Error: El cliente está inactivo.</summary>
    public const string ClienteInactivo = "CLIENTE-INACTIVO";

    /// <summary>Error: El correo electrónico del cliente no está configurado.</summary>
    public const string ClienteCorreoElectronicoNoConfigurado = "CLIENTE-CORREO-ELECTRONICO-NO-CONFIGURADO";

    /// <summary>Error: La verificación 2FA SMS no fue confirmada.</summary>
    public const string Verificacion2FASMSNoConfirmado = "VERIFICACION-2FA-SMS-NO-CONFIRMADO";

    /// <summary>Error: La contraseña ya existe.</summary>
    public const string ContrasenaYaExiste = "CONTRASEÑA-YA-EXISTE";

    /// <summary>Error: Error en la validación PLD de Checkton.</summary>
    public const string ClienteChecktonPldError = "CLIENTE-CHECKTON-PLD-ERROR";

    /// <summary>Error: La validación de Checkton es requerida.</summary>
    public const string ValidacionChecktonRequerida = "VALIDACION-CHECKTON-REQUERIDA";

    /// <summary>
    /// Carga los errores relacionados con la entidad Cliente.
    /// </summary>
    private void ClienteErrors()
    {
        // Error de dispositivo móvil autorizado requerido
        AddServiceError(
            errorCode: DispositivoMovilAutorizadoRequerido,
            message: "El dispositivo móvil autorizado es requerido.",
            description: "El dispositivo móvil autorizado no puede ser nulo o vacío.");
        // Error de ubicación de geolocalización requerida
        AddServiceError(
            errorCode: UbicacionGeolocalizacionRequerido,
            message: "La ubicación de geolocalización es requerida.",
            description: "La ubicación de geolocalización no puede ser nula o vacía.");
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
            message: "El código de verificación está inactivo.",
            description: "El código de verificación proporcionado está inactivo.");
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
            description: "El código de verificación proporcionado del tipo {0} no existe.");
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
        // Error de dirección aún no configurada
        AddServiceError(
            errorCode: DireccionNoConfigurada,
            message: "La dirección aún no está configurada.",
            description: "La dirección aún no está configurada.");
        // Error de dispositivo móvil autorizado duplicado
        AddServiceError(
            errorCode: DispositivoMovilAutorizadoDuplicado,
            message: "El dispositivo movil ya existe como autorizado.",
            description: "El dispositivo movil ya existe como autorizado.");
        // Error cliente inactivo
        AddServiceError(
            errorCode: ClienteInactivo,
            message: "El cliente no está activo.",
            description: "El cliente {0} no está activo. Primero debe activarlo.");
        // Error de correo electrónico no configurado
        AddServiceError(
            errorCode: ClienteCorreoElectronicoNoConfigurado,
            message: "El cliente no tiene correo electrónico configurado",
            description: "El cliente {0} no tiene correo electrónico configurado");
        // Error de tipo de dispositivo inválido
        AddServiceError(
            errorCode: DispositivoInvalido,
            message: "El tipo de dispositivo es inválido.",
            description: "El valor {0} no es un tipo de dispositivo válido.");
        // Error de verificación 2FA SMS no confirmado
        AddServiceError(
            errorCode: Verificacion2FASMSNoConfirmado,
            message: "La verificación 2FA SMS no fue confirmada.",
            description: "La verificación 2FA SMS no fue confirmada. Primero debe confirmar la verificación 2FA SMS.");
        // Error de contraseña ya existe
        AddServiceError(
            errorCode: ContrasenaYaExiste,
            message: "La contraseña ya existe.",
            description: "La contraseña ya existe. Primero debe actualizar la contraseña.");
        // Error de validación PLD de Checkton
        AddServiceError(
            errorCode: ClienteChecktonPldError,
            message: "Error en la validación del cliente.",
            description: "Error en la validación de datos personales del cliente.");
        // Error de validación Checkton requerida
        AddServiceError(
            errorCode: ValidacionChecktonRequerida,
            message: "La validación checkton es requerida.",
            description: "La validación checkton es requerida.");
        // Error de usuario no encontrado
        AddServiceError(
            errorCode: UsuarioNoEncontrado,
            message: "El usuario no fue encontrado.",
            description: "El usuario no existe.");
        // Error de cliente ya registrado
        AddServiceError(
            errorCode: ClienteYaRegistrado,
            message: "El usuario ya se encuentra registrado.",
            description: "Inicia sesión para continuar.");
    }

    /// <summary>Error: El usuario no fue encontrado.</summary>
    public const string UsuarioNoEncontrado = "USUARIO-NO-ENCONTRADO";

    /// <summary>Error: El cliente ya está registrado.</summary>
    public const string ClienteYaRegistrado = "CLIENTE-YA-REGISTRADO";

    /// <summary>Error: La cuenta wallet está inactiva.</summary>
    public const string CuentaWalletInactiva = "CUENTA-WALLET-INACTIVA";

    /// <summary>Error: La cuenta wallet no fue encontrada.</summary>
    public const string CuentaWalletNoEncontrada = "CUENTA-WALLET-NO-ENCONTRADA";

    /// <summary>Error: La dirección está inactiva.</summary>
    public const string DireccionInactiva = "DIRECCION-INACTIVA";

    /// <summary>Error: El tipo de dispositivo es inválido.</summary>
    public const string DispositivoInvalido = "DISPOSITIVO-INVALIDO";

    #endregion

    #region Empresa

    /// <summary>Error: La empresa no fue encontrada.</summary>
    public const string EmpresaNoEncontrada = "EMPRESA-NO-ENCONTRADA";

    /// <summary>Error: La empresa está duplicada.</summary>
    public const string EmpresaDuplicada = "EMPRESA-DUPLICADA";

    /// <summary>Error: La empresa está inactiva.</summary>
    public const string EmpresaInactiva = "EMPRESA-INACTIVA";

    /// <summary>
    /// Carga los errores relacionados con la entidad Empresa.
    /// </summary>
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
            message: "La empresa no está activa.",
            description: "La empresa {0} no está activa. Primero debe activarla.");
    }

    #endregion

    #region TipoDocumentos

    // Errores específicos de TipoDocumentos
    /// <summary>Error: El documento es requerido.</summary>
    public const string DocumentoRequerido = "DOCUMENTO-REQUERIDO";

    /// <summary>Error: El documento ya existe en el tipo de documento.</summary>
    public const string DocumentoYaExisteEnTipoDocumento = "DOCUMENTO-YA-EXISTE-EN-TIPO-DOCUMENTO";

    /// <summary>
    /// Carga los errores relacionados con la entidad TipoDocumento.
    /// </summary>
    private void TipoDocumentosErrors()
    {
        // Aquí se pueden agregar errores específicos para TipoDocumentos
        // Error de documento requerido
        AddServiceError(
            errorCode: DocumentoRequerido,
            message: "El documento es requerido.",
            description: "El documento no puede ser nulo o vacío.");
        // Error de documento ya existente en tipo de documento
        AddServiceError(
            errorCode: DocumentoYaExisteEnTipoDocumento,
            message: "El documento ya existe en el tipo de documento.",
            description:
            "El documento proporcionado {0} de tipo persona {1} ya está asociado con otro tipo de documento.");
    }

    #endregion


    #region Estado

    /// <summary>Error: El estado está duplicado.</summary>
    public const string EstadoDuplicado = "ESTADO-DUPLICADO";

    /// <summary>Error: El estado no fue encontrado.</summary>
    public const string EstadoNoEncontrado = "ESTADO-NO-ENCONTRADO";

    /// <summary>Error: El estado está inactivo.</summary>
    public const string EstadoInactivo = "ESTADO-INACTIVO";

    /// <summary>
    /// Carga los errores relacionados con la entidad Estado.
    /// </summary>
    private void EstadoErrors()
    {
        // Error de estado duplicado
        AddServiceError(
            errorCode: EstadoDuplicado,
            message: "El estado ya existe.",
            description: "El estado {0} ya existe.");
        // Error de estado no encontrado
        AddServiceError(
            errorCode: EstadoNoEncontrado,
            message: "El estado no fue encontrado.",
            description: "El estado {0} no existe.");
        // Error de estado inactivo
        AddServiceError(
            errorCode: EstadoInactivo,
            message: "El estado no está activo.",
            description: "El estado {0} no está activo. Primero debe activarlo.");
    }

    #endregion

    #region Errores de Autenticación

    /// <summary>Error: Error en el claim de usuario.</summary>
    public const string EmClaimUserError = "EM-ERROR-USUARIO-CLAIM";

    /// <summary>
    /// Carga los errores relacionados con la autenticación.
    /// </summary>
    private void AuthenticationErrors()
    {
        // Error de autenticación
        AddServiceError(
            errorCode: EmClaimUserError,
            message: "Error de autenticación",
            description: "El usuario de autenticación no es válido o no fue encontrado");

        // Error: Credenciales inválidas
        AddServiceError(
            errorCode: CredencialesInvalidas,
            message: "Credenciales inválidas",
            description: "Las credenciales proporcionadas no son válidas.");

        // Error: Token inválido
        AddServiceError(
            errorCode: TokenInvalido,
            message: "Token inválido",
            description: "El token proporcionado no es válido.");

        // Error: Refresh token inválido o expirado
        AddServiceError(
            errorCode: RefreshTokenInvalido,
            message: "Refresh token inválido o expirado",
            description: "El token de refresco proporcionado no es válido o ha expirado.");
    }

    /// <summary>Error: Credenciales inválidas.</summary>
    public const string CredencialesInvalidas = "CREDENCIALES-INVALIDAS";

    /// <summary>Error: Token inválido.</summary>
    public const string TokenInvalido = "TOKEN-INVALIDO";

    /// <summary>Error: Refresh token inválido.</summary>
    public const string RefreshTokenInvalido = "REFRESH-TOKEN-INVALIDO";

    #endregion

    #region Broker

    /// <summary>Error: El broker no fue encontrado.</summary>
    public const string BrokerNoEncontrado = "BROKER-NO-ENCONTRADO";

    public const string BrokerExistente = "BROKER-EXISTENTE";
    public const string BrokerInactivo = "BROKER-INACTIVO";

    /// <summary>
    /// Carga los errores relacionados con la entidad Broker.
    /// </summary>
    private void BrokerErrors()
    {
        // Error de broker no encontrado
        AddServiceError(
            errorCode: BrokerNoEncontrado,
            message: "El broker no fue encontrado.",
            description: "El broker con id {0} no existe.");
        // Error de broker existente
        AddServiceError(
            errorCode: BrokerExistente,
            message: "El broker ya existe.",
            description: "El broker {0} ya existe.");
        // Error de broker inactivo
        AddServiceError(
            errorCode: BrokerInactivo,
            message: "El broker no está activo.",
            description: "El broker {0} no está activo. Primero debe activarlo.");
    }

    #endregion

    #region Proveedor

    /// <summary>Error: El proveedor de servicio no fue encontrado.</summary>
    public const string ProveedorNoEncontrado = "PROVEEDOR-NO-ENCONTRADO";

    public const string ProveedorExistente = "PROVEEDOR-EXISTENTE";
    public const string ProveedorInactivo = "PROVEEDOR-INACTIVO";


    /// <summary>Error: El producto del proveedor no fue encontrado.</summary>
    public const string ProductoNoEncontrado = "PRODUCTO-NO-ENCONTRADO";

    public const string ProductoExistente = "PRODUCTO-EXISTENTE";
    public const string ProductoSkuExistente = "PRODUCTO-SKU-EXISTENTE";
    public const string ProductoInactivo = "PRODUCTO-INACTIVO";
    public const string ProductoCategoriaInvalida = "PRODUCTO-CATEGORIA-INVALIDA";


    /// <summary>
    /// Carga los errores relacionados con la entidad Proveedor.
    /// </summary>
    private void ProveedorErrors()
    {
        // Error de proveedor de servicio no encontrado
        AddServiceError(
            errorCode: ProveedorNoEncontrado,
            message: "El proveedor de servicio no fue encontrado.",
            description: "El proveedor de servicio con id {0} no existe.");
        // Error de proveedor existente
        AddServiceError(
            errorCode: ProveedorExistente,
            message: "El proveedor de servicio ya existe.",
            description: "El proveedor de servicio {0} ya existe.");
        // Error de proveedor inactivo
        AddServiceError(
            errorCode: ProveedorInactivo,
            message: "El proveedor de servicio no está activo.",
            description: "El proveedor de servicio {0} no está activo. Primero debe activarlo.");
        // Error de producto del proveedor no encontrado
        AddServiceError(
            errorCode: ProductoNoEncontrado,
            message: "El producto del proveedor de servicio no fue encontrado.",
            description: "El producto del proveedor de servicio con id {0} no existe.");
        // Error de producto existente
        AddServiceError(
            errorCode: ProductoExistente,
            message: "El producto del proveedor de servicio ya existe.",
            description: "El producto del proveedor de servicio {0} ya existe.");
        // Error de producto sku existente
        AddServiceError(
            errorCode: ProductoSkuExistente,
            message: "El SKU del producto ya existe.",
            description: "El SKU {0} ya está registrado para este proveedor.");
        // Error de producto inactivo
        AddServiceError(
            errorCode: ProductoInactivo,
            message: "El producto del proveedor de servicio no está activo.",
            description: "El producto del proveedor de servicio {0} no está activo. Primero debe activarlo.");
        // Error de categoria invalida
        AddServiceError(
            errorCode: ProductoCategoriaInvalida,
            message: "La categoría del producto es inválida.",
            description: "La categoría provides {0} no es válida.");
    }

    #endregion

    #region Validación de Propiedades

    /// <summary>Error: Validación de propiedad requerida.</summary>
    public const string PropertyValidationRequiredError = "ERROR-VALIDACION-PROPIEDAD-REQUERIDA";

    /// <summary>Error: Longitud de propiedad inválida.</summary>
    public const string PropertyValidationLengthInvalid = "ERROR-VALIDACION-PROPIEDAD-LONGITUD-INVALIDA";

    /// <summary>Error: Regex de propiedad inválido.</summary>
    public const string PropertyValidationRegexInvalid = "ERROR-VALIDACION-PROPIEDAD-REGEX-INVALIDO";

    /// <summary>Error: Valor negativo no permitido en propiedad.</summary>
    public const string PropertyValidationNegativeInvalid = "ERROR-VALIDACION-PROPIEDAD-NEGATIVA-INVALIDA";

    /// <summary>Error: Valor cero no permitido en propiedad.</summary>
    public const string PropertyValidationZeroInvalid = "ERROR-VALIDACION-PROPIEDAD-CERO-INVALIDO";

    /// <summary>Error: Valor positivo no permitido en propiedad.</summary>
    public const string PropertyValidationPositiveInvalid = "ERROR-VALIDACION-PROPIEDAD-POSITIVA-INVALIDA";

    /// <summary>Error: Decimales inválidos en propiedad.</summary>
    public const string PropertyValidationDecimalsInvalid = "ERROR-VALIDACION-PROPIEDAD-DECIMALES-INVALIDOS";

    /// <summary>Error: Moneda inválida en propiedad.</summary>
    public const string PropertyValidationCurrencyInvalid = "ERROR-VALIDACION-PROPIEDAD-MONEDA-INVALIDA";

    /// <summary>Error: Propiedad no encontrada para validación.</summary>
    public const string PropertyValidationPropertyNotFound = "ERROR-VALIDACION-PROPIEDAD-NO-ENCONTRADA";

    /// <summary>
    /// Carga los errores relacionados con la validación de propiedades.
    /// </summary>
    private void PropertyValidationErrors()
    {
        // Error de validación: propiedad requerida
        AddServiceError(errorCode: PropertyValidationRequiredError, message: "Error de validación de propiedad",
            description: "La propiedad {0} es requerida.");
        // Error de validación: longitud inválida
        AddServiceError(errorCode: PropertyValidationLengthInvalid, message: "Error de validación de propiedad",
            description: "La propiedad {0} con valor {1} debe tener entre {2} y {3} caracteres de longitud.");
        // Error de validación: regex inválido
        AddServiceError(errorCode: PropertyValidationRegexInvalid, message: "Error de validación de propiedad",
            description: "La propiedad {0} con valor {1} no es válida según el patrón proporcionado {2}.");
        // Error de validación: valor negativo no permitido
        AddServiceError(errorCode: PropertyValidationNegativeInvalid, message: "Error de validación de propiedad",
            description: "La propiedad {0} con valor {1} no permite valores negativos.");
        // Error de validación: valor cero no permitido
        AddServiceError(errorCode: PropertyValidationZeroInvalid, message: "Error de validación de propiedad",
            description: "La propiedad {0} con valor {1} no permite valores cero.");
        // Error de validación: valor positivo no permitido
        AddServiceError(errorCode: PropertyValidationPositiveInvalid, message: "Error de validación de propiedad",
            description: "La propiedad {0} con valor {1} no permite valores positivos.");
        // Error de validación: decimales inválidos
        AddServiceError(errorCode: PropertyValidationDecimalsInvalid, message: "Error de validación de propiedad",
            description: "La propiedad {0} con valor {1} no permite más de {2} decimales.");
        // Error de validación: moneda inválida
        AddServiceError(errorCode: PropertyValidationCurrencyInvalid, message: "Error de validación de propiedad",
            description: "La propiedad {0} con valor {1} no es una divisa/moneda válida.");
        // Error de validación: propiedad no encontrada
        AddServiceError(errorCode: PropertyValidationPropertyNotFound, message: "Error de validación de propiedad",
            description: "La propiedad {0} no fue encontrada en la definición de la propiedad.");
    }

    #endregion

    #region ServicioFavorito

    /// <summary>Error: Servicio favorito no encontrado.</summary>
    public const string ServicioFavoritoNoEncontrado = "SERVICIO-FAVORITO-NO-ENCONTRADO";

    /// <summary>Error: Servicio favorito inactivo.</summary>
    public const string ServicioFavoritoInactivo = "SERVICIO-FAVORITO-INACTIVO";

    /// <summary>
    /// Carga los errores relacionados con la entidad ServicioFavorito.
    /// </summary>
    private void ServicioFavoritoErrors()
    {
        // Error de servicio favorito no encontrado
        AddServiceError(
            errorCode: ServicioFavoritoNoEncontrado,
            message: "Servicio Favorito no encontrado",
            description: "El servicio favorito con id {0} no fue encontrado.");

        // Error de servicio favorito inactivo
        AddServiceError(
            errorCode: ServicioFavoritoInactivo,
            message: "Servicio Favorito inactivo",
            description: "El servicio favorito con id {0} no está activo. Primero debe activarlo.");
    }

    #endregion

    #region Registro

    /// <summary>Error: El estado del registro es inválido para la operación solicitada.</summary>
    public const string InvalidRegistrationState = "ESTADO-REGISTRO-INVALIDO";

    /// <summary>Error: Los términos y condiciones no fueron aceptados.</summary>
    public const string TerminosNoAceptados = "TERMINOS-NO-ACEPTADOS";

    /// <summary>
    /// Carga los errores relacionados con el proceso de Registro.
    /// </summary>
    private void RegistroErrors()
    {
        // Error de estado de registro inválido
        AddServiceError(
            errorCode: InvalidRegistrationState,
            message: "Estado de registro inválido.",
            description: "El usuario no se encuentra en el estado requerido para realizar esta operación.");

        // Error de términos no aceptados
        AddServiceError(
            errorCode: TerminosNoAceptados,
            message: "Términos y condiciones no aceptados.",
            description: "Debe aceptar los términos y condiciones, política de privacidad y PLD para continuar.");
    }

    #endregion

    #region KeyValueConfig

    /// <summary>Error: La configuración KeyValue no fue encontrada.</summary>
    public const string KeyValueConfigNoEncontrado = "KEY-VALUE-CONFIG-NO-ENCONTRADO";

    /// <summary>Error: La configuración KeyValue ya existe.</summary>
    public const string KeyValueConfigYaExiste = "KEY-VALUE-CONFIG-YA-EXISTE";

    /// <summary>Error: La configuración KeyValue está inactiva.</summary>
    public const string KeyValueConfigInactivo = "KEY-VALUE-CONFIG-INACTIVO";

    /// <summary>
    /// Carga los errores relacionados con la entidad KeyValueConfig.
    /// </summary>
    private void KeyValueConfigErrors()
    {
        // Error de configuración no encontrada
        AddServiceError(
            errorCode: KeyValueConfigNoEncontrado,
            message: "La configuración no fue encontrada.",
            description: "La configuración con clave {0} no existe.");

        // Error de configuración ya existente
        AddServiceError(
            errorCode: KeyValueConfigYaExiste,
            message: "La configuración ya existe.",
            description: "La configuración con clave {0} ya existe.");

        // Error de configuración inactiva
        AddServiceError(
            errorCode: KeyValueConfigInactivo,
            message: "La configuración está inactiva.",
            description: "La configuración con clave {0} no está activa. Primero debe activarla.");
    }

    #endregion

    #region GestionWallet

    /// <summary>Error: Operación logística no permitida para tarjeta virtual.</summary>
    public const string TarjetaVirtualNoLogistica = "TARJETA-VIRTUAL-NO-LOGISTICA";

    /// <summary>Error: La tarjeta ya se encuentra vinculada.</summary>
    public const string TarjetaYaVinculada = "TARJETA-YA-VINCULADA";

    /// <summary>Error: La tarjeta ha expirado.</summary>
    public const string TarjetaExpirada = "TARJETA-EXPIRADA";

    /// <summary>Error: La tarjeta no fue encontrada.</summary>
    public const string TarjetaNoEncontrada = "TARJETA-NO-ENCONTRADA";

    /// <summary>Error: La tarjeta vinculada no fue encontrada.</summary>
    public const string TarjetaVinculadaNoEncontrada = "TARJETA-VINCULADA-NO-ENCONTRADA";

    /// <summary>Error: El detalle de pago no fue encontrado.</summary>
    public const string DetallePagoNoEncontrado = "DETALLE-PAGO-NO-ENCONTRADO";

    /// <summary>
    /// Carga los errores relacionados con GestionWallet (Tarjetas).
    /// </summary>
    private void GestionWalletErrors()
    {
        // Error de logística en tarjeta virtual
        AddServiceError(
            errorCode: TarjetaVirtualNoLogistica,
            message: "Operación no permitida.",
            description: "No se pueden realizar operaciones de logística/entrega en una tarjeta virtual.");

        // Error de tarjeta expirada
        AddServiceError(
            errorCode: TarjetaExpirada,
            message: "Tarjeta Expirada.",
            description: "La tarjeta ha expirado y no puede realizar operaciones.");

        // Error de tarjeta ya vinculada
        AddServiceError(
            errorCode: TarjetaYaVinculada,
            message: "Tarjeta ya vinculada.",
            description: "La tarjeta ya se encuentra vinculada a la cuenta.");

        // Error de wallet no encontrada
        AddServiceError(
            errorCode: CuentaWalletNoEncontrada,
            message: "La wallet no fue encontrada.",
            description: "No se encontró wallet para el cliente {0}.");

        // Error de tarjeta no encontrada
        AddServiceError(
            errorCode: TarjetaNoEncontrada,
            message: "Tarjeta no encontrada.",
            description: "La tarjeta con id {0} no fue encontrada.");

        // Error de tarjeta vinculada no encontrada
        AddServiceError(
            errorCode: TarjetaVinculadaNoEncontrada,
            message: "Tarjeta vinculada no encontrada.",
            description: "La tarjeta vinculada con id {0} no fue encontrada.");

        // Error de detalle de pago no encontrado
        AddServiceError(
            errorCode: DetallePagoNoEncontrado,
            message: "Detalle de pago no encontrado.",
            description: "El detalle de pago con id {0} no fue encontrado.");
    }

    #endregion

    #region BitacoraTransaccion

    /// <summary>Error: La transacción no fue encontrada.</summary>
    public const string BitacoraTransaccionNoEncontrada = "BITACORA-TRANSACCION-NO-ENCONTRADA";

    /// <summary>
    /// Carga los errores relacionados con BitacoraTransaccion.
    /// </summary>
    private void BitacoraTransaccionErrors()
    {
        // Error de transacción no encontrada
        AddServiceError(
            errorCode: BitacoraTransaccionNoEncontrada,
            message: "Transaccion no encontrada.",
            description: "Transaccion con ID {0} no encontrada.");
    }

    #endregion
}
