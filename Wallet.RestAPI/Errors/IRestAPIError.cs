#nullable enable
namespace Wallet.RestAPI.Errors;

/// <summary>
/// Interface for Rest API Errors
/// </summary>
public interface IRestAPIError
{
    /// <summary>
    /// Unique error code
    /// </summary>
    string ErrorCode { get; }

    /// <summary>
    /// Error type URL
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Error title
    /// </summary>
    string Title { get; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    int Status { get; }

    /// <summary>
    /// Error instance identifier
    /// </summary>
    string Instance { get; }

    /// <summary>
    /// Gets the error detail with optional formatting arguments
    /// </summary>
    /// <param name="args">Format arguments</param>
    /// <returns>Formatted error detail</returns>
    string Detail(string[]? args = null);
}