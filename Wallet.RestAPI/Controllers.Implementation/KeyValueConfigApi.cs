using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Funcionalidad.Functionality.KeyValueConfigFacade;
using Wallet.RestAPI.Models;
using Wallet.RestAPI.Helpers;


namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the KeyValueConfig API controller.
/// </summary>
//[Authorize]
public class KeyValueConfigApiController(IKeyValueConfigFacade keyValueConfigFacade, IMapper mapper)
    : KeyValueConfigApiControllerBase
{
    /// <inheritdoc/>
    public override async Task<IActionResult> GetConfigurationsAsync(string version)
    {
        // Call facade method
        var configs = await keyValueConfigFacade.ObtenerTodasLasConfiguracionesAsync();
        // Map to response model
        var response = mapper.Map<List<KeyValueConfigResult>>(source: configs);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> CreateKeyValueConfigAsync(KeyValueConfigRequest body, string version)
    {
        var result =
            await keyValueConfigFacade.GuardarKeyValueConfigAsync(key: body.Key, value: body.Value,
                creationUser: this.GetAuthenticatedUserGuid());
        var response = mapper.Map<KeyValueConfigResult>(source: result);
        return Created(uri: $"/{version}/configurations/{response.Key}", value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteKeyValueConfigAsync(string version, string key)
    {
        var result =
            await keyValueConfigFacade.EliminarKeyValueConfigAsync(key: key,
                modificationUser: this.GetAuthenticatedUserGuid());
        var response = mapper.Map<KeyValueConfigResult>(source: result);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetKeyValueConfigByKeyAsync(string version, string key)
    {
        var result = await keyValueConfigFacade.ObtenerKeyValueConfigPorKeyAsync(key: key);
        var response = mapper.Map<KeyValueConfigResult>(source: result);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> UpdateKeyValueConfigAsync(KeyValueConfigUpdateRequest body,
        string version,
        string key)
    {
        var result =
            await keyValueConfigFacade.ActualizarKeyValueConfigAsync(key: key, value: body.Value,
                concurrencyToken: body.ConcurrencyToken,
                modificationUser: this.GetAuthenticatedUserGuid());
        var response = mapper.Map<KeyValueConfigResult>(source: result);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> ActivateKeyValueConfigAsync(string version, string key)
    {
        var result = await keyValueConfigFacade.ActivarKeyValueConfigAsync(key: key,
            modificationUser: this.GetAuthenticatedUserGuid());
        var response = mapper.Map<KeyValueConfigResult>(source: result);
        return Ok(value: response);
    }
}
