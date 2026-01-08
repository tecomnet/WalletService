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
        private async Task<int> GetClienteIdFromWalletId(string idWallet)
        {
            if (!int.TryParse(idWallet, out var id))
            {
                throw new ArgumentException("Invalid Wallet ID format");
            }

            var wallet = await cuentaWalletFacade.ObtenerPorIdAsync(id);
            return wallet.IdCliente;
        }

        public override async Task<IActionResult> VincularTarjetaAsync(VincularTarjetaRequest body, string version,
            string idWallet)
        {
            // Mapping enum manually or verify match.
            var marca = (MarcaTarjeta)body.Marca;
            var idCliente = await GetClienteIdFromWalletId(idWallet);

            var tarjeta = await tarjetaVinculadaFacade.VincularTarjetaAsync(idCliente,
                body.NumeroTarjeta, body.Alias, marca, body.FechaExpiracion.Value, this.GetAuthenticatedUserGuid());
            var response = mapper.Map<TarjetaVinculadaResult>(tarjeta);
            return Created($"/{version}/cuentaWallet/{idWallet}/tarjetasVinculadas/{tarjeta.Id}", response);
        }

        public override async Task<IActionResult> DesvincularTarjetaAsync(string version, string idTarjeta)
        {
            if (!int.TryParse(idTarjeta, out var id)) throw new ArgumentException("Invalid Card ID");

            await tarjetaVinculadaFacade.DesvincularTarjetaAsync(id, this.GetAuthenticatedUserGuid());
            return Ok();
        }

        public override async Task<IActionResult> EstablecerTarjetaFavoritaAsync(SetFavoritaRequest body,
            string version, string idTarjeta)
        {
            if (!int.TryParse(idTarjeta, out var id)) throw new ArgumentException("Invalid Card ID");

            var tarjeta = await tarjetaVinculadaFacade.EstablecerFavoritaAsync(id, body.ConcurrencyToken,
                this.GetAuthenticatedUserGuid());
            return Ok(mapper.Map<TarjetaVinculadaResult>(tarjeta));
        }
    }
}
