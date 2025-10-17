using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Template.RestAPI.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class NotFoundMiddleware
    {
        private readonly RequestDelegate _next;

        public NotFoundMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 404)
            {
                var requestedApiVersion = context.GetRequestedApiVersion();
                if (requestedApiVersion != null)
                {
                    return;
                }

                var allowedVersions = new[] { "0.1" };
                if (context.Request.Path != null)
                {
                    var segments = context.Request.Path.ToString().Split('/');
                    if (segments.Length > 1 && allowedVersions.Contains(segments[1]))
                    {
                        return;
                    }
                }

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var problemDetails = new ProblemDetails();
                problemDetails.Detail = "The API version provided is not supported or it wasn't specified.";
                problemDetails.Type = "EM-CustomProblemDetails";
                problemDetails.Extensions.Add("RestAPIErrors", new
                {
                    // Las propiedades se serializarán directamente en el JSON
                    ErrorCode = "REST-API-BAD-VERSION",
                    Messages = new[] { "The API version provided is not supported or it wasn't specified." }
                });
                await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
            }
        }
    }
}