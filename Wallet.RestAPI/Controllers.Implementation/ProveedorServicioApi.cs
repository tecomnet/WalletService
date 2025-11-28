using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <inheritdoc/>
public class ProveedorServicioApiController(IProveedorServicioFacade proveedorServicioFacade, IMapper mapper)
    : ProveedorServicioApiControllerBase
{
    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteProveedorServicioAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version,
        [FromRoute, Required] int idProveedorServicio)
    {
        // Call facade method
        var proveedor =
            await proveedorServicioFacade.EliminarProveedorServicioAsync(idProveedorServicio: idProveedorServicio,
                modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ProveedorServicioResult>(source: proveedor);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetProveedorServicioAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version,
        [FromRoute, Required] int idProveedorServicio)
    {
        // Call facade method
        var proveedor =
            await proveedorServicioFacade.ObtenerProveedorServicioPorIdAsync(idProveedorServicio: idProveedorServicio);
        // Map to response model
        var response = mapper.Map<ProveedorServicioResult>(source: proveedor);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetProveedoresServicioAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version)
    {
        // Call facade method
        var proveedores = await proveedorServicioFacade.ObtenerProveedoresServicioAsync();
        // Map to response model
        var response = mapper.Map<List<ProveedorServicioResult>>(source: proveedores);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PostProveedorServicioAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version,
        [FromBody] ProveedorServicioRequest body)
    {
        // Convert string to enum
        if (!Enum.TryParse<ProductoCategoria>(value: body.Categoria, result: out var categoria))
        {
            throw new ArgumentException(message: $"El valor {body.Categoria} no es una Categoria válida.");
        }

        // Call facade method
        var proveedor = await proveedorServicioFacade.GuardarProveedorServicioAsync(
            nombre: body.Nombre,
            categoria: categoria,
            urlIcono: body.UrlIcono,
            creationUser: Guid.Empty);
        // Map to result
        var result = mapper.Map<ProveedorServicioResult>(source: proveedor);
        // Return created
        return Created(uri: $"/{version}/proveedorServicio/{proveedor.Id}", value: result);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutActivarProveedorServicioAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version,
        [FromRoute, Required] int idProveedorServicio)
    {
        // Call facade method
        var proveedor =
            await proveedorServicioFacade.ActivarProveedorServicioAsync(idProveedorServicio: idProveedorServicio,
                modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ProveedorServicioResult>(source: proveedor);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutProveedorServicioAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version,
        [FromRoute, Required] int idProveedorServicio, [FromBody] ProveedorServicioRequest body)
    {
        // Convert string to enum
        if (!Enum.TryParse<ProductoCategoria>(value: body.Categoria, result: out var categoria))
        {
            throw new ArgumentException(message: $"El valor {body.Categoria} no es una Categoria válida.");
        }

        // Call facade method
        var proveedor = await proveedorServicioFacade.ActualizarProveedorServicioAsync(
            idProveedorServicio: idProveedorServicio,
            nombre: body.Nombre,
            categoria: categoria,
            urlIcono: body.UrlIcono,
            modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ProveedorServicioResult>(source: proveedor);
        // Return OK response
        return Ok(value: response);
    }
}
