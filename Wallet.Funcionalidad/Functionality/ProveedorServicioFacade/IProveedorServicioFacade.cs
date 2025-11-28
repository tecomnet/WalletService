using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;

public interface IProveedorServicioFacade
{
    /// <summary>
    /// Guarda un nuevo proveedor de servicio.
    /// </summary>
    /// <param name="nombre">El nombre del proveedor de servicio.</param>
    /// <param name="categoria">La categoría del producto asociado al proveedor.</param>
    /// <param name="urlIcono">La URL del icono del proveedor de servicio (opcional).</param>
    /// <param name="creationUser">El identificador del usuario que crea el proveedor.</param>
    /// <param name="testCase">Caso de prueba (opcional).</param>
    /// <returns>El proveedor de servicio guardado.</returns>
    Task<ProveedorServicio> GuardarProveedorServicioAsync(string nombre, Wallet.DOM.Enums.ProductoCategoria categoria,
        string? urlIcono, Guid creationUser, string? testCase = null);

    /// <summary>
    /// Obtiene un proveedor de servicio por su identificador.
    /// </summary>
    /// <param name="idProveedorServicio">El identificador del proveedor de servicio.</param>
    /// <returns>El proveedor de servicio encontrado.</returns>
    Task<ProveedorServicio> ObtenerProveedorServicioPorIdAsync(int idProveedorServicio);

    /// <summary>
    /// Actualiza un proveedor de servicio existente.
    /// </summary>
    /// <param name="idProveedorServicio">El identificador del proveedor de servicio a actualizar.</param>
    /// <param name="nombre">El nuevo nombre del proveedor de servicio.</param>
    /// <param name="categoria">La nueva categoría del producto asociado al proveedor.</param>
    /// <param name="urlIcono">La nueva URL del icono del proveedor de servicio (opcional).</param>
    /// <param name="modificationUser">El identificador del usuario que modifica el proveedor.</param>
    /// <param name="testCase">Caso de prueba (opcional).</param>
    /// <returns>El proveedor de servicio actualizado.</returns>
    Task<ProveedorServicio> ActualizarProveedorServicioAsync(int idProveedorServicio, string nombre,
        Wallet.DOM.Enums.ProductoCategoria categoria, string? urlIcono, Guid modificationUser, string? testCase = null);

    /// <summary>
    /// Elimina (desactiva) lógicamente un proveedor de servicio.
    /// </summary>
    /// <param name="idProveedorServicio">El identificador del proveedor de servicio a eliminar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la eliminación.</param>
    /// <returns>El proveedor de servicio eliminado (desactivado).</returns>
    Task<ProveedorServicio> EliminarProveedorServicioAsync(int idProveedorServicio, Guid modificationUser);

    /// <summary>
    /// Activa un proveedor de servicio previamente desactivado.
    /// </summary>
    /// <param name="idProveedorServicio">El identificador del proveedor de servicio a activar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la activación.</param>
    /// <returns>El proveedor de servicio activado.</returns>
    Task<ProveedorServicio> ActivarProveedorServicioAsync(int idProveedorServicio, Guid modificationUser);

    /// <summary>
    /// Obtiene una lista de todos los proveedores de servicio.
    /// </summary>
    /// <returns>Una lista de objetos ProveedorServicio.</returns>
    Task<List<ProveedorServicio>> ObtenerProveedoresServicioAsync();

    /// <summary>
    /// Guarda un nuevo producto asociado a un proveedor de servicio.
    /// </summary>
    /// <param name="proveedorServicioId">El identificador del proveedor de servicio al que se asocia el producto.</param>
    /// <param name="sku">El SKU (Stock Keeping Unit) del producto.</param>
    /// <param name="nombre">El nombre del producto.</param>
    /// <param name="monto">El monto o precio del producto.</param>
    /// <param name="descripcion">La descripción del producto.</param>
    /// <param name="creationUser">El identificador del usuario que crea el producto.</param>
    /// <returns>El producto de proveedor guardado.</returns>
    Task<ProductoProveedor> GuardarProductoAsync(int proveedorServicioId, string sku, string nombre, decimal monto,
        string descripcion, Guid creationUser);

    /// <summary>
    /// Obtiene un producto de proveedor por su identificador.
    /// </summary>
    /// <param name="idProducto">El identificador del producto.</param>
    /// <returns>El producto de proveedor encontrado.</returns>
    Task<ProductoProveedor> ObtenerProductoPorIdAsync(int idProducto);

    /// <summary>
    /// Obtiene una lista de productos asociados a un proveedor de servicio específico.
    /// </summary>
    /// <param name="proveedorServicioId">El identificador del proveedor de servicio.</param>
    /// <returns>Una lista de objetos ProductoProveedor.</returns>
    Task<List<ProductoProveedor>> ObtenerProductosPorProveedorAsync(int proveedorServicioId);

    /// <summary>
    /// Actualiza un producto de proveedor existente.
    /// </summary>
    /// <param name="idProducto">El identificador del producto a actualizar.</param>
    /// <param name="sku">El nuevo SKU del producto.</param>
    /// <param name="nombre">El nuevo nombre del producto.</param>
    /// <param name="monto">El nuevo monto o precio del producto.</param>
    /// <param name="descripcion">La nueva descripción del producto.</param>
    /// <param name="modificationUser">El identificador del usuario que modifica el producto.</param>
    /// <returns>El producto de proveedor actualizado.</returns>
    Task<ProductoProveedor> ActualizarProductoAsync(int idProducto, string sku, string nombre, decimal monto,
        string descripcion, Guid modificationUser);

    /// <summary>
    /// Elimina (desactiva) lógicamente un producto de proveedor.
    /// </summary>
    /// <param name="idProducto">El identificador del producto a eliminar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la eliminación.</param>
    /// <returns>El producto de proveedor eliminado (desactivado).</returns>
    Task<ProductoProveedor> EliminarProductoAsync(int idProducto, Guid modificationUser);

    /// <summary>
    /// Activa un producto de proveedor previamente desactivado.
    /// </summary>
    /// <param name="idProducto">El identificador del producto a activar.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la activación.</param>
    /// <returns>El producto de proveedor activado.</returns>
    Task<ProductoProveedor> ActivarProductoAsync(int idProducto, Guid modificationUser);
}
