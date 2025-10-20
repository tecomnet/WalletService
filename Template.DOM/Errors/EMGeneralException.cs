namespace Template.DOM.Errors;

public class EMGeneralException : Exception
{
    public IServiceError ServiceError { get; }
    public string Code { get; }

    public string Title { get; }

    public string Description { get; }

    public List<string>? DescriptionDynamicContents { get; }
    public string ServiceName { get; }
    public string ServiceInstance { get; }

    public string ServiceLocation { get; }
    public string Module { get; }
    public EMGeneralException(
        string message,
        string code,
        string title,
        string description,
        string serviceName,
        string? serviceInstance,
        string? serviceLocation,
        string module,
        List<object>? descriptionDynamicContents = null)
        : base(message)
    {
        this.Code = code;
        this.Title = title;
        this.Description = description;
        this.DescriptionDynamicContents = EMGeneralException.ProcessDynamicContent(descriptionDynamicContents);
        this.ServiceName = serviceName;
        this.ServiceInstance = serviceInstance ?? "NA";
        this.ServiceLocation = serviceLocation ?? "NA";
        this.Module = module;
    }
    public EMGeneralException(
        IServiceError serviceError,
        string serviceName,
        string module = "DOM",
        List<object> descriptionDynamicContents = null)
        : base(serviceError.Message)
    {
        this.Code = serviceError.ErrorCode;
        this.Title = serviceError.Message;
        this.Description = serviceError.Description(descriptionDynamicContents.ToArray());
        this.DescriptionDynamicContents = EMGeneralException.ProcessDynamicContent(descriptionDynamicContents);
        this.ServiceName = serviceName;
        this.Module = module;
    }
    public EMGeneralException(string message, Exception inner)
        : base(message, inner)
    {
    }
    
    private static List<string>? ProcessDynamicContent(List<object>? dynamicContent)
    {
        if (dynamicContent == null)
            return (List<string>) null;
        List<string> stringList = new List<string>();
        foreach (object obj in dynamicContent)
            stringList.Add($"{obj}");
        return stringList;
    }
}