using System.Collections.Generic;

#nullable enable
namespace Wallet.RestAPI.Errors;

/// <summary>
/// Extra attributes for detailed error information
/// </summary>
public class RestAPIErrorExtraAttributes
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RestAPIErrorExtraAttributes"/> class.
    /// </summary>
    /// <param name="descriptionDynamicContents">Dynamic content for error description</param>
    /// <param name="module">Module identifier</param>
    /// <param name="serviceName">Service name</param>
    /// <param name="serviceLocation">Service location (URI or method)</param>
    public RestAPIErrorExtraAttributes(
        List<string> descriptionDynamicContents,
        string module,
        string serviceName,
        string? serviceLocation)
    {
        this.DescriptionDynamicContents = descriptionDynamicContents;
        this.Module = module;
        this.ServiceName = serviceName;
        this.ServiceLocation = serviceLocation;
    }

    /// <summary>
    /// Dynamic contents for description formatting
    /// </summary>
    public List<string> DescriptionDynamicContents { get; protected internal set; }

    /// <summary>
    /// Module where error occurred
    /// </summary>
    public string Module { get; private set; }

    /// <summary>
    /// Service name where error occurred
    /// </summary>
    public string ServiceName { get; private set; }

    /// <summary>
    /// Service location where error occurred
    /// </summary>
    public string? ServiceLocation { get; private set; }
}