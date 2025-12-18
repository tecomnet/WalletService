using Microsoft.EntityFrameworkCore;
using Moq;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;
using Wallet.Funcionalidad.Functionality.DetallesPagoServicioFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit;

namespace Wallet.UnitTest.Functionality;

public class DetallesPagoServicioFacadeTest : BaseFacadeTest<IDetallesPagoServicioFacade>, IDisposable
{
    private readonly IDetallesPagoServicioFacade _facade;
    private readonly Mock<IBitacoraTransaccionFacade> _bitacoraTransaccionFacadeMock;
    private readonly Guid _userId = Guid.NewGuid();

    public DetallesPagoServicioFacadeTest() : base(new SetupDataConfig())
    {
        _bitacoraTransaccionFacadeMock = new Mock<IBitacoraTransaccionFacade>();
        _facade = new DetallesPagoServicioFacade(Context, _bitacoraTransaccionFacadeMock.Object);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    private async Task<(CuentaWallet wallet, BitacoraTransaccion transaccion)> SetupDataAsync()
    {
        var empresa = new Empresa("TecomTestDetalles", _userId);
        Context.Empresa.Add(empresa);

        var usuario = new Usuario("+52", "5588888888", null, null, EstatusRegistroEnum.TerminosCondicionesAceptado,
            _userId);
        Context.Usuario.Add(usuario);

        var cliente = new Cliente(usuario, empresa, _userId);
        cliente.AgregarDatosPersonales("Test", "User", "Detalles", new DateOnly(1990, 1, 1), Genero.Femenino, _userId);
        Context.Cliente.Add(cliente);
        await Context.SaveChangesAsync();

        var wallet = new CuentaWallet(cliente.Guid, "MXN", "876543210987654321", _userId);
        Context.CuentaWallet.Add(wallet);
        await Context.SaveChangesAsync();

        var transaccion = new BitacoraTransaccion(wallet.Id, 200m, "SERVICIO", "Cargo", "Completada", _userId);
        Context.BitacoraTransaccion.Add(transaccion);
        await Context.SaveChangesAsync();

        return (wallet, transaccion);
    }

    [Fact]
    public async Task GuardarDetallesAsync_ShouldSaveDetails_WhenValid()
    {
        // Arrange
        var data = await SetupDataAsync();

        // Act
        var result = await _facade.GuardarDetallesAsync(data.transaccion.Id, 101, "REF_CFE_123", _userId, "AUTH_999");

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal(data.transaccion.Id, result.IdTransaccion);
        Assert.Equal("REF_CFE_123", result.NumeroReferencia);
        Assert.Equal("AUTH_999", result.CodigoAutorizacion);
    }

    [Fact]
    public async Task ObtenerPorClienteAsync_ShouldReturnDetails_ForSpecificClient()
    {
        // Arrange
        var data = await SetupDataAsync();
        await _facade.GuardarDetallesAsync(data.transaccion.Id, 102, "REF_TELMEX", _userId, "AUTH_888");

        // Act
        var result = await _facade.ObtenerPorClienteAsync(data.wallet.IdCliente);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, d => d.NumeroReferencia == "REF_TELMEX");
    }

    [Fact]
    public async Task RegistrarPagoServicioAsync_ShouldCreateTransactionAndDetails()
    {
        // Arrange
        var data = await SetupDataAsync();

        // Pre-create the transaction in DB to satisfy FK constraints for the test
        // because we are mocking the Facade which would normally create it.
        var realTransaccion = new BitacoraTransaccion(data.wallet.Id, 500m, "SERVICIO", "Cargo", "Completada", _userId);
        Context.BitacoraTransaccion.Add(realTransaccion);
        await Context.SaveChangesAsync();

        var expectedTransaccion = new BitacoraTransaccion(data.wallet.Id, 500m, "SERVICIO", "Cargo", "Completada",
            _userId);
        // Use the ID of the real transaction we just created
        typeof(BitacoraTransaccion).GetProperty("Id")?.SetValue(expectedTransaccion, realTransaccion.Id);

        // Setup Mock to return the object with the VALID ID
        _bitacoraTransaccionFacadeMock.Setup(x => x.GuardarTransaccionAsync(
                data.wallet.Id,
                500m,
                "SERVICIO_TEST",
                "Cargo",
                "Completada",
                _userId,
                It.IsAny<string?>()))
            .ReturnsAsync(expectedTransaccion);

        // Act
        var result = await _facade.RegistrarPagoServicioAsync(
            data.wallet.Id,
            500m,
            "SERVICIO_TEST",
            "Cargo",
            "Completada",
            null,
            105,
            "REF_SKY_123",
            "AUTH_777",
            _userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(realTransaccion.Id, result.IdTransaccion);
        Assert.Equal("REF_SKY_123", result.NumeroReferencia);

        // Verify Mock Interaction
        _bitacoraTransaccionFacadeMock.Verify(x => x.GuardarTransaccionAsync(
            data.wallet.Id, 500m, "SERVICIO_TEST", "Cargo", "Completada", _userId, It.IsAny<string?>()), Times.Once);

        // Verify Details Persisted in DB
        var savedDetail = await Context.DetallesPagoServicio.FirstOrDefaultAsync(d => d.Id == result.Id);
        Assert.NotNull(savedDetail);
        Assert.Equal(realTransaccion.Id, savedDetail.IdTransaccion);
    }
}
