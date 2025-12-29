using Wallet.DOM.Errors;

namespace Wallet.DOM.Comun;

/// <summary>
/// Clase base para objetos persistentes validables que soportan borrado lógico.
/// Extiende <see cref="PersistentClassLogicalDelete"/> para añadir funcionalidades de validación de propiedades.
/// </summary>
public class ValidatablePersistentObjectLogicalDelete : PersistentClassLogicalDelete
{
    /// <summary>
    /// Obtiene una lista de restricciones de propiedades aplicables a este objeto.
    /// Las clases derivadas deben sobrescribir esta propiedad para definir sus propias restricciones.
    /// </summary>
    protected virtual List<PropertyConstraint> PropertyConstraints
    {
        get => new List<PropertyConstraint>();
    }

    /// <summary>
    /// Constructor protegido predeterminado.
    /// </summary>
    protected ValidatablePersistentObjectLogicalDelete()
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ValidatablePersistentObjectLogicalDelete"/>
    /// con el usuario de creación y un caso de prueba opcional.
    /// </summary>
    /// <param name="creationUser">El identificador único del usuario que crea el objeto.</param>
    /// <param name="testCase">Un identificador opcional para casos de prueba.</param>
    public ValidatablePersistentObjectLogicalDelete(Guid creationUser, string? testCase = null)
        : base(creationUser: creationUser, testCase: testCase)
    {
    }

    /// <summary>
    /// Valida el valor de una propiedad específica del objeto.
    /// </summary>
    /// <param name="propertyName">El nombre de la propiedad a validar.</param>
    /// <param name="value">El valor de la propiedad a validar.</param>
    /// <param name="exceptions">Una lista de excepciones <see cref="EMGeneralException"/> donde se añadirán los errores de validación.</param>
    /// <returns>
    /// <c>true</c> si la propiedad es válida; de lo contrario, <c>false</c>.
    /// Si la propiedad no tiene restricciones definidas, se añade una excepción y se retorna <c>false</c>.
    /// </returns>
    public bool IsPropertyValid(
        string propertyName,
        object? value,
        ref List<EMGeneralException> exceptions)
    {
        // Se crea una lista temporal para almacenar las excepciones de la validación de la propiedad.
        List<EMGeneralException> newExceptions = new List<EMGeneralException>();

        // Verifica si no existe ninguna restricción para la propiedad especificada.
        if (this.PropertyConstraints.All(predicate: (Func<PropertyConstraint, bool>) (pc => pc.PropertyName != propertyName)))
        {
            // Si la propiedad no se encuentra en las restricciones definidas, se genera un error específico.
            IServiceError serviceErrorForCode = new ServiceErrors().GetServiceErrorForCode(errorCode: ServiceErrorsBuilder.PropertyValidationPropertyNotFound);
            List<object> descriptionDynamicContents = new List<object>()
            {
                propertyName
            };
            // Se añade la excepción a la lista de excepciones pasada por referencia.
            exceptions.Add(item: new EMGeneralException(message: serviceErrorForCode.Message, code: serviceErrorForCode.ErrorCode, title: serviceErrorForCode.Title, description: serviceErrorForCode.Description(args: descriptionDynamicContents.ToArray()), serviceName: "PersistentObject", serviceInstance: null, serviceLocation: null, module: "DOM", descriptionDynamicContents: descriptionDynamicContents));
            return false;
        }

        // Busca la restricción de propiedad correspondiente y llama a su método de validación.
        // El resultado de la validación se almacena en 'num' (1 para válido, 0 para inválido).
        int num = this.PropertyConstraints.Single(predicate: (Func<PropertyConstraint, bool>) (pc => pc.PropertyName == propertyName)).IsPropertyValid(value: value, exceptions: out newExceptions) ? 1 : 0;
        
        // Se añaden las excepciones encontradas durante la validación de la propiedad a la lista principal.
        exceptions.AddRange(collection: newExceptions);
        
        // Retorna true si la validación fue exitosa (num es 1), o false en caso contrario.
        return num != 0;
    }
}