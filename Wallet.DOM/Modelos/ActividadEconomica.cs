using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class ActividadEconomica : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Nombre),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.DecimalPropertyConstraint(
            propertyName: nameof(Ingreso),
            isRequired: true,
            allowNegative: false,
            allowZero: false,
            allowPositive: true,
            allowedDecimals: 2),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(OrigenRecurso),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(ArchivoAWS),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 500)
    ];


    [Key]
    public int Id { get; private set; }
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; private set; }
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Ingreso {get; private set;}
    [Required]
    [MaxLength(100)]
    public string OrigenRecurso {get; private set;}
    [Required]
    [MaxLength(500)]
    public string ArchivoAWS { get; private set; }
    
    public ActividadEconomica() : base()
    {
        
    }

    public ActividadEconomica(string nombre, decimal ingreso, string origenRecurso, string archivoAWS, Guid creationUser, string? testCase = null) : base(creationUser, testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate properties
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, ref exceptions);
        IsPropertyValid(propertyName: nameof(Ingreso), value: ingreso, ref exceptions);
        IsPropertyValid(propertyName: nameof(OrigenRecurso), value: origenRecurso, ref exceptions);
        IsPropertyValid(propertyName: nameof(ArchivoAWS), value: archivoAWS, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Seteo de propiedades
        this.Nombre = nombre;
        this.Ingreso = ingreso;
        this.OrigenRecurso = origenRecurso;
        this.ArchivoAWS = archivoAWS;
    }
}   