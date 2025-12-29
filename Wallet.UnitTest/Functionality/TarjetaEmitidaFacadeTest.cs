using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.GestionWallet;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit;

namespace Wallet.UnitTest.Functionality;

public class TarjetaEmitidaFacadeTest : BaseFacadeTest<ITarjetaEmitidaFacade>, IDisposable
{
    private readonly ITarjetaEmitidaFacade _tarjetaEmitidaFacade;
    private readonly Guid _userId = Guid.NewGuid();

    public TarjetaEmitidaFacadeTest() : base(setupConfig: new SetupDataConfig())
    {
        _tarjetaEmitidaFacade = new TarjetaEmitidaFacade(context: Context);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    [Fact]
    public async Task SolicitarTarjetaVirtualAdicionalAsync_ShouldCreateActiveVirtualCard()
    {
        // Arrange
        var cliente = Context.Cliente.First();

        // Act
        var tarjeta = await _tarjetaEmitidaFacade.SolicitarTarjetaVirtualAdicionalAsync(cliente.Id, _userId);

        // Assert
        Assert.NotNull(tarjeta);
        Assert.Equal(TipoTarjeta.Virtual, tarjeta.Tipo);
        Assert.Equal(EstadoTarjeta.Activa, tarjeta.Estado);
    }

    [Fact]
    public async Task SolicitarTarjetaFisicaAsync_ShouldCreateInactivePhysicalCard()
    {
        // Arrange
        var cliente = Context.Cliente.First();
        string nombreImpreso = "TEST USER";

        // Act
        var tarjeta = await _tarjetaEmitidaFacade.SolicitarTarjetaFisicaAsync(cliente.Id, nombreImpreso, _userId);

        // Assert
        Assert.NotNull(tarjeta);
        Assert.Equal(TipoTarjeta.Fisica, tarjeta.Tipo);
        Assert.Equal(EstadoTarjeta.Inactiva, tarjeta.Estado);
        Assert.Equal(EstadoEntrega.Solicitada, tarjeta.EstadoEntrega);
        Assert.Equal(nombreImpreso, tarjeta.NombreImpreso);
    }

    [Fact]
    public async Task ObtenerTarjetasPorClienteAsync_ShouldReturnAllCards()
    {
        // Arrange
        var cliente = Context.Cliente.First();
        // SetupConfig creates 2 cards.

        // Act
        var tarjetas = await _tarjetaEmitidaFacade.ObtenerTarjetasPorClienteAsync(cliente.Id);

        // Assert
        Assert.NotEmpty(tarjetas);
        Assert.True(tarjetas.Count >= 2);
    }

    [Fact]
    public async Task VerificarExpiracion_ShouldMarkAsExpired_WhenDatePassed()
    {
        // Arrange
        var cliente = Context.Cliente.First();
        var cuenta = Context.CuentaWallet.First(c => c.IdCliente == cliente.Id);

        var tarjetaVencida = new TarjetaEmitida(
            idCuentaWallet: cuenta.Id,
            tokenProcesador: "TOKEN_EXP",
            panEnmascarado: "4000******1234",
            tipo: TipoTarjeta.Virtual,
            fechaExpiracion: DateTime.UtcNow.AddYears(1), // Valid future date
            creationUser: _userId
        );
        // Force state active initially
        tarjetaVencida.ActivarTarjeta(_userId);
        Context.TarjetaEmitida.Add(tarjetaVencida);
        await Context.SaveChangesAsync();

        // Force expiration date to past
        Context.Entry(tarjetaVencida).Property(t => t.FechaExpiracion).CurrentValue = DateTime.UtcNow.AddDays(-1);
        await Context.SaveChangesAsync();

        // Act - Calling GetById triggers the lazy check
        var tarjetaRecuperada = await _tarjetaEmitidaFacade.ObtenerTarjetaPorIdAsync(tarjetaVencida.Id);

        // Assert
        Assert.Equal(EstadoTarjeta.Expirada, tarjetaRecuperada.Estado);
    }
}
