using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the UbicacionGeolocalizacion API controller.
/// </summary>
//[Authorize]
public class UbicacionApiController(IUbicacionGeolocalizacionFacade ubicacionFacade, IMapper mapper)
    : UbicacionGeolocalizacionApiControllerBase
{
    // TODO EMD: PENDIENTE IMPLEMENTAR get de ubicacion
    /// <inheritdoc />
    public override async Task<IActionResult> PostUbicacionAsync(string version, int? idCliente, UbicacionRequest body)
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
            idCliente: idCliente.Value,
            latitud: body.Latitud.Value,
            longitud: body.Longitud.Value,
            dispositivo: (DOM.Enums.Dispositivo)body.Dispositivo,
            tipoEvento: body.TipoEvento,
            tipoDispositivo: body.TipoDispositivo,
            agente: body.Agente,
            direccionIp: body.DireccionIP,
            creationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<UbicacionResult>(source: ubicacion);
        // Return OK response
        return Ok(value: response);
    }
}
