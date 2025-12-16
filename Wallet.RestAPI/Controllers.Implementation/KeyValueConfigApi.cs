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
    public override async Task<IActionResult> GetAllKeyValueConfigsAsync(string version)
    {
        // Call facade method
        var configs = await keyValueConfigFacade.ObtenerTodasLasConfiguracionesAsync();
        // Map to response model
        var response = mapper.Map<List<KeyValueConfigResult>>(source: configs);
        // Return OK response
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> CreateKeyValueConfig(KeyValueConfigRequest body, string version)
    {
        var result =
            await keyValueConfigFacade.GuardarKeyValueConfigAsync(key: body.Key, value: body.Value,
                creationUser: this.GetAuthenticatedUserGuid());
        var response = mapper.Map<KeyValueConfigResult>(source: result);
        return Created(uri: $"/{version}/configurations/{response.Key}", value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteKeyValueConfig(string key, string version)
    {
        var result =
            await keyValueConfigFacade.EliminarKeyValueConfigAsync(key: key,
                modificationUser: this.GetAuthenticatedUserGuid());
        var response = mapper.Map<KeyValueConfigResult>(source: result);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> GetKeyValueConfigByKey(string key, string version)
    {
        var result = await keyValueConfigFacade.ObtenerKeyValueConfigPorKeyAsync(key: key);
        var response = mapper.Map<KeyValueConfigResult>(source: result);
        return Ok(value: response);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> UpdateKeyValueConfig(KeyValueConfigUpdateRequest body, string key,
        string version)
    {
        var result =
            await keyValueConfigFacade.ActualizarKeyValueConfigAsync(key: key, value: body.Value,
                modificationUser: this.GetAuthenticatedUserGuid());
        var response = mapper.Map<KeyValueConfigResult>(source: result);
        return Ok(value: response);
    }
}
