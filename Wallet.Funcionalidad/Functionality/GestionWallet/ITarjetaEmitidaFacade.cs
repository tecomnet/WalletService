using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.GestionWallet;

public interface ITarjetaEmitidaFacade
{
    Task<TarjetaEmitida> CrearTarjetaInicialAsync(int idCuentaWallet, Guid creationUser);
    Task<TarjetaEmitida> ObtenerTarjetaPorIdAsync(int idTarjeta);
    Task<List<TarjetaEmitida>> ObtenerTarjetasPorClienteAsync(int idCliente);
    Task CambiarEstadoBloqueoAsync(int idTarjeta, bool bloquear, string concurrencyToken, Guid modificationUser);

    Task ActualizarConfiguracionAsync(int idTarjeta, decimal nuevoLimite, bool comprasLinea, bool retiros,
        string concurrencyToken, Guid modificationUser);

    Task<TarjetaEmitida> SolicitarTarjetaVirtualAdicionalAsync(int idCliente, Guid creationUser);
    Task<TarjetaEmitida> SolicitarTarjetaFisicaAsync(int idCliente, string nombreImpreso, Guid creationUser);
}
