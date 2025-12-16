using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

/// <summary>
/// Fachada para la gestión de estados en el sistema.
/// Proporciona métodos para interactuar con la entidad Estado, incluyendo operaciones CRUD y validaciones.
/// </summary>
public class EstadoFacade(ServiceDbContext context) : IEstadoFacade
{
    /// <summary>
    /// Obtiene un estado por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="idEstado">El identificador único del estado a buscar.</param>
    /// <returns>El objeto <see cref="Estado"/> correspondiente al ID proporcionado.</returns>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el estado no se encuentra o si ocurre un error general.</exception>
    public async Task<Estado> ObtenerEstadoPorIdAsync(int idEstado)
    {
        try
        {
            // Busca el estado por su ID.
            var estado = await context.Estado.FirstOrDefaultAsync(predicate: x => x.Id == idEstado);
            // Si el estado no se encuentra, lanza una excepción.
            if (estado is null)
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.EstadoNoEncontrado,
                    dynamicContent: [idEstado]));
            return estado;
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

    /// <summary>
    /// Obtiene un estado por su nombre de forma asíncrona.
    /// </summary>
    /// <param name="nombre">El nombre del estado a buscar.</param>
    /// <returns>El objeto <see cref="Estado"/> correspondiente al nombre proporcionado.</returns>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el estado no se encuentra o si ocurre un error general.</exception>
    public async Task<Estado> ObtenerEstadoPorNombreAsync(string nombre)
    {
        try
        {
            // Busca el estado por su nombre.
            var estado = await context.Estado.FirstOrDefaultAsync(predicate: x => x.Nombre == nombre);
            // Si el estado no se encuentra, lanza una excepción.
            if (estado is null)
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.EstadoNoEncontrado,
                    dynamicContent: [nombre]));
            return estado;
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

