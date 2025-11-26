using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;

public interface IServicioFavoritoFacade
{
    /// <summary>
    /// Guarda un nuevo servicio favorito
    /// </summary>
    /// <param name="clienteId"></param>
    /// <param name="proveedorServicioId"></param>
    /// <param name="alias"></param>
    /// <param name="numeroReferencia"></param>
    /// <param name="creationUser"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<ServicioFavorito> GuardarServicioFavoritoAsync(int clienteId, int proveedorServicioId, string alias, string numeroReferencia, Guid creationUser, string? testCase = null);

    /// <summary>
    /// Obtiene un servicio favorito por su Id
    /// </summary>
    /// <param name="idServicioFavorito"></param>
    /// <returns></returns>
    Task<ServicioFavorito> ObtenerServicioFavoritoPorIdAsync(int idServicioFavorito);

    /// <summary>
    /// Actualiza un servicio favorito existente
    /// </summary>
    /// <param name="idServicioFavorito"></param>
    /// <param name="alias"></param>
    /// <param name="numeroReferencia"></param>
    /// <param name="modificationUser"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<ServicioFavorito> ActualizarServicioFavoritoAsync(int idServicioFavorito, string alias, string numeroReferencia, Guid modificationUser, string? testCase = null);

    /// <summary>
    /// Elimina (desactiva) un servicio favorito
    /// </summary>
    /// <param name="idServicioFavorito"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    Task<ServicioFavorito> EliminarServicioFavoritoAsync(int idServicioFavorito, Guid modificationUser);

    /// <summary>
    /// Activa un servicio favorito
    /// </summary>
    /// <param name="idServicioFavorito"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    Task<ServicioFavorito> ActivarServicioFavoritoAsync(int idServicioFavorito, Guid modificationUser);

    /// <summary>
    /// Obtiene los servicios favoritos de un cliente
    /// </summary>
    /// <param name="clienteId"></param>
    /// <returns></returns>
    Task<List<ServicioFavorito>> ObtenerServiciosFavoritosPorClienteAsync(int clienteId);
}
