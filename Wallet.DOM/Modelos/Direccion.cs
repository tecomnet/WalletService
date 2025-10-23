using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class Direccion : ValidatablePersistentObjectLogicalDelete
{

    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(CodigoPostal),
            isRequired: true,
            minimumLength: 5,
            maximumLength: 5),

       PropertyConstraint.StringPropertyConstraint(
                   propertyName: nameof(Pais),
                   isRequired: true,
                   minimumLength: 1,
                   maximumLength: 100),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Estado),
            isRequired: true,
            minimumLength: 1,
            maximumLength:100),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Municipio),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Colonia),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Calle),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(NumeroExterior),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 6),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(NumeroInterior),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 6),

        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Referencia),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 250),
    ];



    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(5)]
    public string CodigoPostal { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Pais { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Estado { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Municipio { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Colonia { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Calle { get; private set; }

    [Required]
    [MaxLength(6)]
    public string NumeroExterior { get; private set; }

    [Required]
    [MaxLength(6)]
    public string NumeroInterior { get; private set; }
    [Required]
    [MaxLength(250)]
    public string? Referencia { get; private set; }

    public Cliente Cliente { get; private set; }


    public Direccion() : base()
    {

    }

    /// <summary>
    /// Nueva direccion durante el registro y seleccion del estado
    /// </summary>
    /// <param name="pais"></param>
    /// <param name="estado"></param>
    /// <exception cref="EMGeneralAggregateException"></exception>
    public Direccion(string pais, string estado, Guid creationUser, string? testCase = null) : base(creationUser, testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(Pais), value: pais, ref exceptions);
        IsPropertyValid(propertyName: nameof(Estado), value: estado, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        Pais = pais;
        Estado = estado;
    }

    public void ActualizarDireccion(string codigoPostal,
        string municipio,
        string colonia,
        string calle,
        string numeroExterior,
        string numeroInterior,
        string? referencia,
        Guid modificationUser)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate the properties
        IsPropertyValid(propertyName: nameof(CodigoPostal), value: codigoPostal, ref exceptions);
        IsPropertyValid(propertyName: nameof(Municipio), value: municipio, ref exceptions);
        IsPropertyValid(propertyName: nameof(Colonia), value: colonia, ref exceptions);
        IsPropertyValid(propertyName: nameof(Calle), value: calle, ref exceptions);
        IsPropertyValid(propertyName: nameof(NumeroExterior), value: numeroExterior, ref exceptions);
        IsPropertyValid(propertyName: nameof(NumeroInterior), value: numeroInterior, ref exceptions);
        IsPropertyValid(propertyName: nameof(Referencia), value: referencia, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        CodigoPostal = codigoPostal;
        Municipio = municipio;
        Colonia = colonia;
        Calle = calle;
        NumeroExterior = numeroExterior;
        NumeroInterior = numeroInterior;
        Referencia = referencia;
        base.Update(modificationUser: modificationUser);
    }
}