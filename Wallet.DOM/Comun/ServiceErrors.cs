using Wallet.DOM.Errors;

namespace Wallet.DOM.Comun;

public class ServiceErrors
{
    private readonly ServiceErrorsBuilder _errorCatalog = ServiceErrorsBuilder.Instance();

    public ServiceErrors()
    {
    }

    public IServiceError GetServiceErrorForCode(string errorCode)
    {
        return this._errorCatalog.GetError(errorCode);
    }
}