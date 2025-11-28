using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

/// <summary>
/// Representa un documento de identificación, como un DNI, pasaporte, etc.
/// </summary>
public class Documento : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de validación para las propiedades de la clase Documento.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Nombre),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(TipoPersona),
            isRequired: true)
    ];
    
    /// <summary>
    /// Obtiene el nombre del documento (ej. "DNI", "Pasaporte").
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Nombre { get; private set; }
    
    /// <summary>
    /// Obtiene el tipo de persona al que aplica este documento (ej. Natural, Jurídica).
    /// </summary>
    [Required]
    public TipoPersona TipoPersona { get; private set; }
    
    /// <summary>
    /// Constructor por defecto de la clase Documento.
    /// </summary>
    public Documento() : base()
    {
        
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase Documento con los valores especificados.
    /// </summary>
    /// <param name="nombre">El nombre del documento.</param>
    /// <param name="tipoPersona">El tipo de persona asociado al documento.</param>
    /// <param name="creationUser">El GUID del usuario que crea el documento.</param>
    /// <param name="testCase">Opcional. Un caso de prueba para propósitos de desarrollo o testing.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si hay errores de validación en las propiedades.</exception>
    public Documento(string nombre, TipoPersona tipoPersona, Guid creationUser, string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para recolectar errores de validación.
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad Nombre.
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
        // Valida la propiedad TipoPersona.
        IsPropertyValid(propertyName: nameof(TipoPersona), value: tipoPersona, exceptions: ref exceptions);
        // Si se encontraron excepciones durante la validación, se lanzan como un agregado.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna los valores validados a las propiedades de la instancia.
        this.Nombre = nombre;   
        this.TipoPersona = tipoPersona;
    }
}