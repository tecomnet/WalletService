using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ProveedorFacade;

public interface IProveedorFacade
{
    /// <summary>
    /// Guarda un nuevo proveedor en la base de datos.
    /// </summary>
    /// <param name="nombre">El nombre del proveedor.</param>
    /// <param name="brokerId">El identificador único del broker asociado.</param>
    /// <param name="creationUser">El identificador del usuario que crea el registro.</param>
    /// <param name="testCase">Caso de prueba (opcional).</param>
    /// <returns>El objeto <see cref="Proveedor"/> guardado.</returns>
    Task<Proveedor> GuardarProveedorAsync(string nombre, int brokerId, Guid creationUser, string? testCase = null);

    /// <summary>
    /// Obtiene un proveedor por su identificador único.
    /// </summary>
    /// <param name="idProveedor">El identificador único del proveedor.</param>
    /// <returns>El objeto <see cref="Proveedor"/> encontrado.</returns>
    Task<Proveedor> ObtenerProveedorPorIdAsync(int idProveedor);

    /// <summary>
    /// Actualiza la información de un proveedor existente.
    /// </summary>
    /// <param name="idProveedor">El identificador único del proveedor a actualizar.</param>
    /// <param name="nombre">El nuevo nombre del proveedor.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
    /// <param name="testCase">Caso de prueba (opcional).</param>
    /// <returns>El objeto <see cref="Proveedor"/> actualizado.</returns>
    Task<Proveedor> ActualizarProveedorAsync(int idProveedor, string nombre, Guid modificationUser,
        string? testCase = null);

    /// <summary>
    /// Elimina (lógicamente) un proveedor del sistema.
    /// </summary>
    /// <param name="idProveedor">El identificador único del proveedor a eliminar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la eliminación.</param>
    /// <returns>El objeto <see cref="Proveedor"/> eliminado.</returns>
    Task<Proveedor> EliminarProveedorAsync(int idProveedor, Guid modificationUser);

    /// <summary>
    /// Activa un proveedor previamente eliminado o desactivado.
    /// </summary>
    /// <param name="idProveedor">El identificador único del proveedor a activar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la activación.</param>
    /// <returns>El objeto <see cref="Proveedor"/> activado.</returns>
    Task<Proveedor> ActivarProveedorAsync(int idProveedor, Guid modificationUser);

    /// <summary>
    /// Obtiene una lista de todos los proveedores registrados.
    /// </summary>
    /// <returns>Una lista de objetos <see cref="Proveedor"/>.</returns>
    Task<List<Proveedor>> ObtenerProveedoresAsync();

    /// <summary>
    /// Agrega un nuevo producto a un proveedor existente.
    /// </summary>
    /// <param name="proveedorId">El identificador del proveedor al que se asociará el producto.</param>
    /// <param name="sku">El SKU del producto.</param>
    /// <param name="nombre">El nombre del producto.</param>
    /// <param name="precio">El precio del producto.</param>
    /// <param name="icono">El ícono del producto.</param>
    /// <param name="categoria">La categoría del producto.</param>
    /// <param name="creationUser">El identificador del usuario que crea el producto.</param>
    /// <returns>El objeto <see cref="Producto"/> guardado.</returns>
    Task<Producto> GuardarProductoAsync(int proveedorId, string sku, string nombre, decimal precio, string icono,
        string categoria, Guid creationUser);

    /// <summary>
    /// Obtiene un producto por su identificador único.
    /// </summary>
    /// <param name="idProducto">El identificador único del producto.</param>
    /// <returns>El objeto <see cref="Producto"/> encontrado.</returns>
    Task<Producto> ObtenerProductoPorIdAsync(int idProducto);

    /// <summary>
    /// Obtiene todos los productos asociados a un proveedor específico.
    /// </summary>
    /// <param name="proveedorId">El identificador del proveedor.</param>
    /// <returns>Una lista de objetos <see cref="Producto"/>.</returns>
    Task<List<Producto>> ObtenerProductosPorProveedorAsync(int proveedorId);

    /// <summary>
    /// Actualiza la información de un producto existente.
    /// </summary>
    /// <param name="idProducto">El identificador único del producto a actualizar.</param>
    /// <param name="sku">El nuevo SKU del producto.</param>
    /// <param name="nombre">El nuevo nombre del producto.</param>
    /// <param name="precio">El nuevo precio del producto.</param>
    /// <param name="icono">El nuevo ícono del producto.</param>
    /// <param name="categoria">La nueva categoría del producto.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
    /// <returns>El objeto <see cref="Producto"/> actualizado.</returns>
    Task<Producto> ActualizarProductoAsync(int idProducto, string sku, string nombre, decimal precio, string icono,
        string categoria, Guid modificationUser);

    /// <summary>
    /// Elimina (lógicamente) un producto del sistema.
    /// </summary>
    /// <param name="idProducto">El identificador único del producto a eliminar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la eliminación.</param>
    /// <returns>El objeto <see cref="Producto"/> eliminado.</returns>
    Task<Producto> EliminarProductoAsync(int idProducto, Guid modificationUser);

    /// <summary>
    /// Activa un producto previamente eliminado o desactivado.
    /// </summary>
    /// <param name="idProducto">El identificador único del producto a activar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la activación.</param>
    /// <returns>El objeto <see cref="Producto"/> activado.</returns>
    Task<Producto> ActivarProductoAsync(int idProducto, Guid modificationUser);

    /// <summary>
    /// Obtiene todos los productos registrados.
    /// </summary>
    /// <returns>Una lista de todos los productos.</returns>
    Task<List<Producto>> ObtenerProductosAsync();
}
