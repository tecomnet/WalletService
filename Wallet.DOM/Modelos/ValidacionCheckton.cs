using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class ValidacionCheckton : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(TipoCheckton),
            isRequired: true),
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(Resultado),
            isRequired: true)
    ];

    [Required]
    public TipoCheckton TipoCheckton { get; private set; }
    [Required]
    public bool Resultado { get; private set; }

    public ValidacionCheckton() : base()
    {
        
    }
    public ValidacionCheckton(TipoCheckton tipoCheckton, bool resultado, Guid creationUser, string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate properties
        IsPropertyValid(propertyName: nameof(TipoCheckton), value: tipoCheckton, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Resultado), value: resultado, exceptions: ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Seteo de propiedades
        this.TipoCheckton = tipoCheckton;
        this.Resultado = resultado;
    }
}