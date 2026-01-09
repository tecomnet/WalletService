using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.CuentaWalletFacade;
using Wallet.Funcionalidad.Functionality.GestionWallet;
using Wallet.RestAPI.Controllers;
using Wallet.RestAPI.Helpers;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
{
    public class TarjetaVinculadaApiController(
        ITarjetaVinculadaFacade tarjetaVinculadaFacade,
        ICuentaWalletFacade cuentaWalletFacade,
        IMapper mapper)
        : TarjetaVinculadaApiControllerBase
    {
        private async Task<int> GetClienteIdFromWalletId(int idWallet)
        {
            var wallet = await cuentaWalletFacade.ObtenerPorIdAsync(idWallet: idWallet);
            return wallet.IdCliente;
        }

        public override async Task<IActionResult> VincularTarjetaAsync(VincularTarjetaRequest body, string version,
            int? idWallet)
        {
            // Mapping enum manually or verify match.
            var marca = (MarcaTarjeta)body.Marca;
            var idCliente = await GetClienteIdFromWalletId(idWallet: idWallet.Value);

            var tarjeta = await tarjetaVinculadaFacade.VincularTarjetaAsync(idCliente,
                body.NumeroTarjeta, body.Alias, marca, body.FechaExpiracion.Value, this.GetAuthenticatedUserGuid());
            var response = mapper.Map<TarjetaVinculadaResult>(tarjeta);
            return Created($"/{version}/cuentaWallet/{idWallet}/tarjetasVinculadas/{tarjeta.Id}", response);
        }

        public override async Task<IActionResult> DesvincularTarjetaAsync(string version, int? idTarjeta)
        {
            await tarjetaVinculadaFacade.DesvincularTarjetaAsync(idTarjeta: idTarjeta.Value, this.GetAuthenticatedUserGuid());
            return Ok();
        }

        public override async Task<IActionResult> EstablecerTarjetaFavoritaAsync(SetFavoritaRequest body,
            string version, int? idTarjeta)
        {
            var tarjeta = await tarjetaVinculadaFacade.EstablecerFavoritaAsync(idTarjeta: idTarjeta.Value, body.ConcurrencyToken,
                this.GetAuthenticatedUserGuid());
            return Ok(mapper.Map<TarjetaVinculadaResult>(tarjeta));
        }
    }
}
