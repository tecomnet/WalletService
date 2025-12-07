using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the ServicioFavorito API controller.
/// </summary>
public class ServicioFavoritoApiController(IServicioFavoritoFacade servicioFavoritoFacade, IMapper mapper)
    : ServicioFavoritoApiControllerBase
{
    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteServicioFavoritoAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idServicioFavorito)
    {
        // Call facade method
        var servicio =
            await servicioFavoritoFacade.EliminarServicioFavoritoAsync(idServicioFavorito: idServicioFavorito,
                modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ServicioFavoritoResult>(source: servicio);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetServiciosFavoritosAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idCliente)
    {
        // Call facade method
        var servicios = await servicioFavoritoFacade.ObtenerServiciosFavoritosPorClienteAsync(clienteId: idCliente);
        // Map to response model
        var response = mapper.Map<List<ServicioFavoritoResult>>(source: servicios);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PostServicioFavoritoAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromBody] ServicioFavoritoRequest body)
    {
        // Call facade method
        var servicio = await servicioFavoritoFacade.GuardarServicioFavoritoAsync(
            clienteId: body.ClienteId.Value,
            proveedorId: body.ProveedorId.Value,
            alias: body.Alias,
            numeroReferencia: body.NumeroReferencia,
            creationUser: Guid.Empty);
        // Map to result
        var result = mapper.Map<ServicioFavoritoResult>(source: servicio);
        // Return created
        return Created(uri: $"/{version}/servicioFavorito/{servicio.Id}", value: result);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutServicioFavoritoAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idServicioFavorito, [FromBody] ServicioFavoritoRequest body)
    {
        // Call facade method
        var servicio = await servicioFavoritoFacade.ActualizarServicioFavoritoAsync(
            idServicioFavorito: idServicioFavorito,
            alias: body.Alias,
            numeroReferencia: body.NumeroReferencia,
            modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ServicioFavoritoResult>(source: servicio);
        // Return OK response
        return Ok(value: response);
    }
}
