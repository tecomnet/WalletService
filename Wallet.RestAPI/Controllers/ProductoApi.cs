using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wallet.RestAPI.Attributes;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers
{
    /// <summary>
    /// Base controller for Producto
    /// </summary>
    [ApiController]
    public abstract class ProductoApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Actualizar un producto existente
        /// </summary>
        /// <remarks>Actualiza los detalles de un producto específico.</remarks>
        /// <param name="idProducto">ID del producto a actualizar</param>
        /// <param name="body">Objeto con los datos actualizados del producto</param>
        /// <response code="200">Producto actualizado exitosamente</response>
        /// <response code="400">Datos inválidos suministrados</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="404">Producto no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut]
        [Route(template: "/{version:apiVersion}/producto/{idProducto}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Actualizar un producto existente",
            description: "Actualiza los detalles de un producto específico.")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProductoResult),
            description: "Producto actualizado exitosamente")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Datos inválidos suministrados")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Producto no encontrado")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> ActualizarProducto([FromRoute] [Required] int? idProducto,
            [FromBody] ProductoRequest body);

        /// <summary>
        /// Eliminar un producto
        /// </summary>
        /// <remarks>Elimina un producto del sistema.</remarks>
        /// <param name="idProducto">ID del producto a eliminar</param>
        /// <response code="400">Datos inválidos suministrados</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="404">Producto no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete]
        [Route(template: "/{version:apiVersion}/producto/{idProducto}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Eliminar un producto", description: "Elimina un producto del sistema.")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProductoResult),
            description: "Producto eliminado exitosamente")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Producto no encontrado")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> EliminarProductoAsync([FromRoute] [Required] int? idProducto);

        /// <summary>
        /// Obtener producto por ID
        /// </summary>
        /// <remarks>Devuelve los detalles de un producto específico.</remarks>
        /// <param name="idProducto">ID del producto a consultar</param>
        /// <response code="200">Detalles del producto</response>
        /// <response code="400">Datos inválidos suministrados</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="404">Producto no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [Route(template: "/{version:apiVersion}/producto/{idProducto}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Obtener producto por ID",
            description: "Devuelve los detalles de un producto específico.")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProductoResult), description: "Detalles del producto")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Datos inválidos suministrados")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Producto no encontrado")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> ObtenerProductoPorIdAsync([FromRoute] [Required] int? idProducto);

        /// <summary>
        /// Crear un nuevo producto
        /// </summary>
        /// <remarks>Registra un nuevo producto en el sistema.</remarks>
        /// <param name="idProveedor">Identificador del proveedor de servicio</param>
        /// <param name="body">Objeto con los datos del nuevo producto</param>
        /// <response code="201">Producto creado exitosamente</response>
        /// <response code="400">Datos inválidos suministrados</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [Route(template: "/{version:apiVersion}/proveedor/{idProveedor}/producto")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Crear un nuevo producto", description: "Registra un nuevo producto en el sistema.")]
        [SwaggerResponse(statusCode: 201, type: typeof(ProductoResult), description: "Producto creado exitosamente")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Datos inválidos suministrados")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> CrearProducto([FromRoute] [Required] int? idProveedor,
            [FromBody] ProductoRequest body);

        /// <summary>
        /// Listar todos los productos
        /// </summary>
        /// <remarks>Devuelve una lista de todos los productos registrados.</remarks>
        /// <response code="200">Lista de productos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [Route(template: "/{version:apiVersion}/producto")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Listar todos los productos",
            description: "Devuelve una lista de todos los productos registrados.")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ProductoResult>), description: "OK")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> ObtenerProductosAsync();

        /// <summary>
        /// Obtener productos por proveedor
        /// </summary>
        /// <remarks>Devuelve los productos asociados a un proveedor.</remarks>
        /// <param name="idProveedor">ID del proveedor</param>
        /// <response code="200">Lista de productos</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="404">Proveedor no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [Route(template: "/{version:apiVersion}/proveedor/{idProveedor}/productos")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Obtener productos por proveedor",
            description: "Devuelve los productos asociados a un proveedor.")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ProductoResult>), description: "OK")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Proveedor no encontrado")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> ObtenerProductosPorProveedorAsync([FromRoute] [Required] int? idProveedor);
    }
}
