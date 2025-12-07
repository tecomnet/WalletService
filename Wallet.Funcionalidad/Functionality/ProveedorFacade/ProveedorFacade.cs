using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ProveedorFacade;

/// <summary>
/// Fachada para la gestión de proveedores de servicios y sus productos asociados.
/// Implementa la lógica de negocio para operaciones CRUD sobre proveedores y productos.
/// </summary>
public class ProveedorFacade(ServiceDbContext context) : IProveedorFacade
{
    /// <inheritdoc />
    public async Task<Proveedor> GuardarProveedorAsync(string nombre, int brokerId, Guid creationUser,
        string? testCase = null)
    {
        try
        {
            // Busca el broker asociado.
            var broker = await context.Broker.FirstOrDefaultAsync(x => x.Id == brokerId);
            if (broker == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.BrokerNoEncontrado,
                    dynamicContent: [brokerId],
                    module: this.GetType().Name));
            }

            // Crea una nueva instancia de Proveedor.
            var proveedor = new Proveedor(nombre: nombre, broker: broker, creationUser: creationUser);

            // Agrega el proveedor al contexto.
            await context.Proveedor.AddAsync(entity: proveedor);

            // Guarda los cambios en la base de datos.
            await context.SaveChangesAsync();
            return proveedor;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Captura cualquier excepción no controlada y la envuelve en una EMGeneralAggregateException.
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Proveedor> ObtenerProveedorPorIdAsync(int idProveedor)
    {
        try
        {
            // Busca el proveedor por su ID, incluyendo sus productos asociados.
            var proveedor = await context.Proveedor
                .Include(navigationPropertyPath: p => p.Productos)
                .FirstOrDefaultAsync(predicate: x => x.Id == idProveedor);

            // Si no se encuentra, lanza una excepción.
            if (proveedor == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ProveedorNoEncontrado,
                    dynamicContent: [idProveedor],
                    module: this.GetType().Name));
            }

            return proveedor;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Proveedor> ActualizarProveedorAsync(int idProveedor, string nombre, Guid modificationUser,
        string? testCase = null)
    {
        try
        {
            // Obtiene el proveedor existente.
            var proveedor = await ObtenerProveedorPorIdAsync(idProveedor: idProveedor);
            // Actualiza los datos del proveedor.
            proveedor.Update(nombre: nombre, modificationUser: modificationUser);

            // Guarda los cambios.
            await context.SaveChangesAsync();
            return proveedor;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Proveedor> EliminarProveedorAsync(int idProveedor, Guid modificationUser)
    {
        try
        {
            // Obtiene el proveedor existente.
            var proveedor = await ObtenerProveedorPorIdAsync(idProveedor: idProveedor);
            // Desactiva el proveedor.
            proveedor.Deactivate(modificationUser: modificationUser);
            // Guarda los cambios.
            await context.SaveChangesAsync();
            return proveedor;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Proveedor> ActivarProveedorAsync(int idProveedor, Guid modificationUser)
    {
        try
        {
            // Obtiene el proveedor existente.
            var proveedor = await ObtenerProveedorPorIdAsync(idProveedor: idProveedor);
            // Activa el proveedor.
            proveedor.Activate(modificationUser: modificationUser);
            // Guarda los cambios.
            await context.SaveChangesAsync();
            return proveedor;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<List<Proveedor>> ObtenerProveedoresAsync()
    {
        try
        {
            return await context.Proveedor.ToListAsync();
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Producto> GuardarProductoAsync(int proveedorId, string sku, string nombre,
        decimal precio, string icono, string categoria, Guid creationUser)
    {
        try
        {
            // Obtiene el proveedor al que se asociará el producto.
            var proveedor = await ObtenerProveedorPorIdAsync(idProveedor: proveedorId);
            // Agrega el nuevo producto al proveedor.
            var producto = proveedor.AgregarProducto(sku: sku, nombre: nombre, precio: precio, icono: icono,
                categoria: categoria, creationUser: creationUser);

            // Guarda los cambios.
            await context.SaveChangesAsync();
            return producto;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Producto> ObtenerProductoPorIdAsync(int idProducto)
    {
        try
        {
            // Busca el producto por su ID.
            var producto = await context.Producto
                .FirstOrDefaultAsync(predicate: x => x.Id == idProducto);

            // Si no se encuentra, lanza una excepción.
            if (producto == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ProductoNoEncontrado,
                    dynamicContent: [idProducto],
                    module: this.GetType().Name));
            }

            return producto;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<List<Producto>> ObtenerProductosPorProveedorAsync(int proveedorId)
    {
        try
        {
            // Verifica que el proveedor exista.
            await ObtenerProveedorPorIdAsync(idProveedor: proveedorId);

            // Retorna los productos asociados al proveedor.
            return await context.Producto
                .Where(predicate: x => x.ProveedorId == proveedorId)
                .ToListAsync();
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Producto> ActualizarProductoAsync(int idProducto, string sku, string nombre,
        decimal precio, string icono, string categoria, Guid modificationUser)
    {
        try
        {
            // Obtiene el producto existente.
            var producto = await ObtenerProductoPorIdAsync(idProducto: idProducto);
            // Actualiza los datos del producto.
            producto.Update(sku: sku, nombre: nombre, precio: precio, icono: icono, categoria: categoria,
                modificationUser: modificationUser);

            // Guarda los cambios.
            await context.SaveChangesAsync();
            return producto;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Producto> EliminarProductoAsync(int idProducto, Guid modificationUser)
    {
        try
        {
            // Obtiene el producto existente.
            var producto = await ObtenerProductoPorIdAsync(idProducto: idProducto);
            // Desactiva el producto.
            producto.Deactivate(modificationUser: modificationUser);

            // Guarda los cambios.
            await context.SaveChangesAsync();
            return producto;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Producto> ActivarProductoAsync(int idProducto, Guid modificationUser)
    {
        try
        {
            // Obtiene el producto existente.
            var producto = await ObtenerProductoPorIdAsync(idProducto: idProducto);
            // Activa el producto.
            producto.Activate(modificationUser: modificationUser);

            // Guarda los cambios.
            await context.SaveChangesAsync();
            return producto;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        try
        {
            return await context.Producto.ToListAsync();
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }
}
