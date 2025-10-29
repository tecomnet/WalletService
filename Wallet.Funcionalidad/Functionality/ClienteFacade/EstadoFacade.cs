using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public class EstadoFacade(ServiceDbContext context) : IEstadoFacade
{

    public async Task<Estado> ObtenerEstadoPorIdAsync(int idEstado)
    {
        try
        {
            var estado = await context.Estado.FirstOrDefaultAsync(x => x.Id == idEstado);
            if (estado is null) throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EstadoNoEncontrado,
                dynamicContent: [idEstado]));
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

    // TODO EMD: AGREGAR ADD, UPDATE AND DELETE
    public async Task<Estado> ObtenerEstadoPorNombreAsync(string nombre)
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

    public async Task<List<Estado>> ObtenerTodosAsync(bool? activo = null)
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

    public async Task<Estado> ActivaEstadoAsync(int idEstado, Guid modificationUser)
    {
        try
        {
            // Obtenemos el estado
            var estado = await ObtenerEstadoPorIdAsync(idEstado: idEstado);
            // Marcamos como activo
            estado.Activate(modificationUser: modificationUser);
            // Guardamos cambios
            context.Update(estado);
            await context.SaveChangesAsync();
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

    public async Task<Estado> ActualizaEstadoAsync(int idEstado, string nombre, Guid modificationUser)
    {
        try 
        {
            var estado = await ObtenerEstadoPorIdAsync(idEstado: idEstado);
            // Validamos que el estado este activo
            ValidarEstadoActivo(estado: estado);
            // Validamos duplicidad
            ValidarDuplicidad(nombre: nombre, id: idEstado);
            // Actualizamos el estado
            estado.Actualizar(nombre: nombre, modificationUser: modificationUser);
            // Guardamos cambios
            context.Update(estado);
            await context.SaveChangesAsync();
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

    public async Task<Estado> EliminaEstadoAsync(int idEstado, Guid modificationUser)
    {
        try 
        {
            var estado = await ObtenerEstadoPorIdAsync(idEstado: idEstado);
            // Eliminamos el estado
            estado.Deactivate(modificationUser: modificationUser);
            // Guardamos cambios
            context.Update(estado);
            await context.SaveChangesAsync();
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

    public async Task<Estado> GuardarEstadoAsync(string nombre, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Creamos el estado
            var estado = new Estado(nombre: nombre, creationUser: creationUser, testCase: testCase);
            // Validamos duplicidad
            ValidarDuplicidad(nombre: nombre);
            // Guardamos cambios
            context.Add(estado);
            await context.SaveChangesAsync();
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

    #region Metodos privados
    public void ValidarDuplicidad(string nombre, int id = 0)
    {
        // Obtiene estado existente
        var estadoExistente = context.Estado.FirstOrDefault(x => x.Nombre == nombre && x.Id != id);
        // Duplicado por nombre
        if (estadoExistente != null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EstadoDuplicado,
                dynamicContent: [nombre],
                module: this.GetType().Name));
        }
    }

    private void ValidarEstadoActivo(Estado estado)
    {
        if (!estado.IsActive)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EstadoInactivo,
                dynamicContent: [estado.Nombre]));
        }
    }
    #endregion



}