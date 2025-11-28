using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.Funcionalidad.Models;
using Wallet.Funcionalidad.Services.TokenService;

namespace Wallet.Funcionalidad.Functionality.AuthFacade;

public class AuthFacade(ServiceDbContext context, ITokenService tokenService) : IAuthFacade
{
    public async Task<AuthResultDto> LoginAsync(string login, string password)
    {
        // Login can be email or phone
        var usuario = await context.Usuario
            .FirstOrDefaultAsync(predicate: u => u.CorreoElectronico == login || u.Telefono == login);

        if (usuario == null || usuario.Contrasena != password) // In production, use hashed passwords!
        {
            return new AuthResultDto
            {
                Success = false,
                Errors = new List<string> { "Credenciales inválidas" }
            };
        }

        var claims = new List<Claim>
        {
            new Claim(type: ClaimTypes.Name, value: usuario.Id.ToString()),
            new Claim(type: ClaimTypes.Email, value: usuario.CorreoElectronico ?? ""),
            new Claim(type: ClaimTypes.MobilePhone, value: usuario.Telefono)
        };

        var accessToken = tokenService.GenerateAccessToken(claims: claims);
        var refreshToken = tokenService.GenerateRefreshToken();

        usuario.UpdateRefreshToken(refreshToken: refreshToken, expiryTime: DateTime.UtcNow.AddDays(value: 7), modificationUser: Guid.Empty);

        // Fix: modificationUser should be a Guid. Assuming system update or self update.
        // Since we don't have the user's Guid easily accessible as a Guid type from int Id, we use Guid.Empty or a system user.
        // Actually, Usuario.Id is int. ModificationUser is Guid.
        // Let's use Guid.Empty for login updates or pass it if available.
        // For now, Guid.Empty is acceptable for system actions.

        await context.SaveChangesAsync();

        return new AuthResultDto
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(token: accessToken);
        var userIdStr = principal.Identity?.Name;

        if (string.IsNullOrEmpty(value: userIdStr) || !int.TryParse(s: userIdStr, result: out int userId))
        {
            return new AuthResultDto { Success = false, Errors = new List<string> { "Token inválido" } };
        }

        var usuario = await context.Usuario.FindAsync(keyValues: userId);

        if (usuario == null || usuario.RefreshToken != refreshToken ||
            usuario.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthResultDto
                { Success = false, Errors = new List<string> { "Refresh token inválido o expirado" } };
        }

        var newAccessToken = tokenService.GenerateAccessToken(claims: principal.Claims);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        usuario.UpdateRefreshToken(refreshToken: newRefreshToken, expiryTime: DateTime.UtcNow.AddDays(value: 7), modificationUser: Guid.Empty);
        await context.SaveChangesAsync();

        return new AuthResultDto
        {
            Success = true,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task RevokeTokenAsync(string username)
    {
        // Username is likely the Id in this context or login
        if (int.TryParse(s: username, result: out int userId))
        {
            var usuario = await context.Usuario.FindAsync(keyValues: userId);
            if (usuario != null)
            {
                usuario.UpdateRefreshToken(refreshToken: string.Empty, expiryTime: DateTime.MinValue, modificationUser: Guid.Empty);
                await context.SaveChangesAsync();
            }
        }
    }
}
