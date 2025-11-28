using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Attributes;
using Wallet.RestAPI.Controllers.Base;
using System.Threading.Tasks;

namespace Wallet.RestAPI.Controllers
{ 
    /// <summary>
    /// 
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
        [Route("/{version:apiVersion}/usuario/{idUsuario}")]
        [ValidateModelState]
        [SwaggerOperation("GetUsuario")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        public abstract Task<IActionResult> GetUsuarioAsync([FromRoute][Required][RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version, [FromRoute][Required] int idUsuario);

        /// <summary>
        /// Guarda contraseña
        /// </summary>
        /// <remarks>Agrega contraseña </remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idUsuario">Id del usuario</param>
        /// <param name="body"></param>
        /// <response code="201">Created</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpPost]
        [Route("/{version:apiVersion}/usuario/{idUsuario}/contrasena")]
        [ValidateModelState]
        [SwaggerOperation("PostUsuarioContrasena")]
        [SwaggerResponse(statusCode: 201, type: typeof(bool?), description: "Created")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        public abstract Task<IActionResult> PostUsuarioContrasenaAsync([FromRoute][Required][RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version, [FromRoute][Required] int idUsuario, [FromBody] ContrasenaRequest body);

        /// <summary>
        /// Confirma codigo de verificacion
        /// </summary>
        /// <remarks>Confirma codigo de verificacion de 4 digitos</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idUsuario">Id del usuario</param>
        /// <param name="body"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpPut]
        [Route("/{version:apiVersion}/usuario/{idUsuario}/confirmaVerificacion")]
        [ValidateModelState]
        [SwaggerOperation("PutUsuarioConfirmaVerificacion")]
        [SwaggerResponse(statusCode: 200, type: typeof(bool?), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        public abstract Task<IActionResult> PutUsuarioConfirmaVerificacionAsync([FromRoute][Required][RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version, [FromRoute][Required] int idUsuario, [FromBody] Verificacion2FARequest body);

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
        [Route("/{version:apiVersion}/usuario/{idUsuario}/contrasena")]
        [ValidateModelState]
        [SwaggerOperation("PutUsuarioContrasena")]
        [SwaggerResponse(statusCode: 200, type: typeof(bool?), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        public abstract Task<IActionResult> PutUsuarioContrasenaAsync([FromRoute][Required][RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version, [FromRoute][Required] int idUsuario, [FromBody] ContrasenaUpdateRequest body);

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
        [Route("/{version:apiVersion}/usuario/{idUsuario}/actualizaEmail")]
        [ValidateModelState]
        [SwaggerOperation("PutUsuarioEmail")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        public abstract Task<IActionResult> PutUsuarioEmailAsync([FromRoute][Required][RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version, [FromRoute][Required] int idUsuario, [FromBody] EmailUpdateRequest body);

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
        [Route("/{version:apiVersion}/usuario/{idUsuario}/actualizaTelefono")]
        [ValidateModelState]
        [SwaggerOperation("PutUsuarioTelefono")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Response to client error satus code")]
        public abstract Task<IActionResult> PutUsuarioTelefonoAsync([FromRoute][Required][RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version, [FromRoute][Required] int idUsuario, [FromBody] TelefonoUpdateRequest body);
    }
}
