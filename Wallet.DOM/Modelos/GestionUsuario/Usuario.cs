using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionCliente;

namespace Wallet.DOM.Modelos.GestionUsuario;

/// <summary>
/// Representa un usuario dentro del sistema Wallet.
/// Gestiona la información de autenticación, contacto y relaciones con otros módulos como clientes y empresas.
/// </summary>
public class Usuario : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de validación para las propiedades de la entidad Usuario.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(CodigoPais),
            isRequired: true,
            minimumLength: 3,
            maximumLength: 3),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Telefono),
            isRequired: true,
            minimumLength: 9,
            maximumLength: 10),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(CorreoElectronico),
            isRequired: false,
            minimumLength: 1,
            maximumLength: 150,
            regex: @"^[\w\.-]+@([\w-]+\.)+[\w-]{2,}$"),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Contrasena),
            isRequired: false,
            minimumLength: 1,
            maximumLength: 100),
    ];

    /// <summary>
    /// Código del país del número de teléfono del usuario.
    /// </summary>
    [Required]
    [MaxLength(length: 3)]
    public string CodigoPais { get; private set; }

    /// <summary>
    /// Número de teléfono del usuario.
    /// </summary>
    [Required]
    [MaxLength(length: 10)]
    public string Telefono { get; private set; }

    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    [EmailAddress]
    [MaxLength(length: 150)]
    public string? CorreoElectronico { get; private set; }

    /// <summary>
    /// Contraseña del usuario (hash).
    /// </summary>
    [MaxLength(length: 100)]
    public string? Contrasena { get; private set; }

    /// <summary>
    /// Estatus actual del usuario.
    /// </summary>
    [Required]
    public EstatusRegistroEnum Estatus { get; private set; }

    /// <summary>
    /// Token de refresco para la sesión del usuario.
    /// </summary>
    [MaxLength(length: 200)]
    public string? RefreshToken { get; private set; }

    /// <summary>
    /// Fecha de expiración del token de refresco.
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    /// <summary>
    /// Entidad del cliente asociado, si aplica.
    /// </summary>
    public Cliente? Cliente { get; private set; }

    /// <summary>
    /// Lista de verificaciones de dos factores (2FA) asociadas al usuario.
    /// </summary>
    public List<Verificacion2FA> Verificaciones2Fa { get; private set; } = new();

    /// <summary>
    /// Lista de ubicaciones de geolocalización registradas por el usuario.
    /// </summary>
    public virtual List<UbicacionesGeolocalizacion> UbicacionesGeolocalizacion { get; private set; } = new();

    /// <summary>
    /// Lista de dispositivos móviles autorizados para el usuario.
    /// </summary>
    public List<DispositivoMovilAutorizado> DispositivoMovilAutorizados { get; private set; } = new();

    /// <summary>
    /// Constructor por defecto requerido por Entity Framework.
    /// </summary>
    public Usuario()
    {
        CodigoPais = string.Empty;
        Telefono = string.Empty;
        Estatus = EstatusRegistroEnum.PreRegistro;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="Usuario"/>.
    /// </summary>
    /// <param name="codigoPais">Código del país.</param>
    /// <param name="telefono">Número de teléfono.</param>
    /// <param name="correoElectronico">Correo electrónico (opcional).</param>
    /// <param name="contrasena">Contraseña (opcional).</param>
    /// <param name="estatus">Estatus inicial.</param>
    /// <param name="creationUser">Usuario que crea el registro.</param>
    /// <param name="testCase">Caso de prueba (opcional).</param>
    public Usuario(
        string codigoPais,
        string telefono,
        string? correoElectronico,
        string? contrasena,
        EstatusRegistroEnum estatus,
        Guid creationUser,
        string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        List<EMGeneralException> exceptions = new();
        // Validar propiedades iniciales
        IsPropertyValid(propertyName: nameof(CodigoPais), value: codigoPais, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Telefono), value: telefono, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(CorreoElectronico), value: correoElectronico, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasena, exceptions: ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        this.CodigoPais = codigoPais;
        this.Telefono = telefono;
        this.CorreoElectronico = correoElectronico;
        this.Contrasena = !string.IsNullOrEmpty(value: contrasena) ? BCrypt.Net.BCrypt.HashPassword(inputKey: contrasena) : null;
        this.Estatus = estatus;

        Verificaciones2Fa = new List<Verificacion2FA>();
        this.UbicacionesGeolocalizacion = new List<UbicacionesGeolocalizacion>();
        this.DispositivoMovilAutorizados = new List<DispositivoMovilAutorizado>();
    }

    /// <summary>
    /// Crea una contraseña para el usuario si no tiene una asignada.
    /// </summary>
    /// <param name="contrasena">La nueva contraseña.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Si ya existe una contraseña o la nueva es inválida.</exception>
    public void CrearContrasena(string contrasena, Guid modificationUser)
    {
        // Verificar si ya tiene contraseña
        if (!string.IsNullOrWhiteSpace(value: this.Contrasena))
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenaYaExiste,
                dynamicContent: []));
        }

        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasena, exceptions: ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        this.Contrasena = !string.IsNullOrEmpty(value: contrasena) ? BCrypt.Net.BCrypt.HashPassword(inputKey: contrasena) : null;
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Actualiza la contraseña del usuario validando la actual.
    /// </summary>
    /// <param name="contrasenaNueva">La nueva contraseña.</param>
    /// <param name="confirmacionContrasenaNueva">Confirmación de la nueva contraseña.</param>
    /// <param name="contrasenaActual">La contraseña actual para validación.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Si las contraseñas no coinciden o la actual es incorrecta.</exception>
    public void ActualizarContrasena(string contrasenaNueva, string confirmacionContrasenaNueva,
        string contrasenaActual, Guid modificationUser)
    {
        // Validar coincidencia de contraseñas nuevas
        if (contrasenaNueva != confirmacionContrasenaNueva)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenasNoCoinciden,
                dynamicContent: []));
        }

        // Validar contraseña actual
        if (!string.IsNullOrEmpty(value: this.Contrasena) &&
            !BCrypt.Net.BCrypt.Verify(text: contrasenaActual, hash: this.Contrasena))
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenaActualIncorrecta,
                dynamicContent: []));
        }

        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasenaNueva, exceptions: ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        this.Contrasena = !string.IsNullOrEmpty(value: contrasenaNueva)
            ? BCrypt.Net.BCrypt.HashPassword(inputKey: contrasenaNueva)
            : null;
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Actualiza el número de teléfono del usuario.
    /// </summary>
    /// <param name="codigoPais">Nuevo código de país.</param>
    /// <param name="telefono">Nuevo número de teléfono.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    public void ActualizarTelefono(string codigoPais, string telefono, Guid modificationUser)
    {
        List<EMGeneralException> exceptions = new();

        if (this.CodigoPais != codigoPais)
        {
            IsPropertyValid(propertyName: nameof(CodigoPais), value: codigoPais, exceptions: ref exceptions);
        }

        if (this.Telefono != telefono)
        {
            IsPropertyValid(propertyName: nameof(Telefono), value: telefono, exceptions: ref exceptions);
        }

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        bool hasChanges = false;

        if (this.CodigoPais != codigoPais)
        {
            this.CodigoPais = codigoPais;
            hasChanges = true;
        }

        if (this.Telefono != telefono)
        {
            this.Telefono = telefono;
            hasChanges = true;
        }

        if (hasChanges)
        {
            base.Update(modificationUser: modificationUser);
        }
    }

    /// <summary>
    /// Actualiza el correo electrónico del usuario.
    /// </summary>
    /// <param name="correoElectronico">Nuevo correo electrónico.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    public void ActualizarCorreoElectronico(string correoElectronico, Guid modificationUser)
    {
        if (this.CorreoElectronico == correoElectronico) return;

        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(CorreoElectronico), value: correoElectronico, exceptions: ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        this.CorreoElectronico = correoElectronico;
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Agrega una nueva verificación 2FA y desactiva las anteriores del mismo tipo.
    /// </summary>
    /// <param name="verificacion">La nueva verificación a agregar.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    public void AgregarVerificacion2Fa(Verificacion2FA verificacion, Guid modificationUser)
    {
        if (verificacion == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.Verificacion2FARequerida,
                dynamicContent: []));
        }

        // Desactivar verificaciones anteriores del mismo tipo que no estén verificadas
        var verificacionesViejas = this.Verificaciones2Fa
            .Where(predicate: x =>
                x.Tipo == verificacion.Tipo &&
                x is { Verificado: false, IsActive: true })
            .ToList();
        foreach (var oldVerification in verificacionesViejas)
        {
            oldVerification.Deactivate(modificationUser: modificationUser);
        }

        this.Verificaciones2Fa.Add(item: verificacion);
        if (this.Estatus != EstatusRegistroEnum.RegistroCompletado)
        {
            this.Estatus = EstatusRegistroEnum.PreRegistro;
        }

        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Confirma un código de verificación 2FA.
    /// </summary>
    /// <param name="tipo">Tipo de verificación (SMS, Email, etc.).</param>
    /// <param name="codigo">Código a verificar.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    /// <returns>True si la verificación fue exitosa.</returns>
    /// <exception cref="EMGeneralAggregateException">Si el código no existe, está inactivo, vencido o ya confirmado.</exception>
    public bool ConfirmarVerificacion2Fa(Tipo2FA tipo, string codigo, Guid modificationUser)
    {
        var verificacion = this.Verificaciones2Fa
            .Where(predicate: v => v.Tipo == tipo)
            .OrderByDescending(keySelector: v => v.CreationTimestamp)
            .FirstOrDefault();

        // Validar existencia
        if (verificacion == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionNoEncontrado,
                dynamicContent: [tipo.ToString()]));
        }

        // Validar estado activo
        if (!verificacion.IsActive)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionInactivo,
                dynamicContent: []));
        }

        // Validar si ya fue confirmado
        if (verificacion.Verificado)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionConfirmado,
                dynamicContent: []));
        }

        // Validar vencimiento
        if (DateTime.Now >= verificacion.FechaVencimiento)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionVencido,
                dynamicContent: []));
        }

        verificacion.MarcarComoVerificado(codigo: codigo, modificationUser: modificationUser);
        base.Update(modificationUser: modificationUser);
        return verificacion.Verificado;
    }

    /// <summary>
    /// Agrega una ubicación de geolocalización al usuario.
    /// </summary>
    /// <param name="ubicacion">La ubicación a agregar.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    public void AgregarUbicacionGeolocalizacion(UbicacionesGeolocalizacion ubicacion, Guid modificationUser)
    {
        if (ubicacion == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.UbicacionGeolocalizacionRequerido,
                dynamicContent: []));
        }

        this.UbicacionesGeolocalizacion.Add(item: ubicacion);
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Agrega un dispositivo móvil autorizado.
    /// Si ya existe un dispositivo actual, lo marca como no actual.
    /// </summary>
    /// <param name="dispositivo">El dispositivo a autorizar.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    public void AgregarDispositivoMovilAutorizado(DispositivoMovilAutorizado dispositivo, Guid modificationUser)
    {
        if (dispositivo == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.DispositivoMovilAutorizadoRequerido,
                dynamicContent: []));
        }

        // Verificar duplicados
        if (this.DispositivoMovilAutorizados.Any(predicate: x =>
                x.IdDispositivo == dispositivo.IdDispositivo && x.Token == dispositivo.Token))
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.DispositivoMovilAutorizadoDuplicado,
                dynamicContent: []));
        }

        // Desmarcar dispositivo actual si existe
        var dispositivoActual = this.DispositivoMovilAutorizados.FirstOrDefault(predicate: x => x.Actual);
        if (dispositivoActual != null)
        {
            dispositivoActual.MarcarComoNoActual(modificationUser: modificationUser);
        }

        this.DispositivoMovilAutorizados.Add(item: dispositivo);
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Verifica si un dispositivo está autorizado para este usuario.
    /// </summary>
    /// <param name="idDispositivo">ID del dispositivo.</param>
    /// <param name="token">Token del dispositivo.</param>
    /// <returns>True si el dispositivo está en la lista de autorizados.</returns>
    public bool EsDispositivoAutorizado(string idDispositivo, string token)
    {
        var dispositivo =
            this.DispositivoMovilAutorizados.FirstOrDefault(predicate: x =>
                x.IdDispositivo == idDispositivo && x.Token == token);
        return dispositivo != null;
    }

    /// <summary>
    /// Actualiza el estatus del usuario.
    /// </summary>
    /// <param name="nuevoEstatus">El nuevo estatus.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    public void ActualizarEstatus(EstatusRegistroEnum nuevoEstatus, Guid modificationUser)
    {
        if (this.Estatus == nuevoEstatus) return;
        this.Estatus = nuevoEstatus;
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Actualiza el token de refresco y su fecha de expiración.
    /// </summary>
    /// <param name="refreshToken">Nuevo token de refresco.</param>
    /// <param name="expiryTime">Fecha de expiración.</param>
    /// <param name="modificationUser">Usuario que realiza la modificación.</param>
    public void UpdateRefreshToken(string refreshToken, DateTime expiryTime, Guid modificationUser)
    {
        bool hasChanges = false;
        if (this.RefreshToken != refreshToken)
        {
            this.RefreshToken = refreshToken;
            hasChanges = true;
        }

        if (this.RefreshTokenExpiryTime != expiryTime)
        {
            this.RefreshTokenExpiryTime = expiryTime;
            hasChanges = true;
        }

        if (hasChanges)
        {
            base.Update(modificationUser: modificationUser);
        }
    }

    /// <summary>
    /// Verifica si la contraseña proporcionada coincide con el hash almacenado.
    /// </summary>
    /// <param name="password">Contraseña en texto plano a verificar.</param>
    /// <returns>True si coincide, False en caso contrario.</returns>
    public bool VerificarContrasena(string password)
    {
        if (string.IsNullOrEmpty(value: this.Contrasena)) return false;
        if (string.IsNullOrEmpty(value: password)) return false;
        return BCrypt.Net.BCrypt.Verify(text: password, hash: this.Contrasena);
    }
}
