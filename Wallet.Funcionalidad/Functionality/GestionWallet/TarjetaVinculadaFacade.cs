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
    public async Task<TarjetaVinculada> VincularTarjetaAsync(int idCliente, string numeroTarjeta,
        string alias, MarcaTarjeta marca, DateTime fechaExpiracion, Guid creationUser, string? gatewayCustomerId = null)
    {
        try
        {
            await ValidarClienteYUsuarioActivos(idCliente: idCliente);

            var cuenta = await context.CuentaWallet.FirstOrDefaultAsync(c => c.IdCliente == idCliente);
            if (cuenta == null)
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.CuentaWalletNoEncontrada,
                    dynamicContent: [idCliente],
                    module: this.GetType().Name));

            // Check if card with same number exists for this client (active or inactive)
            // Note: Using IgnoreQueryFilters() to find soft-deleted (inactive) records if Global Query Filters are applied for soft delete,
            // though typically IsActive filter is manual. If standard queries filter IsActive=true, we might need to bypass that.
            // Assuming context.TarjetaVinculada queries might differ based on configuration, but standard EF Core doesn't auto-filter bool IsActive unless configured.
            // However, let's explicitly look for any card for this wallet with this number.

            var tarjetaExistente = await context.TarjetaVinculada
                .FirstOrDefaultAsync(t => t.IdCuentaWallet == cuenta.Id && t.NumeroTarjeta == numeroTarjeta);

            if (tarjetaExistente != null)
            {
                // Validar si la tarjeta ya está activa
                if (tarjetaExistente.IsActive)
                {
                    throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                        errorCode: ServiceErrorsBuilder.TarjetaYaVinculada,
                        dynamicContent: [],
                        module: this.GetType().Name));
                }

                // Si existe pero está inactiva, la reactivamos
                tarjetaExistente.Reactivar(
                    alias: alias,
                    marca: marca,
                    fechaExpiracion: fechaExpiracion,
                    gatewayCustomerId: gatewayCustomerId,
                    modificationUser: creationUser);

                await context.SaveChangesAsync();
                return tarjetaExistente;
            }

            // Crear la entidad TarjetaVinculada
            var tarjeta = new TarjetaVinculada(
                idCuentaWallet: cuenta.Id,
                numeroTarjeta: numeroTarjeta,
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
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<List<TarjetaVinculada>> ObtenerTarjetasPorClienteAsync(int idCliente)
    {
        try
        {
            return await context.TarjetaVinculada
                .Include(t => t.CuentaWallet)
                .Where(t => t.CuentaWallet.IdCliente == idCliente && t.IsActive)
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

    public async Task DesvincularTarjetaAsync(int idTarjeta, Guid modificationUser)
    {
        try
        {
            var tarjeta = await context.TarjetaVinculada
                              .FirstOrDefaultAsync(t => t.Id == idTarjeta)
                          ?? throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                              errorCode: ServiceErrorsBuilder.TarjetaVinculadaNoEncontrada,
                              dynamicContent: [idTarjeta],
                              module: this.GetType().Name));

            // Validar estatus de cliente y usuario
            await context.Entry(entity: tarjeta).Reference(propertyExpression: t => t.CuentaWallet).LoadAsync();
            await ValidarClienteYUsuarioActivos(idCliente: tarjeta.CuentaWallet.IdCliente);

            tarjeta.Deactivate(modificationUser);
            await context.SaveChangesAsync();
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

    public async Task<TarjetaVinculada> EstablecerFavoritaAsync(int idTarjeta, string concurrencyToken,
        Guid modificationUser)
    {
        try
        {
            var tarjetaObjetivo = await context.TarjetaVinculada
                                      .Include(t => t.CuentaWallet)
                                      .FirstOrDefaultAsync(t => t.Id == idTarjeta)
                                  ?? throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                                      errorCode: ServiceErrorsBuilder.TarjetaVinculadaNoEncontrada,
                                      dynamicContent: [idTarjeta],
                                      module: this.GetType().Name));

            var idCliente = tarjetaObjetivo.CuentaWallet.IdCliente;

            await ValidarClienteYUsuarioActivos(idCliente: idCliente);

            if (Convert.ToBase64String(tarjetaObjetivo.ConcurrencyToken) != concurrencyToken)
                throw new DbUpdateConcurrencyException(ServiceErrorsBuilder.Instance()
                    .GetError(ServiceErrorsBuilder.ConcurrencyError).Message);

            // Now find other cards for that account to unset favorite
            var tarjetas = await context.TarjetaVinculada
                .Where(t => t.IdCuentaWallet == tarjetaObjetivo.IdCuentaWallet)
                .ToListAsync();

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
            return tarjetaObjetivo;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not DbUpdateConcurrencyException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
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
                      ?? throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                          errorCode: ServiceErrorsBuilder.ClienteNoEncontrado,
                          dynamicContent: [],
                          module: this.GetType().Name));

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
