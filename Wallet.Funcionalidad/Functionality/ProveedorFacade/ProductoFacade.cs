using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.Errors;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionEmpresa;

namespace Wallet.Funcionalidad.Functionality.ProveedorFacade;

/// <summary>
/// Fachada para la gestión de proveedores de servicios y sus productos asociados.
/// Implementa la lógica de negocio para operaciones CRUD sobre proveedores y productos.
/// </summary>
public partial class ProveedorFacade : IProveedorFacade
{
    /// <inheritdoc />
    public async Task<Producto> GuardarProductoAsync(int proveedorId, string sku, string nombre,
        decimal? precio, string icono, string categoria, Guid creationUser)
    {
        try
        {
            // Obtiene el proveedor al que se asociará el producto.
            var proveedor = await ObtenerProveedorPorIdAsync(idProveedor: proveedorId);
            // Agrega el nuevo producto al proveedor.
            var producto = proveedor.AgregarProducto(sku: sku, nombre: nombre, precio: precio, icono: icono,
                categoria: categoria, creationUser: creationUser);
            // Valida duplicidad
            ValidarProductoDuplicidad(nombre: nombre, sku: sku, idProveedor: proveedorId, id: producto.Id);
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
        decimal? precio, string icono, string categoria, string concurrencyToken, Guid modificationUser)
    {
        try
        {
            // Obtiene el producto existente.
            var producto = await ObtenerProductoPorIdAsync(idProducto: idProducto);
            // Establece el token original para la validación de concurrencia optimista
            if (!string.IsNullOrEmpty(value: concurrencyToken))
            {
                context.Entry(entity: producto).Property(propertyExpression: x => x.ConcurrencyToken).OriginalValue =
                    DomCommon.SafeParseConcurrencyToken(token: concurrencyToken, module: this.GetType().Name);
            }

            // Valida que el producto no esté inactivo.
            ValidarProductoIsActive(producto: producto);
            // Valida duplicidad
            ValidarProductoDuplicidad(nombre: nombre, sku: sku, idProveedor: producto.ProveedorId, id: idProducto);
            // Actualiza los datos del producto.
            producto.Update(sku: sku, nombre: nombre, precio: precio, urlIcono: icono, categoria: categoria,
                modificationUser: modificationUser);

            // Guarda los cambios.
            await context.SaveChangesAsync();
            return producto;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not DbUpdateConcurrencyException)
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

    /// <inheritdoc />
    public async Task<List<Producto>> ObtenerProductosPorCategoriaAsync(string categoria)
    {
        try
        {
            if (!Enum.TryParse<Categoria>(value: categoria, ignoreCase: true, result: out _))
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ProductoCategoriaInvalida,
                    dynamicContent: [categoria],
                    module: this.GetType().Name));
            }

            return await context.Producto
                .Where(predicate: x => x.Categoria == categoria && x.IsActive)
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
    public async Task<Producto> ActualizarProveedorDeProductoAsync(int idProducto, int idProveedor,
        Guid modificationUser)
    {
        try
        {
            // Obtiene el producto existente.
            var producto = await ObtenerProductoPorIdAsync(idProducto: idProducto);
            // Valida que el producto no esté inactivo.
            ValidarProductoIsActive(producto: producto);

            // Obtiene el nuevo proveedor.
            var proveedor = await ObtenerProveedorPorIdAsync(idProveedor: idProveedor);
            ValidarProveedorIsActive(proveedor: proveedor);
            // Valida duplicidad en el nuevo proveedor
            ValidarProductoDuplicidad(nombre: producto.Nombre, sku: producto.Sku, idProveedor: idProveedor,
                id: idProducto);

            // Asigna el nuevo proveedor.
            producto.AsignarProveedor(proveedor: proveedor, modificationUser: modificationUser);

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


    #region Metodos privados

    /// <summary>
    /// Valida si ya existe un producto con el mismo nombre o SKU para el mismo proveedor.
    /// </summary>
    /// <param name="nombre">Nombre del producto a validar.</param>
    /// <param name="sku">SKU del producto a validar.</param>
    /// <param name="idProveedor">ID del proveedor al que pertenece el producto.</param>
    /// <param name="id">ID del producto (opcional, para excluir en actualizaciones).</param>
    /// <exception cref="EMGeneralAggregateException">Si ya existe un producto con ese nombre o SKU.</exception>
    private void ValidarProductoDuplicidad(string nombre, string sku, int idProveedor, int id = 0)
    {
        // Obtiene producto existente si hay coincidencia de nombre O sku
        var existentes = context.Producto
            .Where(predicate: x =>
                (x.Nombre == nombre || x.Sku == sku) &&
                x.ProveedorId == idProveedor &&
                x.Id != id)
            .ToList();

        if (existentes.Count == 0) return;

        // Validar duplicidad de nombre
        if (existentes.Any(predicate: x => x.Nombre == nombre))
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ProductoExistente,
                dynamicContent: [nombre],
                module: this.GetType().Name));
        }

        // Validar duplicidad de SKU
        if (existentes.Any(predicate: x => x.Sku == sku))
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ProductoSkuExistente,
                dynamicContent: [sku],
                module: this.GetType().Name));
        }
    }

    /// <summary>
    /// Valida si el producto se encuentra activa.
    /// </summary>
    /// <param name="producto">El producto a validar.</param>
    /// <exception cref="EMGeneralAggregateException">Si el producto está inactivo.</exception>
    private void ValidarProductoIsActive(Producto producto)
    {
        if (!producto.IsActive)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ProductoInactivo,
                dynamicContent: [producto.Nombre]));
        }
    }

    #endregion
}
