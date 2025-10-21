using System.ComponentModel.DataAnnotations;
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
            maximumLength: 250),
        
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
    //[Required]
    public DateOnly? FechaNacimiento { get; private set; }
    //[Required]
    public Genero? Genero { get; private set; }

    //[Required]
    [EmailAddress]
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
    
    public int EstadoId { get; private set; }
    
    public Estado Estado { get; private set; }
    
    public int EmpresaId { get; private set; }
    
    public Empresa Empresa { get; private set; }
    
    public int DireccionId { get; private set; }
    
    public Direccion Direccion { get; private set; }
    
    public List<Verificacion2FA> Verificaciones2FA { get; private set; }    
    public List<UbicacionesGeolocalizacion> UbicacionesGeolocalizacion { get; private set; }
    public List<DispositivoMovilAutorizado> DispositivoMovilAutorizados { get; private set; }   
    public List<DocumentacionAdjunta> DocumentacionAdjuntas { get; private set; }
    public List<ActividadEconomica> ActividadEconomicas { get; private set; }
    public List<ValidacionCheckton> ValidacionesChecktons { get; private set; }


    public Cliente()
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
    }

    /// <summary>
    /// Completa datos del cliente, genera code 4 digitos por correo electronico
    /// </summary>
    /// <param name="nombre"></param>
    /// <param name="primerApellido"></param>
    /// <param name="segundoApellido"></param>
    /// <param name="fechaNacimiento"></param>
    /// <param name="genero"></param>
    /// <param name="correoElectronico"></param>
    /// <returns></returns>
    /// <exception cref="EMGeneralAggregateException"></exception>
    public void AgregarDatosPersonales(
        string? nombre,
        string? primerApellido,
        string? segundoApellido,
        DateOnly? fechaNacimiento,
        Genero? genero,
        string? correoElectronico,
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
        IsPropertyValid(propertyName: nameof(CorreoElectronico), value: correoElectronico, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        this.Nombre = nombre;
        this.PrimerApellido = primerApellido;
        this.SegundoApellido = segundoApellido;
        this.FechaNacimiento = fechaNacimiento;
        this.Genero = genero;
        this.CorreoElectronico = correoElectronico;
        base.Update(modificationUser: modificationUser);
    }

    public void CrearContrasena(string contrasena, Guid modificationUser)
    {
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

    public void ActualizarContrasena(string newContrasena, string? oldContrasena, Guid modificationUser)
    {
        if (oldContrasena != this.Contrasena)
        {
            throw new Exception("La contraseña actual no es correcta");
        }
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(Contrasena), value: newContrasena, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Set new contrasena
        //  TODO EMD: ENCRIPTAR EN EL FUTURO O USA HASH
        this.Contrasena = newContrasena;   
        base.Update(modificationUser: modificationUser);
    }
    
    
    public void AgregarEmpresa(Empresa empresa, Guid modificationUser)
    {
        this.Empresa = empresa;
        base.Update(modificationUser: modificationUser);
    }
    public void AgregarDireccion(Direccion direccion, Guid modificationUser)
    {
        this.Direccion = direccion;
        base.Update(modificationUser: modificationUser);
    }

    public void AgregarEstado(Estado estado, Guid modificationUser)
    {
        this.Estado = estado;
        base.Update(modificationUser: modificationUser);
    }
}


