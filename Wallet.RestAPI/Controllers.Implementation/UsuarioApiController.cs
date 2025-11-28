using System;
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
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario)
        {
            var usuario = await usuarioFacade.ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);
            var result = mapper.Map<UsuarioResult>(source: usuario);
            return Ok(value: result);
        }

        public override async Task<IActionResult> PostUsuarioContrasenaAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario, [FromBody] ContrasenaRequest body)
        {
            var usuario = await usuarioFacade.GuardarContrasenaAsync(idUsuario: idUsuario, contrasena: body.Contrasena,
                modificationUser: Guid.Empty);
            var result = mapper.Map<UsuarioResult>(source: usuario);
            return Created(uri: "", value: result);
        }

        public override async Task<IActionResult> PutUsuarioConfirmaVerificacionAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario, [FromBody] Verificacion2FARequest body)
        {
            var result = await usuarioFacade.ConfirmarCodigoVerificacion2FAAsync(idUsuario: idUsuario,
                tipo2FA: (Tipo2FA)body.Tipo, codigoVerificacion: body.Codigo, modificationUser: Guid.Empty);
            return Ok(value: result);
        }

        public override async Task<IActionResult> PutUsuarioContrasenaAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario, [FromBody] ContrasenaUpdateRequest body)
        {
            var usuario = await usuarioFacade.ActualizarContrasenaAsync(idUsuario: idUsuario,
                contrasenaActual: body.ContrasenaActual,
                contrasenaNueva: body.ContrasenaNueva, confirmacionContrasenaNueva: body.ContrasenaNuevaConfrimacion,
                modificationUser: Guid.Empty);
            var result = mapper.Map<UsuarioResult>(source: usuario);
            return Ok(value: result);
        }

        public override async Task<IActionResult> PutUsuarioEmailAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario, [FromBody] EmailUpdateRequest body)
        {
            var usuario = await usuarioFacade.ActualizarCorreoElectronicoAsync(idUsuario: idUsuario,
                correoElectronico: body.CorreoElectronico,
                modificationUser: Guid.Empty);
            var result = mapper.Map<UsuarioResult>(source: usuario);
            return Ok(value: result);
        }

        public override async Task<IActionResult> PutUsuarioTelefonoAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario, [FromBody] TelefonoUpdateRequest body)
        {
            var usuario = await usuarioFacade.ActualizarTelefonoAsync(idUsuario: idUsuario, codigoPais: body.CodigoPais,
                telefono: body.Telefono,
                modificationUser: Guid.Empty);
            var result = mapper.Map<UsuarioResult>(source: usuario);
            return Ok(value: result);
        }

        public override async Task<IActionResult> PostUsuarioPreRegistroAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromBody] PreRegistroRequest body)
        {
            var usuario = await usuarioFacade.GuardarUsuarioPreRegistroAsync(codigoPais: body.CodigoPais,
                telefono: body.Telefono,
                creationUser: Guid.Empty);
            var result = mapper.Map<UsuarioResult>(source: usuario);
            return Created(uri: "", value: result);
        }
    }
}
