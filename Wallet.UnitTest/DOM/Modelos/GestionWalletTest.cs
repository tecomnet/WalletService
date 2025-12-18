using Wallet.DOM.Modelos.GestionWallet;
using Xunit;

namespace Wallet.UnitTest.DOM.Modelos;

public class GestionWalletModelosTest
{
    [Fact]
    public void CuentaWallet_Initialization_ShouldWork()
    {
        // Arrange
        var idCliente = Guid.NewGuid();
        var moneda = "MXN";
        var clabe = "123456789012345678";
        var creationUser = Guid.NewGuid();

        // Act
        var cuenta = new CuentaWallet(idCliente, moneda, clabe, creationUser);

        // Assert
        Assert.Equal(idCliente, cuenta.IdCliente);
        Assert.Equal(moneda, cuenta.Moneda);
        Assert.Equal(clabe, cuenta.CuentaCLABE);
        Assert.Equal(0, cuenta.SaldoActual);
        Assert.Equal(creationUser, cuenta.CreationUser);
        Assert.True(cuenta.IsActive);
    }

    [Fact]
    public void CuentaWallet_ActualizarSaldo_ShouldUpdateSaldo()
    {
        // Arrange
        var cuenta = new CuentaWallet(Guid.NewGuid(), "MXN", "123456789012345678", Guid.NewGuid());
        var nuevoSaldo = 100.50m;
        var modUser = Guid.NewGuid();

        // Act
        cuenta.ActualizarSaldo(nuevoSaldo, modUser);

        // Assert
        Assert.Equal(nuevoSaldo, cuenta.SaldoActual);
        Assert.Equal(modUser, cuenta.ModificationUser);
    }

    [Fact]
    public void BitacoraTransaccion_Initialization_ShouldWork()
    {
        // Arrange
        // Arrange
        var creationUser = Guid.NewGuid();
        var bitacora = new BitacoraTransaccion(1, 500.00m, "SPEI", "Abono", "Completada", creationUser,
            Guid.NewGuid().ToString());

        // Assert
        Assert.Equal(1, bitacora.IdBilletera);
        Assert.Equal(500.00m, bitacora.Monto);
        Assert.Equal("SPEI", bitacora.Tipo);
        Assert.NotEqual(DateTime.MinValue, bitacora.CreationTimestamp);
    }
}
