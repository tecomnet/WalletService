using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.CuentaWalletFacade;

public interface ICuentaWalletFacade
{
    Task<CuentaWallet> CrearCuentaWalletAsync(int idCliente, Guid creationUser, string moneda = "MXN");
    Task<CuentaWallet> ObtenerPorClienteAsync(int idCliente);
    Task<CuentaWallet> ObtenerPorIdAsync(int idWallet);
    Task<CuentaWallet> ActualizarSaldoAsync(int idWallet, decimal nuevoSaldo, Guid modificationUser);
}
