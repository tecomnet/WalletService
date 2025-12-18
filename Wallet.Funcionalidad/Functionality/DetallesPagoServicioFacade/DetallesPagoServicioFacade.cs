using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;

namespace Wallet.Funcionalidad.Functionality.DetallesPagoServicioFacade;

public class DetallesPagoServicioFacade(
    ServiceDbContext context,
    IBitacoraTransaccionFacade bitacoraTransaccionFacade)
    : IDetallesPagoServicioFacade
{
    public async Task<DetallesPagoServicio> GuardarDetallesAsync(int idTransaccion, int idProveedor,
        string numeroReferencia, Guid creationUser, string? codigoAutorizacion)
    {
        var detalles = new DetallesPagoServicio(idTransaccion, idProveedor, numeroReferencia, creationUser,
            codigoAutorizacion);

        context.DetallesPagoServicio.Add(detalles);
        await context.SaveChangesAsync();

        return detalles;
    }

    public async Task<DetallesPagoServicio> ObtenerPorIdAsync(int id)
    {
        return await context.DetallesPagoServicio
                   .Include(d => d.Transaccion)
                   .FirstOrDefaultAsync(d => d.Id == id)
               ?? throw new KeyNotFoundException($"Detalle de pago {id} no encontrado.");
    }

    public async Task<List<DetallesPagoServicio>> ObtenerPorClienteAsync(int idCliente)
    {
        return await context.DetallesPagoServicio
            .Include(d => d.Transaccion)
            .ThenInclude(t => t.CuentaWallet)
            .Where(d => d.Transaccion!.CuentaWallet!.IdCliente == idCliente)
            .ToListAsync();
    }

    public async Task<List<DetallesPagoServicio>> ObtenerTodosAsync()
    {
        return await context.DetallesPagoServicio.ToListAsync();
    }

    public async Task<List<DetallesPagoServicio>> ObtenerPorTransaccionAsync(int idTransaccion)
    {
        return await context.DetallesPagoServicio
            .Include(d => d.Transaccion)
            .Where(d => d.IdTransaccion == idTransaccion)
            .ToListAsync();
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
                idBilletera,
                monto,
                nombreServicio, // Tipo de transacción (ej. SERVICIO)
                direccion,
                estatus,
                creationUser,
                refExternaId
            );

            // 2. Crear los detalles del pago de servicio vinculados a la transacción
            var detalles = new DetallesPagoServicio(
                transaccion.Id,
                idProveedor,
                numeroReferencia,
                creationUser,
                codigoAutorizacion
            );

            context.DetallesPagoServicio.Add(detalles);
            await context.SaveChangesAsync();

            // 3. Confirmar la transacción de base de datos
            await transaction.CommitAsync();

            return detalles;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
