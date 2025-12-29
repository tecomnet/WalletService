using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.GestionWallet;
using Wallet.RestAPI.Controllers;
using Wallet.RestAPI.Helpers;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
{
    public class TarjetaEmitidaApiController(ITarjetaEmitidaFacade tarjetaEmitidaFacade, IMapper mapper)
        : TarjetaEmitidaApiControllerBase
    {
        public override async Task<IActionResult> GetTarjetasEmitidasPorClienteAsync(string version, int idCliente)
        {
            var tarjetas = await tarjetaEmitidaFacade.ObtenerTarjetasPorClienteAsync(idCliente);
            var response = mapper.Map<List<TarjetaEmitidaResult>>(tarjetas);
            return Ok(response);
        }

        public override async Task<IActionResult> SolicitarTarjetaVirtualAsync(string version, int idCliente,
            SolicitarTarjetaVirtualRequest body)
        {
            if (idCliente != body.IdCliente)
                return BadRequest(new InlineResponse400
                    { Errors = new List<InlineResponse400Errors> { new() { Detail = "IdCliente mismatch" } } });

            var tarjeta =
                await tarjetaEmitidaFacade.SolicitarTarjetaVirtualAdicionalAsync(idCliente,
                    this.GetAuthenticatedUserGuid());
            var response = mapper.Map<TarjetaEmitidaResult>(tarjeta);
            return Created($"/{version}/tarjetas-emitidas/{tarjeta.Id}", response);
        }

        public override async Task<IActionResult> SolicitarTarjetaFisicaAsync(string version, int idCliente,
            SolicitarTarjetaFisicaRequest body)
        {
            if (idCliente != body.IdCliente)
                return BadRequest(new InlineResponse400
                    { Errors = new List<InlineResponse400Errors> { new() { Detail = "IdCliente mismatch" } } });

            var tarjeta = await tarjetaEmitidaFacade.SolicitarTarjetaFisicaAsync(idCliente, body.NombreImpreso,
                this.GetAuthenticatedUserGuid());
            var response = mapper.Map<TarjetaEmitidaResult>(tarjeta);
            return Created($"/{version}/tarjetas-emitidas/{tarjeta.Id}", response);
        }

        public override async Task<IActionResult> CambiarBloqueoTarjetaAsync(string version, int idTarjeta,
            CambioBloqueoRequest body)
        {
            if (body.Bloquear == null)
                return BadRequest(new InlineResponse400
                    { Errors = new List<InlineResponse400Errors> { new() { Detail = "Bloquear status required" } } });

            await tarjetaEmitidaFacade.CambiarEstadoBloqueoAsync(idTarjeta, body.Bloquear.Value, body.ConcurrencyToken,
                this.GetAuthenticatedUserGuid());
            return Ok();
        }

        public override async Task<IActionResult> ActualizarConfiguracionTarjetaAsync(string version, int idTarjeta,
            ConfiguracionTarjetaRequest body)
        {
            if (body.LimiteDiario == null || body.ComprasEnLinea == null || body.Retiros == null)
                return BadRequest(new InlineResponse400
                {
                    Errors = new List<InlineResponse400Errors>
                        { new() { Detail = "All configuration fields required" } }
                });

            await tarjetaEmitidaFacade.ActualizarConfiguracionAsync(idTarjeta, body.LimiteDiario.Value,
                body.ComprasEnLinea.Value, body.Retiros.Value, body.ConcurrencyToken, this.GetAuthenticatedUserGuid());
            return Ok();
        }
    }
}
