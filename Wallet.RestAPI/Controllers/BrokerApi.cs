using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Wallet.RestAPI.Attributes;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Base
{
    /// <summary>
    /// Base controller for Broker management
    /// </summary>
    [ApiController]
    public abstract class BrokerApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Crear un nuevo broker
        /// </summary>
        /// <remarks>Registra un nuevo broker en el sistema.</remarks>
        /// <param name="body">Objeto con los datos del nuevo broker</param>
        /// <response code="201">Broker creado exitosamente</response>
        /// <response code="400">Datos inválidos suministrados</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [Route("/{version:apiVersion}/broker")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Crear un nuevo broker", description: "Registra un nuevo broker en el sistema.")]
        [SwaggerResponse(statusCode: 201, type: typeof(BrokerResult), description: "Broker creado exitosamente")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Datos inválidos suministrados")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> CrearBroker([FromBody] BrokerRequest body);

        /// <summary>
        /// Listar todos los brokers
        /// </summary>
        /// <remarks>Devuelve una lista de todos los brokers registrados.</remarks>
        /// <response code="200">Lista de brokers</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [Route("/{version:apiVersion}/brokers")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Listar todos los brokers",
            description: "Devuelve una lista de todos los brokers registrados.")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<BrokerResult>), description: "OK")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> ObtenerBrokersAsync();

        /// <summary>
        /// Obtener broker por ID
        /// </summary>
        /// <remarks>Devuelve los detalles de un broker específico.</remarks>
        /// <param name="idBroker">ID del broker a consultar</param>
        /// <response code="200">Detalles del broker</response>
        /// <response code="400">Datos inválidos suministrados</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="404">Broker no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [Route("/{version:apiVersion}/broker/{idBroker}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Obtener broker por ID",
            description: "Devuelve los detalles de un broker específico.")]
        [SwaggerResponse(statusCode: 200, type: typeof(BrokerResult), description: "Detalles del broker")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Datos inválidos suministrados")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Broker no encontrado")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> ObtenerBrokerPorIdAsync([FromRoute] [Required] int? idBroker);

        /// <summary>
        /// Actualizar un broker existente
        /// </summary>
        /// <remarks>Actualiza los detalles de un broker específico.</remarks>
        /// <param name="idBroker">ID del broker a actualizar</param>
        /// <param name="body">Objeto con los datos actualizados del broker</param>
        /// <response code="200">Broker actualizado exitosamente</response>
        /// <response code="400">Datos inválidos suministrados</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="404">Broker no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [Route("/{version:apiVersion}/broker/{idBroker}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Actualizar un broker existente",
            description: "Actualiza los detalles de un broker específico.")]
        [SwaggerResponse(statusCode: 200, type: typeof(BrokerResult), description: "Broker actualizado exitosamente")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Datos inválidos suministrados")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Broker no encontrado")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> ActualizarBroker([FromRoute] [Required] int? idBroker,
            [FromBody] BrokerRequest body);

        /// <summary>
        /// Eliminar un broker
        /// </summary>
        /// <remarks>Elimina un broker del sistema.</remarks>
        /// <param name="idBroker">ID del broker a eliminar</param>
        /// <response code="204">Broker eliminado exitosamente</response>
        /// <response code="400">Datos inválidos suministrados</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="404">Broker no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete]
        [Route("/{version:apiVersion}/broker/{idBroker}")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Eliminar un broker", description: "Elimina un broker del sistema.")]
        [SwaggerResponse(statusCode: 204, description: "Broker eliminado exitosamente")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Datos inválidos suministrados")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Broker no encontrado")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> EliminarBrokerAsync([FromRoute] [Required] int? idBroker);

        /// <summary>
        /// Obtener proveedores de un broker
        /// </summary>
        /// <remarks>Devuelve los proveedores asociados a un broker específico.</remarks>
        /// <param name="idBroker">ID del broker a consultar</param>
        /// <response code="200">Lista de proveedores</response>
        /// <response code="400">Datos inválidos suministrados</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Prohibido</response>
        /// <response code="404">Broker no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [Route("/{version:apiVersion}/broker/{idBroker}/proveedores")]
        [ValidateModelState]
        [SwaggerOperation(summary: "Obtener proveedores de un broker",
            description: "Devuelve los proveedores asociados a un broker específico.")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<ProveedorResult>), description: "OK")]
        [SwaggerResponse(statusCode: 400, type: typeof(InlineResponse400),
            description: "Datos inválidos suministrados")]
        [SwaggerResponse(statusCode: 401, type: typeof(InlineResponse400), description: "No autorizado")]
        [SwaggerResponse(statusCode: 403, type: typeof(InlineResponse400), description: "Prohibido")]
        [SwaggerResponse(statusCode: 404, type: typeof(InlineResponse400), description: "Broker no encontrado")]
        [SwaggerResponse(statusCode: 500, type: typeof(InlineResponse400), description: "Error interno del servidor")]
        public abstract Task<IActionResult> ObtenerProveedoresPorBrokerAsync([FromRoute] [Required] int? idBroker);
    }
}
