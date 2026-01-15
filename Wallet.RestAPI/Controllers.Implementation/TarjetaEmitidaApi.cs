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
        public override async Task<IActionResult> CambiarBloqueoTarjetaAsync(string version, CambioBloqueoRequest body,
            int? idTarjeta)
        {
            if (idTarjeta == null) throw new ArgumentNullException(nameof(idTarjeta));
            // Note: Facade CambiarEstadoBloqueoAsync(int idTarjeta, bool bloquear, ...)
            await tarjetaEmitidaFacade.CambiarEstadoBloqueoAsync(idTarjeta.Value, body.Bloquear.Value,
                body.ConcurrencyToken, this.GetAuthenticatedUserGuid());
            return Ok();
        }

        public override async Task<IActionResult> ActualizarConfiguracionTarjetaAsync(string version,
            ConfiguracionTarjetaRequest body, int? idTarjeta)
        {
            if (idTarjeta == null) throw new ArgumentNullException(nameof(idTarjeta));

            await tarjetaEmitidaFacade.ActualizarConfiguracionAsync(
                idTarjeta.Value,
                body.LimiteDiario.Value,
                body.ComprasEnLinea.Value,
                body.Retiros.Value,
                body.ConcurrencyToken,
                this.GetAuthenticatedUserGuid()
            );
            return Ok();
        }
    }
}
