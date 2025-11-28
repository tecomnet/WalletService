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
            propertyName: nameof(TwilioSid),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
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

    [Required] [MaxLength(length: 100)] public string TwilioSid { get; private set; }

    // TODO EMD: AGREGAR SID DE CONFIRMACION
    [MaxLength(length: 4)] public string? Codigo { get; private set; }
    [Required] public DateTime FechaVencimiento { get; private set; }
    [Required] public Tipo2FA Tipo { get; private set; }
    [Required] public bool Verificado { get; private set; }

    // Relaciones
    public int UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; }


    public Verificacion2FA() : base()
    {
    }

    public Verificacion2FA(string twilioSid, DateTime fechaVencimiento, Tipo2FA tipo, Guid creationUser,
        string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate properties
        IsPropertyValid(propertyName: nameof(TwilioSid), value: twilioSid, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(FechaVencimiento), value: fechaVencimiento, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Tipo), value: tipo, exceptions: ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Assign properties
        TwilioSid = twilioSid;
        FechaVencimiento = fechaVencimiento;
        Tipo = tipo;
        Verificado = false;
    }

    public void MarcarComoVerificado(string codigo, Guid modificationUser)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate properties
        IsPropertyValid(propertyName: nameof(Codigo), value: codigo, exceptions: ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Assign properties
        this.Verificado = true;
        this.Codigo = codigo;
        base.Update(modificationUser: modificationUser);
    }
}