using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;

public class ProveedorServicioFacade(ServiceDbContext context) : IProveedorServicioFacade
{
    public async Task<ProveedorServicio> GuardarProveedorServicioAsync(string nombre,
        DOM.Enums.ProductoCategoria categoria, string? urlIcono, Guid creationUser, string? testCase = null)
    {
        try
        {
            var proveedorServicio = new ProveedorServicio(nombre: nombre, categoria: categoria, urlIcono: urlIcono, creationUser: creationUser);
            await context.ProveedorServicio.AddAsync(entity: proveedorServicio);
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

    public async Task<ProveedorServicio> ObtenerProveedorServicioPorIdAsync(int idProveedorServicio)
    {
        try
        {
            var proveedorServicio = await context.ProveedorServicio
                .Include(navigationPropertyPath: p => p.Productos)
                .FirstOrDefaultAsync(predicate: x => x.Id == idProveedorServicio);

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

    public async Task<ProveedorServicio> ActualizarProveedorServicioAsync(int idProveedorServicio, string nombre,
        Wallet.DOM.Enums.ProductoCategoria categoria, string? urlIcono, Guid modificationUser, string? testCase = null)
    {
        try
        {
            var proveedorServicio = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio: idProveedorServicio);
            proveedorServicio.Update(nombre: nombre, categoria: categoria, urlIcono: urlIcono, modificationUser: modificationUser);

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

    public async Task<ProveedorServicio> EliminarProveedorServicioAsync(int idProveedorServicio, Guid modificationUser)
    {
        try
        {
            var proveedorServicio = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio: idProveedorServicio);
            proveedorServicio.Deactivate(modificationUser: modificationUser);
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

    public async Task<ProveedorServicio> ActivarProveedorServicioAsync(int idProveedorServicio, Guid modificationUser)
    {
        try
        {
            var proveedorServicio = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio: idProveedorServicio);
            proveedorServicio.Activate(modificationUser: modificationUser);
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

    public async Task<ProductoProveedor> GuardarProductoAsync(int proveedorServicioId, string sku, string nombre,
        decimal monto, string descripcion, Guid creationUser)
    {
        try
        {
            var proveedor = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio: proveedorServicioId);
            var producto = proveedor.AgregarProducto(sku: sku, nombre: nombre, monto: monto, descripcion: descripcion, creationUser: creationUser);

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

    public async Task<ProductoProveedor> ObtenerProductoPorIdAsync(int idProducto)
    {
        try
        {
            var producto = await context.ProductoProveedor
                .FirstOrDefaultAsync(predicate: x => x.Id == idProducto);

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

    public async Task<List<ProductoProveedor>> ObtenerProductosPorProveedorAsync(int proveedorServicioId)
    {
        try
        {
            // Verify provider exists
            await ObtenerProveedorServicioPorIdAsync(idProveedorServicio: proveedorServicioId);

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

    public async Task<ProductoProveedor> ActualizarProductoAsync(int idProducto, string sku, string nombre,
        decimal monto, string descripcion, Guid modificationUser)
    {
        try
        {
            var producto = await ObtenerProductoPorIdAsync(idProducto: idProducto);
            producto.Update(sku: sku, nombre: nombre, monto: monto, descripcion: descripcion, modificationUser: modificationUser);

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

    public async Task<ProductoProveedor> EliminarProductoAsync(int idProducto, Guid modificationUser)
    {
        try
        {
            var producto = await ObtenerProductoPorIdAsync(idProducto: idProducto);
            producto.Deactivate(modificationUser: modificationUser);

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

    public async Task<ProductoProveedor> ActivarProductoAsync(int idProducto, Guid modificationUser)
    {
        try
        {
            var producto = await ObtenerProductoPorIdAsync(idProducto: idProducto);
            producto.Activate(modificationUser: modificationUser);

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
