using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IEmpresaFacade
{
    /// <summary>
    /// Obtiene una empresa por su identificador único.
    /// </summary>
    /// <param name="idEmpresa">El identificador único de la empresa.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Empresa"/> encontrado.</returns>
    public Task<Empresa> ObtenerPorIdAsync(int idEmpresa);

    /// <summary>
    /// Obtiene una empresa por su nombre.
    /// </summary>
    /// <param name="nombre">El nombre de la empresa a buscar.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Empresa"/> encontrado.</returns>
    public Task<Empresa> ObtenerPorNombreAsync(string nombre);

    /// <summary>
    /// Obtiene una lista de todas las empresas registradas.
    /// </summary>
    /// <returns>Una tarea que representa la operación asíncrona, con una lista de objetos <see cref="Empresa"/>.</returns>
    public Task<List<Empresa>> ObtenerTodasAsync();

    /// <summary>
    /// Guarda una nueva empresa en el sistema.
    /// </summary>
    /// <param name="nombre">El nombre de la nueva empresa.</param>
    /// <param name="creationUser">El identificador del usuario que crea la empresa.</param>
    /// <param name="testCase">Opcional. Un identificador para casos de prueba.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Empresa"/> creado.</returns>
    public Task<Empresa> GuardarEmpresaAsync(string nombre, Guid creationUser, string? testCase = null);

    /// <summary>
    /// Actualiza el nombre de una empresa existente.
    /// </summary>
    /// <param name="idEmpresa">El identificador único de la empresa a actualizar.</param>
    /// <param name="nombre">El nuevo nombre de la empresa.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Empresa"/> actualizado.</returns>
    public Task<Empresa> ActualizaEmpresaAsync(int idEmpresa, string nombre, string concurrencyToken,
        Guid modificationUser);

    /// <summary>
    /// Elimina (desactiva lógicamente) una empresa por su identificador.
    /// </summary>
    /// <param name="idEmpresa">El identificador único de la empresa a eliminar.</param>
    /// <param name="concurrencyToken">El token de concurrencia para validar que la entidad no haya sido modificada.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la eliminación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Empresa"/> eliminado.</returns>
    public Task<Empresa> EliminaEmpresaAsync(int idEmpresa, string concurrencyToken, Guid modificationUser);

    /// <summary>
    /// Activa una empresa previamente desactivada por su identificador.
    /// </summary>
    /// <param name="idEmpresa">El identificador único de la empresa a activar.</param>
    /// <param name="concurrencyToken">El token de concurrencia para validar que la entidad no haya sido modificada.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la activación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Empresa"/> activado.</returns>
    public Task<Empresa> ActivaEmpresaAsync(int idEmpresa, string concurrencyToken, Guid modificationUser);

    /// <summary>
    /// Obtiene la lista de productos asociados a una empresa.
    /// </summary>
    /// <param name="idEmpresa">El identificador único de la empresa.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con una lista de objetos <see cref="Producto"/>.</returns>
    public Task<List<Producto>> ObtenerProductosPorEmpresaAsync(int idEmpresa);

    /// <summary>
    /// Obtiene la lista de clientes asociados a una empresa.
    /// </summary>
    /// <param name="idEmpresa">El identificador único de la empresa.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con una lista de objetos <see cref="Cliente"/>.</returns>
    public Task<List<Cliente>> ObtenerClientesPorEmpresaAsync(int idEmpresa);

    /// <summary>
    /// Asigna una lista de productos a una empresa.
    /// Si un producto ya está asignado, se ignora (idempotencia).
    /// </summary>
    /// <param name="idEmpresa">El identificador de la empresa.</param>
    /// <param name="idsProductos">La lista de identificadores de productos a asignar.</param>
    /// <param name="modificationUser">El usuario que realiza la modificación.</param>
    /// <returns>La empresa con los productos actualizados.</returns>
    public Task<Empresa> AsignarProductosAsync(int idEmpresa, List<int> idsProductos, Guid modificationUser);

    /// <summary>
    /// Desasigna una lista de productos de una empresa.
    /// Si un producto no está asignado, se ignora.
    /// </summary>
    /// <param name="idEmpresa">El identificador de la empresa.</param>
    /// <param name="idsProductos">La lista de identificadores de productos a desasignar.</param>
    /// <param name="modificationUser">El usuario que realiza la modificación.</param>
    /// <returns>La empresa con los productos actualizados.</returns>
    public Task<Empresa> DesasignarProductosAsync(int idEmpresa, List<int> idsProductos, Guid modificationUser);
}