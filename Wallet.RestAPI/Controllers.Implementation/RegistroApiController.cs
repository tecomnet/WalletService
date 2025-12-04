using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
        public override async Task<IActionResult> ConfirmarNumeroAsync(string version, ConfirmarNumeroRequest body)
        {
            // TODO: Obtener usuario real del contexto si aplicara, pero aquí es un proceso público/semi-público
            var modificationUser = Guid.Empty;
            var result = await registroFacade.ConfirmarNumeroAsync(body.IdUsuario, body.Codigo, modificationUser);
            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> CompletarDatosClienteAsync(string version, DatosClienteRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.CompletarDatosClienteAsync(
                body.IdUsuario,
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
        public override async Task<IActionResult> RegistrarCorreoAsync(string version, RegistrarCorreoRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.RegistrarCorreoAsync(body.IdUsuario, body.Correo, modificationUser);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> VerificarCorreoAsync(string version, VerificarCorreoRequest body)
        {
            var modificationUser = Guid.Empty;
            var result = await registroFacade.VerificarCorreoAsync(body.IdUsuario, body.Codigo, modificationUser);
            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> RegistrarBiometricosAsync(string version,
            RegistrarBiometricosRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.RegistrarDatosBiometricosAsync(body.IdUsuario, body.IdDispositivo,
                body.Token, modificationUser);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> AceptarTerminosAsync(string version, AceptarTerminosRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario =
                await registroFacade.AceptarTerminosCondicionesAsync(body.IdUsuario, body.Version, body.AceptoTerminos,
                    body.AceptoPrivacidad, body.AceptoPld, modificationUser);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> CompletarRegistroAsync(string version, CompletarRegistroRequest body)
        {
            var modificationUser = Guid.Empty;
            var usuario = await registroFacade.CompletarRegistroAsync(body.IdUsuario, body.Contrasena,
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
