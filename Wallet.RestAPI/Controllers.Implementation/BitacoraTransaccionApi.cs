using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.DetallesPagoServicioFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
{
    public class BitacoraTransaccionApiController(
        IDetallesPagoServicioFacade detallesPagoServicioFacade,
        IMapper mapper)
        : BitacoraTransaccionApiControlleBase
    {
        public override async Task<IActionResult> GetDetallesPorTransaccionAsync(string version, int? idTransaccion)
        {
            var detalles = await detallesPagoServicioFacade.ObtenerPorTransaccionAsync(idTransaccion.Value);


            var response = mapper.Map<DetallesPagoServicioResult>(detalles[0]);
            return Ok(response);
        }
    }
}
