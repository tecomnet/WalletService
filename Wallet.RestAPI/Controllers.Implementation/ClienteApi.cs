using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Helpers;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;
/// <inheritdoc/>
public class ClienteApiController(IClienteFacade clienteFacade, IMapper mapper) : ClienteApiControllerBase
{
    // TODO EMD: PENDIENTE IMPLEMENTAR JWT PARA EL USUSARIO QUE REALIZA LA OPERACION
   /// <inheritdoc/>
    public override async Task<IActionResult> PostClienteAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromBody] ClienteRequest body)
    {
        // Llama a la fachada
        var cliente = await clienteFacade.GuardarClientePreRegistroAsync(
            codigoPais: body.CodigoPais,
            telefono: body.Telefono,
            creationUser: Guid.Empty);
        // Mapea el result
        var result = mapper.Map<ClienteResult>(cliente);
        // Retorna created
        return Created(uri: $"/{version}/cliente/{cliente.Id}", result);        
    }


    /// <inheritdoc/>
    public override async Task<IActionResult> GetClienteAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente)
    {
        // Call facade method
        var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);
        // Map to response model
        var response = mapper.Map<ClienteResult>(cliente);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetClientesAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version)
    {
        // Call facade method
        var clientes = await clienteFacade.ObtenerClientesAsync();
        // Map to response model
        var response = mapper.Map<ClienteResult[]>(clientes);
        // Return OK response
        return Ok(value: response);
    }
    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteClienteAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente)
    {
        // Call facade method
        var cliente = await clienteFacade.EliminarClienteAsync(idCliente: idCliente, modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ClienteResult>(cliente);
        // Return OK response
        return Ok(value: response);
    }
    /// <inheritdoc/>
    public override async Task<IActionResult> PutActivarClienteAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente)
    {
        // Call facade method
        var cliente = await clienteFacade.ActivarClienteAsync(idCliente: idCliente, modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ClienteResult>(cliente);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutClienteAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] ClienteUpdateRequest body)
    {
        // Obtienes el valor como entero de forma segura.
        int genero = (int)body.Genero;
        // Intentar convertir el entero al tipo Enum
        if (!Enum.IsDefined(typeof(Wallet.DOM.Enums.Genero), genero))
        {
            //Si es un valor inválido, lanza una excepción de validación o un BadRequest.
            throw new ArgumentException($"El valor {genero} no es un Genero válido.");
        }
        // Call facade method
        var cliente = await clienteFacade.ActualizarClienteDatosPersonalesAsync(
            idCliente: idCliente,
            nombre: body.Nombre,
            primerApellido: body.PrimerApellido,
            segundoApellido: body.SegundoApellido,
            nombreEstado: body.Estado,
            fechaNacimiento: DateOnly.FromDateTime(body.FechaNacimiento.Value),
            genero: (DOM.Enums.Genero)body.Genero,
            correoElectronico: body.CorreoElectronico,
            modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ClienteResult>(cliente);
        // Return OK response
        return Ok(value: response);
    }
 
    /// <inheritdoc/>
    public override async Task<IActionResult> PostContrasenaAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] ContrasenaRequest body)
    {
        // Call facade method
        var cliente = await clienteFacade.GuardarContrasenaAsync(
            idCliente: idCliente,
            contrasena: body.Contrasena,
            modificationUser: Guid.Empty);
        // Map to response model
        //var response = mapper.Map<ClienteResult>(cliente);
        // Return OK response
        return Ok();
    }
    /// <inheritdoc/>
    public override async Task<IActionResult> PutContrasenaAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] ContrasenaUpdateRequest body)
    {
        // Call facade method
        var cliente = await clienteFacade.ActualizarContrasenaAsync(
            idCliente: idCliente,
            contrasenaActual: body.ContrasenaActual,
            contrasenaNueva: body.ContrasenaNueva,
            confirmacionContrasenaNueva: body.ContrasenaNuevaConfrimacion,
            modificationUser: Guid.Empty);
        // Map to response model
        //var response = mapper.Map<ClienteResult>(cliente);
        // Return OK response
        return Ok();
    }
 

    /// <inheritdoc/>
    public override async Task<IActionResult> PutConfirmaVerificacionAsync([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required] string version, [FromRoute, Required] int idCliente, [FromBody] Verificacion2FARequest body)
    {
        // Obtienes el valor como entero de forma segura.
        int tipo2Fa = (int)body.Tipo;
        // Intentar convertir el entero al tipo Enum
        if (!Enum.IsDefined(typeof(Wallet.DOM.Enums.Tipo2FA), tipo2Fa))
        {
            //Si es un valor inválido, lanza una excepción de validación o un BadRequest.
            throw new ArgumentException($"El valor {tipo2Fa} no es un tipo de 2FA válido.");
        }

        var cliente = await clienteFacade.ConfirmarCodigoVerificacion2FAAsync(
            idCliente: idCliente,
            tipo2FA: (DOM.Enums.Tipo2FA)body.Tipo,
            codigoVerificacion: body.Codigo,
            modificationUser: Guid.Empty);
        // Map to response model
        //var response = mapper.Map<ClienteResult>(cliente);
        // Return OK response
        return Ok();    
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
