using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the BitacoraTransaccion API controller.
/// </summary>
[Authorize]
public class BitacoraTransaccionApiController(
    IBitacoraTransaccionFacade bitacoraTransaccionFacade,
    IMapper mapper)
    : BitacoraTransaccionApiControllerBase
{
    /// <inheritdoc />
    public override async Task<IActionResult> GetTransaccionesPorClienteAsync(string version, Guid idCliente)
    {
        // Call facade method
        var transacciones = await bitacoraTransaccionFacade.ObtenerPorClienteAsync(idCliente);

        // Map to response model
        var response = mapper.Map<List<BitacoraTransaccionResult>>(source: transacciones);

        // Return OK response
        return Ok(value: response);
    }
}
