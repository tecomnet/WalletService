using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the Direccion API controller.
/// </summary>
//[Authorize]
public class DireccionApiController(IDireccionFacade direccionFacade, IMapper mapper) : DireccionApiControllerBase
{
    /// <inheritdoc />
    public override async Task<IActionResult> GetDireccionAsync(string version, int? idCliente)
    {
        // Call facade method
        var direccion = await direccionFacade.ObtenerDireccionPorClienteIdAsync(idCliente: idCliente.Value);
        // Map to response model
        var response = mapper.Map<DireccionResult>(source: direccion);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> PutDireccionAsync(string version, int? idCliente,
        DireccionUpdateRequest body)
    {
        // Call facade method
        var direccion = await direccionFacade.ActualizarDireccionCliente(
            idCliente: idCliente.Value,
            codigoPostal: body.CodigoPostal,
            municipio: body.Municipio,
            colonia: body.Colonia,
            calle: body.Calle,
            numeroExterior: body.NumeroExterior,
            numeroInterior: body.NumeroInterior,
            referencia: body.Referencia,
            concurrencyToken: body.ConcurrencyToken,
            modificationUser: this.GetAuthenticatedUserGuid());
        // Map to response model
        var response = mapper.Map<DireccionResult>(source: direccion);
        // Return OK response
        return Ok(value: response);
    }
}
