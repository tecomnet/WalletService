using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the Direccion API controller.
/// </summary>
public class DireccionApiController(IDireccionFacade direccionFacade, IMapper mapper) : DireccionApiControllerBase
{
    /// <inheritdoc/>
    public override async Task<IActionResult> PutDireccionAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version,
        [FromRoute, Required] int idCliente, [FromBody] DireccionUpdateRequest body)
    {
        // Call facade method
        var direccion = await direccionFacade.ActualizarDireccionCliente(
            idCliente: idCliente,
            codigoPostal: body.CodigoPostal,
            municipio: body.Municipio,
            colonia: body.Colonia,
            calle: body.Calle,
            numeroExterior: body.NumeroExterior,
            numeroInterior: body.NumeroInterior,
            referencia: body.Referencia,
            modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<DireccionResult>(source: direccion);
        // Return OK response
        return Ok(value: response);
    }
}
