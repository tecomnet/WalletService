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
                case Tipo2FAEnum.SMS:
                    result = await registroFacade.ConfirmarNumeroAsync(idUsuario: idUsuario.Value, codigo: body.Codigo);
                    break;
                case Tipo2FAEnum.EMAIL:
                    result = await registroFacade.VerificarCorreoAsync(idUsuario: idUsuario.Value, codigo: body.Codigo);
                    break;
                default:
                    return BadRequest(error: "Tipo de verificación no soportado.");
            }

            return Ok(value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostAceptarTerminosAsync(string version, int? idUsuario,
            AceptarTerminosRequest body)
        {
            var usuario =
                await registroFacade.AceptarTerminosCondicionesAsync(idUsuario: idUsuario.Value, version: body.Version,
                    aceptoTerminos: body.AceptoTerminos.Value,
                    aceptoPrivacidad: body.AceptoPrivacidad.Value, aceptoPld: body.AceptoPld.Value);
            return Created(uri: $"/{version}/usuario/{usuario.Id}", value: mapper.Map<UsuarioResult>(source: usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutCompletarRegistroAsync(string version, int? idUsuario,
            CompletarRegistroRequest body)
        {
            var usuario = await registroFacade.CompletarRegistroAsync(idUsuario: idUsuario.Value,
                contrasena: body.Contrasena,
                confirmacionContrasena: body.ConfirmacionContrasena);
            return Ok(value: mapper.Map<UsuarioResult>(source: usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostRegistrarBiometricosAsync(string version, int? idUsuario,
            RegistrarBiometricosRequest body)
        {
            var usuario = await registroFacade.RegistrarDatosBiometricosAsync(idUsuario: idUsuario.Value,
                idDispositivo: body.IdDispositivo,
                token: body.Token, nombre: body.Nombre, caracteristicas: body.Caracteristicas);
            return Created(uri: $"/{version}/usuario/{usuario.Id}", value: mapper.Map<UsuarioResult>(source: usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PutRegistrarCorreoAsync(string version, int? idUsuario,
            RegistrarCorreoRequest body)
        {
            var usuario =
                await registroFacade.RegistrarCorreoAsync(idUsuario: idUsuario.Value, correo: body.Correo);
            return Ok(value: mapper.Map<UsuarioResult>(source: usuario));
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostPreRegistroAsync(string version, PreRegistroRequest body)
        {
            var usuario = await registroFacade.PreRegistroAsync(
                codigoPais: body.CodigoPais,
                telefono: body.Telefono);
            return Created(uri: $"/{version}/usuario/{usuario.Id}", value: mapper.Map<UsuarioResult>(source: usuario));
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
                fechaNacimiento: DateOnly.FromDateTime(dateTime: body.FechaNacimiento.Value),
                genero: (DOM.Enums.Genero)body.Genero);

            return Created(uri: $"/{version}/usuario/{usuario.Id}", value: mapper.Map<UsuarioResult>(source: usuario));
        }
    }
}
