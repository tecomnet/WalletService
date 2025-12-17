using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos.GestionCliente;

/// <summary>
/// Representa una dirección física con sus detalles y reglas de validación.
/// Hereda de <see cref="ValidatablePersistentObjectLogicalDelete"/> para capacidades de validación y borrado lógico.
/// </summary>
public class Direccion : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de propiedad para los campos de la dirección.
    /// Estas restricciones se utilizan para la validación de los datos de la dirección.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        // Restricción para el Código Postal: requerido, longitud fija de 5 caracteres.
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(CodigoPostal),
            isRequired: true,
            minimumLength: 5,
            maximumLength: 5),

        // Restricción para el País: requerido, longitud entre 1 y 100 caracteres.
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Pais),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        // Restricción para el Estado: requerido, longitud entre 1 y 100 caracteres.
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Estado),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        // Restricción para el Municipio: requerido, longitud entre 1 y 100 caracteres.
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Municipio),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        // Restricción para la Colonia: requerido, longitud entre 1 y 100 caracteres.
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Colonia),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        // Restricción para la Calle: requerido, longitud entre 1 y 100 caracteres.
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Calle),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        // Restricción para el Número Exterior: requerido, longitud entre 1 y 6 caracteres.
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(NumeroExterior),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 6),

        // Restricción para el Número Interior: requerido, longitud entre 1 y 6 caracteres.
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(NumeroInterior),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 6),

        // Restricción para la Referencia: requerido, longitud entre 1 y 250 caracteres.
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Referencia),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 250),
    ];


    /// <summary>
    /// Obtiene el código postal de la dirección.
    /// </summary>
    [MaxLength(length: 5)]
    public string? CodigoPostal { get; private set; }

    /// <summary>
    /// Obtiene el país de la dirección.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Pais { get; private set; }

    /// <summary>
    /// Obtiene el estado o provincia de la dirección.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Estado { get; private set; }

    /// <summary>
    /// Obtiene el municipio o delegación de la dirección.
    /// </summary>
    [MaxLength(length: 100)]
    public string? Municipio { get; private set; }

    /// <summary>
    /// Obtiene la colonia o barrio de la dirección.
    /// </summary>
    [MaxLength(length: 100)]
    public string? Colonia { get; private set; }

    /// <summary>
    /// Obtiene la calle de la dirección.
    /// </summary>
    [MaxLength(length: 100)]
    public string? Calle { get; private set; }

    /// <summary>
    /// Obtiene el número exterior de la dirección.
    /// </summary>
    [MaxLength(length: 6)]
    public string? NumeroExterior { get; private set; }

    /// <summary>
    /// Obtiene el número interior de la dirección, si aplica.
    /// </summary>
    [MaxLength(length: 6)]
    public string? NumeroInterior { get; private set; }

    /// <summary>
    /// Obtiene una referencia adicional para la dirección (ej. entre calles, descripción).
    /// </summary>
    [MaxLength(length: 250)]
    public string? Referencia { get; private set; }

    /// <summary>
    /// Obtiene el identificador del cliente al que pertenece esta dirección.
    /// </summary>
    public int ClienteId { get; private set; }

    /// <summary>
    /// Obtiene el objeto Cliente asociado a esta dirección.
    /// </summary>
    public Cliente Cliente { get; private set; }


    /// <summary>
    /// Constructor por defecto para la clase Direccion.
    /// Inicializa la base con valores predeterminados.
    /// </summary>
    public Direccion() : base()
    {
        // Constructor vacío, la inicialización base se encarga de lo necesario.
    }

    /// <summary>
    /// Constructor para crear una nueva dirección durante el registro,
    /// inicializando el país y el estado.
    /// </summary>
    /// <param name="pais">El país de la dirección.</param>
    /// <param name="estado">El estado o provincia de la dirección.</param>
    /// <param name="creationUser">El GUID del usuario que crea la dirección.</param>
    /// <param name="testCase">Opcional: un caso de prueba para propósitos de desarrollo/testing.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si alguna de las propiedades iniciales (país, estado) no es válida.</exception>
    public Direccion(string pais, string estado, Guid creationUser, string? testCase = null) : base(
        creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para recolectar errores de validación.
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad 'Pais'.
        IsPropertyValid(propertyName: nameof(Pais), value: pais, exceptions: ref exceptions);
        // Valida la propiedad 'Estado'.
        IsPropertyValid(propertyName: nameof(Estado), value: estado, exceptions: ref exceptions);
        // Si hay excepciones, las lanza agrupadas.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna los valores validados a las propiedades.
        Pais = pais;
        Estado = estado;
    }

    /// <summary>
    /// Actualiza los detalles de la dirección con la información proporcionada.
    /// Realiza validaciones antes de asignar los nuevos valores.
    /// </summary>
    /// <param name="codigoPostal">El nuevo código postal.</param>
    /// <param name="municipio">El nuevo municipio o delegación.</param>
    /// <param name="colonia">La nueva colonia o barrio.</param>
    /// <param name="calle">La nueva calle.</param>
    /// <param name="numeroExterior">El nuevo número exterior.</param>
    /// <param name="numeroInterior">El nuevo número interior.</param>
    /// <param name="referencia">La nueva referencia adicional.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si alguna de las propiedades actualizadas no es válida.</exception>
    public void ActualizarDireccion(string codigoPostal,
        string municipio,
        string colonia,
        string calle,
        string numeroExterior,
        string numeroInterior,
        string referencia,
        Guid modificationUser)
    {
        List<EMGeneralException> exceptions = new();

        IsPropertyValid(propertyName: nameof(CodigoPostal), value: codigoPostal, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Municipio), value: municipio, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Colonia), value: colonia, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Calle), value: calle, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(NumeroExterior), value: numeroExterior, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(NumeroInterior), value: numeroInterior, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Referencia), value: referencia, exceptions: ref exceptions);

        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        bool hasChanges = false;

        if (this.CodigoPostal != codigoPostal)
        {
            this.CodigoPostal = codigoPostal;
            hasChanges = true;
        }

        if (this.Municipio != municipio)
        {
            this.Municipio = municipio;
            hasChanges = true;
        }

        if (this.Colonia != colonia)
        {
            this.Colonia = colonia;
            hasChanges = true;
        }

        if (this.Calle != calle)
        {
            this.Calle = calle;
            hasChanges = true;
        }

        if (this.NumeroExterior != numeroExterior)
        {
            this.NumeroExterior = numeroExterior;
            hasChanges = true;
        }

        if (this.NumeroInterior != numeroInterior)
        {
            this.NumeroInterior = numeroInterior;
            hasChanges = true;
        }

        if (this.Referencia != referencia)
        {
            this.Referencia = referencia;
            hasChanges = true;
        }

        if (hasChanges)
        {
            base.Update(modificationUser: modificationUser);
        }
    }
}