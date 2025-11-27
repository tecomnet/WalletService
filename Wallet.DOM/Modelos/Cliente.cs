using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class Cliente : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
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

        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(TipoPersona),
            isRequired: true),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Curp),
            isRequired: true,
            minimumLength: 18,
            maximumLength: 18),

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

    //[Required]
    [MaxLength(100)] public string? Nombre { get; private set; }

    //[Required]
    [MaxLength(100)] public string? PrimerApellido { get; private set; }

    //[Required]
    [MaxLength(100)] public string? SegundoApellido { get; private set; }
    [NotMapped] public string? NombreCompleto => $"{this.Nombre} {this.PrimerApellido} {this.SegundoApellido}";

    //[Required]
    public DateOnly? FechaNacimiento { get; private set; }

    //[Required]
    public Genero? Genero { get; private set; }

    //[Required]
    public TipoPersona? TipoPersona { get; private set; }

    //[Required]
    [MaxLength(18)] public string? Curp { get; private set; }

    //[Required]
    [MaxLength(13)] public string? Rfc { get; private set; }

    // TODO EMD: FOTO OPCIONAL, AQUI SE GUARDARA EL GUID O TOKEN QUE RETORNE AWS
    //[Required]
    [MaxLength(500)] public string? FotoAWS { get; private set; }

    public int? EstadoId { get; private set; }

    public Estado? Estado { get; private set; }

    public Direccion? Direccion { get; private set; }

    public int UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; }

    public List<DocumentacionAdjunta> DocumentacionAdjuntas { get; private set; } = new();
    public List<ActividadEconomica> ActividadEconomicas { get; private set; } = new();
    public List<ValidacionCheckton> ValidacionesChecktons { get; private set; } = new();
    public List<ServicioFavorito> ServiciosFavoritos { get; private set; } = new();


    public Cliente() : base()
    {
    }

    public Cliente(
        Usuario usuario,
        Guid creationUser,
        string? testCase = null) : base(creationUser, testCase)
    {
        if (usuario == null) throw new ArgumentNullException(nameof(usuario));
        this.Usuario = usuario;
        this.UsuarioId = usuario.Id;

        this.DocumentacionAdjuntas = new List<DocumentacionAdjunta>();
        this.ActividadEconomicas = new List<ActividadEconomica>();
        this.ValidacionesChecktons = new List<ValidacionCheckton>();
        this.ServiciosFavoritos = new List<ServicioFavorito>();
    }

    /// <summary>
    /// Completa datos del cliente
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

    public void AgregarRfc(string rfc, Guid modificationUser)
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
