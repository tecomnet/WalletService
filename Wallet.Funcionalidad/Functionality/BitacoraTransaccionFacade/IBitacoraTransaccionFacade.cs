using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;

public interface IBitacoraTransaccionFacade
{
    Task<BitacoraTransaccion> GuardarTransaccionAsync(int idBilletera, decimal monto, string tipo, string direccion,
        string estatus, Guid creationUser, string? refExternaId);

    Task<List<BitacoraTransaccion>> ObtenerTodasAsync();
    Task<BitacoraTransaccion> ObtenerPorIdAsync(int id);
    Task<List<BitacoraTransaccion>> ObtenerPorClienteAsync(Guid idCliente);
}
