using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.DetallesPagoServicioFacade;

public interface IDetallesPagoServicioFacade
{
    Task<DetallesPagoServicio> GuardarDetallesAsync(int idTransaccion, int idProveedor, string numeroReferencia,
        Guid creationUser, string? codigoAutorizacion);

    Task<DetallesPagoServicio> ObtenerPorIdAsync(int id);
    Task<List<DetallesPagoServicio>> ObtenerPorClienteAsync(int idCliente);
    Task<List<DetallesPagoServicio>> ObtenerTodosAsync();
    Task<List<DetallesPagoServicio>> ObtenerPorTransaccionAsync(int idTransaccion);

    Task<DetallesPagoServicio> RegistrarPagoServicioAsync(
        int idBilletera,
        decimal monto,
        string nombreServicio,
        string direccion,
        string estatus,
        string? refExternaId,
        int idProveedor,
        string numeroReferencia,
        string? codigoAutorizacion,
        Guid creationUser);
}
