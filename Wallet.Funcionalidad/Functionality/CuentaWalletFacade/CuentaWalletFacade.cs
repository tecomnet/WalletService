using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.Funcionalidad.Functionality.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.CuentaWalletFacade;

public class CuentaWalletFacade(ServiceDbContext context, ITarjetaEmitidaFacade tarjetaEmitidaFacade)
    : ICuentaWalletFacade
{
    public async Task<CuentaWallet> CrearCuentaWalletAsync(int idCliente, Guid creationUser, string moneda = "MXN")
    {
        try
        {
            // Verificar si ya existe una wallet para evitar duplicados
            var existingWallet =
                await context.CuentaWallet.FirstOrDefaultAsync(predicate: w => w.IdCliente == idCliente);
            if (existingWallet != null) return existingWallet;

            // Generar CLABE simulada (18 dígitos)
            // En prod usaría un servicio real o algoritmo específico de banco
            var random = new Random();
            var clabe = $"646{random.NextInt64(minValue: 100000000000000, maxValue: 999999999999999)}";

            var wallet = new CuentaWallet(idCliente: idCliente, moneda: moneda, cuentaCLABE: clabe,
                creationUser: creationUser);

            context.CuentaWallet.Add(entity: wallet);
            await context.SaveChangesAsync();

            // Crear Tarjeta Virtual Inicial Automáticamente
            await tarjetaEmitidaFacade.CrearTarjetaInicialAsync(idCuentaWallet: wallet.Id, creationUser: creationUser);

            return wallet;
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

    public async Task<CuentaWallet> ObtenerPorClienteAsync(int idCliente)
    {
        try
        {
            return await context.CuentaWallet
                       .FirstOrDefaultAsync(predicate: w => w.IdCliente == idCliente)
                   ?? throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                       errorCode: ServiceErrorsBuilder.CuentaWalletNoEncontrada,
                       dynamicContent: [idCliente],
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

    public async Task<CuentaWallet> ObtenerPorIdAsync(int idWallet)
    {
        try
        {
            return await context.CuentaWallet.FindAsync(idWallet)
                   ?? throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                       errorCode: ServiceErrorsBuilder.CuentaWalletNoEncontrada,
                       dynamicContent: [idWallet],
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

    public async Task<CuentaWallet> ActualizarSaldoAsync(int idWallet, decimal nuevoSaldo, Guid modificationUser)
    {
        try
        {
            var wallet = await context.CuentaWallet.FindAsync(keyValues: idWallet)
                         ?? throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                             errorCode: ServiceErrorsBuilder.CuentaWalletNoEncontrada,
                             dynamicContent: [idWallet],
                             module: this.GetType().Name));

            // Validar que la wallet esté activa
            if (!wallet.IsActive)
            {
                // Nota: Usamos una excepción genérica si no tenemos el builder inyectado aquí,
                // pero lo ideal es usar EMGeneralAggregateException consistente con el resto.
                // Dado que esta clase no usa DomCommon ni ServiceErrorsBuilder explícitamente en los usings actuales,
                // agregaré la excepción estándar.
                throw new EMGeneralAggregateException(
                    exception: DomCommon.BuildEmGeneralException(errorCode: ServiceErrorsBuilder.CuentaWalletInactiva,
                        dynamicContent: [], module: this.GetType().Name));
            }

            wallet.ActualizarSaldo(nuevoSaldo: nuevoSaldo, modificationUser: modificationUser);
            context.Entry(entity: wallet).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return wallet;
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
}
