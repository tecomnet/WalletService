using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionCliente;

namespace Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;

public interface IServicioFavoritoFacade
{
    /// <summary>
    /// Guarda un nuevo servicio favorito para un cliente.
    /// </summary>
    /// <param name="clienteId">El identificador único del cliente.</param>
    /// <param name="proveedorId">El identificador único del proveedor de servicio.</param>
    /// <param name="alias">El alias o nombre personalizado asignado al servicio.</param>
    /// <param name="numeroReferencia">El número de referencia del servicio.</param>
    /// <param name="creationUser">El identificador del usuario que crea el favorito.</param>
    /// <param name="testCase">Caso de prueba (opcional).</param>
    /// <returns>El servicio favorito guardado.</returns>
    Task<ServicioFavorito> GuardarServicioFavoritoAsync(int clienteId, int proveedorId, string alias,
        string numeroReferencia, Guid creationUser, string? testCase = null);

    /// <summary>
    /// Obtiene un servicio favorito por su identificador único.
    /// </summary>
    /// <param name="idServicioFavorito">El identificador único del servicio favorito.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="ServicioFavorito"/> encontrado.</returns>
    Task<ServicioFavorito> ObtenerServicioFavoritoPorIdAsync(int idServicioFavorito);

    /// <summary>
    /// Actualiza los datos de un servicio favorito existente.
    /// </summary>
    /// <param name="idServicioFavorito">El identificador único del servicio favorito a actualizar.</param>
    /// <param name="alias">El nuevo alias para el servicio favorito.</param>
    /// <param name="numeroReferencia">El nuevo número de referencia.</param>
    /// <param name="concurrencyToken">Token de concurrencia para control de versiones optimista.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
    /// <param name="testCase">Opcional. Un identificador para casos de prueba.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="ServicioFavorito"/> actualizado.</returns>
    Task<ServicioFavorito> ActualizarServicioFavoritoAsync(int idServicioFavorito, string alias,
        string numeroReferencia, string? concurrencyToken, Guid modificationUser, string? testCase = null);

    /// <summary>
    /// Elimina (desactiva lógicamente) un servicio favorito.
    /// </summary>
    /// <param name="idServicioFavorito">El identificador único del servicio favorito a eliminar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la eliminación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="ServicioFavorito"/> eliminado.</returns>
    Task<ServicioFavorito> EliminarServicioFavoritoAsync(int idServicioFavorito, Guid modificationUser);

    /// <summary>
    /// Activa un servicio favorito previamente desactivado.
    /// </summary>
    /// <param name="idServicioFavorito">El identificador único del servicio favorito a activar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la activación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="ServicioFavorito"/> activado.</returns>
    Task<ServicioFavorito> ActivarServicioFavoritoAsync(int idServicioFavorito, Guid modificationUser);

    /// <summary>
    /// Obtiene una lista de todos los servicios favoritos asociados a un cliente.
    /// </summary>
    /// <param name="clienteId">El identificador único del cliente.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con una lista de objetos <see cref="ServicioFavorito"/>.</returns>
    Task<List<ServicioFavorito>> ObtenerServiciosFavoritosPorClienteAsync(int clienteId);
}
