using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Moq;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.Functionality.AuthFacade;
using Wallet.Funcionalidad.Services.TokenService;

namespace Wallet.UnitTest.Functionality.AuthFacadeTest;

public class AuthFacadeTest
{
    private readonly Mock<ITokenService> _tokenServiceMock = new();

    private ServiceDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ServiceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ServiceDbContext(options: options);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        using var context = CreateContext();
        var authFacade = new AuthFacade(context: context, tokenService: _tokenServiceMock.Object);

        var usuario = new Usuario(codigoPais: "052", telefono: "5512345678", correoElectronico: "test@example.com", contrasena: "password", estatus: "Active", creationUser: Guid.NewGuid());
        context.Usuario.Add(entity: usuario);
        await context.SaveChangesAsync();

        _tokenServiceMock.Setup(expression: x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(value: "access_token");
        _tokenServiceMock.Setup(expression: x => x.GenerateRefreshToken()).Returns(value: "refresh_token");

        // Act
        var result = await authFacade.LoginAsync(login: "test@example.com", password: "password");

        // Assert
        Assert.True(condition: result.Success);
        Assert.Equal(expected: "access_token", actual: result.AccessToken);
        Assert.Equal(expected: "refresh_token", actual: result.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ReturnsError()
    {
        // Arrange
        using var context = CreateContext();
        var authFacade = new AuthFacade(context: context, tokenService: _tokenServiceMock.Object);

        var usuario = new Usuario(codigoPais: "052", telefono: "5512345678", correoElectronico: "test@example.com", contrasena: "password", estatus: "Active", creationUser: Guid.NewGuid());
        context.Usuario.Add(entity: usuario);
        await context.SaveChangesAsync();

        // Act
        var result = await authFacade.LoginAsync(login: "test@example.com", password: "wrong_password");

        // Assert
        Assert.False(condition: result.Success);
        Assert.Contains(expected: "Credenciales inválidas", collection: result.Errors);
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
    {
        // Arrange
        using var context = CreateContext();
        var authFacade = new AuthFacade(context: context, tokenService: _tokenServiceMock.Object);

        var usuario = new Usuario(codigoPais: "052", telefono: "5512345678", correoElectronico: "test@example.com", contrasena: "password", estatus: "Active", creationUser: Guid.NewGuid());
        usuario.UpdateRefreshToken(refreshToken: "valid_refresh_token", expiryTime: DateTime.UtcNow.AddDays(value: 1), modificationUser: Guid.NewGuid());
        context.Usuario.Add(entity: usuario);
        await context.SaveChangesAsync();

        var claims = new List<Claim> { new Claim(type: ClaimTypes.Name, value: usuario.Id.ToString()) };
        var principal = new ClaimsPrincipal(identity: new ClaimsIdentity(claims: claims));

        _tokenServiceMock.Setup(expression: x => x.GetPrincipalFromExpiredToken("expired_access_token")).Returns(value: principal);
        _tokenServiceMock.Setup(expression: x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(value: "new_access_token");
        _tokenServiceMock.Setup(expression: x => x.GenerateRefreshToken()).Returns(value: "new_refresh_token");

        // Act
        var result = await authFacade.RefreshTokenAsync(accessToken: "expired_access_token", refreshToken: "valid_refresh_token");

        // Assert
        Assert.True(condition: result.Success);
        Assert.Equal(expected: "new_access_token", actual: result.AccessToken);
        Assert.Equal(expected: "new_refresh_token", actual: result.RefreshToken);
    }

    [Fact]
    public async Task RevokeTokenAsync_ValidUser_RevokesToken()
    {
        // Arrange
        using var context = CreateContext();
        var authFacade = new AuthFacade(context: context, tokenService: _tokenServiceMock.Object);

        var usuario = new Usuario(codigoPais: "052", telefono: "5512345678", correoElectronico: "test@example.com", contrasena: "password", estatus: "Active", creationUser: Guid.NewGuid());
        usuario.UpdateRefreshToken(refreshToken: "valid_refresh_token", expiryTime: DateTime.UtcNow.AddDays(value: 1), modificationUser: Guid.NewGuid());
        context.Usuario.Add(entity: usuario);
        await context.SaveChangesAsync();

        // Act
        await authFacade.RevokeTokenAsync(username: usuario.Id.ToString());

        // Assert
        var dbUser = await context.Usuario.FindAsync(keyValues: usuario.Id);
        Assert.NotNull(dbUser);
        Assert.True(condition: string.IsNullOrEmpty(value: dbUser.RefreshToken));
    }
}
