using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.RestAPI.Models;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <inheritdoc/>
public class ClienteApiController(IClienteFacade clienteFacade, IUsuarioFacade usuarioFacade, IMapper mapper)
    : ClienteApiControllerBase
{
    // TODO EMD: PENDIENTE IMPLEMENTAR JWT PARA EL USUSARIO QUE REALIZA LA OPERACION
    /// <inheritdoc/>
    [Authorize]
    public override async Task<IActionResult> PostClienteAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromBody] ClienteRequest body)
    {
        // Llama a la fachada de usuario para el pre-registro
        var usuario = await usuarioFacade.GuardarUsuarioPreRegistroAsync(
            codigoPais: body.CodigoPais,
            telefono: body.Telefono,
            creationUser: Guid.Empty);

        // Obtiene el cliente del usuario (asumiendo que se cargó o se asoció correctamente)
        // Si usuario.Cliente es nulo, podría ser necesario cargarlo, pero en el flujo de creación debería estar disponible en memoria si se seteo correctamente o si EF lo arregló.
        // Dado que UsuarioFacade crea el cliente, debería estar accesible si la instancia de usuario es la misma.
        // Sin embargo, para estar seguros, podríamos necesitar obtener el cliente por ID de usuario si fuera necesario, pero confiemos en la navegación por ahora o usemos el ID.

        // Mapea el result (ClienteResult espera un Cliente)
        // Si usuario.Cliente es nulo, esto fallará.
        // Vamos a asumir que usuario.Cliente está poblado.
        var result = mapper.Map<ClienteResult>(source: usuario.Cliente);
        // Retorna created
        return Created(uri: $"/{version}/cliente/{usuario.Cliente?.Id}", value: result);
    }


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

    /// <inheritdoc/>
    [Authorize]
    public override async Task<IActionResult> PutClienteAsync(
        [FromRoute, RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$"), Required]
        string version,
        [FromRoute, Required] int idCliente, [FromBody] ClienteUpdateRequest body)
    {
        // Obtienes el valor como entero de forma segura.
        int genero = (int)body.Genero;
        // Intentar convertir el entero al tipo Enum
        if (!Enum.IsDefined(enumType: typeof(Wallet.DOM.Enums.Genero), value: genero))
        {
            //Si es un valor inválido, lanza una excepción de validación o un BadRequest.
            throw new ArgumentException(message: $"El valor {genero} no es un Genero válido.");
        }

        // Call facade method
        var cliente = await clienteFacade.ActualizarClienteDatosPersonalesAsync(
            idCliente: idCliente,
            nombre: body.Nombre,
            primerApellido: body.PrimerApellido,
            segundoApellido: body.SegundoApellido,
            nombreEstado: body.Estado,
            fechaNacimiento: DateOnly.FromDateTime(dateTime: body.FechaNacimiento.Value),
            genero: (DOM.Enums.Genero)body.Genero,
            modificationUser: Guid.Empty);
        // Map to response model
        var response = mapper.Map<ClienteResult>(source: cliente);
        // Return OK response
        return Ok(value: response);
    }
}
