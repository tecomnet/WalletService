using Wallet.DOM.Modelos.GestionCliente;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IEstadoFacade
{
    /// <summary>
    /// Obtiene un estado por su identificador único de forma asíncrona.
    /// </summary>
    /// <param name="idEstado">El identificador del estado a buscar.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea es el objeto Estado si se encuentra, de lo contrario null.</returns>
    public Task<Estado> ObtenerEstadoPorIdAsync(int idEstado);

    /// <summary>
    /// Obtiene un estado por su nombre de forma asíncrona.
    /// </summary>
    /// <param name="nombre">El nombre del estado a buscar.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea es el objeto Estado si se encuentra, de lo contrario null.</returns>
    public Task<Estado> ObtenerEstadoPorNombreAsync(string nombre);

    /// <summary>
    /// Obtiene una lista de todos los estados de forma asíncrona.
    /// </summary>
    /// <param name="activo">Parámetro opcional para filtrar estados activos (true), inactivos (false) o todos (null).</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea es una lista de objetos Estado.</returns>
    public Task<List<Estado>> ObtenerTodosAsync(bool? activo = null);

    /// <summary>
    /// Guarda un nuevo estado de forma asíncrona.
    /// </summary>
    /// <param name="nombre">El nombre del estado a guardar.</param>
    /// <param name="creationUser">El identificador del usuario que crea el estado.</param>
    /// <param name="testCase">Parámetro opcional para casos de prueba.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea es el objeto Estado guardado.</returns>
    public Task<Estado> GuardarEstadoAsync(string nombre, Guid creationUser, string? testCase = null);

    /// <summary>
    /// Actualiza un estado existente de forma asíncrona.
    /// </summary>
    /// <param name="idEstado">El identificador del estado a actualizar.</param>
    /// <param name="nombre">El nuevo nombre para el estado.</param>
    /// <param name="modificationUser">El identificador del usuario que modifica el estado.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea es el objeto Estado actualizado.</returns>
    public Task<Estado> ActualizaEstadoAsync(int idEstado, string nombre, string? concurrencyToken,
        Guid modificationUser);

    /// <summary>
    /// Elimina (lógicamente) un estado de forma asíncrona.
    /// </summary>
    /// <param name="idEstado">El identificador del estado a eliminar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la eliminación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea es el objeto Estado marcado como inactivo.</returns>
    public Task<Estado> EliminaEstadoAsync(int idEstado, Guid modificationUser);

    /// <summary>
    /// Activa un estado previamente eliminado (lógicamente) de forma asíncrona.
    /// </summary>
    /// <param name="idEstado">El identificador del estado a activar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la activación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea es el objeto Estado marcado como activo.</returns>
    public Task<Estado> ActivaEstadoAsync(int idEstado, Guid modificationUser);
}