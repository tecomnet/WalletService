using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

/// <summary>
/// Representa una actividad económica realizada por una entidad.
/// Hereda de <see cref="ValidatablePersistentObjectLogicalDelete"/> para proporcionar validación y eliminación lógica.
/// </summary>
public class ActividadEconomica : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de validación para las propiedades de la actividad económica.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        // Restricción para el nombre de la actividad económica
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Nombre),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        // Restricción para el ingreso generado por la actividad económica
        PropertyConstraint.DecimalPropertyConstraint(
            propertyName: nameof(Ingreso),
            isRequired: true,
            allowNegative: false,
            allowZero: false,
            allowPositive: true,
            allowedDecimals: 2),
        // Restricción para el origen del recurso de la actividad económica
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(OrigenRecurso),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        // Restricción para el archivo AWS asociado a la actividad económica
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(ArchivoAWS),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 500)
    ];
    
    /// <summary>
    /// Obtiene el nombre de la actividad económica.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Nombre { get; private set; }
    /// <summary>
    /// Obtiene el ingreso generado por la actividad económica.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Ingreso {get; private set;}
    /// <summary>
    /// Obtiene el origen del recurso de la actividad económica.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string OrigenRecurso {get; private set;}
    /// <summary>
    /// Obtiene la referencia al archivo AWS asociado a la actividad económica.
    /// </summary>
    [Required]
    [MaxLength(length: 500)]
    public string ArchivoAWS { get; private set; }
    
    /// <summary>
    /// Constructor por defecto de la clase <see cref="ActividadEconomica"/>.
    /// Inicializa una nueva instancia de la clase.
    /// </summary>
    public ActividadEconomica() : base()
    {
        
    }

    /// <summary>
    /// Constructor para crear una nueva instancia de <see cref="ActividadEconomica"/> con valores iniciales.
    /// Realiza la validación de las propiedades antes de asignarlas.
    /// </summary>
    /// <param name="nombre">El nombre de la actividad económica.</param>
    /// <param name="ingreso">El ingreso generado por la actividad económica.</param>
    /// <param name="origenRecurso">El origen del recurso de la actividad económica.</param>
    /// <param name="archivoAWS">La referencia al archivo AWS asociado.</param>
    /// <param name="creationUser">El GUID del usuario que crea la actividad económica.</param>
    /// <param name="testCase">Opcional: Un caso de prueba para propósitos de desarrollo/pruebas.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si alguna de las propiedades no es válida.</exception>
    public ActividadEconomica(string nombre, decimal ingreso, string origenRecurso, string archivoAWS, Guid creationUser, string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para acumular errores de validación
        List<EMGeneralException> exceptions = new();
        // Valida cada propiedad utilizando las restricciones definidas
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Ingreso), value: ingreso, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(OrigenRecurso), value: origenRecurso, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(ArchivoAWS), value: archivoAWS, exceptions: ref exceptions);
        // Si hay excepciones, se lanzan como una excepción agregada
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asignación de propiedades si todas las validaciones son exitosas
        this.Nombre = nombre;
        this.Ingreso = ingreso;
        this.OrigenRecurso = origenRecurso;
        this.ArchivoAWS = archivoAWS;
    }
}