using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

// TODO EMD: ESTE DEBE SER UN CATALOGO CON ADMIN WEB
public class Estado : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Nombre),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
    ];
    
    [Key]
    public int Id { get; internal set; }
    [Required]
    [MaxLength(100)]
    public string Nombre { get; internal set; }

    public List<Cliente> Clientes { get; private set; }
    
    public Estado() : base() { }

    public Estado(string nombre, Guid creationUser,
        string? testCase = null) : base(creationUser, testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Seteo de propiedades
        this.Nombre = nombre;
    }
}