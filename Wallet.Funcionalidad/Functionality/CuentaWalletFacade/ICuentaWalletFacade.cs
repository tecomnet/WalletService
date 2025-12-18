using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.CuentaWalletFacade;

public interface ICuentaWalletFacade
{
    Task<CuentaWallet> CrearCuentaWalletAsync(Guid idCliente, Guid creationUser, string moneda = "MXN");
    Task<CuentaWallet> ObtenerPorClienteAsync(Guid idCliente);
    Task<CuentaWallet> ActualizarSaldoAsync(int idWallet, decimal nuevoSaldo, Guid modificationUser);
}
