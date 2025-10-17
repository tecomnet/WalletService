namespace Template.DOM.Errors;

public interface IServiceError
{
    string ErrorCode { get; }

    string Message { get; }

    string Description(object[]? args = null);
}