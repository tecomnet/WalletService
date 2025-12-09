using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the DispositivoMovilAutorizado API controller.
/// </summary>
public class DispositivoMovilAutorizadoApiController(
    IDispositivoMovilAutorizadoFacade dispositivoMovilAutorizadoFacade,
    IMapper mapper) : DispositivoMovilAutorizadoApiControllerBase
{
    /// <inheritdoc/>
    public override async Task<IActionResult> GetDispositivoMovilAutorizadoAsync(
        string version,
        int? idCliente, string idDispositivo, string token)
    {
        // Call facade method
        var esDispositivoAutorizado =
            await dispositivoMovilAutorizadoFacade.EsDispositivoAutorizadoAsync(idCliente: idCliente.Value,
                idDispositivo: idDispositivo, token: token);
        // Map to response model
        //var response = mapper.Map<DispositivoMovilAutorizadoResult>(dispositivoMovilAutorizado);
        // Return OK response
        return Ok(value: esDispositivoAutorizado);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PostDispositivoMovilAutorizadoAsync(
        string version,
        int? idCliente, DispositivoMovilAutorizadoRequest body)
    {
        // Call facade method
        var dispositivoMovilAutorizado = await dispositivoMovilAutorizadoFacade.GuardarDispositivoAutorizadoAsync(
            idCliente: idCliente.Value,
            idDispositivo: body.IdDispositivo,
            token: body.Token,
            nombre: body.Nombre,
            caracteristicas: body.Caracteristicas,
            creationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<DispositivoMovilAutorizadoResult>(source: dispositivoMovilAutorizado);
        // Return OK response
        return Ok(value: response);
    }
}
