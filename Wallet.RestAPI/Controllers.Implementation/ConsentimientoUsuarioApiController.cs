using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.ConsentimientosUsuarioFacade;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Controllers.Implementation
{
    /// <summary>
    /// Implementation of the ConsentimientoUsuario API controller.
    /// </summary>
    //[Authorize]
    public class ConsentimientoUsuarioApiController(
        IConsentimientosUsuarioFacade consentimientosUsuarioFacade,
        IMapper mapper)
        : ConsentimientoUsuarioApiControllerBase
    {
        /// <inheritdoc />
        public override async Task<IActionResult> GetConsentimientosUsuarioAsync(string version, int? idUsuario)
        {
            var consentimientos =
                await consentimientosUsuarioFacade.ObtenerUltimosConsentimientosAsync(idUsuario: idUsuario.Value);
            var result = mapper.Map<List<ConsentimientoUsuarioResult>>(source: consentimientos);
            return Ok(value: result);
        }

        /// <inheritdoc />
        public override async Task<IActionResult> PostConsentimientoUsuarioAsync(string version, int? idUsuario,
            ConsentimientoUsuarioRequest body)
        {
            var consentimiento = await consentimientosUsuarioFacade.GuardarConsentimientoAsync(
                idUsuario: idUsuario.Value,
                tipoDocumento: (TipoDocumentoConsentimiento)body.TipoDocumento!,
                version: body.Version,
                creationUser: this
                    .GetAuthenticatedUserGuid()); // Assuming creationUser is handled internally or passed via context

            var result = mapper.Map<ConsentimientoUsuarioResult>(source: consentimiento);
            return Created(uri: "", value: result);
        }
    }
}
