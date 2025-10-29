using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public class EmpresaFacade(ServiceDbContext context):IEmpresaFacade
{
    public async Task<Empresa> ObtenerPorIdAsync(int idEmpresa)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public async Task<Empresa> ActualizaEmpresaAsync(int idEmpresa, Guid modificationUser)
    {
        throw new NotImplementedException();
    }

    public async Task<Empresa> EliminaEmpresaAsync(int idEmpresa, Guid modificationUser)
    {
        throw new NotImplementedException();
    }

    public async Task<Empresa> ActivaEmpresaAsync(int idEmpresa, Guid modificationUser)
    {
        throw new NotImplementedException();
    }
}