using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.CuentaWalletFacade;

public class CuentaWalletFacade(ServiceDbContext context) : ICuentaWalletFacade
{
    public async Task<CuentaWallet> CrearCuentaWalletAsync(Guid idCliente, Guid creationUser, string moneda = "MXN")
    {
        // Verificar si ya existe una wallet para evitar duplicados
        var existingWallet = await context.CuentaWallet.FirstOrDefaultAsync(w => w.IdCliente == idCliente);
        if (existingWallet != null) return existingWallet;

        // Generar CLABE simulada (18 dígitos)
        // En prod usaría un servicio real o algoritmo específico de banco
        var random = new Random();
        var clabe = $"646{random.NextInt64(100000000000000, 999999999999999)}";

        var wallet = new CuentaWallet(idCliente, moneda, clabe, creationUser);

        context.CuentaWallet.Add(wallet);
        await context.SaveChangesAsync();

        return wallet;
    }

    public async Task<CuentaWallet> ObtenerPorClienteAsync(Guid idCliente)
    {
        return await context.CuentaWallet
                   .Include(w => w.BitacoraTransacciones)
                   .FirstOrDefaultAsync(w => w.IdCliente == idCliente)
               ?? throw new KeyNotFoundException($"No se encontró wallet para el cliente {idCliente}");
    }

    public async Task<CuentaWallet> ActualizarSaldoAsync(int idWallet, decimal nuevoSaldo, Guid modificationUser)
    {
        var wallet = await context.CuentaWallet.FindAsync(idWallet)
                     ?? throw new KeyNotFoundException($"Wallet {idWallet} no encontrada.");

        wallet.ActualizarSaldo(nuevoSaldo, modificationUser);
        context.Entry(wallet).State = EntityState.Modified;

        await context.SaveChangesAsync();
        return wallet;
    }
}
