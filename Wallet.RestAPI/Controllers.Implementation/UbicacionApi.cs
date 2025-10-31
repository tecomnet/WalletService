using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;
/// <inheritdoc/>
public class UbicacionApiController(IUbicacionGeolocalizacionFacade ubicacionFacade, IMapper mapper) : UbicacionApiControllerBase
{
    // TODO EMD: PENDIENTE IMPLEMENTAR get de ubicacion
    /// <inheritdoc/>
    public override async Task<IActionResult> PostUbicacionAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] UbicacionRequest body)
    {
        // Call facade method
        var ubicacion = await ubicacionFacade.GuardarUbicacionGeolocalizacionAsync(
            idCliente: idCliente,
            latitud: body.Latitud.Value,
            longitud: body.Longitud.Value,
            dispositivo: (DOM.Enums.Dispositivo)body.Dispositivo,
            tipoEvento: body.TipoEvento,
            tipoDispositivo: body.TipoDispositivo,
            agente: body.Agente,
            direccionIp: body.DireccionIP,
            creationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<UbicacionResult>(ubicacion);
        // Return OK response
        return Ok(response);
    }

}
