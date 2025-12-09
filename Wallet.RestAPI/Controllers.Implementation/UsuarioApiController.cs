using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
{
    /// <summary>
    /// Implementation of the Usuario API controller.
    /// </summary>
    public class UsuarioApiController(IUsuarioFacade usuarioFacade, IMapper mapper) : UsuarioApiControllerBase
    {
        /// <inheritdoc />
        public override async Task<IActionResult> GetUsuarioAsync(string version, int? idUsuario)
        {
            var usuario = await usuarioFacade.ObtenerUsuarioPorIdAsync(idUsuario: idUsuario.Value);
            var result = mapper.Map<UsuarioResult>(source: usuario);
            return Ok(value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutUsuarioContrasenaAsync(string version, int? idUsuario, ContrasenaUpdateRequest body)
        {
            var usuario = await usuarioFacade.ActualizarContrasenaAsync(idUsuario: idUsuario.Value,
                contrasenaActual: body.ContrasenaActual,
                contrasenaNueva: body.ContrasenaNueva, confirmacionContrasenaNueva: body.ContrasenaNuevaConfrimacion,
                modificationUser: Guid.Empty);
            var result = mapper.Map<UsuarioResult>(source: usuario);
            return Ok(value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutUsuarioEmailAsync(string version, int? idUsuario, EmailUpdateRequest body)
        {
            var usuario = await usuarioFacade.ActualizarCorreoElectronicoAsync(idUsuario: idUsuario.Value,
                correoElectronico: body.CorreoElectronico,
                modificationUser: Guid.Empty);
            var result = mapper.Map<UsuarioResult>(source: usuario);
            return Ok(value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutUsuarioTelefonoAsync(string version, int? idUsuario, TelefonoUpdateRequest body)
        {
            var usuario = await usuarioFacade.ActualizarTelefonoAsync(idUsuario: idUsuario.Value, codigoPais: body.CodigoPais,
                telefono: body.Telefono,
                modificationUser: Guid.Empty);
            var result = mapper.Map<UsuarioResult>(source: usuario);
            return Ok(value: result);
        }
    }
}
