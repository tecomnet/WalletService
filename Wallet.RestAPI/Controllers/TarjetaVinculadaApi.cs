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
    /// Controller for Tarjeta Vinculada operations
    /// </summary>
    [ApiController]
    public abstract class TarjetaVinculadaApiControllerBase : ServiceBaseController
    {
        [HttpGet]
        [Route("/{version:apiVersion}/cliente/{idCliente}/tarjetasvinculadas")]
        [ValidateModelState]
        [SwaggerOperation(summary: "GetTarjetasVinculadasPorCliente")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<TarjetaVinculadaResult>), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Bad Request")]
        public abstract Task<IActionResult> GetTarjetasVinculadasPorClienteAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idCliente);

        [HttpPost]
        [Route("/{version:apiVersion}/cliente/{idCliente}/tarjetasvinculadas")]
        [ValidateModelState]
        [SwaggerOperation(summary: "VincularTarjeta")]
        [SwaggerResponse(statusCode: 201, type: typeof(TarjetaVinculadaResult), description: "Created")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Bad Request")]
        public abstract Task<IActionResult> VincularTarjetaAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idCliente,
            [FromBody] VincularTarjetaRequest body);

        [HttpDelete]
        [Route("/{version:apiVersion}/tarjetasvinculadas/{idTarjeta}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "DesvincularTarjeta")]
        [SwaggerResponse(statusCode: 200, description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Bad Request")]
        public abstract Task<IActionResult> DesvincularTarjetaAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idTarjeta);

        [HttpPut]
        [Route("/{version:apiVersion}/tarjetasvinculadas/{idTarjeta}/favorita")]
        [ValidateModelState]
        [SwaggerOperation(summary: "EstablecerTarjetaFavorita")]
        [SwaggerResponse(statusCode: 200, description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400), description: "Bad Request")]
        public abstract Task<IActionResult> EstablecerTarjetaFavoritaAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idTarjeta,
            [FromBody] SetFavoritaRequest body);
    }
}
