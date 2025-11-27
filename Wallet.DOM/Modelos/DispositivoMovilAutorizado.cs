using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class DispositivoMovilAutorizado : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Token),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(IdDispositivo),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Nombre),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Caracteristicas),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
    ];

    [Required] [MaxLength(100)] public string Token { get; private set; }
    [Required] [MaxLength(100)] public string IdDispositivo { get; private set; }
    [Required] [MaxLength(100)] public string Nombre { get; private set; }

    [Required] [MaxLength(100)] public string Caracteristicas { get; private set; }

    [Required] public bool Actual { get; private set; }

    public int UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; }

    public DispositivoMovilAutorizado() : base()
    {
    }

    public DispositivoMovilAutorizado(
        string token,
        string idDispositivo,
        string nombre,
        string caracteristicas,
        Guid creationUser,
        string? testCase = null) : base(creationUser, testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(Token), value: token, ref exceptions);
        IsPropertyValid(propertyName: nameof(IdDispositivo), value: idDispositivo, ref exceptions);
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, ref exceptions);
        IsPropertyValid(propertyName: nameof(Caracteristicas), value: caracteristicas, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        Token = token;
        IdDispositivo = idDispositivo;
        Nombre = nombre;
        Caracteristicas = caracteristicas;
        Actual = true;
    }

    public void MarcarComoNoActual()
    {
        Actual = false;
    }
}