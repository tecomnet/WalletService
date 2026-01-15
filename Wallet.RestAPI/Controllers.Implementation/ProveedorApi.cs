using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.Funcionalidad.Functionality.ProveedorFacade;
using Wallet.RestAPI.Errors;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
///  API para la gestión de proveedores de servicio.
/// </summary>
[ApiController]
//[Authorize]
public class ProveedorApiController : ProveedorApiControllerBase
{
    private readonly IProveedorFacade _proveedorFacade;
    private readonly IMapper _mapper;

    /// <summary>
    ///  Constructor for ProveedorServicioApiController.
    /// </summary>
    /// <param name="proveedorFacade"></param>
    /// <param name="mapper"></param>
    public ProveedorApiController(IProveedorFacade proveedorFacade, IMapper mapper)
    {
        _proveedorFacade = proveedorFacade;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<IActionResult> DeleteProveedorAsync(string version, int? idProveedor)
    {
        var proveedor = await _proveedorFacade.EliminarProveedorAsync(idProveedor: idProveedor.Value,
            modificationUser: this.GetAuthenticatedUserGuid());
        var response = _mapper.Map<ProveedorResult>(source: proveedor);
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetProveedorAsync(string version, int? idProveedor)
    {
        var proveedorEncontrado =
            await _proveedorFacade.ObtenerProveedorPorIdAsync(idProveedor: idProveedor.Value);
        var response = _mapper.Map<ProveedorResult>(source: proveedorEncontrado);
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetProveedoresServicioAsync(string version, string categoria)
    {
        List<Proveedor> proveedores;
        // Viene categoria
        if (!string.IsNullOrWhiteSpace(categoria))
        {
            // Valida si la categoria es un valor del enum valido
            if (!Enum.IsDefined(typeof(Categoria), categoria))
            {
                return this.BadRequest(error:
                    new InlineResponse400(restAPIError: new RestAPIErrors()
                        .GetRestAPIError(errorCode: RestAPIErrors.CategoriaInvalida)));
            }

            // Convierte al enum del dom
            var categoriaEnum = (Categoria)Enum.Parse(typeof(Categoria), categoria);
            // Obtiene solo los de la categoria
            proveedores = await _proveedorFacade.ObtenerProveedoresAsync(categoria: categoriaEnum);
        }
        else
            // Obtiene todos 
            proveedores = await _proveedorFacade.ObtenerProveedoresAsync();

        // Mapeo
        var response = _mapper.Map<List<ProveedorResult>>(source: proveedores);
        // Retorno
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PostProveedorAsync(string version, ProveedorRequest body)
    {
        var proveedorCreado = await _proveedorFacade.GuardarProveedorAsync(
            nombre: body.Nombre,
            urlIcono: body.UrlIcono,
            categoria: (Categoria)body.Categoria,
            brokerId: body.BrokerId.Value,
            creationUser: this.GetAuthenticatedUserGuid());

        // Activating provider if necessary or assuming Guardar activates it. 
        // If specific activation is needed, use proveedorCreado.Id

        var response = _mapper.Map<ProveedorResult>(source: proveedorCreado);
        return Created(uri: $"/proveedor/{response.Id}", value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PutActivarProveedorAsync(string version, int? idProveedor)
    {
        var proveedor = await _proveedorFacade.ActivarProveedorAsync(idProveedor: idProveedor.Value,
            modificationUser: this.GetAuthenticatedUserGuid());
        var response = _mapper.Map<ProveedorResult>(source: proveedor);
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PutProveedorAsync(string version, int? idProveedor,
        ProveedorUpdateRequest body)
    {
        var proveedor = await _proveedorFacade.ActualizarProveedorAsync(
            idProveedor: idProveedor.Value,
            urlIcono: body.UrlIcono,
            nombre: body.Nombre,
            categoria: (Categoria)body.Categoria,
            concurrencyToken: body.ConcurrencyToken,
            modificationUser: this.GetAuthenticatedUserGuid());

        var response = _mapper.Map<ProveedorResult>(source: proveedor);
        return Ok(value: response);
    }
}
