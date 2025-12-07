using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class Cliente : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de validación para las propiedades de la entidad Cliente.
    /// </summary>
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
            isRequired: true, // TODO EMD: La foto AWS es opcional, revisar esta restricción.
            minimumLength: 1,
            maximumLength: 500),
    ];

    /// <summary>
    /// Obtiene o establece el nombre del cliente.
    /// </summary>
    [MaxLength(length: 100)]
    public string? Nombre { get; private set; }

    /// <summary>
    /// Obtiene o establece el primer apellido del cliente.
    /// </summary>
    [MaxLength(length: 100)]
    public string? PrimerApellido { get; private set; }

    /// <summary>
    /// Obtiene o establece el segundo apellido del cliente.
    /// </summary>
    [MaxLength(length: 100)]
    public string? SegundoApellido { get; private set; }

    /// <summary>
    /// Obtiene el nombre completo del cliente, concatenando Nombre, PrimerApellido y SegundoApellido.
    /// </summary>
    [NotMapped]
    public string? NombreCompleto => $"{this.Nombre} {this.PrimerApellido} {this.SegundoApellido}";

    /// <summary>
    /// Obtiene o establece la fecha de nacimiento del cliente.
    /// </summary>
    public DateOnly? FechaNacimiento { get; private set; }

    /// <summary>
    /// Obtiene o establece el género del cliente.
    /// </summary>
    public Genero? Genero { get; private set; }

    /// <summary>
    /// Obtiene o establece el tipo de persona del cliente (Física o Moral).
    /// </summary>
    public TipoPersona? TipoPersona { get; private set; }

    /// <summary>
    /// Obtiene o establece la CURP (Clave Única de Registro de Población) del cliente.
    /// </summary>
    [MaxLength(length: 18)]
    public string? Curp { get; private set; }

    /// <summary>
    /// Obtiene o establece el RFC (Registro Federal de Contribuyentes) del cliente.
    /// </summary>
    [MaxLength(length: 13)]
    public string? Rfc { get; private set; }

    /// <summary>
    /// Obtiene o establece la URL o identificador de la foto del cliente almacenada en AWS.
    /// </summary>
    [MaxLength(length: 500)]
    public string? FotoAWS { get; private set; }

    /// <summary>
    /// Obtiene o establece el identificador del estado asociado al cliente.
    /// </summary>
    public int? EstadoId { get; private set; }

    /// <summary>
    /// Obtiene o establece el objeto Estado asociado al cliente.
    /// </summary>
    public Estado? Estado { get; private set; }

    /// <summary>
    /// Obtiene o establece la dirección del cliente.
    /// </summary>
    public Direccion? Direccion { get; private set; }

    /// <summary>
    /// Obtiene o establece el identificador del usuario asociado a este cliente.
    /// </summary>
    public int UsuarioId { get; private set; }

    /// <summary>
    /// Obtiene o establece el objeto Usuario asociado a este cliente.
    /// </summary>
    public Usuario Usuario { get; private set; }

    /// <summary>
    /// Identificador de la empresa asociada al cliente.
    /// </summary>
    [Required]
    public int EmpresaId { get; private set; }

    /// <summary>
    /// Objeto de navegación para la empresa.
    /// </summary>
    [ForeignKey(name: "EmpresaId")]
    public Empresa Empresa { get; private set; }

    /// <summary>
    /// Obtiene la lista de documentos adjuntos del cliente.
    /// </summary>
    public List<DocumentacionAdjunta> DocumentacionAdjuntas { get; private set; } = new();

    /// <summary>
    /// Obtiene la lista de actividades económicas del cliente.
    /// </summary>
    public List<ActividadEconomica> ActividadEconomicas { get; private set; } = new();

    /// <summary>
    /// Obtiene la lista de validaciones Checkton realizadas para el cliente.
    /// </summary>
    public List<ValidacionCheckton> ValidacionesChecktons { get; private set; } = new();

    /// <summary>
    /// Obtiene la lista de servicios favoritos del cliente.
    /// </summary>
    public List<ServicioFavorito> ServiciosFavoritos { get; private set; } = new();


    /// <summary>
    /// Constructor por defecto de la clase Cliente.
    /// </summary>
    public Cliente() : base()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase Cliente con un usuario, empresa y un usuario de creación.
    /// </summary>
    /// <param name="usuario">El objeto Usuario asociado a este cliente.</param>
    /// <param name="empresa">La empresa a la que pertenece el cliente.</param>
    /// <param name="creationUser">El GUID del usuario que crea el cliente.</param>
    /// <param name="testCase">Parámetro opcional para casos de prueba.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el usuario o la empresa son nulos.</exception>
    public Cliente(
        Usuario usuario,
        Empresa empresa,
        Guid creationUser,
        string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Verifica que el usuario no sea nulo.
        if (usuario == null) throw new ArgumentNullException(paramName: nameof(usuario));
        if (empresa == null) throw new ArgumentNullException(paramName: nameof(empresa));

        this.Usuario = usuario;
        this.UsuarioId = usuario.Id;
        this.Empresa = empresa;
        this.EmpresaId = empresa.Id;

        // Inicializa las listas de colecciones.
        this.DocumentacionAdjuntas = new List<DocumentacionAdjunta>();
        this.ActividadEconomicas = new List<ActividadEconomica>();
        this.ValidacionesChecktons = new List<ValidacionCheckton>();
        this.ServiciosFavoritos = new List<ServicioFavorito>();
    }

    /// <summary>
    /// Completa o actualiza los datos personales del cliente.
    /// </summary>
    /// <param name="nombre">El nombre del cliente.</param>
    /// <param name="primerApellido">El primer apellido del cliente.</param>
    /// <param name="segundoApellido">El segundo apellido del cliente.</param>
    /// <param name="fechaNacimiento">La fecha de nacimiento del cliente.</param>
    /// <param name="genero">El género del cliente.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si alguna validación de propiedad falla.</exception>
    public void AgregarDatosPersonales(
        string? nombre,
        string? primerApellido,
        string? segundoApellido,
        DateOnly? fechaNacimiento,
        Genero? genero,
        Guid modificationUser
    )
    {
        // Inicializa la lista de excepciones para acumular errores de validación.
        List<EMGeneralException> exceptions = new();
        // Valida cada una de las propiedades proporcionadas.
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(PrimerApellido), value: primerApellido, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(SegundoApellido), value: segundoApellido, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(FechaNacimiento), value: fechaNacimiento, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Genero), value: genero, exceptions: ref exceptions);
        // Si hay excepciones, las lanza agrupadas.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna los valores a las propiedades del cliente.
        this.Nombre = nombre;
        this.PrimerApellido = primerApellido;
        this.SegundoApellido = segundoApellido;
        this.FechaNacimiento = fechaNacimiento;
        this.Genero = genero;
        // Actualiza los metadatos de la entidad base.
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Agrega o actualiza la dirección del cliente.
    /// </summary>
    /// <param name="direccion">El objeto Direccion a agregar.</param>
    /// <param name="creationUser">El GUID del usuario que realiza la creación/modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si la dirección proporcionada es nula.</exception>
    public void AgregarDireccion(Direccion direccion, Guid creationUser)
    {
        // Asigna la dirección, lanzando una excepción si es nula.
        this.Direccion = direccion ?? throw new EMGeneralAggregateException(
            exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.DireccionRequerida,
                dynamicContent: []));
        // Actualizamos el cliente (la línea anterior ya asigna, esta es redundante pero se mantiene por el original).
        // this.Direccion = direccion;
        // Actualiza los metadatos de la entidad base.
        base.Update(modificationUser: creationUser);
    }

    /// <summary>
    /// Agrega o actualiza el estado asociado al cliente.
    /// </summary>
    /// <param name="estado">El objeto Estado a agregar.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el estado proporcionado es nulo.</exception>
    public void AgregarEstado(Estado estado, Guid modificationUser)
    {
        // Verifica que el estado no sea nulo.
        if (estado == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EstadoRequerido,
                dynamicContent: []));
        }

        // Asigna el estado al cliente.
        this.Estado = estado;
        // Actualiza los metadatos de la entidad base.
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Agrega o actualiza la foto del cliente almacenada en AWS.
    /// </summary>
    /// <param name="fotoAWS">La URL o identificador de la foto en AWS.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si la validación de la propiedad FotoAWS falla.</exception>
    public void AgregarFotoAWS(string fotoAWS, Guid modificationUser)
    {
        // Inicializa la lista de excepciones.
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad FotoAWS.
        IsPropertyValid(propertyName: nameof(FotoAWS), value: fotoAWS, exceptions: ref exceptions);
        // Si hay excepciones, las lanza.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna el valor de FotoAWS.
        this.FotoAWS = fotoAWS;
        // Actualiza los metadatos de la entidad base.
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Agrega o actualiza el tipo de persona del cliente.
    /// </summary>
    /// <param name="tipoPersona">El objeto TipoPersona a asignar.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    public void AgregarTipoPersona(TipoPersona tipoPersona, Guid modificationUser)
    {
        // Asigna el tipo de persona al cliente.
        this.TipoPersona = tipoPersona;
        // Actualiza los metadatos de la entidad base.
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Agrega o actualiza el RFC del cliente.
    /// </summary>
    /// <param name="rfc">El RFC del cliente.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si la validación de la propiedad Rfc falla.</exception>
    public void AgregarRfc(string rfc, Guid modificationUser)
    {
        // Inicializa la lista de excepciones.
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad Rfc.
        IsPropertyValid(propertyName: nameof(Rfc), value: rfc, exceptions: ref exceptions);
        // Si hay excepciones, las lanza.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna el valor de Rfc.
        this.Rfc = rfc;
        // Actualiza los metadatos de la entidad base.
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Agrega o actualiza el CURP del cliente.
    /// </summary>
    /// <param name="curp">El CURP del cliente.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si la validación de la propiedad Curp falla.</exception>
    public void AgregarCurp(string curp, Guid modificationUser)
    {
        // Inicializa la lista de excepciones.
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad Curp.
        IsPropertyValid(propertyName: nameof(Curp), value: curp, exceptions: ref exceptions);
        // Si hay excepciones, las lanza.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna el valor de Curp.
        this.Curp = curp;
        // Actualiza los metadatos de la entidad base.
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Agrega o actualiza un documento adjunto al cliente.
    /// </summary>
    /// <param name="documentacion">El objeto DocumentacionAdjunta a agregar.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el tipo de persona del cliente no está configurado o si el documento es nulo.</exception>
    private void AgregarDocumentacionAdjunta(DocumentacionAdjunta documentacion, Guid modificationUser)
    {
        // Verifica que el tipo de persona esté configurado.
        if (this.TipoPersona == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.TipoPersonaNoConfigurada,
                dynamicContent: []));
        }

        // Verifica que el documento no sea nulo.
        if (documentacion == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.DocumentacionAdjuntaRequerida,
                dynamicContent: []));
        }

        // Valida si el documento ya existe (por NombreDocumento).
        if (this.DocumentacionAdjuntas.Any(predicate: d => d.Documento.Nombre == documentacion.Documento.Nombre &&
                                                           d.Documento.TipoPersona == this.TipoPersona))
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.DocumentacionAdjuntaYaExiste,
                dynamicContent: [documentacion.Documento.Nombre, this.TipoPersona]));
        }

        // Agrega el nuevo documento a la lista.
        this.DocumentacionAdjuntas.Add(item: documentacion);
        // Actualiza los metadatos de la entidad base.
        base.Update(modificationUser: modificationUser);
    }

    /// <summary>
    /// Agrega o actualiza una validación Checkton para el cliente.
    /// </summary>
    /// <param name="validacion">El objeto ValidacionCheckton a agregar.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el validacion es nulo.</exception>
    public void AgregarValidacionCheckton(ValidacionCheckton validacion, Guid modificationUser)
    {
        // Verifica que el validacion no sea nulo.
        if (validacion == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ValidacionChecktonRequerida,
                dynamicContent: []));
        }

        // Agrega el nuevo validacion a la lista.
        this.ValidacionesChecktons.Add(item: validacion);
        // Actualiza los metadatos de la entidad base.
        base.Update(modificationUser: modificationUser);
    }
}
