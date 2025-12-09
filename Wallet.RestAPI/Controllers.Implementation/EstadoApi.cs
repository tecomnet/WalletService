using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the Estado API controller.
/// </summary>
public class EstadoApiController(IEstadoFacade estadoFacade, IMapper mapper) : EstadoApiControllerBase
{
    /// <inheritdoc/>
    public override async Task<IActionResult> GetEstadosAsync(
        string version)
    {
        var estados = await estadoFacade.ObtenerTodosAsync();
        var response = mapper.Map<List<EstadoResult>>(source: estados);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PostEstadoAsync(
        string version,
        EstadoRequest body)
    {
        var estado = await estadoFacade.GuardarEstadoAsync(nombre: body.Nombre, creationUser: Guid.Empty);
        var response = mapper.Map<EstadoResult>(source: estado);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutEstadoAsync(
        string version,
        int? idEstado, EstadoRequest body)
    {
        var estado = await estadoFacade.ActualizaEstadoAsync(idEstado: idEstado.Value, nombre: body.Nombre,
            modificationUser: Guid.Empty);
        var response = mapper.Map<EstadoResult>(source: estado);
        return Ok(value: response);
    }
}
