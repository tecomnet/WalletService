using Wallet.DOM.Errors;

namespace Wallet.DOM.Comun;

/// <summary>
/// Proporciona métodos para acceder a los errores de servicio definidos en el catálogo de errores.
/// </summary>
public class ServiceErrors
{
    private readonly ServiceErrorsBuilder _errorCatalog = ServiceErrorsBuilder.Instance();

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ServiceErrors"/>.
    /// </summary>
    public ServiceErrors()
    {
    }

    /// <summary>
    /// Obtiene un objeto de error de servicio basado en el código de error especificado.
    /// </summary>
    /// <param name="errorCode">El código del error de servicio a buscar.</param>
    /// <returns>Un objeto que implementa <see cref="IServiceError"/> si se encuentra el código de error; de lo contrario, devuelve un error predeterminado o nulo dependiendo de la implementación de <see cref="ServiceErrorsBuilder.GetError"/>.</returns>
    public IServiceError GetServiceErrorForCode(string errorCode)
    {
        return this._errorCatalog.GetError(errorCode: errorCode);
    }
}