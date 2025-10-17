namespace Template.RestAPI.Errors;

public class DefaultRestAPIError : IRestAPIError
{
    protected string _detail;

    public DefaultRestAPIError()
    {
        this.ErrorCode = "REST-API-DEFAULT-ERROR";
        this.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        this.Title = "Default REST API error";
        this.Status = 400;
        this._detail = "Default REST API error used when no specific one exists.";
        this.Instance = "DEFAULT";
    }

    public string ErrorCode { get; protected set; }

    public string Type { get; protected set; }

    public string Title { get; protected set; }

    public int Status { get; protected set; }

    public string Instance { get; protected set; }

    public string Detail(string[]? args = null)
    {
        return args == null || args.Length == 0 ? this._detail : string.Format(this._detail, (object[]) args);
    }
}
