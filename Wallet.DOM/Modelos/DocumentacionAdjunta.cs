using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos;

/// <summary>
/// Representa la documentación adjunta a un documento, como archivos subidos a AWS.
/// Hereda de <see cref="ValidatablePersistentObjectLogicalDelete"/> para incluir validaciones y gestión de borrado lógico.
/// </summary>
public class DocumentacionAdjunta :  ValidatablePersistentObjectLogicalDelete
{

    /// <summary>
    /// Define las restricciones de validación para las propiedades de <see cref="DocumentacionAdjunta"/>.
    /// </summary>
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
    
    /// <summary>
    /// Obtiene la referencia o URL del archivo adjunto almacenado en AWS.
    /// </summary>
    [Required]
    [MaxLength(length: 500)]
    public string ArchivoAWS { get; private set; }
    
    /// <summary>
    /// Obtiene el objeto <see cref="Documento"/> al que está adjunta esta documentación.
    /// </summary>
    public Documento Documento { get; private set; }


    /// <summary>
    /// Constructor por defecto de <see cref="DocumentacionAdjunta"/>.
    /// </summary>
    public DocumentacionAdjunta() : base()
    {
        
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="DocumentacionAdjunta"/> con los valores especificados.
    /// Realiza validaciones sobre los parámetros de entrada.
    /// </summary>
    /// <param name="documento">El objeto <see cref="Documento"/> al que se adjunta.</param>
    /// <param name="archivoAWS">La referencia o URL del archivo en AWS.</param>
    /// <param name="creationUser">El GUID del usuario que crea la documentación adjunta.</param>
    /// <param name="testCase">Un caso de prueba opcional.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si alguna validación falla.</exception>
    public DocumentacionAdjunta(Documento documento, string archivoAWS, Guid creationUser, string? testCase = null) : base(creationUser: creationUser, testCase: testCase)
    {
        // Inicializa la lista de excepciones para recolectar errores de validación.
        List<EMGeneralException> exceptions = new();
        
        // Valida las propiedades ArchivoAWS y Documento utilizando las restricciones definidas.
        IsPropertyValid(propertyName: nameof(ArchivoAWS), value: archivoAWS, exceptions: ref exceptions);
        IsPropertyValid(propertyName: nameof(Documento), value: documento, exceptions: ref exceptions);
        
        // Si hay excepciones, se lanzan encapsuladas en un EMGeneralAggregateException.
        if (exceptions.Count > 0) throw new EMGeneralAggregateException(exceptions: exceptions);
        
        // Asignación de propiedades después de la validación exitosa.
        this.ArchivoAWS = archivoAWS;
        this.Documento = documento;
    }
}