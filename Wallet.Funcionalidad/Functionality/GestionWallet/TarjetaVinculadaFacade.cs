using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Comun;
using Wallet.DOM;

namespace Wallet.Funcionalidad.Functionality.GestionWallet;

public class TarjetaVinculadaFacade(ServiceDbContext context) : ITarjetaVinculadaFacade
{
    public async Task<TarjetaVinculada> VincularTarjetaAsync(int idCliente, string tokenPasarela, string panEnmascarado,
        string alias, MarcaTarjeta marca, DateTime fechaExpiracion, Guid creationUser, string? gatewayCustomerId = null)
    {
        await ValidarClienteYUsuarioActivos(idCliente: idCliente);

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

        // Validar estatus de cliente y usuario
        await context.Entry(entity: tarjeta).Reference(propertyExpression: t => t.CuentaWallet).LoadAsync();
        await ValidarClienteYUsuarioActivos(idCliente: tarjeta.CuentaWallet.IdCliente);

        tarjeta.Deactivate(modificationUser);
        await context.SaveChangesAsync();
    }

    public async Task EstablecerFavoritaAsync(int idTarjeta, int idCliente, string concurrencyToken,
        Guid modificationUser)
    {
        await ValidarClienteYUsuarioActivos(idCliente: idCliente);

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

    /// <summary>
    /// Valida que el cliente y su usuario asociado estén activos.
    /// </summary>
    /// <param name="idCliente">Identificador del cliente.</param>
    private async Task ValidarClienteYUsuarioActivos(int idCliente)
    {
        var cliente = await context.Cliente
                          .Include(navigationPropertyPath: c => c.Usuario)
                          .FirstOrDefaultAsync(predicate: c => c.Id == idCliente)
                      ?? throw new KeyNotFoundException(message: $"Cliente {idCliente} no encontrado.");

        if (!cliente.IsActive)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ClienteInactivo,
                dynamicContent: [cliente.NombreCompleto ?? "Cliente"],
                module: this.GetType().Name));
        }

        if (!cliente.Usuario.IsActive)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.UsuarioInactivo,
                dynamicContent: [],
                module: this.GetType().Name));
        }
    }
}
