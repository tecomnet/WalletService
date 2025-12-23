#nullable enable
namespace Wallet.RestAPI.Errors;

/// <summary>
/// Default implementation of IRestAPIError
/// </summary>
public class DefaultRestAPIError : IRestAPIError
{
    /// <summary>
    /// Error detail template
    /// </summary>
    protected string _detail;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultRestAPIError"/> class.
    /// </summary>
    public DefaultRestAPIError()
    {
        this.ErrorCode = RestAPIErrors.RestApiDefaultError;
        this.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
        this.Title = "Error Predeterminado de API REST";
        this.Status = 400;
        this._detail = "Default REST API error used when no specific one exists.";
        this.Instance = "DEFAULT";
    }

    /// <inheritdoc />
    public string ErrorCode { get; protected set; }

    /// <inheritdoc />
    public string Type { get; protected set; }

    /// <inheritdoc />
    public string Title { get; protected set; }

    /// <inheritdoc />
    public int Status { get; protected set; }

    /// <inheritdoc />
    public string Instance { get; protected set; }

    /// <inheritdoc />
    public string Detail(string[]? args = null)
    {
        return args == null || args.Length == 0
            ? this._detail
            : string.Format(format: this._detail, args: (object[])args);
    }
}
