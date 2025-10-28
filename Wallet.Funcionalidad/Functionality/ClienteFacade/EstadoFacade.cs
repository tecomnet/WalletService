using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public class EstadoFacade(ServiceDbContext context) : IEstadoFacade
{
    // TODO EMD: AGREGAR ADD, UPDATE AND DELETE
    public async Task<Estado> ObtenerEstado(string nombre)
    {
        try
        {
            var estado = await context.Estado.FirstOrDefaultAsync(x => x.Nombre == nombre);
            if (estado is null) throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EstadoNoEncontrado,
                dynamicContent: [nombre]));
            return estado;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<List<Estado>> ObtenerEstados(bool? activo = null)
    {
        try
        {
            // 1. Empezamos con la consulta base (todos los estados)
            var query = context.Estado.AsQueryable();
            // 2. Aplicamos el filtro SÓLO si se ha proporcionado un valor para 'activo'
            if (activo.HasValue)
            {
                // Si activo es true o false, filtramos donde IsActive coincida con ese valor.
                // Esto ignorará el filtro si 'activo' es null.
                query = query.Where(x => x.IsActive == activo.Value);
            }
            // 3. Ejecutamos la consulta y devolvemos el resultado.
            // Si 'activo' fue null, devolverá todos los estados.
            // Si 'activo' fue true/false, devolverá los estados filtrados.
            return await query.ToListAsync();
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }
}