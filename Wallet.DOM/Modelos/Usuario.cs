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

    [Required] [MaxLength(3)] public string CodigoPais { get; private set; }

    [Required] [MaxLength(10)] public string Telefono { get; private set; }

    [EmailAddress] [MaxLength(150)] public string? CorreoElectronico { get; private set; }

    [MaxLength(100)] public string? Contrasena { get; private set; }

    [Required] [MaxLength(20)] public string Estatus { get; private set; }

    public int? EmpresaId { get; private set; }
    public Empresa? Empresa { get; private set; }

    public Cliente? Cliente { get; private set; }

    public List<Verificacion2FA> Verificaciones2FA { get; private set; } = new();
    public List<UbicacionesGeolocalizacion> UbicacionesGeolocalizacion { get; private set; } = new();
    public List<DispositivoMovilAutorizado> DispositivoMovilAutorizados { get; private set; } = new();

    public Usuario() : base()
    {
    }

    public Usuario(
        string codigoPais,
        string telefono,
        string? correoElectronico,
        string? contrasena,
        string estatus,
        Guid creationUser,
        string? testCase = null) : base(creationUser, testCase)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(CodigoPais), value: codigoPais, ref exceptions);
        IsPropertyValid(propertyName: nameof(Telefono), value: telefono, ref exceptions);
        IsPropertyValid(propertyName: nameof(CorreoElectronico), value: correoElectronico, ref exceptions);
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasena, ref exceptions);
        IsPropertyValid(propertyName: nameof(Estatus), value: estatus, ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        this.CodigoPais = codigoPais;
        this.Telefono = telefono;
        this.CorreoElectronico = correoElectronico;
        this.Contrasena = contrasena;
        this.Estatus = estatus;

        this.Verificaciones2FA = new List<Verificacion2FA>();
        this.UbicacionesGeolocalizacion = new List<UbicacionesGeolocalizacion>();
        this.DispositivoMovilAutorizados = new List<DispositivoMovilAutorizado>();
    }

    public void CrearContrasena(string contrasena, Guid modificationUser)
    {
        if (!string.IsNullOrWhiteSpace(this.Contrasena))
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenaYaExiste,
                dynamicContent: []));
        }

        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasena, ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.Contrasena = contrasena;
        base.Update(modificationUser: modificationUser);
    }

    public void ActualizarContrasena(string contrasenaNueva, string confirmacionContrasenaNueva,
        string contrasenaActual, Guid modificationUser)
    {
        if (contrasenaNueva != confirmacionContrasenaNueva)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenasNoCoinciden,
                dynamicContent: []));
        }

        if (this.Contrasena != contrasenaActual)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenaActualIncorrecta,
                dynamicContent: []));
        }

        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasenaNueva, ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.Contrasena = contrasenaNueva;
        base.Update(modificationUser: modificationUser);
    }

    public void ActualizarTelefono(string codigoPais, string telefono, Guid modificationUser)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(CodigoPais), value: codigoPais, ref exceptions);
        IsPropertyValid(propertyName: nameof(Telefono), value: telefono, ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.CodigoPais = codigoPais;
        this.Telefono = telefono;
        base.Update(modificationUser: modificationUser);
    }

    public void ActualizarCorreoElectronico(string correoElectronico, Guid modificationUser)
    {
        List<EMGeneralException> exceptions = new();
        IsPropertyValid(propertyName: nameof(CorreoElectronico), value: correoElectronico, ref exceptions);
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.CorreoElectronico = correoElectronico;
        base.Update(modificationUser: modificationUser);
    }

    public void AgregarVerificacion2FA(Verificacion2FA verificacion, Guid modificationUser)
    {
        if (verificacion == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.Verificacion2FARequerida,
                dynamicContent: []));
        }

        var verificacionesViejas = this.Verificaciones2FA
            .Where(x =>
                x.Tipo == verificacion.Tipo &&
                x is { Verificado: false, IsActive: true })
            .ToList();
        foreach (var oldVerification in verificacionesViejas)
        {
            oldVerification.Deactivate(modificationUser: modificationUser);
        }

        this.Verificaciones2FA.Add(verificacion);
        base.Update(modificationUser: modificationUser);
    }

    public bool ConfirmarVerificacion2FA(Tipo2FA tipo, string codigo, Guid modificationUser)
    {
        var verificacion = this.Verificaciones2FA?
            .Where(v => v.Tipo == tipo)
            .OrderByDescending(v => v.CreationTimestamp)
            .FirstOrDefault();
        if (verificacion == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionNoEncontrado,
                dynamicContent: [tipo.ToString()]));
        }

        if (!verificacion.IsActive)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionInactivo,
                dynamicContent: []));
        }

        if (verificacion.Verificado)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionConfirmado,
                dynamicContent: []));
        }

        if (DateTime.Now >= verificacion.FechaVencimiento)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionVencido,
                dynamicContent: []));
        }

        verificacion.MarcarComoVerificado(codigo: codigo, modificationUser: modificationUser);
        base.Update(modificationUser: modificationUser);
        return verificacion.Verificado;
    }

    public void AgregarUbicacionGeolocalizacion(UbicacionesGeolocalizacion ubicacion, Guid modificationUser)
    {
        if (ubicacion == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.UbicacionGeolocalizacionRequerido,
                dynamicContent: []));
        }

        this.UbicacionesGeolocalizacion.Add(ubicacion);
        base.Update(modificationUser: modificationUser);
    }

    public void AgregarDispositivoMovilAutorizado(DispositivoMovilAutorizado dispositivo, Guid modificationUser)
    {
        if (dispositivo == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.DispositivoMovilAutorizadoRequerido,
                dynamicContent: []));
        }

        if (this.DispositivoMovilAutorizados.Any(x =>
                x.IdDispositivo == dispositivo.IdDispositivo && x.Token == dispositivo.Token))
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.DispositivoMovilAutorizadoDuplicado,
                dynamicContent: []));
        }

        var dispositivoActual = this.DispositivoMovilAutorizados.FirstOrDefault(x => x.Actual);
        if (dispositivoActual != null)
        {
            dispositivoActual.MarcarComoNoActual();
        }

        this.DispositivoMovilAutorizados.Add(dispositivo);
        base.Update(modificationUser: modificationUser);
    }

    public bool EsDispositivoAutorizado(string idDispositivo, string token)
    {
        var dispositivo =
            this.DispositivoMovilAutorizados.FirstOrDefault(x => x.IdDispositivo == idDispositivo && x.Token == token);
        return dispositivo != null;
    }

    public void AgregarEmpresa(Empresa empresa, Guid modificationUser)
    {
        if (empresa == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EmpresaRequerida,
                dynamicContent: []));
        }

        this.Empresa = empresa;
        this.EmpresaId = empresa.Id;
        base.Update(modificationUser: modificationUser);
    }
}
