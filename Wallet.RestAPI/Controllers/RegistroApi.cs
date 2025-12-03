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
    /// Base controller for Registration operations.
    /// </summary>
    [ApiController]
    public abstract class RegistroApi : ServiceBaseController
    {
        /// <summary>
        /// Confirma el número de teléfono
        /// </summary>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/registro/confirmarNumero")]
        [ValidateModelState]
        [SwaggerOperation(summary: "ConfirmarNumero")]
        [SwaggerResponse(statusCode: 200, type: typeof(bool), description: "OK")]
        public abstract Task<IActionResult> ConfirmarNumeroAsync(
            [FromRoute] [Required] string version,
            [FromBody] ConfirmarNumeroRequest body);

        /// <summary>
        /// Completa los datos del cliente
        /// </summary>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/registro/datosCliente")]
        [ValidateModelState]
        [SwaggerOperation(summary: "CompletarDatosCliente")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        public abstract Task<IActionResult> CompletarDatosClienteAsync(
            [FromRoute] [Required] string version,
            [FromBody] DatosClienteRequest body);

        /// <summary>
        /// Registra el correo electrónico
        /// </summary>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/registro/correo")]
        [ValidateModelState]
        [SwaggerOperation(summary: "RegistrarCorreo")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        public abstract Task<IActionResult> RegistrarCorreoAsync(
            [FromRoute] [Required] string version,
            [FromBody] RegistrarCorreoRequest body);

        /// <summary>
        /// Verifica el correo electrónico
        /// </summary>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/registro/verificarCorreo")]
        [ValidateModelState]
        [SwaggerOperation(summary: "VerificarCorreo")]
        [SwaggerResponse(statusCode: 200, type: typeof(bool), description: "OK")]
        public abstract Task<IActionResult> VerificarCorreoAsync(
            [FromRoute] [Required] string version,
            [FromBody] VerificarCorreoRequest body);

        /// <summary>
        /// Registra datos biométricos (dispositivo)
        /// </summary>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/registro/biometricos")]
        [ValidateModelState]
        [SwaggerOperation(summary: "RegistrarBiometricos")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        public abstract Task<IActionResult> RegistrarBiometricosAsync(
            [FromRoute] [Required] string version,
            [FromBody] RegistrarBiometricosRequest body);

        /// <summary>
        /// Acepta términos y condiciones
        /// </summary>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/registro/terminos")]
        [ValidateModelState]
        [SwaggerOperation(summary: "AceptarTerminos")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        public abstract Task<IActionResult> AceptarTerminosAsync(
            [FromRoute] [Required] string version,
            [FromBody] AceptarTerminosRequest body);

        /// <summary>
        /// Completa el registro (contraseña)
        /// </summary>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/registro/completar")]
        [ValidateModelState]
        [SwaggerOperation(summary: "CompletarRegistro")]
        [SwaggerResponse(statusCode: 200, type: typeof(UsuarioResult), description: "OK")]
        public abstract Task<IActionResult> CompletarRegistroAsync(
            [FromRoute] [Required] string version,
            [FromBody] CompletarRegistroRequest body);

        /// <summary>
        /// Pre-registro de usuario
        /// </summary>
        /// <remarks>Crea un usuario y cliente en estado de pre-registro</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="body"></param>
        /// <response code="201">Created</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/registro/preRegistro")]
        [ValidateModelState]
        [SwaggerOperation(summary: "PostUsuarioPreRegistro")]
        [SwaggerResponse(statusCode: 201, type: typeof(UsuarioResult), description: "Created")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        public abstract Task<IActionResult> PreRegistroAsync(
            [FromRoute] [Required] string version,
            [FromBody] PreRegistroRequest body);
    }
}
