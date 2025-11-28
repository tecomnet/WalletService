using System.Collections.Generic;

namespace Wallet.RestAPI.Errors;

public class RestAPIError : DefaultRestAPIError
{
    public RestAPIError(
        string errorCode,
        string? type,
        string title,
        int status,
        string detail,
        string instance,
        List<string> descriptionDynamicContents,
        string module,
        string serviceName,
        string serviceLocation)
    {
        this.ErrorCode = errorCode;
        this.Type = type ?? "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        this.Title = title;
        this.Status = status;
        this._detail = detail;
        this.Instance = instance;
        this.ExtraAttributes = new RestAPIErrorExtraAttributes(descriptionDynamicContents: descriptionDynamicContents, module: module, serviceName: serviceName, serviceLocation: serviceLocation);
    }

    public RestAPIError(
        string errorCode,
        string? type,
        string title,
        int status,
        string detail,
        string instance,
        string module,
        string serviceName,
        string serviceLocation)
    {
        this.ErrorCode = errorCode;
        this.Type = type ?? "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        this.Title = title;
        this.Status = status;
        this._detail = detail;
        this.Instance = instance;
        this.ExtraAttributes = new RestAPIErrorExtraAttributes(descriptionDynamicContents: new List<string>(), module: module, serviceName: serviceName, serviceLocation: serviceLocation);
    }

    public RestAPIErrorExtraAttributes ExtraAttributes { get; private set; }

    public void UpdateDynamicContent(List<string> dynamicContent)
    {
        this.ExtraAttributes.DescriptionDynamicContents = dynamicContent;
    }
}