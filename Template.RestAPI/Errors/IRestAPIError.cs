namespace Template.RestAPI.Errors;

public interface IRestAPIError
{
    string ErrorCode { get; }

    string Type { get; }

    string Title { get; }

    int Status { get; }

    string Instance { get; }

    string Detail(string[]? args = null);
}