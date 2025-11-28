using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;
using System;
using Newtonsoft.Json;
using Serilog;
using Wallet.DOM.Errors;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Base
{/// <summary>
 /// Base controller to handle errors
 /// </summary>
    [Route(template: "{version:apiVersion}/[controller]")]
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
            _requestBody = JsonConvert.SerializeObject(value: context.ActionArguments);
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
                HandleAutoMapperMappingException(context: context);
                return;
            }

            if (context.Exception is EMGeneralException emGeneralException)
            {
                HandleEMGeneralException(context: context, emGeneralException: emGeneralException);
                return;
            }

            // ADD PROPERTIES TO THE SPECIFIC COLUMNS OUT OF THE STANDARD OF SERILOG THIS WERE
            // CONFIGURED IN THE PROGRAM.CS FILE
            LogContext.PushProperty(name: "Code", value: "EM-GENERIC-500-EXCEPTION");
            LogContext.PushProperty(name: "Controller_Name", value: context.ActionDescriptor.RouteValues[key: "controller"]);
            LogContext.PushProperty(name: "Method_Name", value: context.ActionDescriptor.RouteValues[key: "action"]);
            LogContext.PushProperty(name: "Request", value: _requestBody);
            Log.Error(exception: context.Exception, messageTemplate: context.Exception.Message);
        }

        private static void HandleAutoMapperMappingException(ActionExecutedContext context)
        {
            var inlineResponse400 =
                new InlineResponse400(
                    error: new InlineResponse400Errors(
                        type: nameof(AutoMapperMappingException),
                        status: 500,
                        errorCode: "AUTOMAPPER-MAPPING-EXCEPTION",
                        title: nameof(AutoMapperMappingException),
                        detail: context.Exception?.Message ?? string.Empty,
                        instance: Environment.GetEnvironmentVariable(variable: "ASPNETCORE_ENVIRONMENT") == "Development" ? context.Exception?.StackTrace : ""));
            context.Result = new ObjectResult(value: inlineResponse400)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }

        private static void HandleEMGeneralException(ActionExecutedContext context, EMGeneralException emGeneralException)
        {
            var emGeneralAggregateException =
                new EMGeneralAggregateException(exception: emGeneralException);
            context.Result = new ObjectResult(value: new InlineResponse400(aggregateException: emGeneralAggregateException))
            {
                StatusCode = 400
            };

            context.ExceptionHandled = true;
        }
    }
}
