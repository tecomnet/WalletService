using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the UbicacionGeolocalizacion API controller.
/// </summary>
public class UbicacionApiController(IUbicacionGeolocalizacionFacade ubicacionFacade, IMapper mapper)
    : UbicacionGeolocalizacionApiControllerBase
{
    // TODO EMD: PENDIENTE IMPLEMENTAR get de ubicacion
    /// <inheritdoc/>
    public override async Task<IActionResult> PostUbicacionAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version,
        [FromRoute, Required] int idCliente, [FromBody] UbicacionRequest body)
    {
        // Obtienes el valor como entero de forma segura.
        int dispositivo = (int)body.Dispositivo;
        // Intentar convertir el entero al tipo Enum
        if (!Enum.IsDefined(enumType: typeof(Wallet.DOM.Enums.Dispositivo), value: dispositivo))
        {
            //Si es un valor inválido, lanza una excepción de validación o un BadRequest.
            throw new ArgumentException(message: $"El valor {dispositivo} no es un tipo de dispositivo válido.");
        }

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
        var response = mapper.Map<UbicacionResult>(source: ubicacion);
        // Return OK response
        return Ok(value: response);
    }
}
