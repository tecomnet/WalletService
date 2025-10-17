using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;
using System;
using Newtonsoft.Json;
using Serilog;
using Template.DOM.Errors;
using Template.RestAPI.Models;

namespace Template.RestAPI.Controllers.Base
{/// <summary>
 /// Base controller to handle errors
 /// </summary>
    [Route("{version:apiVersion}/[controller]")]
    [ApiController]
    public class ServiceBaseController : ControllerBase, IActionFilter
    {
        private string _requestBody = "";

        /// <summary>
        /// Saves the request when executing an action
        /// </summary>
        /// <param name="context">Executing context</param>
        [NonAction]
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _requestBody = JsonConvert.SerializeObject(context.ActionArguments);
        }
        /// <summary>
        /// Logs an error to DB when detected
        /// </summary>
        /// <param name="context">Executed context</param>
        [NonAction]
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                return;
            }

            if (context.Exception is AutoMapperMappingException)
            {
                HandleAutoMapperMappingException(context);
                return;
            }

            if (context.Exception is EmGeneralException emGeneralException)
            {
                HandleEMGeneralException(context, emGeneralException);
                return;
            }

            // ADD PROPERTIES TO THE SPECIFIC COLUMNS OUT OF THE STANDARD OF SERILOG THIS WERE
            // CONFIGURED IN THE PROGRAM.CS FILE
            LogContext.PushProperty("Code", "EM-GENERIC-500-EXCEPTION");
            LogContext.PushProperty("Controller_Name", context.ActionDescriptor.RouteValues["controller"]);
            LogContext.PushProperty("Method_Name", context.ActionDescriptor.RouteValues["action"]);
            LogContext.PushProperty("Request", _requestBody);
            Log.Error(context.Exception, context.Exception.Message);
        }

        private static void HandleAutoMapperMappingException(ActionExecutedContext context)
        {
            var inlineResponse400 =
                new InlineResponse400(
                    new InlineResponse400Errors(
                        nameof(AutoMapperMappingException),
                        500,
                        "AUTOMAPPER-MAPPING-EXCEPTION",
                        nameof(AutoMapperMappingException),
                        context.Exception?.Message ?? string.Empty,
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? context.Exception?.StackTrace : ""));
            context.Result = new ObjectResult(inlineResponse400)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }

        private static void HandleEMGeneralException(ActionExecutedContext context, EmGeneralException emGeneralException)
        {
            var emGeneralAggregateException =
                new EmGeneralAggregateException(emGeneralException);
            context.Result = new ObjectResult(new InlineResponse400(emGeneralAggregateException))
            {
                StatusCode = 400
            };

            context.ExceptionHandled = true;
        }
    }
}
