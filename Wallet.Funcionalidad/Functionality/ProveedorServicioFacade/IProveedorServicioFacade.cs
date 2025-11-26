using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;

public interface IProveedorServicioFacade
{
    /// <summary>
    /// Guarda un nuevo proveedor de servicio
    /// </summary>
    /// <param name="nombre"></param>
    /// <param name="categoria"></param>
    /// <param name="urlIcono"></param>
    /// <param name="creationUser"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<ProveedorServicio> GuardarProveedorServicioAsync(string nombre, Wallet.DOM.Enums.ProductoCategoria categoria, string? urlIcono, Guid creationUser, string? testCase = null);

    /// <summary>
    /// Obtiene un proveedor de servicio por su Id
    /// </summary>
    /// <param name="idProveedorServicio"></param>
    /// <returns></returns>
    Task<ProveedorServicio> ObtenerProveedorServicioPorIdAsync(int idProveedorServicio);

    /// <summary>
    /// Actualiza un proveedor de servicio existente
    /// </summary>
    /// <param name="idProveedorServicio"></param>
    /// <param name="nombre"></param>
    /// <param name="categoria"></param>
    /// <param name="urlIcono"></param>
    /// <param name="modificationUser"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    Task<ProveedorServicio> ActualizarProveedorServicioAsync(int idProveedorServicio, string nombre, Wallet.DOM.Enums.ProductoCategoria categoria, string? urlIcono, Guid modificationUser, string? testCase = null);

    /// <summary>
    /// Elimina (desactiva) un proveedor de servicio
    /// </summary>
    /// <param name="idProveedorServicio"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    Task<ProveedorServicio> EliminarProveedorServicioAsync(int idProveedorServicio, Guid modificationUser);

    /// <summary>
    /// Activa un proveedor de servicio
    /// </summary>
    /// <param name="idProveedorServicio"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    Task<ProveedorServicio> ActivarProveedorServicioAsync(int idProveedorServicio, Guid modificationUser);

    /// <summary>
    /// Obtiene todos los proveedores de servicio
    /// </summary>
    /// <returns></returns>
    Task<List<ProveedorServicio>> ObtenerProveedoresServicioAsync();
}
