using System.Collections.Generic;

#nullable enable
namespace Wallet.RestAPI.Errors;

/// <summary>
/// Specific implementation of IRestAPIError with extra attributes
/// </summary>
public class RestAPIError : DefaultRestAPIError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RestAPIError"/> class.
    /// </summary>
    /// <param name="errorCode">Unique error code</param>
    /// <param name="type">Error type URL</param>
    /// <param name="title">Error title</param>
    /// <param name="status">HTTP status code</param>
    /// <param name="detail">Error detail</param>
    /// <param name="instance">Error instance identifier</param>
    /// <param name="descriptionDynamicContents">Dynamic content for description</param>
    /// <param name="module">Module name</param>
    /// <param name="serviceName">Service name</param>
    /// <param name="serviceLocation">Service location</param>
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
        this.ExtraAttributes = new RestAPIErrorExtraAttributes(descriptionDynamicContents: descriptionDynamicContents,
            module: module, serviceName: serviceName, serviceLocation: serviceLocation);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RestAPIError"/> class.
    /// </summary>
    /// <param name="errorCode">Unique error code</param>
    /// <param name="type">Error type URL</param>
    /// <param name="title">Error title</param>
    /// <param name="status">HTTP status code</param>
    /// <param name="detail">Error detail</param>
    /// <param name="instance">Error instance identifier</param>
    /// <param name="module">Module name</param>
    /// <param name="serviceName">Service name</param>
    /// <param name="serviceLocation">Service location</param>
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
        this.ExtraAttributes = new RestAPIErrorExtraAttributes(descriptionDynamicContents: new List<string>(),
            module: module, serviceName: serviceName, serviceLocation: serviceLocation);
    }

    /// <summary>
    /// Gets the extra attributes associated with the error
    /// </summary>
    public RestAPIErrorExtraAttributes ExtraAttributes { get; private set; }

    /// <summary>
    /// Updates the dynamic content of the error
    /// </summary>
    /// <param name="dynamicContent">The dynamic content used to format error messages</param>
    public void UpdateDynamicContent(List<string> dynamicContent)
    {
        this.ExtraAttributes.DescriptionDynamicContents = dynamicContent;
    }
}