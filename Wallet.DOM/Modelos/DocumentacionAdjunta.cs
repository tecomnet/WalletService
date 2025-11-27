using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class DocumentacionAdjunta :  ValidatablePersistentObjectLogicalDelete
{

    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(ArchivoAWS),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 500),
        PropertyConstraint.ObjectPropertyConstraint(
            propertyName: nameof(Documento),
            isRequired: true)
    ];
    
    [Required]
    [MaxLength(500)]
    public string ArchivoAWS { get; private set; }
    
    public Documento Documento { get; private set; }


    public DocumentacionAdjunta() : base()
    {
        
    }

    public DocumentacionAdjunta(Documento documento, string archivoAWS, Guid creationUser, string? testCase = null) : base(creationUser, testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate properties
        IsPropertyValid(propertyName: nameof(ArchivoAWS), value: archivoAWS, ref exceptions);
        IsPropertyValid(propertyName: nameof(Documento), value: documento, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Seteo de propiedades
        this.ArchivoAWS = archivoAWS;
        this.Documento = documento;
    }
}