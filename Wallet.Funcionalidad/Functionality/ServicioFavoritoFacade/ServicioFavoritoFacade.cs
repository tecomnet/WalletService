using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;

namespace Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;

public class ServicioFavoritoFacade(ServiceDbContext context,
    IClienteFacade clienteFacade,
    IProveedorServicioFacade proveedorServicioFacade) : IServicioFavoritoFacade
{

    public async Task<ServicioFavorito> GuardarServicioFavoritoAsync(int clienteId, int proveedorServicioId,
        string alias, string numeroReferencia, Guid creationUser, string? testCase = null)
    {
        try
        {
            // 1. Fetch Cliente
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(clienteId);
            // 2. Fetch ProveedorServicio
            var proveedorServicio = await proveedorServicioFacade.ObtenerProveedorServicioPorIdAsync(proveedorServicioId);
            // 3. Create ServicioFavorito using the new constructor
            var servicioFavorito = new ServicioFavorito(cliente, proveedorServicio, alias, numeroReferencia, creationUser);

            await context.ServicioFavorito.AddAsync(servicioFavorito);
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

    public async Task<ServicioFavorito> ObtenerServicioFavoritoPorIdAsync(int idServicioFavorito)
    {
        try
        {
            var servicioFavorito = await context.ServicioFavorito
                .Include(s => s.ProveedorServicio)
                .FirstOrDefaultAsync(x => x.Id == idServicioFavorito);

            if (servicioFavorito == null)
            {
                throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
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

    public async Task<ServicioFavorito> ActualizarServicioFavoritoAsync(int idServicioFavorito, string alias,
        string numeroReferencia, Guid modificationUser, string? testCase = null)
    {
        try
        {
            var servicioFavorito = await ObtenerServicioFavoritoPorIdAsync(idServicioFavorito);

            // Need to add Update method to ServicioFavorito as well
            servicioFavorito.Update(alias, numeroReferencia, modificationUser);

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

    public async Task<ServicioFavorito> EliminarServicioFavoritoAsync(int idServicioFavorito, Guid modificationUser)
    {
        try
        {
            var servicioFavorito = await ObtenerServicioFavoritoPorIdAsync(idServicioFavorito);
            servicioFavorito.Deactivate(modificationUser);
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

    public async Task<ServicioFavorito> ActivarServicioFavoritoAsync(int idServicioFavorito, Guid modificationUser)
    {
        try
        {
            var servicioFavorito = await ObtenerServicioFavoritoPorIdAsync(idServicioFavorito);
            servicioFavorito.Activate(modificationUser);
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

    public async Task<List<ServicioFavorito>> ObtenerServiciosFavoritosPorClienteAsync(int clienteId)
    {
        try
        {
            return await context.ServicioFavorito
                .Where(x => x.ClienteId == clienteId)
                .Include(s => s.ProveedorServicio)
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