    /// <summary>
    /// Obtiene una lista de todos los estados de forma asíncrona, opcionalmente filtrados por su estado de actividad.
    /// </summary>
    /// <param name="activo">
    /// Opcional. Si es <c>true</c>, devuelve solo estados activos.
    /// Si es <c>false</c>, devuelve solo estados inactivos.
    /// Si es <c>null</c> (por defecto), devuelve todos los estados.
    /// </param>
    /// <returns>Una lista de objetos <see cref="Estado"/>.</returns>
    /// <exception cref="EMGeneralAggregateException">Se lanza si ocurre un error general durante la consulta.</exception>
    public async Task<List<Estado>> ObtenerTodosAsync(bool? activo = null)
    {
        try
        {
            // 1. Empezamos con la consulta base (todos los estados).
            var query = context.Estado.AsQueryable();
            // 2. Aplicamos el filtro SÓLO si se ha proporcionado un valor para 'activo'.
            if (activo.HasValue)
            {
                // Si activo es true o false, filtramos donde IsActive coincida con ese valor.
                // Esto ignorará el filtro si 'activo' es null.
                query = query.Where(predicate: x => x.IsActive == activo.Value);
            }

            // 3. Ejecutamos la consulta y devolvemos el resultado.
            // Si 'activo' fue null, devolverá todos los estados.
            // Si 'activo' fue true/false, devolverá los estados filtrados.
            return await query.ToListAsync();
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

    /// <summary>
    /// Activa un estado existente de forma asíncrona.
    /// </summary>
    /// <param name="idEstado">El identificador del estado a activar.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <returns>El objeto <see cref="Estado"/> actualizado y activo.</returns>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el estado no se encuentra o si ocurre un error general.</exception>
    public async Task<Estado> ActivaEstadoAsync(int idEstado, Guid modificationUser)
    {
        try
        {
            // Obtenemos el estado por su ID.
            var estado = await ObtenerEstadoPorIdAsync(idEstado: idEstado);
            // Marcamos el estado como activo utilizando el método de la entidad.
            estado.Activate(modificationUser: modificationUser);
            // Guardamos los cambios en la base de datos.
            context.Update(entity: estado);
            await context.SaveChangesAsync();
            return estado;
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

    /// <summary>
    /// Actualiza el nombre de un estado existente de forma asíncrona.
    /// </summary>
    /// <param name="idEstado">El identificador del estado a actualizar.</param>
    /// <param name="nombre">El nuevo nombre para el estado.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <returns>El objeto <see cref="Estado"/> actualizado.</returns>
    /// <exception cref="EMGeneralAggregateException">
    /// Se lanza si el estado no se encuentra, si está inactivo, si el nuevo nombre ya existe, o si ocurre un error general.
    /// </exception>
    public async Task<Estado> ActualizaEstadoAsync(int idEstado, string nombre, string? concurrencyToken,
        Guid modificationUser)
    {
        try
        {
            // Obtiene el estado por su ID.
            var estado = await ObtenerEstadoPorIdAsync(idEstado: idEstado);
            // Valida que el estado esté activo antes de actualizarlo.
            ValidarEstadoActivo(estado: estado);

            // Manejo de ConcurrencyToken
            if (!string.IsNullOrEmpty(concurrencyToken))
            {
                context.Entry(estado).Property(x => x.ConcurrencyToken).OriginalValue =
                    Convert.FromBase64String(concurrencyToken);
            }

            // Valida que el nuevo nombre no cause duplicidad con otros estados (excluyendo el actual).
            ValidarDuplicidad(nombre: nombre, id: idEstado);
            // Actualiza el estado utilizando el método de la entidad.
            estado.Actualizar(nombre: nombre, modificationUser: modificationUser);
            // Guarda los cambios en la base de datos.
            context.Update(entity: estado);
            await context.SaveChangesAsync();
            return estado;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ConcurrencyError,
                dynamicContent: []));
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

    /// <summary>
    /// Desactiva (elimina lógicamente) un estado existente de forma asíncrona.
    /// </summary>
    /// <param name="idEstado">El identificador del estado a desactivar.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <returns>El objeto <see cref="Estado"/> desactivado.</returns>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el estado no se encuentra o si ocurre un error general.</exception>
    public async Task<Estado> EliminaEstadoAsync(int idEstado, Guid modificationUser)
    {
        try
        {
            // Obtiene el estado por su ID.
            var estado = await ObtenerEstadoPorIdAsync(idEstado: idEstado);
            // Desactiva el estado utilizando el método de la entidad.
            estado.Deactivate(modificationUser: modificationUser);
            // Guarda los cambios en la base de datos.
            context.Update(entity: estado);
            await context.SaveChangesAsync();
            return estado;
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

    /// <summary>
    /// Guarda un nuevo estado en la base de datos de forma asíncrona.
    /// </summary>
    /// <param name="nombre">El nombre del nuevo estado.</param>
    /// <param name="creationUser">El GUID del usuario que crea el estado.</param>
    /// <param name="testCase">Opcional. Un caso de prueba asociado al estado.</param>
    /// <returns>El objeto <see cref="Estado"/> recién creado.</returns>
    /// <exception cref="EMGeneralAggregateException">
    /// Se lanza si el nombre del estado ya existe o si ocurre un error general.
    /// </exception>
    public async Task<Estado> GuardarEstadoAsync(string nombre, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Crea una nueva instancia de Estado.
            var estado = new Estado(nombre: nombre, creationUser: creationUser, testCase: testCase);
            // Valida que no exista otro estado con el mismo nombre.
            ValidarDuplicidad(nombre: nombre);
            // Agrega el nuevo estado al contexto y guarda los cambios en la base de datos.
            context.Add(entity: estado);
            await context.SaveChangesAsync();
            return estado;
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

    #region Metodos privados

    /// <summary>
    /// Valida si ya existe un estado con el mismo nombre, excluyendo opcionalmente un ID específico.
    /// </summary>
    /// <param name="nombre">El nombre a validar.</param>
    /// <param name="id">Opcional. El ID del estado a excluir de la validación (útil para actualizaciones).</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si se encuentra un estado duplicado.</exception>
    public void ValidarDuplicidad(string nombre, int id = 0)
    {
        // Obtiene un estado existente con el mismo nombre, pero con un ID diferente al proporcionado (si se proporciona).
        var estadoExistente = context.Estado.FirstOrDefault(predicate: x => x.Nombre == nombre && x.Id != id);
        // Si se encuentra un estado con el mismo nombre, lanza una excepción de duplicidad.
        if (estadoExistente != null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EstadoDuplicado,
                dynamicContent: [nombre],
                module: this.GetType().Name));
        }
    }

    /// <summary>
    /// Valida si un estado está activo.
    /// </summary>
    /// <param name="estado">El objeto <see cref="Estado"/> a validar.</param>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el estado no está activo.</exception>
    private void ValidarEstadoActivo(Estado estado)
    {
        // Si el estado no está activo, lanza una excepción.
        if (!estado.IsActive)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EstadoInactivo,
                dynamicContent: [estado.Nombre]));
        }
    }

    #endregion
}