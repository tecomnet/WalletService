using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.Helper;

namespace Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;

public class ServicioFavoritoFacade : IServicioFavoritoFacade
{
    private readonly ServiceDbContext _context;

    public ServicioFavoritoFacade(ServiceDbContext context)
    {
        _context = context;
    }

    public async Task<ServicioFavorito> GuardarServicioFavoritoAsync(int clienteId, int proveedorServicioId,
        string alias, string numeroReferencia, Guid creationUser, string? testCase = null)
    {
        try
        {
            var servicioFavorito =
                new ServicioFavorito(clienteId, proveedorServicioId, alias, numeroReferencia, creationUser);
            // Ignoring testCase as per previous decision

            await _context.ServicioFavorito.AddAsync(servicioFavorito);
            await _context.SaveChangesAsync();
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
            var servicioFavorito = await _context.ServicioFavorito
                .Include(s => s.ProveedorServicio)
                .FirstOrDefaultAsync(x => x.Id == idServicioFavorito);

            if (servicioFavorito == null)
            {
                var serviceError = new ServiceErrors().GetServiceErrorForCode("SERVICIO-FAVORITO-NOT-FOUND");
                throw new EMGeneralAggregateException(
                    new EMGeneralException(
                        serviceError.Message,
                        serviceError.ErrorCode,
                        serviceError.Title,
                        serviceError.Description(new object[] { idServicioFavorito }),
                        "ServicioFavorito",
                        null,
                        null,
                        "DOM",
                        new List<object> { idServicioFavorito }
                    )
                );
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

            await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();
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
            return await _context.ServicioFavorito
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
