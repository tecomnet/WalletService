using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;
/// <inheritdoc/>
public class ClienteApiController(IClienteFacade facade, IMapper mapper) : ClienteApiControllerBase
{
    // TODO EMD: PENDIENTE IMPLEMENTAR JWT PARA EL USUSARIO QUE REALIZA LA OPERACION
    /// <inheritdoc/>

    public override async Task<IActionResult> DeleteClienteAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente)
    {
        // Call facade method
        var cliente = await facade.EliminarClienteAsync(idCliente: idCliente, modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ClienteResult>(cliente);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override Task<IActionResult> GetClienteAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<IActionResult> GetClientesAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<IActionResult> PostClienteAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromBody] ClienteRequest body)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<IActionResult> PostContrasenaAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] ContrasenaRequest body)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<IActionResult> PutActivarClienteAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<IActionResult> PutClienteAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] ClienteUpdateRequest body)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<IActionResult> PutConfirmaVerificacionAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] Verificacion2FARequest body)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<IActionResult> PutContrasenaAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] ContrasenaUpdateRequest body)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<IActionResult> PutEmailAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] EmailUpdateRequest body)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public override Task<IActionResult> PutTelefonoAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] TelefonoUpdateRequest body)
    {
        throw new System.NotImplementedException();
    }

}
