using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.Errors;
using Wallet.RestAPI.Models;

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
            // Build the standard exception structure
            var exception = DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ConcurrencyError,
                dynamicContent: [],
                module: nameof(ConcurrencyExceptionFilter));

            var aggregateException = new EMGeneralAggregateException(exception: exception);
            var responseBody = new InlineResponse400(aggregateException: aggregateException);

            // 409 Conflict is the standard HTTP status for optimistic concurrency control failures.
            context.Result = new ObjectResult(value: responseBody)
            {
                StatusCode = (int)HttpStatusCode.Conflict
            };
            context.ExceptionHandled = true;
        }
    }
}
