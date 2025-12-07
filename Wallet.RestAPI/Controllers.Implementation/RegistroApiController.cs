using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.RegistroFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
{
    /// <inheritdoc />
    public class RegistroApiController(
        IRegistroFacade registroFacade,
        IMapper mapper) : RegistroApi
    {
        /// <inheritdoc />
        public override async Task<IActionResult> ConfirmarAsync(string version, int idUsuario,
            ConfirmacionRequest body)
        {
            var modificationUser = Guid.Empty;
            bool result = false;

            switch (body.Tipo)
            {
                case Tipo2FAEnum.SMSEnum:
                    result = await registroFacade.ConfirmarNumeroAsync(idUsuario, body.Codigo, modificationUser);
                    break;
                case Tipo2FAEnum.EMAILEnum:
                    result = await registroFacade.VerificarCorreoAsync(idUsuario, body.Codigo, modificationUser);
                    break;
                default:
                    return BadRequest("Tipo de verificación no soportado.");
            }

            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> CompletarDatosClienteAsync(string version, int idUsuario,
            DatosClienteRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.CompletarDatosClienteAsync(
                idUsuario,
                body.Nombre,
                body.ApellidoPaterno,
                body.ApellidoMaterno,
                body.NombreEstado,
                DateOnly.FromDateTime(body.FechaNacimiento),
                (DOM.Enums.Genero)body.Genero,
                modificationUser);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> RegistrarCorreoAsync(string version, int idUsuario,
            RegistrarCorreoRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.RegistrarCorreoAsync(idUsuario, body.Correo, modificationUser);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> RegistrarBiometricosAsync(string version, int idUsuario,
            RegistrarBiometricosRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.RegistrarDatosBiometricosAsync(idUsuario, body.IdDispositivo,
                body.Token, body.Nombre, body.Caracteristicas, modificationUser);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> AceptarTerminosAsync(string version, int idUsuario,
            AceptarTerminosRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario =
                await registroFacade.AceptarTerminosCondicionesAsync(idUsuario, body.Version, body.AceptoTerminos,
                    body.AceptoPrivacidad, body.AceptoPld, modificationUser);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> CompletarRegistroAsync(string version, int idUsuario,
            CompletarRegistroRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.CompletarRegistroAsync(idUsuario, body.Contrasena,
                body.ConfirmacionContrasena, modificationUser);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc/>
        public override async Task<IActionResult> PreRegistroAsync(
            [FromRoute] [Required] string version,
            [FromBody] PreRegistroRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario =
                await registroFacade.PreRegistroAsync(body.CodigoPais, body.Telefono, modificationUser);
            return Created($"/{version}/usuario/{usuario.Id}", mapper.Map<UsuarioResult>(usuario));
        }
    }
}
