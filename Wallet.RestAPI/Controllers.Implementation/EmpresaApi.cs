using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        int? idEmpresa, EmpresaUpdateRequest body)
    {
        // Call facade method
        var empresa = await empresaFacade.ActualizaEmpresaAsync(idEmpresa: idEmpresa.Value, nombre: body.Nombre,
            concurrencyToken: body.ConcurrencyToken,
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
        AsignarProductosRequest body,
        string version, int? idEmpresa)
    {
        // Call facade method
        var empresa = await empresaFacade.AsignarProductosAsync(
            idEmpresa: idEmpresa.Value,
            idsProductos: body.ProductoIds?.Where(predicate: x => x.HasValue).Select(selector: x => x.Value).ToList() ??
                          new List<int>(),
            modificationUser: this.GetAuthenticatedUserGuid());

        // Map to response model
        var response = mapper.Map<EmpresaResult>(source: empresa);

        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> DesasignarProductosAsync(
        AsignarProductosRequest body,
        string version,
        int? idEmpresa)
    {
        // Call facade method
        var empresa = await empresaFacade.DesasignarProductosAsync(
            idEmpresa: idEmpresa.Value,
            idsProductos: body.ProductoIds?.Where(predicate: x => x.HasValue).Select(selector: x => x.Value).ToList() ??
                          new List<int>(),
            modificationUser: this.GetAuthenticatedUserGuid());

        // Map to response model
        var response = mapper.Map<EmpresaResult>(source: empresa);

        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetEmpresaAsync(string version, int? idEmpresa)
    {
        // Call facade method
        var empresa = await empresaFacade.ObtenerPorIdAsync(idEmpresa: idEmpresa.Value);
        // Map to response model
        var response = mapper.Map<EmpresaResult>(source: empresa);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteEmpresaAsync(string version, int? idEmpresa,
        string concurrencyToken)
    {
        // Call facade method
        var empresa =
            await empresaFacade.EliminaEmpresaAsync(idEmpresa: idEmpresa.Value, concurrencyToken: concurrencyToken,
                modificationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<EmpresaResult>(source: empresa);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutActivarEmpresaAsync(StatusChangeRequest body, string version,
        int? idEmpresa)
    {
        // Call facade method
        var empresa =
            await empresaFacade.ActivaEmpresaAsync(idEmpresa: idEmpresa.Value, concurrencyToken: body.ConcurrencyToken,
                modificationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<EmpresaResult>(source: empresa);
        // Return OK response
        return Ok(value: response);
    }
}
