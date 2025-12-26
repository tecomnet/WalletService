using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.Funcionalidad.Functionality.CuentaWalletFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit;

namespace Wallet.UnitTest.Functionality;

public class CuentaWalletFacadeTest : BaseFacadeTest<ICuentaWalletFacade>, IDisposable
{
    private readonly ICuentaWalletFacade _cuentaWalletFacade;
    private readonly Guid _userId = Guid.NewGuid();

    public CuentaWalletFacadeTest() : base(new SetupDataConfig())
    {
        _cuentaWalletFacade = new CuentaWalletFacade(Context);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    [Fact]
    public async Task CrearCuentaWalletAsync_ShouldCreateWallet_WhenCalled()
    {
        // Arrange
        var empresa = new Empresa("Tecom", _userId);
        Context.Empresa.Add(empresa);

        var usuario = new Usuario("+52", "5500000000", null, null, EstatusRegistroEnum.TerminosCondicionesAceptado,
            _userId);
        Context.Usuario.Add(usuario);

        var cliente = new Cliente(usuario, empresa, _userId);
        cliente.AgregarDatosPersonales(
            nombre: "Juan",
            primerApellido: "Perez",
            segundoApellido: "Lopez",
            fechaNacimiento: new DateOnly(1990, 1, 1),
            genero: Genero.Masculino,
            modificationUser: _userId
        );

        Context.Cliente.Add(cliente);
        await Context.SaveChangesAsync();

        // Act
        var wallet = await _cuentaWalletFacade.CrearCuentaWalletAsync(cliente.Id, _userId);

        // Assert
        Assert.NotNull(wallet);
        Assert.Equal(cliente.Id, wallet.IdCliente);
        Assert.Equal("MXN", wallet.Moneda);
        Assert.NotNull(wallet.CuentaCLABE);
        Assert.Equal(18, wallet.CuentaCLABE.Length);
        Assert.Equal(0, wallet.SaldoActual);
    }

    [Fact]
    public async Task ObtenerPorClienteAsync_ShouldReturnWallet_WhenExists()
    {
        // Arrange
        var empresa = new Empresa("Tecom2", _userId);
        Context.Empresa.Add(empresa);

        var usuario = new Usuario("+52", "5511111111", null, null, EstatusRegistroEnum.TerminosCondicionesAceptado,
            _userId);
        Context.Usuario.Add(usuario);

        var cliente = new Cliente(usuario, empresa, _userId);
        cliente.AgregarDatosPersonales("Maria", "Gomez", "Ruiz", new DateOnly(1995, 5, 5), Genero.Femenino, _userId);

        Context.Cliente.Add(cliente);
        await Context.SaveChangesAsync();

        var wallet = await _cuentaWalletFacade.CrearCuentaWalletAsync(cliente.Id, _userId);

        // Act
        var retrievedWallet = await _cuentaWalletFacade.ObtenerPorClienteAsync(cliente.Id);

        // Assert
        Assert.NotNull(retrievedWallet);
        Assert.Equal(wallet.Id, retrievedWallet!.Id);
    }

    [Fact]
    public async Task ActualizarSaldoAsync_ShouldUpdateBalance_WhenCalled()
    {
        // Arrange
        var empresa = new Empresa("Tecom3", _userId);
        Context.Empresa.Add(empresa);

        var usuario = new Usuario("+52", "5522222222", null, null, EstatusRegistroEnum.TerminosCondicionesAceptado,
            _userId);
        Context.Usuario.Add(usuario);

        var cliente = new Cliente(usuario, empresa, _userId);
        cliente.AgregarDatosPersonales("Pedro", "Diaz", "Sanz", new DateOnly(1988, 8, 8), Genero.Masculino, _userId);

        Context.Cliente.Add(cliente);
        await Context.SaveChangesAsync();

        var wallet = await _cuentaWalletFacade.CrearCuentaWalletAsync(cliente.Id, _userId);

        // Act
        await _cuentaWalletFacade.ActualizarSaldoAsync(wallet.Id, 500.00m, _userId);
        var updatedWallet = await _cuentaWalletFacade.ObtenerPorClienteAsync(cliente.Id);

        // Assert
        Assert.NotNull(updatedWallet);
        Assert.Equal(500.00m, updatedWallet!.SaldoActual);
    }
}
