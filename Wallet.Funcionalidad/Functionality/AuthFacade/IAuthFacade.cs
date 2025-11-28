using Wallet.Funcionalidad.Models;

namespace Wallet.Funcionalidad.Functionality.AuthFacade;

public interface IAuthFacade
{
    Task<AuthResultDto> LoginAsync(string login, string password);
    Task<AuthResultDto> RefreshTokenAsync(string accessToken, string refreshToken);
    Task RevokeTokenAsync(string username);
}
