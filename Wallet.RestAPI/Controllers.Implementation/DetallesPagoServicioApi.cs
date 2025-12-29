using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.DetallesPagoServicioFacade;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the DetallesPagoServicio API controller.
/// </summary>
[Authorize]
public class DetallesPagoServicioApiController(
    IDetallesPagoServicioFacade detallesPagoServicioFacade,
    IMapper mapper)
    : DetallesPagoServicioApiControllerBase
{
    /// <inheritdoc />
    public override async Task<IActionResult> GetDetallesPorTransaccionAsync(string version, int idTransaccion)
    {
        // Call facade method
        var detalles = await detallesPagoServicioFacade.ObtenerPorTransaccionAsync(idTransaccion: idTransaccion);

        // Map to response model
        var response = mapper.Map<List<DetallesPagoServicioResult>>(source: detalles);

        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetDetallesPagoServicioPorClienteAsync(string version, int idCliente)
    {
        // Call facade method
        var detalles = await detallesPagoServicioFacade.ObtenerPorClienteAsync(idCliente: idCliente);

        // Map to response model
        var response = mapper.Map<List<DetallesPagoServicioResult>>(source: detalles);

        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> RegistrarPagoServicioAsync(string version,
        RegistrarPagoServicioRequest body)
    {
        // Call facade method
        var detalle = await detallesPagoServicioFacade.RegistrarPagoServicioAsync(
            idBilletera: body.IdBilletera,
            monto: body.Monto,
            nombreServicio: body.NombreServicio,
            direccion: body.Direccion,
            estatus: "Completada", // Default status, could come from request if needed
            refExternaId: body.RefExternaId,
            idProveedor: body.IdProveedor,
            numeroReferencia: body.NumeroReferencia,
            codigoAutorizacion: body.CodigoAutorizacion,
            creationUser: this.GetAuthenticatedUserGuid()
        );

        // Map to response model
        var response = mapper.Map<DetallesPagoServicioResult>(source: detalle);

        // Return Created response
        return Created(uri: $"/{version}/transacciones/{detalle.IdTransaccion}/detalles", value: response);
    }
}
