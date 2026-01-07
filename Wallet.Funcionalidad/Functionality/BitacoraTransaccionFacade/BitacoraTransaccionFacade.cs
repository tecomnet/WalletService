using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.DOM;
using Wallet.DOM.Errors;

namespace Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;

public class BitacoraTransaccionFacade(ServiceDbContext context) : IBitacoraTransaccionFacade
{
    public async Task<BitacoraTransaccion> GuardarTransaccionAsync(int idBilletera, decimal monto, string tipo,
        string direccion,
        string estatus, Guid creationUser, string? refExternaId)
    {
        try
        {
            var transaccion =
                new BitacoraTransaccion(idBilletera: idBilletera, monto: monto, tipo: tipo, direccion: direccion,
                    estatus: estatus, creationUser: creationUser, refExternaId: refExternaId);

            context.BitacoraTransaccion.Add(entity: transaccion);
            await context.SaveChangesAsync();

            return transaccion;
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

    public async Task<List<BitacoraTransaccion>> ObtenerTodasAsync()
    {
        try
        {
            return await context.BitacoraTransaccion
                .Include(navigationPropertyPath: b => b.CuentaWallet)
                .OrderByDescending(keySelector: b => b.CreationTimestamp)
                .ToListAsync();
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

    public async Task<BitacoraTransaccion> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await context.BitacoraTransaccion
                       .Include(navigationPropertyPath: b => b.CuentaWallet)
                       .FirstOrDefaultAsync(predicate: b => b.Id == id) ??
                   throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                       errorCode: ServiceErrorsBuilder.BitacoraTransaccionNoEncontrada,
                       dynamicContent: [id],
                       module: this.GetType().Name));
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

    public async Task<List<BitacoraTransaccion>> ObtenerPorClienteAsync(int idCliente)
    {
        try
        {
            return await context.BitacoraTransaccion
                .Include(navigationPropertyPath: b => b.CuentaWallet)
                .Where(predicate: b => b.CuentaWallet!.IdCliente == idCliente)
                .OrderByDescending(keySelector: b => b.CreationTimestamp)
                .ToListAsync();
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
