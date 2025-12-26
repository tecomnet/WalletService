using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

/// <summary>
/// Fachada que implementa la lógica de negocio para la gestión de empresas.
/// Permite crear, actualizar, consultar y eliminar empresas.
/// </summary>
public class EmpresaFacade(ServiceDbContext context) : IEmpresaFacade
{
    /// <inheritdoc />
    public async Task<Empresa> ObtenerPorIdAsync(int idEmpresa)
    {
        try
        {
            var empresa = await context.Empresa.FirstOrDefaultAsync(predicate: x => x.Id == idEmpresa);
            if (empresa is null)
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.EmpresaNoEncontrada,
                    dynamicContent: [idEmpresa]));
            return empresa;
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

    /// <inheritdoc />
    public async Task<Empresa> ObtenerPorNombreAsync(string nombre)
    {
        try
        {
            var empresa = await context.Empresa.FirstOrDefaultAsync(predicate: x => x.Nombre == nombre);
            if (empresa is null)
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.EmpresaNoEncontrada,
                    dynamicContent: [nombre]));
            return empresa;
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

    /// <inheritdoc />
    public async Task<List<Empresa>> ObtenerTodasAsync()
    {
        try
        {
            var empresas = await context.Empresa.ToListAsync();
            return empresas;
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

    /// <inheritdoc />
    public async Task<Empresa> GuardarEmpresaAsync(string nombre, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Creamos la empresa
            var empresa = new Empresa(nombre: nombre, creationUser: creationUser, testCase: testCase);
            // Validamos duplicidad
            ValidarDuplicidad(nombre: nombre);
            // Guardamos cambios
            context.Add(entity: empresa);
            await context.SaveChangesAsync();
            return empresa;
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

    /// <inheritdoc />
    public async Task<Empresa> ActualizaEmpresaAsync(int idEmpresa, string nombre, string concurrencyToken,
        Guid modificationUser)
    {
        try
        {
            var empresa = await ObtenerPorIdAsync(idEmpresa: idEmpresa);
            // Establece el token original para la validación de concurrencia optimista
            context.Entry(empresa).Property(x => x.ConcurrencyToken).OriginalValue =
                Convert.FromBase64String(concurrencyToken);
            // Validamos que la empresa este activa
            ValidarEmpresaActiva(empresa: empresa);
            // Validamos duplicidad
            ValidarDuplicidad(nombre: nombre, id: idEmpresa);
            // Actualizamos la empresa
            empresa.Actualizar(nombre: nombre, modificationUser: modificationUser);
            // Guardamos cambios
            context.Update(entity: empresa);
            await context.SaveChangesAsync();
            return empresa;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not DbUpdateConcurrencyException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Empresa> EliminaEmpresaAsync(int idEmpresa, string concurrencyToken, Guid modificationUser)
    {
        try
        {
            var empresa = await ObtenerPorIdAsync(idEmpresa: idEmpresa);
            // Establece el token original para la validación de concurrencia optimista
            context.Entry(empresa).Property(x => x.ConcurrencyToken).OriginalValue =
                Convert.FromBase64String(concurrencyToken);
            // Eliminamos la empresa
            empresa.Deactivate(modificationUser: modificationUser);
            // Guardamos cambios
            context.Update(entity: empresa);
            await context.SaveChangesAsync();
            return empresa;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not DbUpdateConcurrencyException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Empresa> ActivaEmpresaAsync(int idEmpresa, string concurrencyToken, Guid modificationUser)
    {
        try
        {
            var empresa = await ObtenerPorIdAsync(idEmpresa: idEmpresa);
            // Establece el token original para la validación de concurrencia optimista
            context.Entry(empresa).Property(x => x.ConcurrencyToken).OriginalValue =
                Convert.FromBase64String(concurrencyToken);
            // Activamos la empresa
            empresa.Activate(modificationUser: modificationUser);
            // Guardamos cambios
            context.Update(entity: empresa);
            await context.SaveChangesAsync();
            return empresa;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not DbUpdateConcurrencyException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<List<Producto>> ObtenerProductosPorEmpresaAsync(int idEmpresa)
    {
        try
        {
            var empresa = await context.Empresa
                .Include(e => e.Productos)
                .FirstOrDefaultAsync(e => e.Id == idEmpresa);

            if (empresa is null)
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.EmpresaNoEncontrada,
                    dynamicContent: [idEmpresa]));

            return empresa.Productos.ToList();
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
    public async Task<List<Cliente>> ObtenerClientesPorEmpresaAsync(int idEmpresa)
    {
        try
        {
            var empresa = await context.Empresa
                .Include(e => e.Clientes)
                .FirstOrDefaultAsync(e => e.Id == idEmpresa);

            if (empresa is null)
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.EmpresaNoEncontrada,
                    dynamicContent: [idEmpresa]));

            return empresa.Clientes.ToList();
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
    public async Task<Empresa> AsignarProductosAsync(int idEmpresa, List<int> idsProductos, Guid modificationUser)
    {
        try
        {
            var empresa = await context.Empresa
                .Include(e => e.Productos)
                .FirstOrDefaultAsync(e => e.Id == idEmpresa);

            if (empresa is null)
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.EmpresaNoEncontrada,
                    dynamicContent: [idEmpresa]));

            ValidarEmpresaActiva(empresa: empresa);

            // Obtener los productos que se quieren asignar y verificar que existan
            var productosEntidad = await context.Producto
                .Where(p => idsProductos.Contains(p.Id))
                .ToListAsync();

            // Cargar solo IDs de productos existentes para chequeo eficiente en memoria
            var existingProductIds = new HashSet<int>(empresa.Productos.Select(p => p.Id));

            foreach (var producto in productosEntidad)
            {
                // Si la empresa no tiene asignado este producto, se agrega
                if (!existingProductIds.Contains(producto.Id))
                {
                    empresa.Productos.Add(producto);
                    existingProductIds.Add(producto.Id); // Mantener set actualizado
                    // Actualizamos usuario de modificación en la empresa
                    empresa.Actualizar(nombre: empresa.Nombre, modificationUser: modificationUser);
                }
            }

            await context.SaveChangesAsync();
            return empresa;
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
    public async Task<Empresa> DesasignarProductosAsync(int idEmpresa, List<int> idsProductos, Guid modificationUser)
    {
        try
        {
            var empresa = await context.Empresa
                .Include(e => e.Productos)
                .FirstOrDefaultAsync(e => e.Id == idEmpresa);

            if (empresa is null)
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.EmpresaNoEncontrada,
                    dynamicContent: [idEmpresa]));

            ValidarEmpresaActiva(empresa: empresa);

            // Identificar los productos a remover que efectivamente están en la lista de la empresa
            var productosARemover = empresa.Productos
                .Where(p => idsProductos.Contains(p.Id))
                .ToList();

            foreach (var producto in productosARemover)
            {
                empresa.Productos.Remove(producto);
                // Actualizamos usuario de modificación
                empresa.Actualizar(nombre: empresa.Nombre, modificationUser: modificationUser);
            }

            await context.SaveChangesAsync();
            return empresa;
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
    /// Valida si ya existe una empresa con el mismo nombre.
    /// </summary>
    /// <param name="nombre">Nombre de la empresa a validar.</param>
    /// <param name="id">ID de la empresa (opcional, para excluir en actualizaciones).</param>
    /// <exception cref="EMGeneralAggregateException">Si ya existe una empresa con ese nombre.</exception>
    private void ValidarDuplicidad(string nombre, int id = 0)
    {
        // Obtiene estado existente
        var estadoExistente = context.Empresa.FirstOrDefault(predicate: x => x.Nombre == nombre && x.Id != id);
        // Duplicado por nombre
        if (estadoExistente != null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EmpresaDuplicada,
                dynamicContent: [nombre],
                module: this.GetType().Name));
        }
    }

    /// <summary>
    /// Valida si la empresa se encuentra activa.
    /// </summary>
    /// <param name="empresa">La empresa a validar.</param>
    /// <exception cref="EMGeneralAggregateException">Si la empresa está inactiva.</exception>
    private void ValidarEmpresaActiva(Empresa empresa)
    {
        if (!empresa.IsActive)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EmpresaInactiva,
                dynamicContent: [empresa.Nombre]));
        }
    }

    #endregion
}