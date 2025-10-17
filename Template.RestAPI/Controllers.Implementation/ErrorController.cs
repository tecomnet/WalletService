using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Template.DOM.Errors;
using Template.RestAPI.Models;

namespace Template.RestAPI.Controllers.Implementation
{
    /// <summary>
    /// Default redirection when an unhandled error occurs
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Will execute this method as default when an unhandled error occurs
        /// </summary>
        /// <returns>Standard InlineResponse400</returns>
        [Route("Error")]
        public object Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context == null)
            {
                return BadRequest();
            }

            var exception = context.Error;
            var emGeneralAggregateException = new EmGeneralAggregateException(new EmGeneralException(exception.Message, exception));
            return BadRequest(new InlineResponse400(emGeneralAggregateException));
        }
    }
}
