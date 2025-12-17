using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the ServicioFavorito API controller.
/// </summary>
//[Authorize]
public class ServicioFavoritoApiController(IServicioFavoritoFacade servicioFavoritoFacade, IMapper mapper)
    : ServicioFavoritoApiControllerBase
{
    /// <inheritdoc />
    public override async Task<IActionResult> DeleteServicioFavoritoAsync(string version, int? idServicioFavorito)
    {
        // Call facade method
        var servicio =
            await servicioFavoritoFacade.EliminarServicioFavoritoAsync(idServicioFavorito: idServicioFavorito.Value,
                modificationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<ServicioFavoritoResult>(source: servicio);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetServiciosFavoritosAsync(string version, int? idCliente)
    {
        // Call facade method
        var servicios =
            await servicioFavoritoFacade.ObtenerServiciosFavoritosPorClienteAsync(clienteId: idCliente.Value);
        // Map to response model
        var response = mapper.Map<List<ServicioFavoritoResult>>(source: servicios);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PostServicioFavoritoAsync(
        string version,
        ServicioFavoritoRequest body)
    {
        // Call facade method
        var servicio = await servicioFavoritoFacade.GuardarServicioFavoritoAsync(
            clienteId: body.ClienteId.Value,
            proveedorId: body.ProveedorId.Value,
            alias: body.Alias,
            numeroReferencia: body.NumeroReferencia,
            creationUser: this.GetAuthenticatedUserGuid());
        // Map to result
        var result = mapper.Map<ServicioFavoritoResult>(source: servicio);
        // Return created
        return Created(uri: $"/{version}/servicioFavorito/{servicio.Id}", value: result);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PutServicioFavoritoAsync(string version, int? idServicioFavorito,
        ServicioFavoritoUpdateRequest body)
    {
        // Call facade method
        var servicio = await servicioFavoritoFacade.ActualizarServicioFavoritoAsync(
            idServicioFavorito: idServicioFavorito.Value,
            alias: body.Alias,
            numeroReferencia: body.NumeroReferencia,
            concurrencyToken: body.ConcurrencyToken,
            modificationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<ServicioFavoritoResult>(source: servicio);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PutActivarServicioFavoritoAsync(string version, int? idServicioFavorito)
    {
        // Call facade method
        var servicio = await servicioFavoritoFacade.ActivarServicioFavoritoAsync(
            idServicioFavorito: idServicioFavorito.Value,
            modificationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<ServicioFavoritoResult>(source: servicio);
        // Return OK response
        return Ok(value: response);
    }
}
