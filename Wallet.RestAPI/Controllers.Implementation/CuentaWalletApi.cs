using System.Threading.Tasks;
using AutoMapper;
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
        var wallet = await cuentaWalletFacade.ObtenerPorClienteAsync(idCliente: idCliente);

        // Map to response model
        var response = mapper.Map<CuentaWalletResult>(source: wallet);

        // Return OK response
        return Ok(value: response);
    }
}
