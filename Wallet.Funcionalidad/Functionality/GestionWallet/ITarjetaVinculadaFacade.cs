using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.GestionWallet;

public interface ITarjetaVinculadaFacade
{
    Task<TarjetaVinculada> VincularTarjetaAsync(int idCliente, string tokenPasarela, string panEnmascarado,
        string alias, DOM.Enums.MarcaTarjeta marca, DateTime fechaExpiracion, Guid creationUser,
        string? gatewayCustomerId = null);

    Task<List<TarjetaVinculada>> ObtenerTarjetasPorClienteAsync(int idCliente);
    Task DesvincularTarjetaAsync(int idTarjeta, Guid modificationUser);
    Task EstablecerFavoritaAsync(int idTarjeta, int idCliente, string concurrencyToken, Guid modificationUser);
}
