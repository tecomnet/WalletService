using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionWallet;

namespace Wallet.Funcionalidad.Functionality.GestionWallet;

public class TarjetaEmitidaFacade(ServiceDbContext context) : ITarjetaEmitidaFacade
{
    public async Task<TarjetaEmitida> CrearTarjetaInicialAsync(int idCuentaWallet, Guid creationUser)
    {
        // 1. Generar datos simulados para la tarjeta (PAN, Token)
        // En un escenario real, esto vendría de una integración con un procesador (e.g. Galileo, Marqeta)
        // 1. Generar datos simulados
        var datosSimulados = GenerarDatosTarjetaSimulados();
        var panSimulado = datosSimulados.Pan;
        var tokenProcesador = datosSimulados.Token;

        // 2. Crear la tarjeta (Por defecto Virtual)
        var tarjeta = new TarjetaEmitida(
            idCuentaWallet: idCuentaWallet,
            tokenProcesador: tokenProcesador,
            panEnmascarado: panSimulado,
            tipo: TipoTarjeta.Virtual,
            fechaExpiracion: DateTime.UtcNow.AddYears(value: 3), // 3 años de vigencia
            creationUser: creationUser
        );

        // 3. Activar la tarjeta inmediatamente (Regla de negocio: Virtual nace activa)
        tarjeta.ActivarTarjeta(modificationUser: creationUser);

        // 4. Persistir
        context.TarjetaEmitida.Add(entity: tarjeta);
        await context.SaveChangesAsync();

        return tarjeta;
    }

    public async Task<TarjetaEmitida> ObtenerTarjetaPorIdAsync(int idTarjeta)
    {
        var tarjeta = await context.TarjetaEmitida
                          .FirstOrDefaultAsync(predicate: t => t.Id == idTarjeta)
                      ?? throw new KeyNotFoundException(message: $"Tarjeta con ID {idTarjeta} no encontrada.");

        // Lazy Expiration Check
        // Usamos Guid.Empty como "System User" para esta operación automática
        if (tarjeta.VerificarExpiracion(modificationUser: Guid.Empty))
        {
            await context.SaveChangesAsync();
        }

        return tarjeta;
    }

    public async Task<List<TarjetaEmitida>> ObtenerTarjetasPorClienteAsync(int idCliente)
    {
        return await context.TarjetaEmitida
            .Include(t => t.CuentaWallet)
            .Where(t => t.CuentaWallet != null && t.CuentaWallet.IdCliente == idCliente)
            .ToListAsync();
    }

    public async Task CambiarEstadoBloqueoAsync(int idTarjeta, bool bloquear, string concurrencyToken,
        Guid modificationUser)
    {
        var tarjeta = await ObtenerTarjetaPorIdAsync(idTarjeta);

        if (Convert.ToBase64String(tarjeta.ConcurrencyToken) != concurrencyToken)
            throw new DbUpdateConcurrencyException("La tarjeta ha sido modificada por otro usuario.");

        tarjeta.BloquearTemporalmente(bloquear, modificationUser);
        await context.SaveChangesAsync();
    }

    public async Task ActualizarConfiguracionAsync(int idTarjeta, decimal nuevoLimite, bool comprasLinea, bool retiros,
        string concurrencyToken, Guid modificationUser)
    {
        var tarjeta = await ObtenerTarjetaPorIdAsync(idTarjeta);

        if (Convert.ToBase64String(tarjeta.ConcurrencyToken) != concurrencyToken)
            throw new DbUpdateConcurrencyException("La tarjeta ha sido modificada por otro usuario.");

        tarjeta.ConfigurarReglas(nuevoLimite, comprasLinea, retiros, modificationUser);
        await context.SaveChangesAsync();
    }


    public async Task<TarjetaEmitida> SolicitarTarjetaVirtualAdicionalAsync(int idCliente, Guid creationUser)
    {
        var cuenta = await context.CuentaWallet
                         .FirstOrDefaultAsync(c => c.IdCliente == idCliente)
                     ?? throw new KeyNotFoundException($"No se encontró cuenta wallet para el cliente {idCliente}.");

        var datosSimulados = GenerarDatosTarjetaSimulados();

        var tarjeta = new TarjetaEmitida(
            idCuentaWallet: cuenta.Id,
            tokenProcesador: datosSimulados.Token,
            panEnmascarado: datosSimulados.Pan,
            tipo: TipoTarjeta.Virtual,
            fechaExpiracion: DateTime.UtcNow.AddYears(3),
            creationUser: creationUser
        );

        // Las virtuales nacen activas
        tarjeta.ActivarTarjeta(creationUser);

        context.TarjetaEmitida.Add(tarjeta);
        await context.SaveChangesAsync();

        return tarjeta;
    }

    public async Task<TarjetaEmitida> SolicitarTarjetaFisicaAsync(int idCliente, string nombreImpreso,
        Guid creationUser)
    {
        var cuenta = await context.CuentaWallet
                         .FirstOrDefaultAsync(c => c.IdCliente == idCliente)
                     ?? throw new KeyNotFoundException($"No se encontró cuenta wallet para el cliente {idCliente}.");

        var datosSimulados = GenerarDatosTarjetaSimulados();

        var tarjeta = new TarjetaEmitida(
            idCuentaWallet: cuenta.Id,
            tokenProcesador: datosSimulados.Token,
            panEnmascarado: datosSimulados.Pan,
            tipo: TipoTarjeta.Fisica,
            fechaExpiracion: DateTime.UtcNow.AddYears(5), // Físicas suelen durar más
            creationUser: creationUser,
            nombreImpreso: nombreImpreso
        );

        // Las físicas nacen inactivas y con estado de entrega Solicitada (Logic inside constructor handles validation/defaults mostly, but specific logic check)
        // Constructor sets EstadoEntrega = Solicitada for Fisica.
        // Constructor sets Estado = Inactiva by default.

        context.TarjetaEmitida.Add(tarjeta);
        await context.SaveChangesAsync();

        return tarjeta;
    }

    private (string Pan, string Token) GenerarDatosTarjetaSimulados()
    {
        var random = new Random();
        // Generar un número aleatorio de 4 dígitos para el final del PAN
        var suffix = random.Next(1000, 9999);
        var panSimulado = $"400000******{suffix}";
        var tokenProcesador = Guid.NewGuid().ToString("N");

        return (panSimulado, tokenProcesador);
    }
}
