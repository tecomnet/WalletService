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
        IMapper mapper) : RegistroApiControllerBase
    {
        /// <inheritdoc />
        public override async Task<IActionResult> PutConfirmarAsync(ConfirmacionRequest body, string version, int? idUsuario)
        {
            var modificationUser = Guid.Empty;
            bool result = false;

            switch (body.Tipo)
            {
                case Tipo2FAEnum.SMSEnum:
                    result = await registroFacade.ConfirmarNumeroAsync(idUsuario.Value, body.Codigo, modificationUser);
                    break;
                case Tipo2FAEnum.EMAILEnum:
                    result = await registroFacade.VerificarCorreoAsync(idUsuario.Value, body.Codigo, modificationUser);
                    break;
                default:
                    return BadRequest("Tipo de verificación no soportado.");
            }

            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostAceptarTerminosAsync(string version, int? idUsuario, AceptarTerminosRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario =
                await registroFacade.AceptarTerminosCondicionesAsync(idUsuario.Value, body.Version, body.AceptoTerminos,
                    body.AceptoPrivacidad, body.AceptoPld, modificationUser);
            return Created($"/{version}/usuario/{usuario.Id}", mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutCompletarRegistroAsync(string version, int? idUsuario, CompletarRegistroRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.CompletarRegistroAsync(idUsuario.Value, body.Contrasena,
                body.ConfirmacionContrasena, modificationUser);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostRegistrarBiometricosAsync(string version, int? idUsuario, RegistrarBiometricosRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.RegistrarDatosBiometricosAsync(idUsuario.Value, body.IdDispositivo,
                body.Token, body.Nombre, body.Caracteristicas, modificationUser);
            return Created($"/{version}/usuario/{usuario.Id}", mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutRegistrarCorreoAsync(string version, int? idUsuario, RegistrarCorreoRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.RegistrarCorreoAsync(idUsuario.Value, body.Correo, modificationUser);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostPreRegistroAsync(string version, PreRegistroRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario =
                await registroFacade.PreRegistroAsync(body.CodigoPais, body.Telefono, modificationUser);
            return Created($"/{version}/usuario/{usuario.Id}", mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostDatosClienteAsync(string version, int? idUsuario, DatosClienteRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.CompletarDatosClienteAsync(
                idUsuario.Value,
                body.Nombre,
                body.ApellidoPaterno,
                body.ApellidoMaterno,
                body.NombreEstado,
                DateOnly.FromDateTime(body.FechaNacimiento),
                (DOM.Enums.Genero)body.Genero,
                modificationUser);
            return Created($"/{version}/usuario/{usuario.Id}", mapper.Map<UsuarioResult>(usuario));
        }
    }
}
