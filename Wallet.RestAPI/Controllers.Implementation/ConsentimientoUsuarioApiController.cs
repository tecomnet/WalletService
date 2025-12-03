using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.ConsentimientosUsuarioFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation
{
    /// <summary>
    /// Implementation of the ConsentimientoUsuario API controller.
    /// </summary>
    public class ConsentimientoUsuarioApiController(
        IConsentimientosUsuarioFacade consentimientosUsuarioFacade,
        IMapper mapper)
        : ConsentimientoUsuarioApi
    {
        /// <inheritdoc/>
        public override async Task<IActionResult> PostConsentimientoUsuarioAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario, [FromBody] ConsentimientoUsuarioRequest body)
        {
            var consentimiento = await consentimientosUsuarioFacade.GuardarConsentimientoAsync(
                idUsuario: idUsuario,
                tipoDocumento: (TipoDocumentoConsentimiento)body.TipoDocumento!,
                version: body.Version,
                creationUser: Guid.Empty); // Assuming creationUser is handled internally or passed via context

            var result = mapper.Map<ConsentimientoUsuarioResult>(source: consentimiento);
            return Created(uri: "", value: result);
        }

        /// <inheritdoc/>
        public override async Task<IActionResult> GetConsentimientosUsuarioAsync(
            [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
            string version,
            [FromRoute] [Required] int idUsuario)
        {
            var consentimientos = await consentimientosUsuarioFacade.ObtenerUltimosConsentimientosAsync(idUsuario: idUsuario);
            var result = mapper.Map<List<ConsentimientoUsuarioResult>>(source: consentimientos);
            return Ok(value: result);
        }
    }
}
