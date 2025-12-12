using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;
/// <inheritdoc/>
public class DispositivoMovilAutorizadoApiController(IDispositivoMovilAutorizadoFacade dispositivoMovilAutorizadoFacade, IMapper mapper) : DispositivoMovilAutorizadoApiControllerBase
{
    public override async Task<IActionResult> GetDispositivoMovilAutorizadoAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int? idCliente, [FromQuery] string idDispositivo, [FromQuery] string token)
    {
        // Call facade method
        var esDispositivoAutorizado = await dispositivoMovilAutorizadoFacade.EsDispositivoAutorizadoAsync(idCliente: idCliente.Value, idDispositivo: idDispositivo, token: token);
        // Map to response model
        //var response = mapper.Map<DispositivoMovilAutorizadoResult>(dispositivoMovilAutorizado);
        // Return OK response
        return Ok(esDispositivoAutorizado);
    }


    /// <inheritdoc/>
    public override async Task<IActionResult> PostDispositivoMovilAutorizadoAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] DispositivoMovilAutorizadoRequest body)
    {
        // Call facade method
        var dispositivoMovilAutorizado = await dispositivoMovilAutorizadoFacade.GuardarDispositivoAutorizadoAsync(
            idCliente: idCliente,
            idDispositivo: body.IdDispositivo,
            token: body.Token,
            nombre: body.Nombre,
            caracteristicas: body.Caracteristicas,
            creationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<DispositivoMovilAutorizadoResult>(dispositivoMovilAutorizado);
        // Return OK response
        return Ok(response);
    }

}
