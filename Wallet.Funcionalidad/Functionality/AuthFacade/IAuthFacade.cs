using Wallet.Funcionalidad.Models;

namespace Wallet.Funcionalidad.Functionality.AuthFacade;

/// <summary>
/// Interfaz para la fachada de autenticación, proporcionando métodos para la gestión de usuarios.
/// </summary>
public interface IAuthFacade
{
    /// <summary>
    /// Intenta autenticar a un usuario con las credenciales proporcionadas.
    /// </summary>
    /// <param name="login">El nombre de usuario o correo electrónico.</param>
    /// <param name="password">La contraseña del usuario.</param>
    /// <returns>Un objeto <see cref="AuthResultDto"/> que contiene los tokens de autenticación si la operación es exitosa.</returns>
    Task<AuthResultDto> LoginAsync(string login, string password);

    /// <summary>
    /// Refresca los tokens de acceso y de refresco existentes.
    /// </summary>
    /// <param name="accessToken">El token de acceso actual.</param>
    /// <param name="refreshToken">El token de refresco actual.</param>
    /// <returns>Un objeto <see cref="AuthResultDto"/> que contiene los nuevos tokens de autenticación.</returns>
    Task<AuthResultDto> RefreshTokenAsync(string accessToken, string refreshToken);

    /// <summary>
    /// Revoca todos los tokens de refresco asociados a un usuario específico.
    /// </summary>
    /// <param name="username">El nombre de usuario para el cual se revocarán los tokens.</param>
    Task RevokeTokenAsync(string username);
}
