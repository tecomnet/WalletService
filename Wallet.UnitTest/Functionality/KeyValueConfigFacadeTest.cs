using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.KeyValueConfigFacade;
using Wallet.UnitTest.FixtureBase;
using Xunit;

namespace Wallet.UnitTest.Functionality;

public class KeyValueConfigFacadeTest : DatabaseTestFixture, IDisposable
{
    private readonly KeyValueConfigFacade _facade;
    private readonly ServiceDbContext _context;

    public KeyValueConfigFacadeTest()
    {
        _context = CreateContext();
        _facade = new KeyValueConfigFacade(_context);
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
        var result = await _facade.GuardarKeyValueConfigAsync(key, value, user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(key, result.Key);
        Assert.Equal(value, result.Value);
        Assert.NotNull(await _context.KeyValueConfig.FirstOrDefaultAsync(x => x.Key == key));
    }

    [Fact]
    public async Task GuardarKeyValueConfigAsync_Throws_IfAlreadyExists()
    {
        // Arrange
        var key = "TestKey_" + Guid.NewGuid();
        var value = "TestValue";
        var user = Guid.NewGuid();
        await _facade.GuardarKeyValueConfigAsync(key, value, user);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<EMGeneralAggregateException>(() =>
            _facade.GuardarKeyValueConfigAsync(key, value, user));

        // Assert error code
        // Assert error code
        Assert.Contains(ex.InnerExceptions, e => e.Code == ServiceErrorsBuilder.KeyValueConfigYaExiste);
    }

    [Fact]
    public async Task ObtenerKeyValueConfigPorKeyAsync_Success()
    {
        // Arrange
        var key = "TestKey_" + Guid.NewGuid();
        var value = "TestValue";
        var user = Guid.NewGuid();
        await _facade.GuardarKeyValueConfigAsync(key, value, user);

        // Act
        var result = await _facade.ObtenerKeyValueConfigPorKeyAsync(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(key, result.Key);
    }

    [Fact]
    public async Task ObtenerTodasLasConfiguracionesAsync_Success()
    {
        // Arrange
        var key1 = "TestKey1_" + Guid.NewGuid();
        var key2 = "TestKey2_" + Guid.NewGuid();
        var user = Guid.NewGuid();
        await _facade.GuardarKeyValueConfigAsync(key1, "Val1", user);
        await _facade.GuardarKeyValueConfigAsync(key2, "Val2", user);

        // Act
        var results = await _facade.ObtenerTodasLasConfiguracionesAsync();

        // Assert
        Assert.NotNull(results);
        Assert.True(results.Count >= 2);
        Assert.Contains(results, x => x.Key == key1);
        Assert.Contains(results, x => x.Key == key2);
    }
}
