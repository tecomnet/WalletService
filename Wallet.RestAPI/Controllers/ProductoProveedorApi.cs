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
    /// Base controller for ProductoProveedor operations.
    /// </summary>
    [ApiController]
    public abstract class ProductoProveedorApiControllerBase : ServiceBaseController
    {
        /// <summary>
        /// Elimina un producto
        /// </summary>
        /// <remarks>Elimina un producto</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idProducto">Id del producto</param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpDelete]
        [Route(template: "/{version:apiVersion}/productoProveedor/{idProducto}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "DeleteProductoProveedor")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProductoProveedorResult), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        public abstract Task<IActionResult> DeleteProductoProveedorAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idProducto);

        /// <summary>
        /// Obtiene un producto por id
        /// </summary>
        /// <remarks>Obtiene un producto por id</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idProducto">Id del producto</param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpGet]
        [Route(template: "/{version:apiVersion}/productoProveedor/{idProducto}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "GetProductoProveedor")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProductoProveedorResult), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        public abstract Task<IActionResult> GetProductoProveedorAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idProducto);

        /// <summary>
        /// Obtiene los productos de un proveedor
        /// </summary>
        /// <remarks>Obtiene los productos de un proveedor</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idProveedorServicio">Id del proveedor de servicio</param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpGet]
        [Route(template: "/{version:apiVersion}/proveedorServicio/{idProveedorServicio}/productos")]
        [ValidateModelState]
        [SwaggerOperation(summary: "GetProductosPorProveedor")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ProductoProveedorResult>), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        public abstract Task<IActionResult> GetProductosPorProveedorAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idProveedorServicio);

        /// <summary>
        /// Guarda un producto
        /// </summary>
        /// <remarks>Guarda un producto</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idProveedorServicio">Id del proveedor de servicio</param>
        /// <param name="body"></param>
        /// <response code="201">Created</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/proveedorServicio/{idProveedorServicio}/producto")]
        [ValidateModelState]
        [SwaggerOperation(summary: "PostProductoProveedor")]
        [SwaggerResponse(statusCode: 201, type: typeof(ProductoProveedorResult), description: "Created")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        public abstract Task<IActionResult> PostProductoProveedorAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idProveedorServicio, [FromBody] ProductoProveedorRequest body);

        /// <summary>
        /// Activa un producto
        /// </summary>
        /// <remarks>Activa un producto</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idProducto">Id del producto</param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpPut]
        [Route(template: "/{version:apiVersion}/productoProveedor/{idProducto}/activar")]
        [ValidateModelState]
        [SwaggerOperation(summary: "PutActivarProductoProveedor")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProductoProveedorResult), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        public abstract Task<IActionResult> PutActivarProductoProveedorAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idProducto);

        /// <summary>
        /// Actualiza un producto
        /// </summary>
        /// <remarks>Actualiza un producto</remarks>
        /// <param name="version">Version of the API to use</param>
        /// <param name="idProducto">Id del producto</param>
        /// <param name="body"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Response to client error satus code</response>
        /// <response code="401">Response to client error satus code</response>
        /// <response code="404">Response to client error satus code</response>
        [HttpPut]
        [Route(template: "/{version:apiVersion}/productoProveedor/{idProducto}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "PutProductoProveedor")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProductoProveedorResult), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400),
            description: "Response to client error satus code")]
        public abstract Task<IActionResult> PutProductoProveedorAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idProducto, [FromBody] ProductoProveedorRequest body);
    }
}
