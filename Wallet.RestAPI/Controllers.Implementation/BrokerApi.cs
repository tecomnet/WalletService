using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.BrokerFacade;
using Wallet.RestAPI.Helpers;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
{
    /// <summary>
    /// Implementation of the Broker API controller.
    /// </summary>
    [ApiController]
    //[Authorize]
    public class BrokerApiController(IBrokerFacade brokerFacade, IMapper mapper) : BrokerApiControllerBase
    {
        /// <inheritdoc />
        public override async Task<IActionResult> DeleteBrokerAsync(string version, int? idBroker)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(paramName: nameof(idBroker), message: "El ID del broker es requerido.");
            }

            var broker = await brokerFacade.EliminarBrokerAsync(idBroker: idBroker.Value, modificationUser: this.GetAuthenticatedUserGuid());
            var result = mapper.Map<BrokerResult>(source: broker);
            return Ok(value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> GetBrokersAsync(string version)
        {
            var brokers = await brokerFacade.ObtenerBrokersAsync();
            var result = mapper.Map<List<BrokerResult>>(source: brokers);
            return Ok(value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> GetBrokerAsync(string version, int? idBroker)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(paramName: nameof(idBroker), message: "El ID del broker es requerido.");
            }

            var broker = await brokerFacade.ObtenerBrokerPorIdAsync(idBroker: idBroker.Value);
            var result = mapper.Map<BrokerResult>(source: broker);
            return Ok(value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> GetProveedoresPorBrokerAsync(string version, int? idBroker)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(paramName: nameof(idBroker), message: "El ID del broker es requerido.");
            }

            var proveedores = await brokerFacade.ObtenerProveedoresPorBrokerAsync(idBroker: idBroker.Value);
            var result = mapper.Map<List<ProveedorResult>>(source: proveedores);
            return Ok(value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostBrokerAsync(string version, BrokerRequest body)
        {
            var broker = await brokerFacade.GuardarBrokerAsync(
                nombre: body.Nombre,
                creationUser: this.GetAuthenticatedUserGuid());

            var result = mapper.Map<BrokerResult>(source: broker);
            return Created(uri: $"/broker/{result.Id}", value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutActivarBrokerAsync(string version, int? idBroker)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(paramName: nameof(idBroker), message: "El ID del broker es requerido.");
            }

            var broker = await brokerFacade.ActivarBrokerAsync(idBroker: idBroker.Value, modificationUser: this.GetAuthenticatedUserGuid());
            var result = mapper.Map<BrokerResult>(source: broker);
            return Ok(value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutBrokerAsync(string version, int? idBroker,
            BrokerUpdateRequest body)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(paramName: nameof(idBroker), message: "El ID del broker es requerido.");
            }

            var broker = await brokerFacade.ActualizarBrokerAsync(
                idBroker: idBroker.Value,
                nombre: body.Nombre,
                concurrencyToken: body.ConcurrencyToken,
                modificationUser: this.GetAuthenticatedUserGuid());

            var result = mapper.Map<BrokerResult>(source: broker);
            return Ok(value: result);
        }
    }
}
