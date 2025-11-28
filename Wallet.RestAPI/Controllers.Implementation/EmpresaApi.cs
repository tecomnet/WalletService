using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <inheritdoc/>
public class EmpresaApiController(IEmpresaFacade empresaFacade, IMapper mapper) : EmpresaApiControllerBase
{
    /// <inheritdoc/>
    public override async Task<IActionResult> GetEmpresasAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version)
    {
        // Call facade method
        var empresas = await empresaFacade.ObtenerTodasAsync();
        // Map to response model
        var response = mapper.Map<List<EmpresaResult>>(empresas);
        // Return OK response
        return Ok(response);    
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PostEmpresaAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromBody] EmpresaRequest body)
    {
        // Call facade method
        var empresa = await empresaFacade.GuardarEmpresaAsync(nombre: body.Nombre, creationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<EmpresaResult>(empresa);
        // Return OK response
        return Ok(response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutEmpresaAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int? idEmpresa, [FromBody] EmpresaRequest body)
    {
        // Call facade method
        var empresa = await empresaFacade.ActualizaEmpresaAsync(idEmpresa: idEmpresa.Value, nombre: body.Nombre, modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<EmpresaResult>(empresa);
        // Return OK response
        return Ok(response);
    }

}
