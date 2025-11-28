using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

/// <summary>
/// Representa el tipo de documento de una persona, como DNI, Pasaporte, CUIT, etc.
/// Hereda de ValidatablePersistentObjectLogicalDelete para manejar validación y borrado lógico.
/// </summary>
public class TipoDocumento : ValidatablePersistentObjectLogicalDelete
{
    /// <summary>
    /// Define las restricciones de las propiedades para la validación del objeto.
    /// </summary>
    protected override List<PropertyConstraint> PropertyConstraints =>
    [
        PropertyConstraint.StringPropertyConstraint(
            propertyName: nameof(Nombre),
            isRequired: true,
            minimumLength: 1,
            maximumLength: 100)
    ];

    /// <summary>
    /// Obtiene el nombre del tipo de documento (ej. "DNI", "Pasaporte").
    /// Es un campo requerido y tiene una longitud máxima de 100 caracteres.
    /// </summary>
    [Required]
    [MaxLength(length: 100)]
    public string Nombre { get; private set; }

    /// <summary>
    /// Obtiene la lista de documentos asociados a este tipo de documento.
    /// </summary>
    public List<Documento> Documentos { get; private set; }

    /// <summary>
    /// Constructor por defecto de TipoDocumento.
    /// Utilizado principalmente por el ORM.
    /// </summary>
    public TipoDocumento() : base() { }

    /// <summary>
    /// Constructor para crear una nueva instancia de TipoDocumento con un nombre específico.
    /// </summary>
    /// <param name="nombre">El nombre del tipo de documento.</param>
    /// <param name="creationUser">El ID del usuario que crea el tipo de documento.</param>
    /// <param name="testCase">Caso de prueba opcional para validación.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si las validaciones iniciales fallan.</exception>
    public TipoDocumento(string nombre, Guid creationUser, string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para recolectar errores de validación
        List<EMGeneralException> exceptions = new();
        // Valida la propiedad 'Nombre' usando las restricciones definidas
        IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
        // Si hay excepciones, las lanza agrupadas en una EMGeneralAggregateException
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        // Asigna el nombre validado a la propiedad
        this.Nombre = nombre;
    }

    /// <summary>
    /// Agrega un documento a la lista de documentos asociados a este tipo de documento.
    /// </summary>
    /// <param name="documento">El objeto Documento a agregar.</param>
    /// <exception cref="EMGeneralAggregateException">
    /// Se lanza si el documento es nulo, o si ya existe un documento con el mismo nombre y tipo de persona.
    /// </exception>
    public void AgregarDocumento(Documento documento)
    {
        // Inicializa la lista de documentos si es nula
        if (this.Documentos == null)
        {
            this.Documentos = new List<Documento>();
        }
        // Valida que el documento a agregar no sea nulo
        if (documento == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                   errorCode: ServiceErrorsBuilder.DocumentoRequerido,
                   dynamicContent: []));
        }
        // Verifica si ya existe un documento con el mismo nombre y tipo de persona en la lista
        if (this.Documentos.Any(predicate: d => d.Nombre == documento.Nombre && d.TipoPersona == documento.TipoPersona))
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                   errorCode: ServiceErrorsBuilder.DocumentoYaExisteEnTipoDocumento,
                   dynamicContent: [documento.Nombre, documento.TipoPersona]));
        }
        // Agrega el documento a la lista
        this.Documentos.Add(item: documento);
    }

}