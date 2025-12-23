using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Moq;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
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

        var usuario = new Usuario(codigoPais: "052", telefono: "5512345678", correoElectronico: "test@example.com",
            contrasena: "password", estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: Guid.NewGuid());
        context.Usuario.Add(entity: usuario);
        await context.SaveChangesAsync();

        _tokenServiceMock.Setup(expression: x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns(value: "access_token");
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

        var usuario = new Usuario(codigoPais: "052", telefono: "5512345678", correoElectronico: "test@example.com",
            contrasena: "password", estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: Guid.NewGuid());
        context.Usuario.Add(entity: usuario);
        await context.SaveChangesAsync();

        // Act
        var result = await authFacade.LoginAsync(login: "test@example.com", password: "wrong_password");

        // Assert
        Assert.False(condition: result.Success);
        Assert.Contains(expected: "Credenciales inválidas", collection: result.Errors);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_GeneratesCorrectClaims()
    {
        // Arrange
        using var context = CreateContext();
        var authFacade = new AuthFacade(context: context, tokenService: _tokenServiceMock.Object);

        var creationUser = Guid.NewGuid();
        var usuario = new Usuario(codigoPais: "052", telefono: "5512345678", correoElectronico: "test@example.com",
            contrasena: "password", estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: creationUser);
        context.Usuario.Add(entity: usuario);

        var empresa = new Empresa("Tecomnet", creationUser);
        context.Empresa.Add(empresa);

        var cliente = new Cliente(usuario, empresa, creationUser);
        context.Cliente.Add(cliente);

        await context.SaveChangesAsync();

        _tokenServiceMock.Setup(expression: x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns(value: "access_token");
        _tokenServiceMock.Setup(expression: x => x.GenerateRefreshToken()).Returns(value: "refresh_token");

        // Act
        await authFacade.LoginAsync(login: "test@example.com", password: "password");

        // Assert
        _tokenServiceMock.Verify(x => x.GenerateAccessToken(It.Is<IEnumerable<Claim>>(c =>
            c.Any(claim => claim.Type == "UsuarioId" && claim.Value == usuario.Id.ToString()) &&
            c.Any(claim => claim.Type == "CorreoElectronico" && claim.Value == usuario.CorreoElectronico) &&
            c.Any(claim => claim.Type == "Telefono" && claim.Value == usuario.Telefono) &&
            c.Any(claim => claim.Type == "ClienteId" && claim.Value == cliente.Id.ToString()) &&
            c.Any(claim => claim.Type == "Guid" && claim.Value == usuario.Guid.ToString())
        )), Times.Once);
    }


    [Fact]
    public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
    {
        // Arrange
        using var context = CreateContext();
        var authFacade = new AuthFacade(context: context, tokenService: _tokenServiceMock.Object);

        var usuario = new Usuario(codigoPais: "052", telefono: "5512345678", correoElectronico: "test@example.com",
            contrasena: "password", estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: Guid.NewGuid());
        usuario.UpdateRefreshToken(refreshToken: "valid_refresh_token", expiryTime: DateTime.UtcNow.AddDays(value: 1),
            modificationUser: Guid.NewGuid());
        context.Usuario.Add(entity: usuario);
        await context.SaveChangesAsync();

        var claims = new List<Claim> { new Claim(type: ClaimTypes.Name, value: usuario.Id.ToString()) };
        var principal = new ClaimsPrincipal(identity: new ClaimsIdentity(claims: claims));

        _tokenServiceMock.Setup(expression: x => x.GetPrincipalFromExpiredToken("expired_access_token"))
            .Returns(value: principal);
        _tokenServiceMock.Setup(expression: x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns(value: "new_access_token");
        _tokenServiceMock.Setup(expression: x => x.GenerateRefreshToken()).Returns(value: "new_refresh_token");

        // Act
        var result =
            await authFacade.RefreshTokenAsync(accessToken: "expired_access_token",
                refreshToken: "valid_refresh_token");

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

        var usuario = new Usuario(codigoPais: "052", telefono: "5512345678", correoElectronico: "test@example.com",
            contrasena: "password", estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: Guid.NewGuid());
        usuario.UpdateRefreshToken(refreshToken: "valid_refresh_token", expiryTime: DateTime.UtcNow.AddDays(value: 1),
            modificationUser: Guid.NewGuid());
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
