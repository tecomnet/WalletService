using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.KeyValueConfigFacade;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.Functionality;

public class KeyValueConfigFacadeTest : DatabaseTestFixture, IDisposable
{
    private readonly KeyValueConfigFacade _facade;
    private readonly ServiceDbContext _context;

    public KeyValueConfigFacadeTest()
    {
        _context = CreateContext();
        _facade = new KeyValueConfigFacade(context: _context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GuardarKeyValueConfigAsync_Success()
    {
        // Arrange
        var key = "TestKey_" + Guid.NewGuid();
        var value = "TestValue";
        var user = Guid.NewGuid();

        // Act
        var result = await _facade.GuardarKeyValueConfigAsync(key: key, value: value, creationUser: user);

        // Assert
        Assert.NotNull(@object: result);
        Assert.Equal(expected: key, actual: result.Key);
        Assert.Equal(expected: value, actual: result.Value);
        Assert.NotNull(@object: await _context.KeyValueConfig.FirstOrDefaultAsync(predicate: x => x.Key == key));
    }

    [Fact]
    public async Task GuardarKeyValueConfigAsync_Throws_IfAlreadyExists()
    {
        // Arrange
        var key = "TestKey_" + Guid.NewGuid();
        var value = "TestValue";
        var user = Guid.NewGuid();
        await _facade.GuardarKeyValueConfigAsync(key: key, value: value, creationUser: user);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<EMGeneralAggregateException>(testCode: () =>
            _facade.GuardarKeyValueConfigAsync(key: key, value: value, creationUser: user));

        // Assert error code
        // Assert error code
        Assert.Contains(collection: ex.InnerExceptions, filter: e => e.Code == ServiceErrorsBuilder.KeyValueConfigYaExiste);
    }

    [Fact]
    public async Task ObtenerKeyValueConfigPorKeyAsync_Success()
    {
        // Arrange
        var key = "TestKey_" + Guid.NewGuid();
        var value = "TestValue";
        var user = Guid.NewGuid();
        await _facade.GuardarKeyValueConfigAsync(key: key, value: value, creationUser: user);

        // Act
        var result = await _facade.ObtenerKeyValueConfigPorKeyAsync(key: key);

        // Assert
        Assert.NotNull(@object: result);
        Assert.Equal(expected: key, actual: result.Key);
    }

    [Fact]
    public async Task ObtenerTodasLasConfiguracionesAsync_Success()
    {
        // Arrange
        var key1 = "TestKey1_" + Guid.NewGuid();
        var key2 = "TestKey2_" + Guid.NewGuid();
        var user = Guid.NewGuid();
        await _facade.GuardarKeyValueConfigAsync(key: key1, value: "Val1", creationUser: user);
        await _facade.GuardarKeyValueConfigAsync(key: key2, value: "Val2", creationUser: user);

        // Act
        var results = await _facade.ObtenerTodasLasConfiguracionesAsync();

        // Assert
        Assert.NotNull(@object: results);
        Assert.True(condition: results.Count >= 2);
        Assert.Contains(collection: results, filter: x => x.Key == key1);
        Assert.Contains(collection: results, filter: x => x.Key == key2);
    }
}
