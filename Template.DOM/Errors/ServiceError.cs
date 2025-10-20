namespace Template.DOM.Errors;

public class ServiceError : IServiceError
{
    private readonly string _description = "This is a default service error issued when the specific error cannot be found.";
    public string ErrorCode { get; protected set; }

    public string Message { get; protected set; }
    public string Title { get; }


    public string Description(object[]? args = null)
    {
        return args == null || args.Length == 0 ? this._description : string.Format(this._description, args);
    }
    public ServiceError(string errorCode, string message, string description)
    {
        this.ErrorCode = errorCode;
        this.Message = message;
        this.Title = message;
        this._description = description;
    }
}