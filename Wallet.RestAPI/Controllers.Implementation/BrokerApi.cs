using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
                throw new ArgumentNullException(nameof(idBroker), "El ID del broker es requerido.");
            }

            var broker = await brokerFacade.EliminarBrokerAsync(idBroker.Value, this.GetAuthenticatedUserGuid());
            var result = mapper.Map<BrokerResult>(broker);
            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> GetBrokersAsync(string version)
        {
            var brokers = await brokerFacade.ObtenerBrokersAsync();
            var result = mapper.Map<List<BrokerResult>>(brokers);
            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> GetBrokerAsync(string version, int? idBroker)
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
        public override async Task<IActionResult> GetProveedoresPorBrokerAsync(string version, int? idBroker)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(nameof(idBroker), "El ID del broker es requerido.");
            }

            var proveedores = await brokerFacade.ObtenerProveedoresPorBrokerAsync(idBroker.Value);
            var result = mapper.Map<List<ProveedorResult>>(proveedores);
            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostBrokerAsync(string version, BrokerRequest body)
        {
            var broker = await brokerFacade.GuardarBrokerAsync(
                nombre: body.Nombre,
                creationUser: this.GetAuthenticatedUserGuid());

            var result = mapper.Map<BrokerResult>(broker);
            return Created(uri: $"/broker/{result.Id}", value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutActivarBrokerAsync(string version, int? idBroker)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(nameof(idBroker), "El ID del broker es requerido.");
            }

            var broker = await brokerFacade.ActivarBrokerAsync(idBroker.Value, this.GetAuthenticatedUserGuid());
            var result = mapper.Map<BrokerResult>(broker);
            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutBrokerAsync(string version, int? idBroker, BrokerRequest body)
        {
            if (idBroker == null)
            {
                throw new ArgumentNullException(nameof(idBroker), "El ID del broker es requerido.");
            }

            var broker = await brokerFacade.ActualizarBrokerAsync(
                idBroker: idBroker.Value,
                nombre: body.Nombre,
                modificationUser: this.GetAuthenticatedUserGuid());

            var result = mapper.Map<BrokerResult>(broker);
            return Ok(result);
        }
    }
}
