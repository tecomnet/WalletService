using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the ProductoProveedor API controller.
/// </summary>
public class ProductoProveedorApiController(IProveedorServicioFacade proveedorServicioFacade, IMapper mapper)
    : ProductoProveedorApiControllerBase
{
    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteProductoProveedorAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idProducto)
    {
        var producto =
            await proveedorServicioFacade.EliminarProductoAsync(idProducto: idProducto, modificationUser: Guid.Empty);
        var response = mapper.Map<ProductoProveedorResult>(source: producto);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetProductoProveedorAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idProducto)
    {
        var producto = await proveedorServicioFacade.ObtenerProductoPorIdAsync(idProducto: idProducto);
        var response = mapper.Map<ProductoProveedorResult>(source: producto);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetProductosPorProveedorAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idProveedorServicio)
    {
        var productos =
            await proveedorServicioFacade.ObtenerProductosPorProveedorAsync(proveedorServicioId: idProveedorServicio);
        var response = mapper.Map<List<ProductoProveedorResult>>(source: productos);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PostProductoProveedorAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idProveedorServicio,
        [FromBody] ProductoProveedorRequest body)
    {
        var producto = await proveedorServicioFacade.GuardarProductoAsync(
            proveedorServicioId: idProveedorServicio,
            sku: body.Sku,
            nombre: body.Nombre,
            monto: body.Monto,
            descripcion: body.Descripcion,
            creationUser: Guid.Empty);

        var result = mapper.Map<ProductoProveedorResult>(source: producto);
        return Created(uri: $"/{version}/productoProveedor/{producto.Id}", value: result);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutActivarProductoProveedorAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idProducto)
    {
        var producto =
            await proveedorServicioFacade.ActivarProductoAsync(idProducto: idProducto, modificationUser: Guid.Empty);
        var response = mapper.Map<ProductoProveedorResult>(source: producto);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutProductoProveedorAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idProducto,
        [FromBody] ProductoProveedorRequest body)
    {
        var producto = await proveedorServicioFacade.ActualizarProductoAsync(
            idProducto: idProducto,
            sku: body.Sku,
            nombre: body.Nombre,
            monto: body.Monto,
            descripcion: body.Descripcion,
            modificationUser: Guid.Empty);

        var response = mapper.Map<ProductoProveedorResult>(source: producto);
        return Ok(value: response);
    }
}
