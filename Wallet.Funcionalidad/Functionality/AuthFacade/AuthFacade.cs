using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.Funcionalidad.Models;
using Wallet.Funcionalidad.Services.TokenService;
using Wallet.DOM;
using Wallet.DOM.Errors;

namespace Wallet.Funcionalidad.Functionality.AuthFacade;

/// <summary>
/// Proporciona una fachada para las operaciones de autenticación y gestión de tokens.
/// </summary>
public class AuthFacade(ServiceDbContext context, ITokenService tokenService) : IAuthFacade
{
    /// <summary>
    /// Intenta autenticar a un usuario con las credenciales proporcionadas y genera tokens de acceso y refresco.
    /// </summary>
    /// <param name="login">El nombre de usuario, que puede ser un correo electrónico o un número de teléfono.</param>
    /// <param name="password">La contraseña del usuario.</param>
    /// <returns>Un <see cref="AuthResultDto"/> que contiene el resultado de la autenticación, incluyendo tokens si es exitosa.</returns>
    public async Task<AuthResultDto> LoginAsync(string login, string password)
    {
        try
        {
            // El login puede ser un correo electrónico o un número de teléfono.
            var usuario = await context.Usuario
                .Include(navigationPropertyPath: user => user.Cliente)
                .FirstOrDefaultAsync(predicate: u => u.CorreoElectronico == login || u.Telefono == login);

            // Verifica si el usuario existe y si la contraseña es correcta.
            // Se utiliza el método VerificarContrasena que usa BCrypt internamente.
            if (usuario == null || !usuario.VerificarContrasena(password: password))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = new List<string>
                        { ServiceErrorsBuilder.Instance().GetError(ServiceErrorsBuilder.CredencialesInvalidas).Message }
                };
            }

            // Crea los claims para el token de acceso.
            var claims = new List<Claim>
            {
                new Claim(type: "UsuarioId", value: usuario.Id.ToString()), // El Id del usuario como nombre.
                new Claim(type: "CorreoElectronico",
                    value: usuario.CorreoElectronico ?? ""), // Correo electrónico del usuario.
                new Claim(type: "Telefono", value: usuario.Telefono), // Número de teléfono del usuario.
                new Claim(type: "ClienteId",
                    value: usuario.Cliente?.Id.ToString() ?? string.Empty), // El Id del cliente como nombre.
                new Claim(type: "Guid", value: usuario.Guid.ToString()) // El Id del cliente como nombre.
            };

            // Genera el token de acceso y el token de refresco.
            var accessToken = tokenService.GenerateAccessToken(claims: claims);
            var refreshToken = tokenService.GenerateRefreshToken();

            // Actualiza el token de refresco del usuario en la base de datos.
            // La fecha de expiración se establece para 7 días en el futuro.
            // 'modificationUser' se establece como Guid.Empty ya que es una acción del sistema o del propio usuario.
            usuario.UpdateRefreshToken(refreshToken: refreshToken, expiryTime: DateTime.UtcNow.AddDays(value: 7),
                modificationUser: Guid.Empty);

            // Guarda los cambios en la base de datos.
            await context.SaveChangesAsync();

            return new AuthResultDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IdCliente = usuario.Cliente?.Id ?? 0
            };
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <summary>
    /// Refresca el token de acceso y el token de refresco utilizando un token de refresco válido.
    /// </summary>
    /// <param name="accessToken">El token de acceso caducado.</param>
    /// <param name="refreshToken">El token de refresco actual.</param>
    /// <returns>Un <see cref="AuthResultDto"/> con los nuevos tokens si el refresco es exitoso.</returns>
    public async Task<AuthResultDto> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        try
        {
            // Obtiene el principal (identidad del usuario) del token de acceso caducado.
            var principal = tokenService.GetPrincipalFromExpiredToken(token: accessToken);
            var userIdStr = principal.Identity?.Name;

            // Valida si el ID del usuario se puede extraer del token y es un entero válido.
            if (string.IsNullOrEmpty(value: userIdStr) || !int.TryParse(s: userIdStr, result: out int userId))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = new List<string>
                        { ServiceErrorsBuilder.Instance().GetError(ServiceErrorsBuilder.TokenInvalido).Message }
                };
            }

            // Busca al usuario por su ID.
            var usuario = await context.Usuario.FindAsync(keyValues: userId);

            // Verifica la validez del token de refresco:
            // 1. El usuario existe.
            // 2. El token de refresco proporcionado coincide con el almacenado.
            // 3. El token de refresco no ha caducado.
            if (usuario == null || usuario.RefreshToken != refreshToken ||
                usuario.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = new List<string>
                        { ServiceErrorsBuilder.Instance().GetError(ServiceErrorsBuilder.RefreshTokenInvalido).Message }
                };
            }

            // Genera un nuevo token de acceso y un nuevo token de refresco.
            var newAccessToken = tokenService.GenerateAccessToken(claims: principal.Claims);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            // Actualiza el token de refresco del usuario en la base de datos con el nuevo token y su nueva fecha de expiración.
            usuario.UpdateRefreshToken(refreshToken: newRefreshToken, expiryTime: DateTime.UtcNow.AddDays(value: 7),
                modificationUser: Guid.Empty);
            await context.SaveChangesAsync();

            return new AuthResultDto
            {
                Success = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <summary>
    /// Revoca el token de refresco de un usuario, invalidando futuras solicitudes de refresco.
    /// </summary>
    /// <param name="username">El nombre de usuario o ID del usuario cuyo token de refresco se va a revocar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    public async Task RevokeTokenAsync(string username)
    {
        try
        {
            // Intenta parsear el 'username' como un ID de usuario entero.
            if (int.TryParse(s: username, result: out int userId))
            {
                // Busca al usuario por su ID.
                var usuario = await context.Usuario.FindAsync(keyValues: userId);
                if (usuario != null)
                {
                    // Revoca el token de refresco estableciéndolo como vacío y la fecha de expiración al mínimo.
                    usuario.UpdateRefreshToken(refreshToken: string.Empty, expiryTime: DateTime.MinValue,
                        modificationUser: Guid.Empty);
                    await context.SaveChangesAsync(); // Guarda los cambios en la base de datos.
                }
            }
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }
}
