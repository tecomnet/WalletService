using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.BrokerFacade;
using Wallet.RestAPI.Controllers.Base;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
{
    /// <summary>
    /// Implementation of the Broker API controller.
    /// </summary>
    [ApiController]
    public class BrokerApiController(IBrokerFacade brokerFacade, IMapper mapper) : BrokerApiControllerBase
    {
        /// <inheritdoc />
        public override async Task<IActionResult> CrearBroker([FromBody] BrokerRequest body)
        {
            var broker = await brokerFacade.GuardarBrokerAsync(
                nombre: body.Nombre,
                creationUser: Guid.Empty);

            var result = mapper.Map<BrokerResult>(broker);
            return Created(uri: $"/broker/{result.Id}", value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> ObtenerBrokersAsync()
        {
            var brokers = await brokerFacade.ObtenerBrokersAsync();
            var result = mapper.Map<List<BrokerResult>>(brokers);
            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> ObtenerBrokerPorIdAsync([FromRoute] [Required] int? idBroker)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(nameof(idBroker), "El ID del broker es requerido.");
            }

            var broker = await brokerFacade.ObtenerBrokerPorIdAsync(idBroker.Value);
            var result = mapper.Map<BrokerResult>(broker);
            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> ActualizarBroker([FromRoute] [Required] int? idBroker,
            [FromBody] BrokerRequest body)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(nameof(idBroker), "El ID del broker es requerido.");
            }

            var broker = await brokerFacade.ActualizarBrokerAsync(
                idBroker: idBroker.Value,
                nombre: body.Nombre,
                modificationUser: Guid.Empty);

            var result = mapper.Map<BrokerResult>(broker);
            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> EliminarBrokerAsync([FromRoute] [Required] int? idBroker)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(nameof(idBroker), "El ID del broker es requerido.");
            }

            await brokerFacade.EliminarBrokerAsync(idBroker.Value, Guid.Empty);
            return NoContent();
        }

        /// <inheritdoc />
        public override async Task<IActionResult> ObtenerProveedoresPorBrokerAsync([FromRoute] [Required] int? idBroker)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(nameof(idBroker), "El ID del broker es requerido.");
            }

            var proveedores = await brokerFacade.ObtenerProveedoresPorBrokerAsync(idBroker.Value);
            var result = mapper.Map<List<ProveedorResult>>(proveedores);
            return Ok(result);
        }
    }
}
