using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wallet.RestAPI.Attributes;
using Wallet.RestAPI.Controllers.Base;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers
{
    /// <summary>
    /// Base controller for Usuario operations.
    /// </summary>
    [ApiController]
    public abstract class UsuarioApiControllerBase : ServiceBaseController
    {
        /// <summary>
        /// Obtiene un usuario por id
        /// </summary>
        /// <remarks>Obtiene un usuario por id</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idUsuario">Id del usuario</param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpGet]
        [Route(template: "/{version:apiVersion}/usuario/{idUsuario}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "GetUsuario")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [Authorize]
        public abstract Task<IActionResult> GetUsuarioAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario);


        /// <summary>
        /// Actualiza contraseña
        /// </summary>
        /// <remarks>Actualiza la contraseña </remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idUsuario">Id del usuario</param>
        /// <param name="body"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpPut]
        [Route(template: "/{version:apiVersion}/usuario/{idUsuario}/contrasena")]
        [ValidateModelState]
        [SwaggerOperation(summary: "PutUsuarioContrasena")]
        [SwaggerResponse(statusCode: 200, type: typeof(bool?), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [Authorize]
        public abstract Task<IActionResult> PutUsuarioContrasenaAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario, [FromBody] ContrasenaUpdateRequest body);

        /// <summary>
        /// Actualiza email
        /// </summary>
        /// <remarks>Actualiza el email</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idUsuario">Id del usuario</param>
        /// <param name="body"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpPut]
        [Route(template: "/{version:apiVersion}/usuario/{idUsuario}/actualizaEmail")]
        [ValidateModelState]
        [SwaggerOperation(summary: "PutUsuarioEmail")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [Authorize]
        public abstract Task<IActionResult> PutUsuarioEmailAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario, [FromBody] EmailUpdateRequest body);

        /// <summary>
        /// Actualiza telefono
        /// </summary>
        /// <remarks>Actualiza el telefono</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idUsuario">Id del usuario</param>
        /// <param name="body"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpPut]
        [Route(template: "/{version:apiVersion}/usuario/{idUsuario}/actualizaTelefono")]
        [ValidateModelState]
        [SwaggerOperation(summary: "PutUsuarioTelefono")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [Authorize]
        public abstract Task<IActionResult> PutUsuarioTelefonoAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario, [FromBody] TelefonoUpdateRequest body);
    }
}
