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
    public override async Task<IActionResult> DeleteProductoAsync(string version, int? idProducto)
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
    public override async Task<IActionResult> GetProductoAsync(string version, int? idProducto)
    {
        if (idProducto == null)
        {
            throw new ArgumentNullException(nameof(idProducto), "El ID del producto es requerido.");
        }

        var producto = await proveedorFacade.ObtenerProductoPorIdAsync(idProducto: idProducto.Value);
        var result = mapper.Map<ProductoResult>(source: producto);
        return Ok(value: result);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetProductosPorProveedorAsync(string version, int? idProveedor)
    {
        if (idProveedor == null)
        {
            throw new ArgumentNullException(nameof(idProveedor), "El ID del proveedor es requerido.");
        }

        var productos = await proveedorFacade.ObtenerProductosPorProveedorAsync(proveedorId: idProveedor.Value);
        var response = mapper.Map<List<ProductoResult>>(source: productos);
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PostProductoAsync(string version, int? idProveedor, ProductoRequest body)
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
            icono: body.UrlIcon,
            categoria: body.Categoria,
            creationUser: Guid.Empty);

        var result = mapper.Map<ProductoResult>(source: producto);
        return Created(uri: $"/proveedor/{idProveedor}/producto/{result.Id}", value: result);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PutActivarProductoAsync(string version, int? idProducto)
    {
        if (idProducto == null)
        {
            throw new ArgumentNullException(nameof(idProducto), "El ID del producto es requerido.");
        }

        var result =
            await proveedorFacade.ActivarProductoAsync(idProducto: idProducto.Value, modificationUser: Guid.Empty);
        return Ok(value: mapper.Map<ProductoResult>(source: result));
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PutProductoAsync(string version, int? idProducto, ProductoRequest body)
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
            modificationUser: Guid.Empty);

        var result = mapper.Map<ProductoResult>(source: producto);
        return Ok(value: result);
    }
}
