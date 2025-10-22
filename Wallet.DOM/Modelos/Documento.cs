using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class Documento : ValidatablePersistentObjectLogicalDelete
{
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

    [Key]
    public int Id { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; private set; }
    [Required]
    public TipoPersona TipoPersona { get; private set; }
    
    
    public Documento(string nombre, TipoPersona tipoPersona, Guid creationUser, string? testCase = null) : base(creationUser, testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate properties
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, ref exceptions);
        IsPropertyValid(propertyName: nameof(TipoPersona), value: tipoPersona, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Seteo de propiedades
        this.Nombre = nombre;   
        this.TipoPersona = tipoPersona;
    }
}