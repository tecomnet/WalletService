using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.UnitTest.DOM.Modelos;

public class GestionWalletModelosTest
{
    [Fact]
    public void CuentaWallet_Initialization_ShouldWork()
    {
        // Arrange
        var idCliente = 123;
        var moneda = "MXN";
        var clabe = "123456789012345678";
        var creationUser = Guid.NewGuid();

        // Act
        var cuenta = new CuentaWallet(idCliente: idCliente, moneda: moneda, cuentaCLABE: clabe, creationUser: creationUser);

        // Assert
        Assert.Equal(expected: idCliente, actual: cuenta.IdCliente);
        Assert.Equal(expected: moneda, actual: cuenta.Moneda);
        Assert.Equal(expected: clabe, actual: cuenta.CuentaCLABE);
        Assert.Equal(expected: 0, actual: cuenta.SaldoActual);
        Assert.Equal(expected: creationUser, actual: cuenta.CreationUser);
        Assert.True(condition: cuenta.IsActive);
    }

    [Fact]
    public void CuentaWallet_ActualizarSaldo_ShouldUpdateSaldo()
    {
        // Arrange
        var cuenta = new CuentaWallet(idCliente: 123, moneda: "MXN", cuentaCLABE: "123456789012345678", creationUser: Guid.NewGuid());
        var nuevoSaldo = 100.50m;
        var modUser = Guid.NewGuid();

        // Act
        cuenta.ActualizarSaldo(nuevoSaldo: nuevoSaldo, modificationUser: modUser);

        // Assert
        Assert.Equal(expected: nuevoSaldo, actual: cuenta.SaldoActual);
        Assert.Equal(expected: modUser, actual: cuenta.ModificationUser);
    }

    [Fact]
    public void BitacoraTransaccion_Initialization_ShouldWork()
    {
        // Arrange
        // Arrange
        var creationUser = Guid.NewGuid();
        var bitacora = new BitacoraTransaccion(cuentaWalletId: 1, monto: 500.00m, tipo: "SPEI", direccion: "Abono", estatus: "Completada", creationUser: creationUser,
            refExternaId: Guid.NewGuid().ToString());

        // Assert
        Assert.Equal(expected: 1, actual: bitacora.CuentaWalletId);
        Assert.Equal(expected: 500.00m, actual: bitacora.Monto);
        Assert.Equal(expected: "SPEI", actual: bitacora.Tipo);
        Assert.NotEqual(expected: DateTime.MinValue, actual: bitacora.CreationTimestamp);
    }
}
