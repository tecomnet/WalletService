using System;
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
        public override async Task<IActionResult> PutConfirmarAsync(ConfirmacionRequest body, string version,
            int? idUsuario)
        {
            bool result = false;

            switch (body.Tipo)
            {
                case Tipo2FAEnum.SMSEnum:
                    result = await registroFacade.ConfirmarNumeroAsync(idUsuario.Value, body.Codigo);
                    break;
                case Tipo2FAEnum.EMAILEnum:
                    result = await registroFacade.VerificarCorreoAsync(idUsuario.Value, body.Codigo);
                    break;
                default:
                    return BadRequest("Tipo de verificación no soportado.");
            }

            return Ok(result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostAceptarTerminosAsync(string version, int? idUsuario,
            AceptarTerminosRequest body)
        {
            var usuario =
                await registroFacade.AceptarTerminosCondicionesAsync(idUsuario.Value, body.Version,
                    body.AceptoTerminos.Value,
                    body.AceptoPrivacidad.Value, body.AceptoPld.Value);
            return Created($"/{version}/usuario/{usuario.Id}", mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutCompletarRegistroAsync(string version, int? idUsuario,
            CompletarRegistroRequest body)
        {
            var usuario = await registroFacade.CompletarRegistroAsync(idUsuario.Value, body.Contrasena,
                body.ConfirmacionContrasena);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostRegistrarBiometricosAsync(string version, int? idUsuario,
            RegistrarBiometricosRequest body)
        {
            var usuario = await registroFacade.RegistrarDatosBiometricosAsync(idUsuario.Value, body.IdDispositivo,
                body.Token, body.Nombre, body.Caracteristicas);
            return Created($"/{version}/usuario/{usuario.Id}", mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutRegistrarCorreoAsync(string version, int? idUsuario,
            RegistrarCorreoRequest body)
        {
            var usuario = await registroFacade.RegistrarCorreoAsync(idUsuario.Value, body.Correo);
            return Ok(mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostPreRegistroAsync(string version, PreRegistroRequest body)
        {
            var usuario = await registroFacade.PreRegistroAsync(
                codigoPais: body.CodigoPais,
                telefono: body.Telefono);
            return Created($"/{version}/usuario/{usuario.Id}", mapper.Map<UsuarioResult>(usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostDatosClienteAsync(string version, int? idUsuario,
            DatosClienteRequest body)
        {
            var usuario = await registroFacade.CompletarDatosClienteAsync(
                idUsuario: idUsuario.Value,
                nombre: body.Nombre,
                apellidoPaterno: body.ApellidoPaterno,
                apellidoMaterno: body.ApellidoMaterno,
                nombreEstado: body.NombreEstado,
                fechaNacimiento: DateOnly.FromDateTime(body.FechaNacimiento.Value),
                genero: (DOM.Enums.Genero)body.Genero);
            return Created($"/{version}/usuario/{usuario.Id}", mapper.Map<UsuarioResult>(usuario));
        }
    }
}
