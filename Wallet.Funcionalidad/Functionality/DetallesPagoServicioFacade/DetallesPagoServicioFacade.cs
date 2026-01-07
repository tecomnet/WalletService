using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;
using Wallet.DOM;
using Wallet.DOM.Errors;

namespace Wallet.Funcionalidad.Functionality.DetallesPagoServicioFacade;

public class DetallesPagoServicioFacade(
    ServiceDbContext context,
    IBitacoraTransaccionFacade bitacoraTransaccionFacade)
    : IDetallesPagoServicioFacade
{
    public async Task<DetallesPagoServicio> GuardarDetallesAsync(int idTransaccion, int idProveedor,
        string numeroReferencia, Guid creationUser, string? codigoAutorizacion)
    {
        try
        {
            var detalles = new DetallesPagoServicio(idTransaccion: idTransaccion, idProveedor: idProveedor,
                numeroReferencia: numeroReferencia, creationUser: creationUser,
                codigoAutorizacion: codigoAutorizacion);

            context.DetallesPagoServicio.Add(entity: detalles);
            await context.SaveChangesAsync();

            return detalles;
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

    public async Task<DetallesPagoServicio> ObtenerPorIdAsync(int id)
    {
        try
        {
            return await context.DetallesPagoServicio
                       .Include(navigationPropertyPath: d => d.Transaccion)
                       .FirstOrDefaultAsync(predicate: d => d.Id == id)
                   ?? throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                       errorCode: ServiceErrorsBuilder.DetallePagoNoEncontrado,
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

    public async Task<List<DetallesPagoServicio>> ObtenerPorClienteAsync(int idCliente)
    {
        try
        {
            return await context.DetallesPagoServicio
                .Include(navigationPropertyPath: d => d.Transaccion)
                .ThenInclude(navigationPropertyPath: t => t.CuentaWallet)
                .Where(predicate: d => d.Transaccion!.CuentaWallet!.IdCliente == idCliente)
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

    public async Task<List<DetallesPagoServicio>> ObtenerTodosAsync()
    {
        try
        {
            return await context.DetallesPagoServicio.ToListAsync();
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

    public async Task<List<DetallesPagoServicio>> ObtenerPorTransaccionAsync(int idTransaccion)
    {
        try
        {
            return await context.DetallesPagoServicio
                .Include(navigationPropertyPath: d => d.Transaccion)
                .Where(predicate: d => d.IdTransaccion == idTransaccion)
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

    public async Task<DetallesPagoServicio> RegistrarPagoServicioAsync(
        int idBilletera,
        decimal monto,
        string nombreServicio,
        string direccion,
        string estatus,
        string? refExternaId,
        int idProveedor,
        string numeroReferencia,
        string? codigoAutorizacion,
        Guid creationUser)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // 1. Crear la transacción en bitácora
            var transaccion = await bitacoraTransaccionFacade.GuardarTransaccionAsync(
                idBilletera: idBilletera,
                monto: monto,
                tipo: nombreServicio, // Tipo de transacción (ej. SERVICIO)
                direccion: direccion,
                estatus: estatus,
                creationUser: creationUser,
                refExternaId: refExternaId
            );

            // 2. Crear los detalles del pago de servicio vinculados a la transacción
            var detalles = new DetallesPagoServicio(
                idTransaccion: transaccion.Id,
                idProveedor: idProveedor,
                numeroReferencia: numeroReferencia,
                creationUser: creationUser,
                codigoAutorizacion: codigoAutorizacion
            );

            context.DetallesPagoServicio.Add(entity: detalles);
            await context.SaveChangesAsync();

            // 3. Confirmar la transacción de base de datos
            await transaction.CommitAsync();

            return detalles;
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            if (exception is not EMGeneralAggregateException)
            {
                throw GenericExceptionManager.GetAggregateException(
                    serviceName: DomCommon.ServiceName,
                    module: this.GetType().Name,
                    exception: exception);
            }

            throw;
        }
    }
}
