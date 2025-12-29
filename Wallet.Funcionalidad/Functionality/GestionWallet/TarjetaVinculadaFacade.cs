using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.DOM.Enums;

namespace Wallet.Funcionalidad.Functionality.GestionWallet;

public class TarjetaVinculadaFacade(ServiceDbContext context) : ITarjetaVinculadaFacade
{
    public async Task<TarjetaVinculada> VincularTarjetaAsync(int idCliente, string tokenPasarela, string panEnmascarado,
        string alias, MarcaTarjeta marca, DateTime fechaExpiracion, Guid creationUser, string? gatewayCustomerId = null)
    {
        var cuenta = await context.CuentaWallet.FirstOrDefaultAsync(c => c.IdCliente == idCliente);
        if (cuenta == null)
            throw new KeyNotFoundException($"No se encontró una cuenta Wallet para el cliente {idCliente}");

        // Crear la entidad TarjetaVinculada
        var tarjeta = new TarjetaVinculada(
            idCuentaWallet: cuenta.Id,
            tokenPasarela: tokenPasarela,
            panEnmascarado: panEnmascarado,
            alias: alias,
            marca: marca,
            fechaExpiracion: fechaExpiracion,
            creationUser: creationUser,
            gatewayCustomerId: gatewayCustomerId
        );

        // Persistir
        context.TarjetaVinculada.Add(entity: tarjeta);
        await context.SaveChangesAsync();


        return tarjeta;
    }

    public async Task<List<TarjetaVinculada>> ObtenerTarjetasPorClienteAsync(int idCliente)
    {
        return await context.TarjetaVinculada
            .Include(t => t.CuentaWallet)
            .Where(t => t.CuentaWallet.IdCliente == idCliente)
            .ToListAsync();
    }

    public async Task DesvincularTarjetaAsync(int idTarjeta, Guid modificationUser)
    {
        var tarjeta = await context.TarjetaVinculada
                          .FirstOrDefaultAsync(t => t.Id == idTarjeta)
                      ?? throw new KeyNotFoundException($"Tarjeta vinculada con ID {idTarjeta} no encontrada.");

        tarjeta.Deactivate(modificationUser);
        await context.SaveChangesAsync();
    }

    public async Task EstablecerFavoritaAsync(int idTarjeta, int idCliente, string concurrencyToken,
        Guid modificationUser)
    {
        // First find the account for the client
        var cuenta = await context.CuentaWallet.FirstOrDefaultAsync(c => c.IdCliente == idCliente);
        if (cuenta == null)
            throw new KeyNotFoundException($"No se encontró una cuenta Wallet para el cliente {idCliente}");

        // Now find cards for that account
        var tarjetas = await context.TarjetaVinculada
            .Where(t => t.IdCuentaWallet == cuenta.Id)
            .ToListAsync();

        var tarjetaObjetivo = tarjetas.FirstOrDefault(t => t.Id == idTarjeta);

        if (tarjetaObjetivo == null)
            throw new KeyNotFoundException(
                $"Tarjeta vinculada con ID {idTarjeta} no encontrada para el cliente {idCliente}.");

        if (Convert.ToBase64String(tarjetaObjetivo.ConcurrencyToken) != concurrencyToken)
            throw new DbUpdateConcurrencyException("La tarjeta ha sido modificada por otro usuario.");

        foreach (var tarjeta in tarjetas)
        {
            if (tarjeta.Id == idTarjeta)
            {
                if (!tarjeta.EsFavorita)
                    tarjeta.EstablecerComoFavorita(true, modificationUser);
            }
            else if (tarjeta.EsFavorita)
            {
                tarjeta.EstablecerComoFavorita(false, modificationUser);
            }
        }

        await context.SaveChangesAsync();
    }
}
