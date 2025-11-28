using System.Security.Claims;

namespace Wallet.Funcionalidad.Services.TokenService;

/// <summary>
/// Define los métodos para la generación y validación de tokens de acceso y refresco.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Genera un token de acceso JWT (JSON Web Token) basado en una colección de claims.
    /// </summary>
    /// <param name="claims">Colección de claims que se incluirán en el token de acceso.</param>
    /// <returns>Una cadena que representa el token de acceso generado.</returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// Genera un token de refresco. Este token se utiliza para obtener nuevos tokens de acceso sin requerir las credenciales del usuario.
    /// </summary>
    /// <returns>Una cadena que representa el token de refresco generado.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Obtiene un ClaimsPrincipal de un token JWT expirado. Esto es útil para extraer la identidad del usuario de un token que ya no es válido para autenticación.
    /// </summary>
    /// <param name="token">El token JWT expirado del cual se extraerá el ClaimsPrincipal.</param>
    /// <returns>Un ClaimsPrincipal que contiene las claims del token expirado.</returns>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
