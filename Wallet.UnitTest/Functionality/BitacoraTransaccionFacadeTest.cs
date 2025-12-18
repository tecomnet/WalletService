using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit;

namespace Wallet.UnitTest.Functionality;

public class BitacoraTransaccionFacadeTest : BaseFacadeTest<IBitacoraTransaccionFacade>, IDisposable
{
    private readonly IBitacoraTransaccionFacade _facade;
    private readonly Guid _userId = Guid.NewGuid();

    public BitacoraTransaccionFacadeTest() : base(new SetupDataConfig())
    {
        _facade = new BitacoraTransaccionFacade(Context);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    private async Task<CuentaWallet> CriarWalletAsync()
    {
        var empresa = new Empresa("TecomTest", _userId);
        Context.Empresa.Add(empresa);

        var usuario = new Usuario("+52", "5599999999", null, null, EstatusRegistroEnum.TerminosCondicionesAceptado,
            _userId);
        Context.Usuario.Add(usuario);

        var cliente = new Cliente(usuario, empresa, _userId);
        cliente.AgregarDatosPersonales("Test", "User", "Bitacora", new DateOnly(1990, 1, 1), Genero.Masculino, _userId);
        Context.Cliente.Add(cliente);
        await Context.SaveChangesAsync();

        // Use facade or manual creation? Manual is safer for unit test isolation vs dependencies loop.
        // But implementation requires valid IDs.
        var wallet = new CuentaWallet(cliente.Id, "MXN", "123456789012345678", _userId);
        Context.CuentaWallet.Add(wallet);
        await Context.SaveChangesAsync();

        return wallet;
    }

    [Fact]
    public async Task GuardarTransaccionAsync_ShouldSaveTransaction_WhenValid()
    {
        // Arrange
        var wallet = await CriarWalletAsync();

        // Act
        var result = await _facade.GuardarTransaccionAsync(wallet.Id, 100.50m, "DEPOSITO", "Abono", "Completada",
            _userId, "REF123");

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal(wallet.Id, result.IdBilletera);
        Assert.Equal(100.50m, result.Monto);
        Assert.Equal("REF123", result.RefExternaId);
    }

    [Fact]
    public async Task ObtenerTodasAsync_ShouldReturnList_WhenTransactionsExist()
    {
        // Arrange
        var wallet = await CriarWalletAsync();
        await _facade.GuardarTransaccionAsync(wallet.Id, 50m, "TEST", "Abono", "Completada", _userId, null);

        // Act
        var result = await _facade.ObtenerTodasAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, t => t.Monto == 50m);
    }

    [Fact]
    public async Task ObtenerPorClienteAsync_ShouldReturnTransactions_ForSpecificClient()
    {
        // Arrange
        var wallet = await CriarWalletAsync();
        await _facade.GuardarTransaccionAsync(wallet.Id, 75m, "TEST", "Abono", "Completada", _userId, null);

        // Act
        var result = await _facade.ObtenerPorClienteAsync(wallet.IdCliente);

        // Assert
        Assert.Single(result);
        Assert.Equal(75m, result[0].Monto);
    }
}
