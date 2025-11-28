using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Wallet.DOM;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Filters;

public class ServiceExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        Exception exceptionToHandle = context.Exception;
        
        // --- 1. Manejo de Excepciones Controladas (EMGeneralAggregateException) ---
        if (exceptionToHandle is EMGeneralAggregateException aggregateException)
        {
            // Asumimos que podemos obtener el código de error de la InnerException
            string? errorCode = (aggregateException.InnerException as dynamic)?.Code;
            
            // Creamos la respuesta de error 400 por defecto
            var statusCode = (int)HttpStatusCode.BadRequest; // 400
            var responseBody = new InlineResponse400(aggregateException: aggregateException);

            // Mapeo del código de error al Status Code HTTP
            if (errorCode == ServiceErrorsBuilder.ClienteNoEncontrado ||
                errorCode == ServiceErrorsBuilder.EstadoNoEncontrado ||
                errorCode == ServiceErrorsBuilder.EmpresaNoEncontrada ||
                errorCode == ServiceErrorsBuilder.CodigoVerificacionNoEncontrado)
            {
                statusCode = (int)HttpStatusCode.NotFound; // 404
            }
            
            // Establecemos el resultado, deteniendo la propagación
            context.Result = new ObjectResult(value: responseBody)
            {
                StatusCode = statusCode
            };
        }
        // --- 2. Manejo de Excepciones NO Controladas (Cualquier otra Exception) ---
        else
        {
            // 1. Encapsular la excepción no controlada en una EMGeneralAggregateException
            // Esto replica la lógica del catch (Exception ex) de tu Controller
            var encapsulatedException = GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                // Asumimos que podemos obtener el nombre del controller/clase desde el contexto si es necesario,
                // por simplicidad usaremos el nombre del filtro.
                module: "RestApi", 
                exception: exceptionToHandle);
            
            // 2. Crear una respuesta de error genérica (HTTP 500 es el estándar para errores no esperados)
            var statusCode = (int)HttpStatusCode.InternalServerError; // 500
            var responseBody = new InlineResponse400(aggregateException: encapsulatedException);

            // 3. Establecer el resultado
            context.Result = new ObjectResult(value: responseBody)
            {
                StatusCode = statusCode
            };
        }
        
        // Indicamos que la excepción ha sido manejada, independientemente de si fue 400, 404 o 500
        context.ExceptionHandled = true;
    }
}
