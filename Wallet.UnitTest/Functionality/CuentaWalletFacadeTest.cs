using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.CuentaWalletFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Moq;
using Wallet.Funcionalidad.Functionality.GestionWallet;

namespace Wallet.UnitTest.Functionality;

public class CuentaWalletFacadeTest : BaseFacadeTest<ICuentaWalletFacade>, IDisposable
{
    private readonly ICuentaWalletFacade _cuentaWalletFacade;
    private readonly Mock<ITarjetaEmitidaFacade> _mockTarjetaEmitidaFacade;
    private readonly Guid _userId = Guid.NewGuid();

    public CuentaWalletFacadeTest() : base(setupConfig: new SetupDataConfig())
    {
        _mockTarjetaEmitidaFacade = new Mock<ITarjetaEmitidaFacade>();
        _cuentaWalletFacade = new CuentaWalletFacade(context: Context, tarjetaEmitidaFacade: _mockTarjetaEmitidaFacade.Object);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    [Fact]
    public async Task CrearCuentaWalletAsync_ShouldCreateWallet_WhenCalled()
    {
        // Arrange
        var empresa = new Empresa(nombre: "Tecom", creationUser: _userId);
        Context.Empresa.Add(entity: empresa);

        var usuario = new Usuario(codigoPais: "+52", telefono: "5500000000", correoElectronico: null, contrasena: null, estatus: EstatusRegistroEnum.TerminosCondicionesAceptado,
            creationUser: _userId);
        Context.Usuario.Add(entity: usuario);

        var cliente = new Cliente(usuario: usuario, empresa: empresa, creationUser: _userId);
        cliente.AgregarDatosPersonales(
            nombre: "Juan",
            primerApellido: "Perez",
            segundoApellido: "Lopez",
            fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1),
            genero: Genero.Masculino,
            modificationUser: _userId
        );

        Context.Cliente.Add(entity: cliente);
        await Context.SaveChangesAsync();

        // Act
        var wallet = await _cuentaWalletFacade.CrearCuentaWalletAsync(idCliente: cliente.Id, creationUser: _userId);

        // Assert
        Assert.NotNull(@object: wallet);

        // Verify card creation was called
        _mockTarjetaEmitidaFacade.Verify(expression: t => t.CrearTarjetaInicialAsync(wallet.Id, _userId), times: Times.Once);

        Assert.Equal(expected: cliente.Id, actual: wallet.IdCliente);
        Assert.Equal(expected: "MXN", actual: wallet.Moneda);
        Assert.NotNull(@object: wallet.CuentaCLABE);
        Assert.Equal(expected: 18, actual: wallet.CuentaCLABE.Length);
        Assert.Equal(expected: 0, actual: wallet.SaldoActual);
    }

    [Fact]
    public async Task ObtenerPorClienteAsync_ShouldReturnWallet_WhenExists()
    {
        // Arrange
        var empresa = new Empresa(nombre: "Tecom2", creationUser: _userId);
        Context.Empresa.Add(entity: empresa);

        var usuario = new Usuario(codigoPais: "+52", telefono: "5511111111", correoElectronico: null, contrasena: null, estatus: EstatusRegistroEnum.TerminosCondicionesAceptado,
            creationUser: _userId);
        Context.Usuario.Add(entity: usuario);

        var cliente = new Cliente(usuario: usuario, empresa: empresa, creationUser: _userId);
        cliente.AgregarDatosPersonales(nombre: "Maria", primerApellido: "Gomez", segundoApellido: "Ruiz", fechaNacimiento: new DateOnly(year: 1995, month: 5, day: 5), genero: Genero.Femenino, modificationUser: _userId);

        Context.Cliente.Add(entity: cliente);
        await Context.SaveChangesAsync();

        var wallet = await _cuentaWalletFacade.CrearCuentaWalletAsync(idCliente: cliente.Id, creationUser: _userId);

        // Act
        var retrievedWallet = await _cuentaWalletFacade.ObtenerPorClienteAsync(idCliente: cliente.Id);

        // Assert
        Assert.NotNull(@object: retrievedWallet);
        Assert.Equal(expected: wallet.Id, actual: retrievedWallet!.Id);
    }

    [Fact]
    public async Task ActualizarSaldoAsync_ShouldUpdateBalance_WhenCalled()
    {
        // Arrange
        var empresa = new Empresa(nombre: "Tecom3", creationUser: _userId);
        Context.Empresa.Add(entity: empresa);

        var usuario = new Usuario(codigoPais: "+52", telefono: "5522222222", correoElectronico: null, contrasena: null, estatus: EstatusRegistroEnum.TerminosCondicionesAceptado,
            creationUser: _userId);
        Context.Usuario.Add(entity: usuario);

        var cliente = new Cliente(usuario: usuario, empresa: empresa, creationUser: _userId);
        cliente.AgregarDatosPersonales(nombre: "Pedro", primerApellido: "Diaz", segundoApellido: "Sanz", fechaNacimiento: new DateOnly(year: 1988, month: 8, day: 8), genero: Genero.Masculino, modificationUser: _userId);

        Context.Cliente.Add(entity: cliente);
        await Context.SaveChangesAsync();

        var wallet = await _cuentaWalletFacade.CrearCuentaWalletAsync(idCliente: cliente.Id, creationUser: _userId);

        // Act
        await _cuentaWalletFacade.ActualizarSaldoAsync(idWallet: wallet.Id, nuevoSaldo: 500.00m, modificationUser: _userId);
        var updatedWallet = await _cuentaWalletFacade.ObtenerPorClienteAsync(idCliente: cliente.Id);

        // Assert
        Assert.NotNull(@object: updatedWallet);
        Assert.Equal(expected: 500.00m, actual: updatedWallet!.SaldoActual);
    }
}
