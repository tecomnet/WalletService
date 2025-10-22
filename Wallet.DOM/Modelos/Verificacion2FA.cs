using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class Verificacion2FA : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Codigo),
            isRequired: true,
            minimumLength: 4,
            maximumLength: 4),
        PropertyConstraint.DateTimePropertyConstraint(
            propertyName: nameof(FechaVencimiento),
            isRequired: true),
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(Tipo),
            isRequired: true),
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(Verificado),
            isRequired: true)
    ];


    [Key]
    public int Id { get; private set; }
    [Required]
    [MaxLength(4)]
    public string Codigo { get; private set; }
    [Required]
    public DateTime FechaVencimiento { get; private set; }
    [Required]
    public Tipo2FA Tipo { get; private set; }
    [Required]
    public bool Verificado { get; private set; }


    public Verificacion2FA(string codigo, DateTime fechaVencimiento, Tipo2FA tipo, Guid creationUser, string? testCase = null) : base(creationUser, testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate properties
        IsPropertyValid(propertyName: nameof(Codigo), value: codigo, ref exceptions);
        IsPropertyValid(propertyName: nameof(FechaVencimiento), value: fechaVencimiento, ref exceptions);
        IsPropertyValid(propertyName: nameof(Tipo), value: tipo, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Assign properties
        Codigo = codigo;
        FechaVencimiento = fechaVencimiento;
        Tipo = tipo;
        Verificado = false;
    }

    public void MarcarComoVerificado(Guid modificationUser)
    {
        this.Verificado = true;
        base.Update(modificationUser: modificationUser);
    }
}