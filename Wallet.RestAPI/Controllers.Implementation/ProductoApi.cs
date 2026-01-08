using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ProveedorFacade;
using Wallet.RestAPI.Controllers;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;

/// <inheritdoc />
//[Authorize]
public class ProductoApiController(IProveedorFacade proveedorFacade, IMapper mapper) : ProductoApiControllerBase
{
    /// <inheritdoc />
    public override async Task<IActionResult> DeleteProductoAsync(string version, int? idProducto)
    {
        if (idProducto == null)
        {
            throw new ArgumentNullException(paramName: nameof(idProducto), message: "El ID del producto es requerido.");
        }

        var result =
            await proveedorFacade.EliminarProductoAsync(idProducto: idProducto.Value,
                modificationUser: this.GetAuthenticatedUserGuid());
        return Ok(value: mapper.Map<ProductoResult>(source: result));
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetProductoAsync(string version, int? idProducto)
    {
        if (idProducto == null)
        {
            throw new ArgumentNullException(paramName: nameof(idProducto), message: "El ID del producto es requerido.");
        }

        var producto = await proveedorFacade.ObtenerProductoPorIdAsync(idProducto: idProducto.Value);
        var result = mapper.Map<ProductoResult>(source: producto);
        return Ok(value: result);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetProductosAsync(string version, string categoria)
    {
        var productos = string.IsNullOrWhiteSpace(categoria)
            ? await proveedorFacade.ObtenerProductosAsync()
            : await proveedorFacade.ObtenerProductosPorCategoriaAsync(categoria);

        var result = mapper.Map<List<ProductoResult>>(source: productos);
        return Ok(value: result);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetProductosPorProveedorAsync(string version, int? idProveedor)
    {
        if (idProveedor == null)
        {
            throw new ArgumentNullException(paramName: nameof(idProveedor),
                message: "El ID del proveedor es requerido.");
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
            throw new ArgumentNullException(paramName: nameof(idProveedor),
                message: "El ID del proveedor es requerido.");
        }

        var producto = await proveedorFacade.GuardarProductoAsync(
            proveedorId: idProveedor.Value,
            sku: body.Sku,
            nombre: body.Nombre,
            precio: (decimal?)body.Precio,
            icono: body.UrlIcono,
            categoria: EnumExtensions.GetEnumMemberValue(body.Categoria),
            creationUser: this.GetAuthenticatedUserGuid());

        var result = mapper.Map<ProductoResult>(source: producto);
        return Created(uri: $"/proveedor/{idProveedor}/producto/{result.Id}", value: result);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PutActivarProductoAsync(string version, int? idProducto)
    {
        if (idProducto == null)
        {
            throw new ArgumentNullException(paramName: nameof(idProducto), message: "El ID del producto es requerido.");
        }

        var result =
            await proveedorFacade.ActivarProductoAsync(idProducto: idProducto.Value,
                modificationUser: this.GetAuthenticatedUserGuid());
        return Ok(value: mapper.Map<ProductoResult>(source: result));
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PutActualizarProveedorAsync(string version, int? idProducto, int? body)
    {
        if (idProducto == null)
        {
            throw new ArgumentNullException(paramName: nameof(idProducto), message: "El ID del producto es requerido.");
        }

        if (body == null)
        {
            throw new ArgumentNullException(paramName: nameof(body), message: "El ID del proveedor es requerido.");
        }

        var result = await proveedorFacade.ActualizarProveedorDeProductoAsync(
            idProducto: idProducto.Value,
            idProveedor: body.Value,
            modificationUser: this.GetAuthenticatedUserGuid());

        return Ok(value: mapper.Map<ProductoResult>(source: result));
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PutProductoAsync(string version, int? idProducto,
        ProductoUpdateRequest body)
    {
        if (idProducto == null)
        {
            throw new ArgumentNullException(paramName: nameof(idProducto), message: "El ID del producto es requerido.");
        }

        var producto = await proveedorFacade.ActualizarProductoAsync(
            idProducto: idProducto.Value,
            sku: body.Sku,
            nombre: body.Nombre,
            precio: (decimal?)body.Precio,
            icono: body.UrlIcono,
            categoria: EnumExtensions.GetEnumMemberValue(body.Categoria),
            concurrencyToken: body.ConcurrencyToken,
            modificationUser: this.GetAuthenticatedUserGuid());

        var result = mapper.Map<ProductoResult>(source: producto);
        return Ok(value: result);
    }
}
