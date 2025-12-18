using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;

public class BitacoraTransaccionFacade(ServiceDbContext context) : IBitacoraTransaccionFacade
{
    public async Task<BitacoraTransaccion> GuardarTransaccionAsync(int idBilletera, decimal monto, string tipo,
        string direccion,
        string estatus, Guid creationUser, string? refExternaId)
    {
        var transaccion =
            new BitacoraTransaccion(idBilletera, monto, tipo, direccion, estatus, creationUser, refExternaId);

        context.BitacoraTransaccion.Add(transaccion);
        await context.SaveChangesAsync();

        return transaccion;
    }

    public async Task<List<BitacoraTransaccion>> ObtenerTodasAsync()
    {
        return await context.BitacoraTransaccion
            .Include(b => b.CuentaWallet)
            .OrderByDescending(b => b.CreationTimestamp)
            .ToListAsync();
    }

    public async Task<BitacoraTransaccion> ObtenerPorIdAsync(int id)
    {
        return await context.BitacoraTransaccion
                   .Include(b => b.CuentaWallet)
                   .FirstOrDefaultAsync(b => b.Id == id) ??
               throw new KeyNotFoundException($"Transaccion con ID {id} no encontrada.");
    }

    public async Task<List<BitacoraTransaccion>> ObtenerPorClienteAsync(Guid idCliente)
    {
        return await context.BitacoraTransaccion
            .Include(b => b.CuentaWallet)
            .Where(b => b.CuentaWallet!.IdCliente == idCliente)
            .OrderByDescending(b => b.CreationTimestamp)
            .ToListAsync();
    }
}
