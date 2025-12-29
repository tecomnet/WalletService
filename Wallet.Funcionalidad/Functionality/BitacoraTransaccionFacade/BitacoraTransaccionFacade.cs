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
            new BitacoraTransaccion(idBilletera: idBilletera, monto: monto, tipo: tipo, direccion: direccion, estatus: estatus, creationUser: creationUser, refExternaId: refExternaId);

        context.BitacoraTransaccion.Add(entity: transaccion);
        await context.SaveChangesAsync();

        return transaccion;
    }

    public async Task<List<BitacoraTransaccion>> ObtenerTodasAsync()
    {
        return await context.BitacoraTransaccion
            .Include(navigationPropertyPath: b => b.CuentaWallet)
            .OrderByDescending(keySelector: b => b.CreationTimestamp)
            .ToListAsync();
    }

    public async Task<BitacoraTransaccion> ObtenerPorIdAsync(int id)
    {
        return await context.BitacoraTransaccion
                   .Include(navigationPropertyPath: b => b.CuentaWallet)
                   .FirstOrDefaultAsync(predicate: b => b.Id == id) ??
               throw new KeyNotFoundException(message: $"Transaccion con ID {id} no encontrada.");
    }

    public async Task<List<BitacoraTransaccion>> ObtenerPorClienteAsync(int idCliente)
    {
        return await context.BitacoraTransaccion
            .Include(navigationPropertyPath: b => b.CuentaWallet)
            .Where(predicate: b => b.CuentaWallet!.IdCliente == idCliente)
            .OrderByDescending(keySelector: b => b.CreationTimestamp)
            .ToListAsync();
    }
}
