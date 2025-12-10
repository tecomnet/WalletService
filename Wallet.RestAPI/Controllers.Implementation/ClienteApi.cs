using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;


namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the Cliente API controller.
/// </summary>
[Authorize]
public class ClienteApiController(IClienteFacade clienteFacade, IMapper mapper)
    : ClienteApiControllerBase
{
    // TODO EMD: PENDIENTE IMPLEMENTAR JWT PARA EL USUSARIO QUE REALIZA LA OPERACION
    public override async Task<IActionResult> DeleteClienteAsync(string version, int? idCliente)
    {
        // Call facade method
        var cliente =
            await clienteFacade.EliminarClienteAsync(idCliente: idCliente.Value,
                modificationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<ClienteResult>(source: cliente);
        // Return OK response
        return Ok(value: response);
    }

    public override async Task<IActionResult> GetClienteAsync(string version, int? idCliente)
    {
        // Call facade method
        var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente.Value);
        // Map to response model
        var response = mapper.Map<ClienteResult>(source: cliente);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    [Authorize]
    public override async Task<IActionResult> GetClientesAsync(
        string version)
    {
        // Call facade method
        var clientes = await clienteFacade.ObtenerClientesAsync();
        // Map to response model
        var response = mapper.Map<List<ClienteResult>>(source: clientes);
        // Return OK response
        return Ok(value: response);
    }

    public override async Task<IActionResult> GetServiciosFavoritosPorClienteAsync(string version, int? idCliente)
    {
        // Call facade method
        var serviciosFavoritos = await clienteFacade.ObtenerServiciosFavoritosAsync(idCliente: idCliente.Value);
        // Map to response model
        var response = mapper.Map<List<ServicioFavoritoResult>>(source: serviciosFavoritos);
        // Return OK response
        return Ok(value: response);
    }

    public override async Task<IActionResult> PutActivarClienteAsync(string version, int? idCliente)
    {
        // Call facade method
        var cliente = await clienteFacade.ActivarClienteAsync(idCliente: idCliente.Value,
            modificationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<ClienteResult>(source: cliente);
        // Return OK response
        return Ok(value: response);
    }
}


