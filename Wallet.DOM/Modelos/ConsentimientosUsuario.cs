using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

/// <summary>
/// Representa los consentimientos de un usuario en el sistema.
/// Hereda de <see cref="ValidatablePersistentObjectLogicalDelete"/> para incluir validaciones y lógica de borrado lógico.
/// </summary>
public class ConsentimientosUsuario : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de las propiedades de la clase para la validación.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Version),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 20)
    ];

    /// <summary>
    /// Obtiene o establece el identificador único del usuario al que pertenece el consentimiento.
    /// Es una clave foránea a la entidad <see cref="Usuario"/>.
    /// </summary>
    [ForeignKey("Usuario")] public int IdUsuario { get; private set; }

    /// <summary>
    /// Obtiene o establece el tipo de documento de consentimiento.
    /// </summary>
    [Required] public TipoDocumentoConsentimiento TipoDocumento { get; private set; }

    /// <summary>
    /// Obtiene o establece la versión del documento de consentimiento.
    /// Este campo es requerido y tiene una longitud máxima de 20 caracteres.
    /// </summary>
    [Required] [MaxLength(length: 20)] public string Version { get; private set; }

    /// <summary>
    /// Obtiene o establece la fecha en que el usuario aceptó el consentimiento.
    /// </summary>
    public DateTime FechaAceptacion { get; private set; }

    /// <summary>
    /// Propiedad de navegación para acceder a la entidad <see cref="Usuario"/> asociada.
    /// </summary>
    public virtual Usuario Usuario { get; private set; }

    /// <summary>
    /// Constructor por defecto de la clase <see cref="ConsentimientosUsuario"/>.
    /// </summary>
    public ConsentimientosUsuario() : base()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ConsentimientosUsuario"/> con los datos especificados.
    /// </summary>
    /// <param name="idUsuario">El identificador único del usuario.</param>
    /// <param name="tipoDocumento">El tipo de documento de consentimiento.</param>
    /// <param name="version">La versión del documento de consentimiento.</param>
    /// <param name="fechaAceptacion">La fecha de aceptación del consentimiento.</param>
    /// <param name="creationUser">El GUID del usuario que crea este registro.</param>
    /// <param name="testCase">Opcional: un identificador de caso de prueba.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si hay errores de validación en las propiedades.</exception>
    public ConsentimientosUsuario(
        int idUsuario,
        TipoDocumentoConsentimiento tipoDocumento,
        string version,
        DateTime fechaAceptacion,
        Guid creationUser,
        string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Lista para acumular excepciones de validación de propiedades.
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad 'Version' usando las restricciones definidas.
        IsPropertyValid(propertyName: nameof(Version), value: version, exceptions: ref exceptions);

        // Si se encontraron excepciones durante la validación, lanza un EMGeneralAggregateException.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);

        // Asigna los valores proporcionados a las propiedades de la instancia.
        IdUsuario = idUsuario;
        TipoDocumento = tipoDocumento;
        Version = version;
        FechaAceptacion = fechaAceptacion;
    }
}
