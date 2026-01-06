using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionEmpresa;

namespace Wallet.Funcionalidad.Functionality.ProveedorFacade;

/// <summary>
/// Fachada para la gestión de proveedores de servicios y sus productos asociados.
/// Implementa la lógica de negocio para operaciones CRUD sobre proveedores y productos.
/// </summary>
public partial class ProveedorFacade(ServiceDbContext context) : IProveedorFacade
{
    /// <inheritdoc />
    public async Task<Proveedor> GuardarProveedorAsync(string nombre, string urlIcono, Categoria categoria, int brokerId,  Guid creationUser,
        string? testCase = null)
    {
        try
        {
            // Busca el broker asociado.
            var broker = await context.Broker.FirstOrDefaultAsync(predicate: x => x.Id == brokerId);
            if (broker == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.BrokerNoEncontrado,
                    dynamicContent: [brokerId],
                    module: this.GetType().Name));
            }

            // Crea una nueva instancia de Proveedor.
            var proveedor = new Proveedor(nombre: nombre, urlIcono: urlIcono, broker: broker, categoria: categoria,
                creationUser: creationUser);
            ValidarProveedorDuplicado(nombre: nombre);
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
    public async Task<Proveedor> ActualizarProveedorAsync(int idProveedor, string nombre, Categoria categoria, string urlIcono, 
        string concurrencyToken, Guid modificationUser,
        string? testCase = null)
    {
        try
        {
            // Obtiene el proveedor existente.
            var proveedor = await ObtenerProveedorPorIdAsync(idProveedor: idProveedor);
            // Establece el token original para la validación de concurrencia optimista
            context.Entry(entity: proveedor).Property(propertyExpression: x => x.ConcurrencyToken).OriginalValue =
                Convert.FromBase64String(s: concurrencyToken);
            ValidarProveedorIsActive(proveedor: proveedor);
            ValidarProveedorDuplicado(nombre: nombre, id: idProveedor);
            // Actualiza los datos del proveedor.
            proveedor.Update(nombre: nombre, urlIcono: urlIcono, categoria: categoria, modificationUser: modificationUser);

            // Guarda los cambios.
            await context.SaveChangesAsync();
            return proveedor;
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
    public async Task<List<Proveedor>> ObtenerProveedoresAsync(Categoria? categoria = null)
    {
        try
        {
            List<Proveedor> proveedores;
            if (categoria != null)
                proveedores = await context.Proveedor.ToListAsync();
            else
                proveedores = await context.Proveedor. Where(p=>p.Categoria == categoria).ToListAsync();
            return proveedores;
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
    /// Valida si ya existe un proveedor con el mismo nombre.
    /// </summary>
    /// <param name="nombre">Nombre de el proveedor a validar.</param>
    /// <param name="id">ID de el proveedor (opcional, para excluir en actualizaciones).</param>
    /// <exception cref="EMGeneralAggregateException">Si ya existe un proveedor con ese nombre.</exception>
    private void ValidarProveedorDuplicado(string nombre, int id = 0)
    {
        // Obtiene estado existente
        var existe = context.Proveedor.FirstOrDefault(predicate: x => x.Nombre == nombre && x.Id != id);
        // Duplicado por nombre
        if (existe != null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ProveedorExistente,
                dynamicContent: [nombre],
                module: this.GetType().Name));
        }
    }

    /// <summary>
    /// Valida si el proveedor se encuentra activa.
    /// </summary>
    /// <param name="proveedor">La proveedor a validar.</param>
    /// <exception cref="EMGeneralAggregateException">Si el proveedor está inactiva.</exception>
    private void ValidarProveedorIsActive(Proveedor proveedor)
    {
        if (!proveedor.IsActive)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ProveedorInactivo,
                dynamicContent: [proveedor.Nombre]));
        }
    }

    #endregion
}
