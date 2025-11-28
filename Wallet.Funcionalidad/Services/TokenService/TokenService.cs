using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Wallet.Funcionalidad.Services.TokenService;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(key: Encoding.UTF8.GetBytes(s: configuration[key: "Jwt:Key"] ?? throw new InvalidOperationException(message: "Jwt:Key is missing")));
        var creds = new SigningCredentials(key: key, algorithm: SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration[key: "Jwt:Issuer"],
            audience: configuration[key: "Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(value: 15), // Short lived access token
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token: token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(data: randomNumber);
        return Convert.ToBase64String(inArray: randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // Might want to validate in production
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key: Encoding.UTF8.GetBytes(s: configuration[key: "Jwt:Key"] ?? throw new InvalidOperationException(message: "Jwt:Key is missing"))),
            ValidateLifetime = false // Here we are validating an expired token
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token: token, validationParameters: tokenValidationParameters, validatedToken: out SecurityToken securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(value: SecurityAlgorithms.HmacSha256, comparisonType: StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException(message: "Invalid token");

        return principal;
    }
}
