using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.Funcionalidad.Functionality.ProveedorFacade;

namespace Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;

/// <summary>
/// Fachada para la gestión de servicios favoritos de los clientes.
/// </summary>
public class ServicioFavoritoFacade(
    ServiceDbContext context,
    IClienteFacade clienteFacade,
    IProveedorFacade proveedorFacade) : IServicioFavoritoFacade
{
    /// <inheritdoc />
    public async Task<ServicioFavorito> GuardarServicioFavoritoAsync(int clienteId, int proveedorId,
        string alias, string numeroReferencia, Guid creationUser, string? testCase = null)
    {
        try
        {
            // 1. Obtiene el cliente por su ID.
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: clienteId);
            // 2. Obtiene el proveedor de servicio por su ID.
            var proveedor =
                await proveedorFacade.ObtenerProveedorPorIdAsync(
                    idProveedor: proveedorId);
            // 3. Crea una nueva instancia de ServicioFavorito.
            var servicioFavorito = new ServicioFavorito(cliente: cliente, proveedor: proveedor,
                alias: alias, numeroReferencia: numeroReferencia, creationUser: creationUser);

            // Agrega el servicio favorito al contexto.
            await context.ServicioFavorito.AddAsync(entity: servicioFavorito);
            // Guarda los cambios en la base de datos.
            await context.SaveChangesAsync();
            return servicioFavorito;
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
    public async Task<ServicioFavorito> ObtenerServicioFavoritoPorIdAsync(int idServicioFavorito)
    {
        try
        {
            // Busca el servicio favorito por su ID, incluyendo la información del proveedor de servicio.
            var servicioFavorito = await context.ServicioFavorito
                .Include(navigationPropertyPath: s => s.Proveedor)
                .FirstOrDefaultAsync(predicate: x => x.Id == idServicioFavorito);

            // Si no se encuentra, lanza una excepción.
            if (servicioFavorito == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ServicioFavoritoNoEncontrado,
                    dynamicContent: [idServicioFavorito],
                    module: this.GetType().Name));
            }

            return servicioFavorito;
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
    public async Task<ServicioFavorito> ActualizarServicioFavoritoAsync(int idServicioFavorito, string alias,
        string numeroReferencia, string? concurrencyToken, Guid modificationUser, string? testCase = null)
    {
        try
        {
            // Obtiene el servicio favorito existente.
            var servicioFavorito = await ObtenerServicioFavoritoPorIdAsync(idServicioFavorito: idServicioFavorito);

            // Manejo de ConcurrencyToken
            if (!string.IsNullOrEmpty(concurrencyToken))
            {
                context.Entry(servicioFavorito).Property(x => x.ConcurrencyToken).OriginalValue =
                    Convert.FromBase64String(concurrencyToken);
            }

            // Actualiza los datos del servicio favorito.
            servicioFavorito.Update(alias: alias, numeroReferencia: numeroReferencia,
                modificationUser: modificationUser);

            // Guarda los cambios.
            await context.SaveChangesAsync();
            return servicioFavorito;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ConcurrencyError,
                dynamicContent: []));
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
    public async Task<ServicioFavorito> EliminarServicioFavoritoAsync(int idServicioFavorito, Guid modificationUser)
    {
        try
        {
            // Obtiene el servicio favorito existente.
            var servicioFavorito = await ObtenerServicioFavoritoPorIdAsync(idServicioFavorito: idServicioFavorito);
            // Desactiva el servicio favorito.
            servicioFavorito.Deactivate(modificationUser: modificationUser);
            // Guarda los cambios.
            await context.SaveChangesAsync();
            return servicioFavorito;
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
    public async Task<ServicioFavorito> ActivarServicioFavoritoAsync(int idServicioFavorito, Guid modificationUser)
    {
        try
        {
            // Obtiene el servicio favorito existente.
            var servicioFavorito = await ObtenerServicioFavoritoPorIdAsync(idServicioFavorito: idServicioFavorito);
            // Activa el servicio favorito.
            servicioFavorito.Activate(modificationUser: modificationUser);
            // Guarda los cambios.
            await context.SaveChangesAsync();
            return servicioFavorito;
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
    public async Task<List<ServicioFavorito>> ObtenerServiciosFavoritosPorClienteAsync(int clienteId)
    {
        try
        {
            // Retorna la lista de servicios favoritos del cliente, incluyendo la información del proveedor.
            return await context.ServicioFavorito
                .Where(predicate: x => x.ClienteId == clienteId)
                .Include(navigationPropertyPath: s => s.Proveedor)
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
}
