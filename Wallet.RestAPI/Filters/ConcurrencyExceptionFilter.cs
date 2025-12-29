using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Wallet.RestAPI.Filters;

/// <summary>
/// Filter to handle EF Core concurrency exceptions globally.
/// Translates DbUpdateConcurrencyException to HTTP 409 Conflict.
/// </summary>
public class ConcurrencyExceptionFilter : IExceptionFilter
{
    /// <inheritdoc />
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is DbUpdateConcurrencyException)
        {
            // 409 Conflict is the standard HTTP status for optimistic concurrency control failures.
            context.Result = new StatusCodeResult(statusCode: (int)HttpStatusCode.Conflict);
            context.ExceptionHandled = true;
        }
    }
}
