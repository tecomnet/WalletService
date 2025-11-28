using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public class DispositivoMovilAutorizadoFacadeFacade (ServiceDbContext context, IClienteFacade clienteFacade) : IDispositivoMovilAutorizadoFacade 
{
    public async Task<DispositivoMovilAutorizado> GuardarDispositivoAutorizadoAsync(
        int idCliente, 
        string token,
        string idDispositivo,
        string nombre, 
        string caracteristicas,
        Guid creationUser, 
        string? testCase = null)
    {
        try
        {
            // Obtenemos el cliente
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);
            // Creamos el dispositivo
            var dispositivo = new DispositivoMovilAutorizado(
                token: token,
                idDispositivo: idDispositivo,
                nombre: nombre,
                caracteristicas: caracteristicas,
                creationUser: creationUser,
                testCase: testCase);
            // Agregamos el dispositivo
            cliente.AgregarDispositivoMovilAutorizado(dispositivo: dispositivo, modificationUser: creationUser);
            // Guardamos cambios
            await context.SaveChangesAsync();
            // Retornamos el dispositivo
            return dispositivo;
        }
        catch(Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<bool> EsDispositivoAutorizadoAsync(int idCliente, string idDispositivo, string token)
    {
        try
        {
            // Obtenemos el cliente
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);
            // Verificamos si el dispositivo está autorizado
            return cliente.EsDispositivoAutorizado(idDispositivo: idDispositivo, token: token); 
        }
        catch(Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }
}