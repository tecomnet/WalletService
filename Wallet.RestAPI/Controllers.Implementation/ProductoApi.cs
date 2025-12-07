using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ProveedorFacade;
using Wallet.RestAPI.Controllers;
using Wallet.RestAPI.Models;

/// <inheritdoc />
public class ProductoApiController(IProveedorFacade proveedorFacade, IMapper mapper) : ProductoApiControllerBase
{
    /// <inheritdoc />
    public override async Task<IActionResult> ActualizarProducto([FromRoute] [Required] int? idProducto,
        [FromBody] ProductoRequest body)
    {
        if (idProducto == null)
        {
            throw new ArgumentNullException(nameof(idProducto), "El ID del producto es requerido.");
        }

        if (body.Precio == null)
        {
            throw new ArgumentNullException(nameof(body.Precio), "El Precio es requerido.");
        }

        var producto = await proveedorFacade.ActualizarProductoAsync(
            idProducto: idProducto.Value,
            sku: body.Sku,
            nombre: body.Nombre,
            precio: body.Precio.Value,
            icono: body.Icono,
            categoria: body.Categoria,
            modificationUser: Guid.Empty);

        var result = mapper.Map<ProductoResult>(source: producto);
        return Ok(value: result);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> EliminarProductoAsync([FromRoute] [Required] int? idProducto)
    {
        if (idProducto == null)
        {
            throw new ArgumentNullException(nameof(idProducto), "El ID del producto es requerido.");
        }

        var result =
            await proveedorFacade.EliminarProductoAsync(idProducto: idProducto.Value, modificationUser: Guid.Empty);
        return Ok(value: mapper.Map<ProductoResult>(source: result));
    }

    /// <inheritdoc />
    public override async Task<IActionResult> ObtenerProductoPorIdAsync([FromRoute] [Required] int? idProducto)
    {
        if (idProducto == null)
        {
            throw new ArgumentNullException(nameof(idProducto), "El ID del producto es requerido.");
        }

        var producto = await proveedorFacade.ObtenerProductoPorIdAsync(idProducto: idProducto.Value);
        var result = mapper.Map<ProductoResult>(source: producto);
        return Ok(value: result);
    }

    /// <summary>
    ///  Listar todos los productos
    /// </summary>
    /// <returns></returns>
    public override async Task<IActionResult> ObtenerProductosAsync()
    {
        var productos = await proveedorFacade.ObtenerProductosAsync();
        var response = mapper.Map<List<ProductoResult>>(source: productos);
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> CrearProducto([FromRoute] [Required] int? idProveedor,
        [FromBody] ProductoRequest body)
    {
        if (idProveedor == null)
        {
            throw new ArgumentNullException(nameof(idProveedor), "El ID del proveedor es requerido.");
        }

        if (body.Precio == null)
        {
            throw new ArgumentNullException(nameof(body.Precio), "El Precio es requerido.");
        }

        var producto = await proveedorFacade.GuardarProductoAsync(
            proveedorId: idProveedor.Value,
            sku: body.Sku,
            nombre: body.Nombre,
            precio: body.Precio.Value,
            icono: body.Icono,
            categoria: body.Categoria,
            creationUser: Guid.Empty);

        var result = mapper.Map<ProductoResult>(source: producto);
        return Created(uri: $"/proveedor/{idProveedor}/producto/{result.Id}", value: result);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> ObtenerProductosPorProveedorAsync([FromRoute] [Required] int? idProveedor)
    {
        if (idProveedor == null)
        {
            throw new ArgumentNullException(nameof(idProveedor), "El ID del proveedor es requerido.");
        }

        var productos = await proveedorFacade.ObtenerProductosPorProveedorAsync(proveedorId: idProveedor.Value);
        var response = mapper.Map<List<ProductoResult>>(source: productos);
        return Ok(value: response);
    }
}
