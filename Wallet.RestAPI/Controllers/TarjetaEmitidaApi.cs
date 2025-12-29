using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wallet.RestAPI.Attributes;
using Wallet.RestAPI.Controllers.Base;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers
{
    /// <summary>
    /// Controller for Tarjeta Emitida operations
    /// </summary>
    [ApiController]
    public abstract class TarjetaEmitidaApiControllerBase : ServiceBaseController
    {
        [HttpGet]
        [Route("/{version:apiVersion}/cliente/{idCliente}/tarjetasemitidas")]
        [ValidateModelState]
        [SwaggerOperation(summary: "GetTarjetasEmitidasPorCliente")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<TarjetaEmitidaResult>), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Bad Request")]
        public abstract Task<IActionResult> GetTarjetasEmitidasPorClienteAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idCliente);

        [HttpPost]
        [Route("/{version:apiVersion}/cliente/{idCliente}/tarjetasemitidas/virtual")]
        [ValidateModelState]
        [SwaggerOperation(summary: "SolicitarTarjetaVirtual")]
        [SwaggerResponse(statusCode: 201, type: typeof(TarjetaEmitidaResult), description: "Created")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Bad Request")]
        public abstract Task<IActionResult> SolicitarTarjetaVirtualAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idCliente,
            [FromBody] SolicitarTarjetaVirtualRequest body);

        [HttpPost]
        [Route("/{version:apiVersion}/cliente/{idCliente}/tarjetasemitidas/fisica")]
        [ValidateModelState]
        [SwaggerOperation(summary: "SolicitarTarjetaFisica")]
        [SwaggerResponse(statusCode: 201, type: typeof(TarjetaEmitidaResult), description: "Created")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Bad Request")]
        public abstract Task<IActionResult> SolicitarTarjetaFisicaAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idCliente,
            [FromBody] SolicitarTarjetaFisicaRequest body);

        [HttpPut]
        [Route("/{version:apiVersion}/tarjetasemitidas/{idTarjeta}/bloqueo")]
        [ValidateModelState]
        [SwaggerOperation(summary: "CambiarBloqueoTarjeta")]
        [SwaggerResponse(statusCode: 200, description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Bad Request")]
        public abstract Task<IActionResult> CambiarBloqueoTarjetaAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idTarjeta,
            [FromBody] CambioBloqueoRequest body);

        [HttpPut]
        [Route("/{version:apiVersion}/tarjetasemitidas/{idTarjeta}/configuracion")]
        [ValidateModelState]
        [SwaggerOperation(summary: "ActualizarConfiguracionTarjeta")]
        [SwaggerResponse(statusCode: 200, description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Bad Request")]
        public abstract Task<IActionResult> ActualizarConfiguracionTarjetaAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idTarjeta,
            [FromBody] ConfiguracionTarjetaRequest body);
    }
}
