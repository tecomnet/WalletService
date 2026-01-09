using Microsoft.EntityFrameworkCore;
using Moq;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;
using Wallet.Funcionalidad.Functionality.DetallesPagoServicioFacade;
using Wallet.Funcionalidad.Functionality.ProveedorFacade;
using Wallet.UnitTest.Functionality.Configuration;

namespace Wallet.UnitTest.Functionality;

public class DetallesPagoServicioFacadeTest : BaseFacadeTest<IDetallesPagoServicioFacade>, IDisposable
{
    private readonly IDetallesPagoServicioFacade _facade;
    private readonly Mock<IBitacoraTransaccionFacade> _bitacoraTransaccionFacadeMock;
    private readonly Mock<IProveedorFacade> _proveedorFacadeMock;
    private readonly Guid _userId = Guid.NewGuid();

    public DetallesPagoServicioFacadeTest() : base(setupConfig: new SetupDataConfig())
    {
        _bitacoraTransaccionFacadeMock = new Mock<IBitacoraTransaccionFacade>();
        _facade = new DetallesPagoServicioFacade(context: Context, bitacoraTransaccionFacade: _bitacoraTransaccionFacadeMock.Object, productoFacade: _proveedorFacadeMock.Object);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    private async Task<(CuentaWallet wallet, BitacoraTransaccion transaccion)> SetupDataAsync()
    {
        var empresa = new Empresa(nombre: "TecomTestDetalles", creationUser: _userId);
        Context.Empresa.Add(entity: empresa);

        var usuario = new Usuario(codigoPais: "+52", telefono: "5588888888", correoElectronico: null, contrasena: null, estatus: EstatusRegistroEnum.TerminosCondicionesAceptado,
            creationUser: _userId);
        Context.Usuario.Add(entity: usuario);

        var cliente = new Cliente(usuario: usuario, empresa: empresa, creationUser: _userId);
        cliente.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "Detalles", fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Genero.Femenino, modificationUser: _userId);
        Context.Cliente.Add(entity: cliente);
        await Context.SaveChangesAsync();

        var wallet = new CuentaWallet(idCliente: cliente.Id, moneda: "MXN", cuentaCLABE: "876543210987654321", creationUser: _userId);
        Context.CuentaWallet.Add(entity: wallet);
        await Context.SaveChangesAsync();

        var transaccion = new BitacoraTransaccion(cuentaWalletId: wallet.Id, monto: 200m, tipo: "SERVICIO", direccion: "Cargo", estatus: "Completada", creationUser: _userId);
        Context.BitacoraTransaccion.Add(entity: transaccion);
        await Context.SaveChangesAsync();

        return (wallet, transaccion);
    }

    [Fact]
    public async Task GuardarDetallesAsync_ShouldSaveDetails_WhenValid()
    {
        // Arrange
        var data = await SetupDataAsync();

        // Act
        var result = await _facade.GuardarDetallesAsync(idTransaccion: data.transaccion.Id, idProducto: 101, numeroReferencia: "REF_CFE_123", creationUser: _userId, codigoAutorizacion: "AUTH_999");

        // Assert
        Assert.NotNull(@object: result);
        Assert.NotEqual(expected: 0, actual: result.Id);
        Assert.Equal(expected: data.transaccion.Id, actual: result.BitacoraTransaccionId);
        Assert.Equal(expected: "REF_CFE_123", actual: result.NumeroReferencia);
        Assert.Equal(expected: "AUTH_999", actual: result.CodigoAutorizacion);
    }

    [Fact]
    public async Task ObtenerPorClienteAsync_ShouldReturnDetails_ForSpecificClient()
    {
        // Arrange
        var data = await SetupDataAsync();
        await _facade.GuardarDetallesAsync(idTransaccion: data.transaccion.Id, idProducto: 102, numeroReferencia: "REF_TELMEX", creationUser: _userId, codigoAutorizacion: "AUTH_888");

        // Act
        var result = await _facade.ObtenerPorClienteAsync(idCliente: data.wallet.IdCliente);

        // Assert
        Assert.NotEmpty(collection: result);
        Assert.Contains(collection: result, filter: d => d.NumeroReferencia == "REF_TELMEX");
    }

    [Fact]
    public async Task RegistrarPagoServicioAsync_ShouldCreateTransactionAndDetails()
    {
        // Arrange
        var data = await SetupDataAsync();

        // Pre-create the transaction in DB to satisfy FK constraints for the test
        // because we are mocking the Facade which would normally create it.
        var realTransaccion = new BitacoraTransaccion(cuentaWalletId: data.wallet.Id, monto: 500m, tipo: "SERVICIO", direccion: "Cargo", estatus: "Completada", creationUser: _userId);
        Context.BitacoraTransaccion.Add(entity: realTransaccion);
        await Context.SaveChangesAsync();

        var expectedTransaccion = new BitacoraTransaccion(cuentaWalletId: data.wallet.Id, monto: 500m, tipo: "SERVICIO", direccion: "Cargo", estatus: "Completada",
            creationUser: _userId);
        // Use the ID of the real transaction we just created
        typeof(BitacoraTransaccion).GetProperty(name: "Id")?.SetValue(obj: expectedTransaccion, value: realTransaccion.Id);

        // Setup Mock to return the object with the VALID ID
        _bitacoraTransaccionFacadeMock.Setup(expression: x => x.GuardarTransaccionAsync(
                data.wallet.Id,
                500m,
                "SERVICIO_TEST",
                "Cargo",
                "Completada",
                _userId))
            .ReturnsAsync(value: expectedTransaccion);

        // Act
        var result = await _facade.RegistrarPagoServicioAsync(
            idBilletera: data.wallet.Id,
            monto: 500m,
            tipo: "SERVICIO_TEST",
            idProducto: 105,
            direccion: "Cargo",
            estatus: "Completada",
            numeroReferencia: "REF_SKY_123",
            creationUser: _userId);

        // Assert
        Assert.NotNull(@object: result);
        Assert.Equal(expected: realTransaccion.Id, actual: result.Id);
        Assert.Equal(expected: "REF_SKY_123", actual: result.DetallesPagoServicio.NumeroReferencia);

        // Verify Mock Interaction
        _bitacoraTransaccionFacadeMock.Verify(expression: x => x.GuardarTransaccionAsync(
            data.wallet.Id, 500m, "SERVICIO_TEST", "Cargo", "Completada", _userId), times: Times.Once);

        // Verify Details Persisted in DB
        var savedDetail = await Context.DetallesPagoServicio.FirstOrDefaultAsync(predicate: d => d.Id == result.Id);
        Assert.NotNull(@object: savedDetail);
        Assert.Equal(expected: realTransaccion.Id, actual: savedDetail.BitacoraTransaccionId);
    }
}
