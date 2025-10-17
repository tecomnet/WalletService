using System.Collections.Generic;

namespace Template.RestAPI.Errors;

public class RestAPIErrorExtraAttributes
{
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

    public List<string> DescriptionDynamicContents { get; protected internal set; }

    public string Module { get; private set; }

    public string ServiceName { get; private set; }

    public string? ServiceLocation { get; private set; }
}