using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.DetallesPagoServicioFacade;

public interface IDetallesPagoServicioFacade
{
    Task<DetallesPagoServicio> GuardarDetallesAsync(int idTransaccion, int idProducto, string numeroReferencia,
        Guid creationUser, string? codigoAutorizacion);

    Task<DetallesPagoServicio> ObtenerPorIdAsync(int id);
    Task<List<DetallesPagoServicio>> ObtenerPorClienteAsync(int idCliente);
    Task<List<DetallesPagoServicio>> ObtenerTodosAsync();
    Task<List<DetallesPagoServicio>> ObtenerPorTransaccionAsync(int idTransaccion);

    Task<BitacoraTransaccion> RegistrarPagoServicioAsync(
        int idBilletera,
        decimal monto,
        string tipo,
        string direccion,
        string estatus,
        int idProducto,
        string numeroReferencia,
        Guid creationUser);
}
