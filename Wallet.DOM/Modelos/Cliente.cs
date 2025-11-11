using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class Cliente : ValidatablePersistentObjectLogicalDelete
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
            propertyName: nameof(Nombre),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(PrimerApellido),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(SegundoApellido),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        PropertyConstraint.DateOnlyPropertyConstraint(
            propertyName: nameof(FechaNacimiento),
            isRequired: true),

        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(Genero),
            isRequired: true),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(CorreoElectronico),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 150,
            regex: @"^[\w\.-]+@([\w-]+\.)+[\w-]{2,}$"),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Contrasena),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(TipoPersona),
            isRequired: true),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Curp),
            isRequired: true,
            minimumLength: 18,
            maximumLength:18),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Rfc),
            isRequired: true,
            minimumLength: 13,
            maximumLength: 13),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(FotoAWS),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 500),
    ];


    [Key]
    public int Id { get; private set; }
    [Required]
    [MaxLength(3)]
    public string CodigoPais { get; private set; }

    [Required]
    [MaxLength(10)]
    public string Telefono { get; private set; }

    //[Required]
    [MaxLength(100)]
    public string? Nombre { get; private set; }

    //[Required]
    [MaxLength(100)]
    public string? PrimerApellido { get; private set; }

    //[Required]
    [MaxLength(100)]
    public string? SegundoApellido { get; private set; }
    [NotMapped]
    public string? NombreCompleto { get { return $"{this.Nombre} {this.PrimerApellido} {this.SegundoApellido}"; } }

    //[Required]
    public DateOnly? FechaNacimiento { get; private set; }
    //[Required]
    public Genero? Genero { get; private set; }

    //[Required]
    [EmailAddress]
    [MaxLength(150)]
    public string? CorreoElectronico { get; private set; }

    //[Required]
    [MaxLength(100)]
    public string? Contrasena { get; private set; }

    //[Required]
    public TipoPersona? TipoPersona { get; private set; }

    //[Required]
    [MaxLength(18)]
    public string? Curp { get; private set; }

    //[Required]
    [MaxLength(13)]
    public string? Rfc { get; private set; }

    // TODO EMD: FOTO OPCIONAL, AQUI SE GUARDARA EL GUID O TOKEN QUE RETORNE AWS
    //[Required]
    [MaxLength(500)]
    public string? FotoAWS { get; private set; }

    public int? EstadoId { get; private set; }

    public Estado? Estado { get; private set; }

    public int? EmpresaId { get; private set; }

    public Empresa? Empresa { get; private set; }

    public Direccion? Direccion { get; private set; }

    public List<Verificacion2FA> Verificaciones2FA { get; private set; } = new();
    public List<UbicacionesGeolocalizacion> UbicacionesGeolocalizacion { get; private set; } = new();
    public List<DispositivoMovilAutorizado> DispositivoMovilAutorizados { get; private set; } = new();
    public List<DocumentacionAdjunta> DocumentacionAdjuntas { get; private set; } = new();
    public List<ActividadEconomica> ActividadEconomicas { get; private set; } = new();
    public List<ValidacionCheckton> ValidacionesChecktons { get; private set; } = new();


    public Cliente() : base()
    {

    }

    /// <summary>
    /// Nuevo cliente, para generar code 4 digitos por sms
    /// </summary>
    /// <param name="codigoPais"></param>
    /// <param name="telefono"></param>
    /// <param name="creationUser"></param>
    /// <param name="testCase"></param>
    /// <exception cref="EMGeneralAggregateException"></exception>
    public Cliente(
        string codigoPais,
        string telefono,
        Guid creationUser,
        string? testCase = null) : base(creationUser, testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(CodigoPais), value: codigoPais, ref exceptions);
        IsPropertyValid(propertyName: nameof(Telefono), value: telefono, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.CodigoPais = codigoPais;
        this.Telefono = telefono;
        // Inicializa las listas
        this.Verificaciones2FA = new List<Verificacion2FA>();
        this.UbicacionesGeolocalizacion = new List<UbicacionesGeolocalizacion>();
        this.DispositivoMovilAutorizados = new List<DispositivoMovilAutorizado>();
        this.DocumentacionAdjuntas = new List<DocumentacionAdjunta>();
        this.ActividadEconomicas = new List<ActividadEconomica>();
        this.ValidacionesChecktons = new List<ValidacionCheckton>();
    }

    /// <summary>
    /// Completa datos del cliente, genera code 4 digitos por correo electronico
    /// </summary>
    /// <param name="nombre"></param>
    /// <param name="primerApellido"></param>
    /// <param name="segundoApellido"></param>
    /// <param name="fechaNacimiento"></param>
    /// <param name="genero"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    /// <exception cref="EMGeneralAggregateException"></exception>
    public void AgregarDatosPersonales(
        string? nombre,
        string? primerApellido,
        string? segundoApellido,
        DateOnly? fechaNacimiento,
        Genero? genero,
        Guid modificationUser
    )
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, ref exceptions);
        IsPropertyValid(propertyName: nameof(PrimerApellido), value: primerApellido, ref exceptions);
        IsPropertyValid(propertyName: nameof(SegundoApellido), value: segundoApellido, ref exceptions);
        IsPropertyValid(propertyName: nameof(FechaNacimiento), value: fechaNacimiento, ref exceptions);
        IsPropertyValid(propertyName: nameof(Genero), value: genero, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.Nombre = nombre;
        this.PrimerApellido = primerApellido;
        this.SegundoApellido = segundoApellido;
        this.FechaNacimiento = fechaNacimiento;
        this.Genero = genero;
        base.Update(modificationUser: modificationUser);
    }

    public void CrearContrasena(string contrasena, Guid modificationUser)
    {
        // Si ya existe la contrasena, debe actualizar
        if (!string.IsNullOrWhiteSpace(this.Contrasena))
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenaYaExiste,
                dynamicContent: []));
        }
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasena, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        //  TODO EMD: ENCRIPTAR EN EL FUTURO O USA HASH
        // Set contrasena
        this.Contrasena = contrasena;
        base.Update(modificationUser: modificationUser);
    }

    public void ActualizarContrasena(string contrasenaNueva, string confirmacionContrasenaNueva, string contrasenaActual, Guid modificationUser)
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
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(Contrasena), value: contrasenaNueva, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Set new contrasena
        //  TODO EMD: ENCRIPTAR EN EL FUTURO O USA HASH
        this.Contrasena = contrasenaNueva;
        base.Update(modificationUser: modificationUser);
    }

    public void ActualizarTelefono(string codigoPais, string telefono, Guid modificationUser)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(CodigoPais), value: codigoPais, ref exceptions);
        IsPropertyValid(propertyName: nameof(Telefono), value: telefono, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.CodigoPais = codigoPais;
        this.Telefono = telefono;
        base.Update(modificationUser: modificationUser);
    }

    public void ActualizarCorreoElectronico(string correoElectronico, Guid modificationUser)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(CorreoElectronico), value: correoElectronico, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.CorreoElectronico = correoElectronico;
        base.Update(modificationUser: modificationUser);
    }

    public void AgregarVerificacion2FA(Verificacion2FA verificacion, Guid modificationUser)
    {
        // Verifica que el objeto a agregar no sea nulo 
        if (verificacion == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                            errorCode: ServiceErrorsBuilder.Verificacion2FARequerida,
                            dynamicContent: []));
        }
        // Si el codigo ya existe, retronamos, solo faltara confirmarlo
        // IDENTIFICA y DESACTIVA todos los códigos activos y no verificados del MISMO TIPO
        var verificacionesViejas = this.Verificaciones2FA
            // Filtramos por las que NO SE VERIFICADAS Y DEL MISMO TIPO
            .Where(x => 
                        x.Tipo == verificacion.Tipo &&
                        x is { Verificado: false, IsActive: true })
            // Convertir a lista para iterar sin problemas con el Where
            .ToList();
        // 4. Desactiva cada código de verificación viejo/pendiente
        foreach (var oldVerification in verificacionesViejas)
        {
            // Desactivamos codigo viejo
            oldVerification.Deactivate(modificationUser: modificationUser);
        }
        // 5. Agrega la nueva verificación (la más reciente)
        this.Verificaciones2FA.Add(verificacion);
        base.Update(modificationUser: modificationUser);
    }

    public bool ConfirmarVerificacion2FA(Tipo2FA tipo, string codigo, Guid modificationUser)
    {
        // Busca la verificación 2FA activa y no verificada del tipo y código proporcionados
        var verificacion = this.Verificaciones2FA?
            .Where(v => v.Tipo == tipo) // 1. Filtrar por tipo.
            .OrderByDescending(v => v.CreationTimestamp) // 2. Ordenar de más nuevo a más viejo.
            .FirstOrDefault();
        // Verifica si se encontró la verificación
        if (verificacion == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionNoEncontrado,
                dynamicContent: [tipo.ToString()]));
        }

        // Verifica si el código está activo
        if (!verificacion.IsActive)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionInactivo,
                dynamicContent: []));
        }

        // Verifica si el código ya ha sido verificado
        if (verificacion.Verificado)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionConfirmado,
                dynamicContent: []));
        }

        // Verifica si el código ha vencido
        if (DateTime.Now >= verificacion.FechaVencimiento)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.CodigoVerificacionVencido,
                dynamicContent: []));
        }
        // Marca la verificación como verificada
        verificacion.MarcarComoVerificado(codigo: codigo, modificationUser: modificationUser);
        base.Update(modificationUser: modificationUser);
        return verificacion.Verificado;
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
        base.Update(modificationUser: modificationUser);
    }
    public void AgregarDireccion(Direccion direccion, Guid creationUser)
    {
        // Se agrega la direccion solo con estado y pais
        this.Direccion = direccion ?? throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
            errorCode: ServiceErrorsBuilder.DireccionRequerida,
            dynamicContent: []));
        // Actualizamos el cliente con la direccion
        this.Direccion = direccion;
        // Actualizamos el cliente
        base.Update(modificationUser: creationUser);
    }

    public void AgregarEstado(Estado estado, Guid modificationUser)
    {
        if (estado == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.EstadoRequerido,
                    dynamicContent: []));
        }
        this.Estado = estado;
        base.Update(modificationUser: modificationUser);
    }

    public void AgregarFotoAWS(string fotoAWS, Guid modificationUser)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(FotoAWS), value: fotoAWS, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Set fotoAWS
        this.FotoAWS = fotoAWS;
        base.Update(modificationUser: modificationUser);
    }

    public void AgregarTipoPersona(TipoPersona tipoPersona, Guid modificationUser)
    {
        this.TipoPersona = tipoPersona;
        base.Update(modificationUser: modificationUser);
    }

    public void AgregarRfc( string rfc, Guid modificationUser)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(Rfc), value: rfc, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Set rfc
        this.Rfc = rfc;
        base.Update(modificationUser: modificationUser);
    }
    public void AgregarCurp(string curp, Guid modificationUser)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(Curp), value: curp, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Set curp 
        this.Curp = curp;
        base.Update(modificationUser: modificationUser);
    }

    public void AgregarUbicacionGeolocalizacion(UbicacionesGeolocalizacion ubicacion, Guid modificationUser)
    {
        // Verifica que la ubicacion no sea nula
        if (ubicacion == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.UbicacionGeolocalizacionRequerido,
                    dynamicContent: []));
        }
        // Agrega la nueva ubicacion a la lista
        this.UbicacionesGeolocalizacion.Add(ubicacion);
        base.Update(modificationUser: modificationUser);
    }

    public void AgregarDispositivoMovilAutorizado(DispositivoMovilAutorizado dispositivo, Guid modificationUser)
    {
        // Verifica que el dispositivo no sea nulo
        if (dispositivo == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.DispositivoMovilAutorizadoRequerido,
                    dynamicContent: []));
        }
        // Valida dispoitivo duplicado por IdDispositivo y Token
        if (this.DispositivoMovilAutorizados.Any(x => x.IdDispositivo == dispositivo.IdDispositivo && x.Token == dispositivo.Token))
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.DispositivoMovilAutorizadoDuplicado,
                    dynamicContent: []));
        }
        // Encuentra el dispositivo actual en la lista
        var dispositivoActual = this.DispositivoMovilAutorizados.FirstOrDefault(x => x.Actual);
        // Si existe un dispositivo actual, marcarlo como no actual
        if (dispositivoActual != null)
        {
            dispositivoActual.MarcarComoNoActual();
        }
        // Agrega el nuevo dispositivo a la lista y este quedara como el actual
        this.DispositivoMovilAutorizados.Add(dispositivo);
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Agrega un documento adjunto
    /// </summary>
    /// <param name="documentacion"></param>
    /// <param name="modificationUser"></param>
    /// <exception cref="EMGeneralAggregateException"></exception>
    private void AgregarDocumentacionAdjunta(DocumentacionAdjunta documentacion, Guid modificationUser)
    {
        // Verifica que el tipo de persona esté configurado
        if (this.TipoPersona == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.TipoPersonaNoConfigurada,
                    dynamicContent: []));
        }
        // Verifica que el documento no sea nulo
        if (documentacion == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.DocumentacionAdjuntaRequerida,
                    dynamicContent: []));
        }
        // Valida si el documento ya existe (por NombreDocumento)
        if (this.DocumentacionAdjuntas.Any(d => d.Documento.Nombre == documentacion.Documento.Nombre &&
         d.Documento.TipoPersona == this.TipoPersona))
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                   errorCode: ServiceErrorsBuilder.DocumentacionAdjuntaYaExiste,
                   dynamicContent: [documentacion.Documento.Nombre, this.TipoPersona]));
        }
        // Agrega el nuevo documento a la lista
        this.DocumentacionAdjuntas.Add(documentacion);
        base.Update(modificationUser: modificationUser);
    }


    public bool EsDispositivoAutorizado(string idDispositivo, string token)
    {
        var dispositivo = this.DispositivoMovilAutorizados.FirstOrDefault(x => x.IdDispositivo == idDispositivo && x.Token == token);
        return dispositivo != null;
    }
    
    public void AgregarValidacionCheckton(ValidacionCheckton validacion, Guid modificationUser)
    {
        // Verifica que el validacion no sea nulo
        if (validacion == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ValidacionChecktonRequerida,
                    dynamicContent: []));
        }
        // Agrega el nuevo validacion a la lista
        this.ValidacionesChecktons.Add(validacion);
        base.Update(modificationUser: modificationUser);
    }
}


