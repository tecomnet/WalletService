using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IClienteFacade
{
    /// <summary>
    /// Obtiene un cliente específico por su identificador único.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Cliente"/> encontrado.</returns>
    public Task<Cliente> ObtenerClientePorIdAsync(int idCliente);


    /// <summary>
    /// Actualiza los datos personales de un cliente existente.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente a actualizar.</param>
    /// <param name="nombre">El nuevo nombre del cliente.</param>
    /// <param name="primerApellido">El nuevo primer apellido del cliente.</param>
    /// <param name="segundoApellido">El nuevo segundo apellido del cliente.</param>
    /// <param name="nombreEstado">El nombre del estado de residencia del cliente.</param>
    /// <param name="fechaNacimiento">La fecha de nacimiento del cliente.</param>
    /// <param name="genero">El género del cliente.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
    /// <param name="testCase">Opcional. Un identificador para casos de prueba.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Cliente"/> actualizado.</returns>
    public Task<Cliente> ActualizarClienteDatosPersonalesAsync(int idCliente, string nombre, string primerApellido,
        string segundoApellido, string nombreEstado, DateOnly fechaNacimiento, Genero genero, Guid modificationUser,
        string? testCase = null);


    /// <summary>
    /// Elimina (desactiva lógicamente) un cliente por su identificador.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente a eliminar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la eliminación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Cliente"/> eliminado.</returns>
    public Task<Cliente> EliminarClienteAsync(int idCliente, Guid modificationUser);

    /// <summary>
    /// Activa un cliente previamente desactivado por su identificador.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente a activar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la activación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Cliente"/> activado.</returns>
    public Task<Cliente> ActivarClienteAsync(int idCliente, Guid modificationUser);

    /// <summary>
    /// Obtiene una lista de todos los clientes registrados en el sistema.
    /// </summary>
    /// <returns>Una tarea que representa la operación asíncrona, con una lista de objetos <see cref="Cliente"/>.</returns>
    public Task<List<Cliente>> ObtenerClientesAsync();

    /// <summary>
    /// Obtiene la lista de servicios favoritos asociados a un cliente.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con una lista de objetos <see cref="ServicioFavorito"/>.</returns>
    public Task<List<ServicioFavorito>> ObtenerServiciosFavoritosAsync(int idCliente);
}
    
