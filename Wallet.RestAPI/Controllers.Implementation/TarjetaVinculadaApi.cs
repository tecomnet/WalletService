using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.GestionWallet;
using Wallet.RestAPI.Controllers;
using Wallet.RestAPI.Helpers;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
{
    public class TarjetaVinculadaApiController(ITarjetaVinculadaFacade tarjetaVinculadaFacade, IMapper mapper)
        : TarjetaVinculadaApiControllerBase
    {
        public override async Task<IActionResult> GetTarjetasVinculadasPorClienteAsync(string version, int idCliente)
        {
            var tarjetas = await tarjetaVinculadaFacade.ObtenerTarjetasPorClienteAsync(idCliente);
            var response = mapper.Map<List<TarjetaVinculadaResult>>(tarjetas);
            return Ok(response);
        }

        public override async Task<IActionResult> VincularTarjetaAsync(string version, int idCliente,
            VincularTarjetaRequest body)
        {
            if (idCliente != body.IdCliente)
                return BadRequest(new InlineResponse400
                    { Errors = new List<InlineResponse400Errors> { new() { Detail = "IdCliente mismatch" } } });

            // Map DTO enum to DOM enum manually if needed or casting safe if integers match. 
            // Since DTO enum has same integer values, casting is safe or mapper config handles it.
            // Using explicit cast or conversion for safety.
            var marca = (MarcaTarjeta)(int)body.Marca.Value;

            var tarjeta = await tarjetaVinculadaFacade.VincularTarjetaAsync(idCliente, body.TokenPasarela,
                body.PanEnmascarado, body.Alias, marca, body.FechaExpiracion.Value, this.GetAuthenticatedUserGuid());
            var response = mapper.Map<TarjetaVinculadaResult>(tarjeta);
            return Created($"/{version}/tarjetas-vinculadas/{tarjeta.Id}", response);
        }

        public override async Task<IActionResult> DesvincularTarjetaAsync(string version, int idTarjeta)
        {
            await tarjetaVinculadaFacade.DesvincularTarjetaAsync(idTarjeta, this.GetAuthenticatedUserGuid());
            return Ok();
        }

        public override async Task<IActionResult> EstablecerTarjetaFavoritaAsync(string version, int idTarjeta,
            SetFavoritaRequest body)
        {
            await tarjetaVinculadaFacade.EstablecerFavoritaAsync(idTarjeta, body.IdCliente.Value, body.ConcurrencyToken,
                this.GetAuthenticatedUserGuid());
            return Ok();
        }
    }
}
