using Template.DOM.Errors;

namespace Template.DOM.Comun;

public class ServiceErrors
{
    private readonly ServiceErrorsBuilder _errorCatalog = new ServiceErrorsBuilder();

    public ServiceErrors() => this.PropertyValidationErrors();

    public IServiceError GetServiceErrorForCode(string errorCode)
    {
        return this._errorCatalog.GetError(errorCode);
    }

    private void PropertyValidationErrors()
    {
        this._errorCatalog.AddServiceError("PROPERTY-VALIDATION-REQUIRED-ERROR", "Error de validación de propiedad", "La propiedad {0} es requerida.");
        this._errorCatalog.AddServiceError("PROPERTY-VALIDATION-LENGTH-INVALID", "Error de validación de propiedad", "La propiedad {0} con valor {1} debe tener entre {2} y {3} caracteres de longitud.");
        this._errorCatalog.AddServiceError("PROPERTY-VALIDATION-REGEX-INVALID", "Error de validación de propiedad", "La propiedad {0} con valor {1} no es válida según el patrón proporcionado {2}.");
        this._errorCatalog.AddServiceError("PROPERTY-VALIDATION-NEGATIVE-INVALID", "Error de validación de propiedad", "La propiedad {0} con valor {1} no permite valores negativos.");
        this._errorCatalog.AddServiceError("PROPERTY-VALIDATION-ZERO-INVALID", "Error de validación de propiedad", "La propiedad {0} con valor {1} no permite valores cero.");
        this._errorCatalog.AddServiceError("PROPERTY-VALIDATION-POSITIVE-INVALID", "Error de validación de propiedad", "La propiedad {0} con valor {1} no permite valores positivos.");
        this._errorCatalog.AddServiceError("PROPERTY-VALIDATION-DECIMALS-INVALID", "Error de validación de propiedad", "La propiedad {0} con valor {1} no permite más de {2} decimales.");
        this._errorCatalog.AddServiceError("PROPERTY-VALIDATION-CURRENCY-INVALID", "Error de validación de propiedad", "La propiedad {0} con valor {1} no es una divisa/moneda válida.");
        this._errorCatalog.AddServiceError("PROPERTY-VALIDATION-PROPERTY-NOT-FOUND", "Error de validación de propiedad", "La propiedad {0} no fue encontrada en la definición de la propiedad.");
    }
}