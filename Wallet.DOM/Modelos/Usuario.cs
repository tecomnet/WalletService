using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class Usuario : ValidatablePersistentObjectLogicalDelete
{
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

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Estatus),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 20),
    ];

    [Required] [MaxLength(length: 3)] public string CodigoPais { get; private set; }

    [Required] [MaxLength(length: 10)] public string Telefono { get; private set; }

    [EmailAddress] [MaxLength(length: 150)] public string? CorreoElectronico { get; private set; }

    [MaxLength(length: 100)] public string? Contrasena { get; private set; }

    [Required] [MaxLength(length: 20)] public string Estatus { get; private set; }

    [MaxLength(length: 200)] public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    public int? EmpresaId { get; private set; }
    public Empresa? Empresa { get; private set; }

    public Cliente? Cliente { get; private set; }

    public List<Verificacion2FA> Verificaciones2Fa { get; private set; } = new();
    public virtual List<UbicacionesGeolocalizacion> UbicacionesGeolocalizacion { get; private set; } = new();
    public List<DispositivoMovilAutorizado> DispositivoMovilAutorizados { get; private set; } = new();

    public Usuario()
    {
        CodigoPais = string.Empty;
        Telefono = string.Empty;
        Estatus = string.Empty;
    }

    public Usuario(
        string codigoPais,
        string telefono,
        string? correoElectronico,
        string? contrasena,
        string estatus,
        Guid creationUser,
        string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(CodigoPais), value: codigoPais, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Telefono), value: telefono, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(CorreoElectronico), value: correoElectronico, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasena, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Estatus), value: estatus, exceptions: ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        this.CodigoPais = codigoPais;
        this.Telefono = telefono;
        this.CorreoElectronico = correoElectronico;
        this.Contrasena = contrasena;
        this.Estatus = estatus;

        Verificaciones2Fa = new List<Verificacion2FA>();
        this.UbicacionesGeolocalizacion = new List<UbicacionesGeolocalizacion>();
        this.DispositivoMovilAutorizados = new List<DispositivoMovilAutorizado>();
    }

    public void CrearContrasena(string contrasena, Guid modificationUser)
    {
        if (!string.IsNullOrWhiteSpace(value: this.Contrasena))
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenaYaExiste,
                dynamicContent: []));
        }

        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasena, exceptions: ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.Contrasena = contrasena;
        Update(modificationUser: modificationUser);
    }

    public void ActualizarContrasena(string contrasenaNueva, string confirmacionContrasenaNueva,
        string contrasenaActual, Guid modificationUser)
    {
        if (contrasenaNueva != confirmacionContrasenaNueva)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenasNoCoinciden,
                dynamicContent: []));
        }

        if (this.Contrasena != contrasenaActual)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenaActualIncorrecta,
                dynamicContent: []));
        }

        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasenaNueva, exceptions: ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.Contrasena = contrasenaNueva;
        Update(modificationUser: modificationUser);
    }

    public void ActualizarTelefono(string codigoPais, string telefono, Guid modificationUser)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(CodigoPais), value: codigoPais, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Telefono), value: telefono, exceptions: ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.CodigoPais = codigoPais;
        this.Telefono = telefono;
        Update(modificationUser: modificationUser);
    }

    public void ActualizarCorreoElectronico(string correoElectronico, Guid modificationUser)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(CorreoElectronico), value: correoElectronico, exceptions: ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.CorreoElectronico = correoElectronico;
        Update(modificationUser: modificationUser);
    }

    public void AgregarVerificacion2Fa(Verificacion2FA verificacion, Guid modificationUser)
    {
        if (verificacion == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.Verificacion2FARequerida,
                dynamicContent: []));
        }

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
        Update(modificationUser: modificationUser);
    }

    public bool ConfirmarVerificacion2Fa(Tipo2FA tipo, string codigo, Guid modificationUser)
    {
        var verificacion = this.Verificaciones2Fa
            .Where(predicate: v => v.Tipo == tipo)
            .OrderByDescending(keySelector: v => v.CreationTimestamp)
            .FirstOrDefault();
        if (verificacion == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionNoEncontrado,
                dynamicContent: [tipo.ToString()]));
        }

        if (!verificacion.IsActive)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionInactivo,
                dynamicContent: []));
        }

        if (verificacion.Verificado)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionConfirmado,
                dynamicContent: []));
        }

        if (DateTime.Now >= verificacion.FechaVencimiento)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionVencido,
                dynamicContent: []));
        }

        verificacion.MarcarComoVerificado(codigo: codigo, modificationUser: modificationUser);
        Update(modificationUser: modificationUser);
        return verificacion.Verificado;
    }

    public void AgregarUbicacionGeolocalizacion(UbicacionesGeolocalizacion ubicacion, Guid modificationUser)
    {
        if (ubicacion == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.UbicacionGeolocalizacionRequerido,
                dynamicContent: []));
        }

        this.UbicacionesGeolocalizacion.Add(item: ubicacion);
        Update(modificationUser: modificationUser);
    }

    public void AgregarDispositivoMovilAutorizado(DispositivoMovilAutorizado dispositivo, Guid modificationUser)
    {
        if (dispositivo == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.DispositivoMovilAutorizadoRequerido,
                dynamicContent: []));
        }

        if (this.DispositivoMovilAutorizados.Any(predicate: x =>
                x.IdDispositivo == dispositivo.IdDispositivo && x.Token == dispositivo.Token))
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.DispositivoMovilAutorizadoDuplicado,
                dynamicContent: []));
        }

        var dispositivoActual = this.DispositivoMovilAutorizados.FirstOrDefault(predicate: x => x.Actual);
        if (dispositivoActual != null)
        {
            dispositivoActual.MarcarComoNoActual();
        }

        this.DispositivoMovilAutorizados.Add(item: dispositivo);
        Update(modificationUser: modificationUser);
    }

    public bool EsDispositivoAutorizado(string idDispositivo, string token)
    {
        var dispositivo =
            this.DispositivoMovilAutorizados.FirstOrDefault(predicate: x => x.IdDispositivo == idDispositivo && x.Token == token);
        return dispositivo != null;
    }

    public void AgregarEmpresa(Empresa empresa, Guid modificationUser)
    {
        if (empresa == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EmpresaRequerida,
                dynamicContent: []));
        }

        this.Empresa = empresa;
        this.EmpresaId = empresa.Id;
        Update(modificationUser: modificationUser);
    }

    public void UpdateRefreshToken(string refreshToken, DateTime expiryTime, Guid modificationUser)
    {
        this.RefreshToken = refreshToken;
        this.RefreshTokenExpiryTime = expiryTime;
        Update(modificationUser: modificationUser);
    }
}
