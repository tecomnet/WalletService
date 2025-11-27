using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

public class TipoDocumento : ValidatablePersistentObjectLogicalDelete
{
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Nombre),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100)
    ];

    [Required]
    [MaxLength(100)]
    public string Nombre { get; private set; }

    public List<Documento> Documentos { get; private set; }

    public TipoDocumento() : base() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nombre"></param>
    /// <param name="creationUser"></param>
    /// <param name="testCase"></param>
    /// <exception cref="EMGeneralAggregateException"></exception>
    public TipoDocumento(string nombre, Guid creationUser, string? testCase = null) : base(creationUser, testCase)
    {
        // Initialize the list of exceptions
        List<EMGeneralException> exceptions = new();
        // Validate properties
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, ref exceptions);
        // If there are exceptions, throw them
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Seteo de propiedades);
        this.Nombre = nombre;
    }

    /// <summary>
    /// Agrega un documento
    /// </summary>
    /// <param name="documento"></param>
    /// <exception cref="EMGeneralAggregateException"></exception>
    public void AgregarDocumento(Documento documento)
    {
        if (this.Documentos == null)
        {
            this.Documentos = new List<Documento>();
        }
        if (documento == null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                   errorCode: ServiceErrorsBuilder.DocumentoRequerido,
                   dynamicContent: []));
        }
        if (this.Documentos.Any(d => d.Nombre == documento.Nombre && d.TipoPersona == documento.TipoPersona))
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                   errorCode: ServiceErrorsBuilder.DocumentoYaExisteEnTipoDocumento,
                   dynamicContent: [documento.Nombre, documento.TipoPersona]));
        }
        this.Documentos.Add(documento);
    }

}