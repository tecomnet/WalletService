using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;
using Wallet.DOM;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ProveedorFacade;

namespace Wallet.Funcionalidad.Functionality.DetallesPagoServicioFacade;

public class DetallesPagoServicioFacade(
    ServiceDbContext context,
    IBitacoraTransaccionFacade bitacoraTransaccionFacade,
    IProveedorFacade productoFacade)
    : IDetallesPagoServicioFacade
{
    public async Task<DetallesPagoServicio> GuardarDetallesAsync(int idTransaccion, int idProducto,
        string numeroReferencia, Guid creationUser, string? codigoAutorizacion)
    {
        try
        {
            // Obtener el producto
            var producto = await productoFacade.ObtenerProductoPorIdAsync(idProducto: idProducto);
            
            var detalles = new DetallesPagoServicio(transaccionId: idTransaccion, producto: producto,
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
            var result = await context.DetallesPagoServicio
                .Include(navigationPropertyPath: d => d.Transaccion)
                .Where(predicate: d => d.BitacoraTransaccionId == idTransaccion)
                .ToListAsync();

            if (result == null || result.Count == 0)
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.DetallePagoNoEncontrado,
                    dynamicContent: [idTransaccion],
                    module: this.GetType().Name));

            return result;
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

    public async Task<BitacoraTransaccion> RegistrarPagoServicioAsync(
        int idBilletera,
        decimal monto,
        string tipo,
        string direccion,
        string estatus,
        int idProducto,
        string numeroReferencia,
        Guid creationUser)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // 1. Crear la transacción en bitácora
            var transaccion = await bitacoraTransaccionFacade.GuardarTransaccionAsync(
                idBilletera: idBilletera,
                monto: monto,
                tipo: tipo, // Tipo de transacción (ej. SERVICIO)
                direccion: direccion,
                estatus: estatus,
                creationUser: creationUser
            );

            // Obtener el producto
            var producto = await productoFacade.ObtenerProductoPorIdAsync(idProducto: idProducto);
            
            // 2. Crear los detalles del pago de servicio vinculados a la transacción
            var detalles = new DetallesPagoServicio(
                transaccionId: transaccion.Id,
                producto: producto,
                numeroReferencia: numeroReferencia,
                creationUser: creationUser,
                codigoAutorizacion: "Auto-001"
            );

            context.DetallesPagoServicio.Add(entity: detalles);
            await context.SaveChangesAsync();

            // 3. Confirmar la transacción de base de datos
            await transaction.CommitAsync();

            return transaccion;
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
