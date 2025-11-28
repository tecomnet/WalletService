using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Wallet.Funcionalidad.Functionality.AuthFacade;
using Wallet.RestAPI.Models;

namespace Wallet.RestAPI.Controllers.Implementation;

/// <summary>
/// Implementation of the Authentication API controller.
/// </summary>
public class AuthApi(IAuthFacade authFacade, IMapper mapper) : AuthApiControllerBase
{
    /// <inheritdoc />
    public override async Task<IActionResult> LoginAsync(
        [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
        string version,
        [FromBody] LoginRequest body)
    {
        var result = await authFacade.LoginAsync(login: body.Login, password: body.Password);
        if (!result.Success)
        {
            return Unauthorized(value: result.Errors);
        }

        return Ok(value: mapper.Map<AuthResult>(source: result));
    }

    /// <inheritdoc />
    public override async Task<IActionResult> RefreshAsync(
        [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
        string version,
        [FromBody] RefreshTokenRequest body)
    {
        var result = await authFacade.RefreshTokenAsync(accessToken: body.AccessToken, refreshToken: body.RefreshToken);
        if (!result.Success)
        {
            return BadRequest(error: result.Errors);
        }

        return Ok(value: mapper.Map<AuthResult>(source: result));
    }

    /// <inheritdoc />
    [Authorize]
    public override async Task<IActionResult> RevokeAsync(
        [FromRoute] [Required] [RegularExpression(pattern: "^(?<major>[0-9]+).(?<minor>[0-9]+)$")]
        string version,
        [FromBody] RevokeTokenRequest body)
    {
        // We use the username from the body as per the spec, but we could also validate against the token.
        await authFacade.RevokeTokenAsync(username: body.Username);
        return NoContent();
    }
}
