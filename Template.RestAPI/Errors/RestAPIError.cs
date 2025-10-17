using System.Collections.Generic;

namespace Template.RestAPI.Errors;

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
        this.ExtraAttributes = new RestAPIErrorExtraAttributes(descriptionDynamicContents, module, serviceName, serviceLocation);
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
        this.ExtraAttributes = new RestAPIErrorExtraAttributes(new List<string>(), module, serviceName, serviceLocation);
    }

    public RestAPIErrorExtraAttributes ExtraAttributes { get; private set; }

    public void UpdateDynamicContent(List<string> dynamicContent)
    {
        this.ExtraAttributes.DescriptionDynamicContents = dynamicContent;
    }
}