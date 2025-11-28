using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM.Errors;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
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
            var emGeneralAggregateException = new EMGeneralAggregateException(new EMGeneralException(exception.Message, exception));
            return BadRequest(new InlineResponse400(emGeneralAggregateException));
        }
    }
}
