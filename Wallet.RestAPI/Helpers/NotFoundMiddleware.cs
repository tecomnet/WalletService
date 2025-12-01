using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Helpers
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
            await _next(context: context);

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
                    if (segments.Length > 1)
                    {
                        var segment = segments[1];
                        // Si es una versión permitida, retornamos (es un 404 real dentro de la versión)
                        if (allowedVersions.Contains(segment))
                        {
                            await WriteNotFoundResponse(context);
                            return;
                        }

                        // Si NO parece una versión (ej. "swagger", "health", "favicon.ico"), retornamos (es un 404 real)
                        // Esto evita que endpoints sin versión o recursos estáticos den error de versión.
                        if (!System.Text.RegularExpressions.Regex.IsMatch(segment, @"^\d+\.\d+$"))
                        {
                            await WriteNotFoundResponse(context);
                            return;
                        }

                        // Si llegamos aquí, es porque parece una versión (ej. "0.2") pero no está en allowedVersions.
                        // Dejamos que continúe para devolver el error de versión no soportada.
                    }
                }

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var problemDetails = new ProblemDetails();
                problemDetails.Detail = "The API version provided is not supported or it wasn't specified.";
                problemDetails.Type = "EM-CustomProblemDetails";
                problemDetails.Extensions.Add(key: "RestAPIErrors", value: new
                {
                    // Las propiedades se serializarán directamente en el JSON
                    ErrorCode = "REST-API-BAD-VERSION",
                    Messages = new[] { "The API version provided is not supported or it wasn't specified." }
                });
                await context.Response.WriteAsync(text: JsonConvert.SerializeObject(value: problemDetails));
            }
        }

        private async Task WriteNotFoundResponse(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";
            var problemDetails = new ProblemDetails();
            problemDetails.Detail = "The requested resource was not found.";
            problemDetails.Type = "EM-CustomProblemDetails";
            problemDetails.Extensions.Add(key: "RestAPIErrors", value: new
            {
                ErrorCode = "RESOURCE-NOT-FOUND",
                Messages = new[] { "The requested resource was not found." }
            });
            await context.Response.WriteAsync(text: JsonConvert.SerializeObject(value: problemDetails));
        }
    }
}