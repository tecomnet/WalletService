using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Wallet.Funcionalidad.Services.TokenService;

/// <summary>
/// Proporciona servicios para la generación y validación de tokens JWT y tokens de refresco.
/// </summary>
public class TokenService(IConfiguration configuration) : ITokenService
{
    /// <summary>
    /// Genera un token de acceso JWT basado en una colección de claims.
    /// </summary>
    /// <param name="claims">Colección de claims a incluir en el token.</param>
    /// <returns>Un token de acceso JWT como cadena.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si la clave JWT no está configurada.</exception>
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        // Obtiene la clave secreta del JWT de la configuración y la codifica en bytes.
        var key = new SymmetricSecurityKey(key: Encoding.UTF8.GetBytes(s: configuration[key: "Jwt:Key"] ?? throw new InvalidOperationException(message: "Jwt:Key is missing")));
        // Crea las credenciales de firma usando la clave y el algoritmo HMAC SHA256.
        var creds = new SigningCredentials(key: key, algorithm: SecurityAlgorithms.HmacSha256);

        // Configura los detalles del token JWT.
        var token = new JwtSecurityToken(
            issuer: configuration[key: "Jwt:Issuer"], // Emisor del token
            audience: configuration[key: "Jwt:Audience"], // Audiencia del token
            claims: claims, // Claims a incluir
            expires: DateTime.UtcNow.AddMinutes(value: 15), // El token expira en 15 minutos (token de acceso de corta duración)
            signingCredentials: creds // Credenciales de firma
        );

        // Escribe el token JWT y lo devuelve como una cadena.
        return new JwtSecurityTokenHandler().WriteToken(token: token);
    }

    /// <summary>
    /// Genera un token de refresco aleatorio y seguro.
    /// </summary>
    /// <returns>Un token de refresco como cadena codificada en Base64.</returns>
    public string GenerateRefreshToken()
    {
        // Crea un array de bytes para almacenar el número aleatorio.
        var randomNumber = new byte[32];
        // Usa un generador de números aleatorios criptográficamente seguro.
        using var rng = RandomNumberGenerator.Create();
        // Rellena el array con bytes aleatorios.
        rng.GetBytes(data: randomNumber);
        // Convierte los bytes aleatorios a una cadena Base64.
        return Convert.ToBase64String(inArray: randomNumber);
    }

    /// <summary>
    /// Obtiene el principal (ClaimsPrincipal) de un token JWT expirado.
    /// Esto es útil para validar tokens de refresco.
    /// </summary>
    /// <param name="token">El token JWT expirado.</param>
    /// <returns>Un ClaimsPrincipal que representa la identidad del usuario.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si la clave JWT no está configurada.</exception>
    /// <exception cref="SecurityTokenException">Se lanza si el token es inválido o no usa el algoritmo esperado.</exception>
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        // Configura los parámetros de validación del token.
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // No valida la audiencia (puede ser necesario en producción)
            ValidateIssuer = false, // No valida el emisor
            ValidateIssuerSigningKey = true, // Valida la clave de firma del emisor
            // Establece la clave de firma usando la clave JWT de la configuración.
            IssuerSigningKey = new SymmetricSecurityKey(key: Encoding.UTF8.GetBytes(s: configuration[key: "Jwt:Key"] ?? throw new InvalidOperationException(message: "Jwt:Key is missing"))),
            ValidateLifetime = false // Importante: No valida la vida útil, ya que estamos procesando un token expirado.
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        // Valida el token con los parámetros definidos.
        var principal = tokenHandler.ValidateToken(token: token, validationParameters: tokenValidationParameters, validatedToken: out SecurityToken securityToken);
        // Intenta convertir el token validado a un JwtSecurityToken.
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        // Verifica si el token es nulo o si el algoritmo de la cabecera no es HMAC SHA256.
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(value: SecurityAlgorithms.HmacSha256, comparisonType: StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException(message: "Invalid token"); // Lanza una excepción si el token es inválido.

        return principal;
    }
}
