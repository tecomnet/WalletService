using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
{
    public class UsuarioApiController(IUsuarioFacade usuarioFacade, IMapper mapper) : UsuarioApiControllerBase
    {
        public override async Task<IActionResult> GetUsuarioAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idUsuario)
        {
            var usuario = await usuarioFacade.ObtenerUsuarioPorIdAsync(idUsuario);
            var result = mapper.Map<UsuarioResult>(usuario);
            return Ok(result);
        }

        public override async Task<IActionResult> PostUsuarioContrasenaAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idUsuario, [FromBody] ContrasenaRequest body)
        {
            var usuario = await usuarioFacade.GuardarContrasenaAsync(idUsuario, body.Contrasena, Guid.Empty);
            return Created("", true);
        }

        public override async Task<IActionResult> PutUsuarioConfirmaVerificacionAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idUsuario, [FromBody] Verificacion2FARequest body)
        {
            var result = await usuarioFacade.ConfirmarCodigoVerificacion2FAAsync(idUsuario,
                (Tipo2FA)body.Tipo, body.Codigo, Guid.Empty);
            return Ok(result);
        }

        public override async Task<IActionResult> PutUsuarioContrasenaAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idUsuario, [FromBody] ContrasenaUpdateRequest body)
        {
            var usuario = await usuarioFacade.ActualizarContrasenaAsync(idUsuario, body.ContrasenaActual,
                body.ContrasenaNueva, body.ContrasenaNuevaConfrimacion, Guid.Empty);
            return Ok(true);
        }

        public override async Task<IActionResult> PutUsuarioEmailAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idUsuario, [FromBody] EmailUpdateRequest body)
        {
            var usuario = await usuarioFacade.ActualizarCorreoElectronicoAsync(idUsuario, body.CorreoElectronico,
                Guid.Empty);
            var result = mapper.Map<UsuarioResult>(usuario);
            return Ok(result);
        }

        public override async Task<IActionResult> PutUsuarioTelefonoAsync(
            [FromRoute] [Required] [RegularExpression("^(?<major>[0-9]+).(?<minor>[0-9]+)$")] string version,
            [FromRoute] [Required] int idUsuario, [FromBody] TelefonoUpdateRequest body)
        {
            var usuario = await usuarioFacade.ActualizarTelefonoAsync(idUsuario, body.CodigoPais, body.Telefono,
                Guid.Empty);
            var result = mapper.Map<UsuarioResult>(usuario);
            return Ok(result);
        }
    }
}
