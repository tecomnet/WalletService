using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the Empresa API controller.
/// </summary>
//[Authorize]
public class EmpresaApiController(IEmpresaFacade empresaFacade, IMapper mapper) : EmpresaApiControllerBase
{
    /// <inheritdoc/>
    public override async Task<IActionResult> GetEmpresasAsync(
        string version)
    {
        // Call facade method
        var empresas = await empresaFacade.ObtenerTodasAsync();
        // Map to response model
        var response = mapper.Map<List<EmpresaResult>>(source: empresas);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PostEmpresaAsync(
        string version,
        EmpresaRequest body)
    {
        // Call facade method
        var empresa =
            await empresaFacade.GuardarEmpresaAsync(nombre: body.Nombre, creationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<EmpresaResult>(source: empresa);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutEmpresaAsync(
        string version,
        int? idEmpresa, EmpresaRequest body)
    {
        // Call facade method
        var empresa = await empresaFacade.ActualizaEmpresaAsync(idEmpresa: idEmpresa.Value, nombre: body.Nombre,
            modificationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<EmpresaResult>(source: empresa);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetProductosPorEmpresaAsync(
        string version,
        int? idEmpresa)
    {
        // Call facade method
        var productos = await empresaFacade.ObtenerProductosPorEmpresaAsync(idEmpresa: idEmpresa.Value);
        // Map to response model
        var response = mapper.Map<List<ProductoResult>>(source: productos);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetClientesPorEmpresaAsync(
        string version,
        int? idEmpresa)
    {
        // Call facade method
        var clientes = await empresaFacade.ObtenerClientesPorEmpresaAsync(idEmpresa: idEmpresa.Value);
        // Map to response model
        var response = mapper.Map<List<ClienteResult>>(source: clientes);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> AsignarProductosAsync(
        string version,
        [FromRoute] int idEmpresa,
        [FromBody] AsignarProductosRequest body)
    {
        // Call facade method
        var empresa = await empresaFacade.AsignarProductosAsync(
            idEmpresa: idEmpresa,
            idsProductos: body.ProductoIds,
            modificationUser: this.GetAuthenticatedUserGuid());

        // Map to response model
        var response = mapper.Map<EmpresaResult>(source: empresa);

        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> DesasignarProductosAsync(
        string version,
        [FromRoute] int idEmpresa,
        [FromBody] AsignarProductosRequest body)
    {
        // Call facade method
        var empresa = await empresaFacade.DesasignarProductosAsync(
            idEmpresa: idEmpresa,
            idsProductos: body.ProductoIds,
            modificationUser: this.GetAuthenticatedUserGuid());

        // Map to response model
        var response = mapper.Map<EmpresaResult>(source: empresa);

        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetEmpresaAsync(string version, int idEmpresa)
    {
        // Call facade method
        var empresa = await empresaFacade.ObtenerPorIdAsync(idEmpresa: idEmpresa);
        // Map to response model
        var response = mapper.Map<EmpresaResult>(source: empresa);
        // Return OK response
        return Ok(value: response);
    }
}
