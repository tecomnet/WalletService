using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public class EmpresaFacade(ServiceDbContext context) : IEmpresaFacade
{
    public async Task<Empresa> ObtenerPorIdAsync(int idEmpresa)
    {
        try
        {
            var estado = await context.Empresa.FirstOrDefaultAsync(x => x.Id == idEmpresa);
            if (estado is null) throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EmpresaNoEncontrada,
                dynamicContent: [idEmpresa]));
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

    public async Task<Empresa> ObtenerPorNombreAsync(string nombre)
    {
        try
        {
            var estado = await context.Empresa.FirstOrDefaultAsync(x => x.Nombre == nombre);
            if (estado is null) throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EmpresaNoEncontrada,
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

    public async Task<Empresa> GuardarEmpresaAsync(string nombre, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Creamos la empresa
            var empresa = new Empresa(nombre: nombre, creationUser: creationUser, testCase: testCase);
            // Validamos duplicidad
            ValidarDuplicidad(nombre: nombre);
            // Guardamos cambios
            context.Add(empresa);
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

    public async Task<Empresa> ActualizaEmpresaAsync(int idEmpresa, string nombre, Guid modificationUser)
    {
        try
        {
            var empresa = await ObtenerPorIdAsync(idEmpresa: idEmpresa);
            // Validamos que la empresa este activa
            ValidarEmpresaActiva(empresa: empresa);
            // Validamos duplicidad
            ValidarDuplicidad(nombre: nombre, id: idEmpresa);
            // Actualizamos la empresa
            empresa.Actualizar(nombre: nombre, modificationUser: modificationUser);
            // Guardamos cambios
            context.Update(empresa);
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

    public async Task<Empresa> EliminaEmpresaAsync(int idEmpresa, Guid modificationUser)
    {
        try
        {
            var empresa = await ObtenerPorIdAsync(idEmpresa: idEmpresa);
            // Eliminamos la empresa
            empresa.Deactivate(modificationUser: modificationUser);
            // Guardamos cambios
            context.Update(empresa);
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

    public async Task<Empresa> ActivaEmpresaAsync(int idEmpresa, Guid modificationUser)
    {
        try
        {
            var empresa = await ObtenerPorIdAsync(idEmpresa: idEmpresa);
            // Activamos la empresa
            empresa.Activate(modificationUser: modificationUser);
            // Guardamos cambios
            context.Update(empresa);
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

    #region Metodos privados

    private void ValidarDuplicidad(string nombre, int id = 0)
    {
        // Obtiene estado existente
        var estadoExistente = context.Empresa.FirstOrDefault(x => x.Nombre == nombre && x.Id != id);
        // Duplicado por nombre
        if (estadoExistente != null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EmpresaDuplicada,
                dynamicContent: [nombre],
                module: this.GetType().Name));
        }
    }

    private void ValidarEmpresaActiva(Empresa empresa)
    {
        if (!empresa.IsActive)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.EmpresaInactiva,
                dynamicContent: [empresa.Nombre]));
        }
    }

    #endregion
}