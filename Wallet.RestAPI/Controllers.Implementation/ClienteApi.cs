using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;


namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the Cliente API controller.
/// </summary>
public class ClienteApiController(IClienteFacade clienteFacade, IMapper mapper)
    : ClienteApiControllerBase
{
    // TODO EMD: PENDIENTE IMPLEMENTAR JWT PARA EL USUSARIO QUE REALIZA LA OPERACION


    /// <inheritdoc/>
    [Authorize]
    public override async Task<IActionResult> GetClienteAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idCliente)
    {
        // Call facade method
        var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);
        // Map to response model
        var response = mapper.Map<ClienteResult>(source: cliente);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    [Authorize]
    public override async Task<IActionResult> GetClientesAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version)
    {
        // Call facade method
        var clientes = await clienteFacade.ObtenerClientesAsync();
        // Map to response model
        var response = mapper.Map<List<ClienteResult>>(source: clientes);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    [Authorize]
    public override async Task<IActionResult> DeleteClienteAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idCliente)
    {
        // Call facade method
        var cliente = await clienteFacade.EliminarClienteAsync(idCliente: idCliente, modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ClienteResult>(source: cliente);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    [Authorize]
    public override async Task<IActionResult> PutActivarClienteAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idCliente)
    {
        // Call facade method
        var cliente = await clienteFacade.ActivarClienteAsync(idCliente: idCliente, modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ClienteResult>(source: cliente);
        // Return OK response
        return Ok(value: response);
    }
}


