using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

/// <summary>
/// Representa una validación de tipo "Checkton", que encapsula el tipo de checkton y su resultado.
/// Hereda de <see cref="ValidatablePersistentObjectLogicalDelete"/> para incluir validación y borrado lógico.
/// </summary>
public class ValidacionCheckton : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de las propiedades para la validación de este objeto.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        // Restricción para la propiedad TipoCheckton, indicando que es requerida.
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(TipoCheckton),
            isRequired: true),
        // Restricción para la propiedad Resultado, indicando que es requerida.
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(Resultado),
            isRequired: true)
    ];

    /// <summary>
    /// Obtiene el tipo de Checkton asociado a esta validación.
    /// Es una propiedad requerida.
    /// </summary>
    [Required]
    public TipoCheckton TipoCheckton { get; private set; }
    /// <summary>
    /// Obtiene el resultado booleano de la validación del Checkton.
    /// Es una propiedad requerida.
    /// </summary>
    [Required]
    public bool Resultado { get; private set; }

    /// <summary>
    /// Constructor por defecto de la clase <see cref="ValidacionCheckton"/>.
    /// Inicializa una nueva instancia sin valores específicos, utilizando el constructor base.
    /// </summary>
    public ValidacionCheckton() : base()
    {
        
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ValidacionCheckton"/> con los valores especificados.
    /// Realiza validaciones de las propiedades antes de asignarlas.
    /// </summary>
    /// <param name="tipoCheckton">El tipo de Checkton para esta validación.</param>
    /// <param name="resultado">El resultado booleano de la validación.</param>
    /// <param name="creationUser">El identificador del usuario que crea esta validación.</param>
    /// <param name="testCase">Opcional. Un caso de prueba asociado a esta validación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si alguna de las propiedades iniciales no es válida.</exception>
    public ValidacionCheckton(TipoCheckton tipoCheckton, bool resultado, Guid creationUser, string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para capturar errores de validación.
        List<EMGeneralException> exceptions = new();
        
        // Valida la propiedad TipoCheckton.
        IsPropertyValid(propertyName: nameof(TipoCheckton), value: tipoCheckton, exceptions: ref exceptions);
        // Valida la propiedad Resultado.
        IsPropertyValid(propertyName: nameof(Resultado), value: resultado, exceptions: ref exceptions);
        
        // Si hay excepciones, las lanza como una excepción agregada.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        
        // Asignación de propiedades una vez que la validación ha sido exitosa.
        this.TipoCheckton = tipoCheckton;
        this.Resultado = resultado;
    }
}