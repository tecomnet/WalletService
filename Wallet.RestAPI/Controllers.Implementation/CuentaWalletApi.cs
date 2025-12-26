using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.CuentaWalletFacade;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the CuentaWallet API controller.
/// </summary>
public class CuentaWalletApiController(ICuentaWalletFacade cuentaWalletFacade, IMapper mapper)
    : CuentaWalletApiControllerBase
{
    /// <inheritdoc />
    public override async Task<IActionResult> GetCuentaWalletPorClienteAsync(string version, int idCliente)
    {
        // Call facade method
        var wallet = await cuentaWalletFacade.ObtenerPorClienteAsync(idCliente);

        // Map to response model
        var response = mapper.Map<CuentaWalletResult>(source: wallet);

        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> CreateCuentaWalletAsync(CrearCuentaWalletRequest body, string version)
    {
        if (body.IdCliente == null)
            return BadRequest("IdCliente is required");

        // Call facade method
        var wallet = await cuentaWalletFacade.CrearCuentaWalletAsync(
            idCliente: body.IdCliente.Value,
            creationUser: this.GetAuthenticatedUserGuid(),
            moneda: body.Moneda);

        // Map to response model
        var response = mapper.Map<CuentaWalletResult>(source: wallet);

        // Return Created response. 
        // Note: Ideally we should return CreatedAtAction but for now returning 201 with body is fine matching swagger.
        return Created(uri: $"/{version}/cuentaWallet/{wallet.Id}", value: response);
    }
}
