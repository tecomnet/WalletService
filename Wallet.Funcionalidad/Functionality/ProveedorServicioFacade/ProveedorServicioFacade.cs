using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;

/// <summary>
/// Fachada para la gestión de proveedores de servicios y sus productos asociados.
/// Implementa la lógica de negocio para operaciones CRUD sobre proveedores y productos.
/// </summary>
public class ProveedorServicioFacade(ServiceDbContext context) : IProveedorServicioFacade
{
    /// <inheritdoc />
    public async Task<ProveedorServicio> GuardarProveedorServicioAsync(string nombre,
        DOM.Enums.ProductoCategoria categoria, string? urlIcono, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Crea una nueva instancia de ProveedorServicio.
            var proveedorServicio = new ProveedorServicio(nombre: nombre, categoria: categoria, urlIcono: urlIcono,
                creationUser: creationUser);
            // Agrega el proveedor al contexto.
            await context.ProveedorServicio.AddAsync(entity: proveedorServicio);
            // Guarda los cambios en la base de datos.
            await context.SaveChangesAsync();
            return proveedorServicio;
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
    public async Task<ProveedorServicio> ObtenerProveedorServicioPorIdAsync(int idProveedorServicio)
    {
        try
        {
            // Busca el proveedor por su ID, incluyendo sus productos asociados.
            var proveedorServicio = await context.ProveedorServicio
                .Include(navigationPropertyPath: p => p.Productos)
                .FirstOrDefaultAsync(predicate: x => x.Id == idProveedorServicio);

            // Si no se encuentra, lanza una excepción.
            if (proveedorServicio == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ProveedorServicioNoEncontrado,
                    dynamicContent: [idProveedorServicio],
                    module: this.GetType().Name));
            }

            return proveedorServicio;
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
    public async Task<ProveedorServicio> ActualizarProveedorServicioAsync(int idProveedorServicio, string nombre,
        Wallet.DOM.Enums.ProductoCategoria categoria, string? urlIcono, Guid modificationUser, string? testCase = null)
    {
        try
        {
            // Obtiene el proveedor existente.
            var proveedorServicio = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio: idProveedorServicio);
            // Actualiza los datos del proveedor.
            proveedorServicio.Update(nombre: nombre, categoria: categoria, urlIcono: urlIcono,
                modificationUser: modificationUser);

            // Guarda los cambios.
            await context.SaveChangesAsync();
            return proveedorServicio;
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
    public async Task<ProveedorServicio> EliminarProveedorServicioAsync(int idProveedorServicio, Guid modificationUser)
    {
        try
        {
            // Obtiene el proveedor existente.
            var proveedorServicio = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio: idProveedorServicio);
            // Desactiva el proveedor.
            proveedorServicio.Deactivate(modificationUser: modificationUser);
            // Guarda los cambios.
            await context.SaveChangesAsync();
            return proveedorServicio;
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
    public async Task<ProveedorServicio> ActivarProveedorServicioAsync(int idProveedorServicio, Guid modificationUser)
    {
        try
        {
            // Obtiene el proveedor existente.
            var proveedorServicio = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio: idProveedorServicio);
            // Activa el proveedor.
            proveedorServicio.Activate(modificationUser: modificationUser);
            // Guarda los cambios.
            await context.SaveChangesAsync();
            return proveedorServicio;
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
    public async Task<List<ProveedorServicio>> ObtenerProveedoresServicioAsync()
    {
        try
        {
            return await context.ProveedorServicio.ToListAsync();
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
    public async Task<ProductoProveedor> GuardarProductoAsync(int proveedorServicioId, string sku, string nombre,
        decimal monto, string descripcion, Guid creationUser)
    {
        try
        {
            // Obtiene el proveedor al que se asociará el producto.
            var proveedor = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio: proveedorServicioId);
            // Agrega el nuevo producto al proveedor.
            var producto = proveedor.AgregarProducto(sku: sku, nombre: nombre, monto: monto, descripcion: descripcion,
                creationUser: creationUser);

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
    public async Task<ProductoProveedor> ObtenerProductoPorIdAsync(int idProducto)
    {
        try
        {
            // Busca el producto por su ID.
            var producto = await context.ProductoProveedor
                .FirstOrDefaultAsync(predicate: x => x.Id == idProducto);

            // Si no se encuentra, lanza una excepción.
            if (producto == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ProductoProveedorNoEncontrado,
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
    public async Task<List<ProductoProveedor>> ObtenerProductosPorProveedorAsync(int proveedorServicioId)
    {
        try
        {
            // Verifica que el proveedor exista.
            await ObtenerProveedorServicioPorIdAsync(idProveedorServicio: proveedorServicioId);

            // Retorna los productos asociados al proveedor.
            return await context.ProductoProveedor
                .Where(predicate: x => x.ProveedorServicioId == proveedorServicioId)
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
    public async Task<ProductoProveedor> ActualizarProductoAsync(int idProducto, string sku, string nombre,
        decimal monto, string descripcion, Guid modificationUser)
    {
        try
        {
            // Obtiene el producto existente.
            var producto = await ObtenerProductoPorIdAsync(idProducto: idProducto);
            // Actualiza los datos del producto.
            producto.Update(sku: sku, nombre: nombre, monto: monto, descripcion: descripcion,
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
    public async Task<ProductoProveedor> EliminarProductoAsync(int idProducto, Guid modificationUser)
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
    public async Task<ProductoProveedor> ActivarProductoAsync(int idProducto, Guid modificationUser)
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
}
