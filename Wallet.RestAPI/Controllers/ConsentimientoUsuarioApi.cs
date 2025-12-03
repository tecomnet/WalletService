using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Attributes;
using Wallet.RestAPI.Controllers.Base;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Wallet.RestAPI.Controllers
{
    /// <summary>
    /// Base controller for ConsentimientoUsuario operations.
    /// </summary>
    [ApiController]
    public abstract class ConsentimientoUsuarioApi : ServiceBaseController
    {
        /// <summary>
        /// Guarda un consentimiento de usuario
        /// </summary>
        /// <remarks>Guarda un consentimiento de usuario</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idUsuario">Id del usuario</param>
        /// <param name="body"></param>
        /// <response code="201">Created</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/usuario/{idUsuario}/consentimiento")]
        [ValidateModelState]
        [SwaggerOperation(summary: "PostConsentimientoUsuario")]
        [SwaggerResponse(statusCode: 201, type: typeof(ConsentimientoUsuarioResult), description: "Created")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [Authorize]
        public abstract Task<IActionResult> PostConsentimientoUsuarioAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario, [FromBody] ConsentimientoUsuarioRequest body);

        /// <summary>
        /// Obtiene los últimos consentimientos del usuario
        /// </summary>
        /// <remarks>Obtiene los últimos consentimientos del usuario</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idUsuario">Id del usuario</param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpGet]
        [Route(template: "/{version:apiVersion}/usuario/{idUsuario}/consentimiento")]
        [ValidateModelState]
        [SwaggerOperation(summary: "GetConsentimientosUsuario")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ConsentimientoUsuarioResult>), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [Authorize]
        public abstract Task<IActionResult> GetConsentimientosUsuarioAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario);
    }
}
