using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.Funcionalidad.Functionality.BitacoraTransaccionFacade;
using Wallet.UnitTest.Functionality.Configuration;

namespace Wallet.UnitTest.Functionality;

public class BitacoraTransaccionFacadeTest : BaseFacadeTest<IBitacoraTransaccionFacade>, IDisposable
{
    private readonly IBitacoraTransaccionFacade _facade;
    private readonly Guid _userId = Guid.NewGuid();

    public BitacoraTransaccionFacadeTest() : base(setupConfig: new SetupDataConfig())
    {
        _facade = new BitacoraTransaccionFacade(context: Context);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    private async Task<CuentaWallet> CriarWalletAsync()
    {
        var empresa = new Empresa(nombre: "TecomTest", creationUser: _userId);
        Context.Empresa.Add(entity: empresa);

        var usuario = new Usuario(codigoPais: "+52", telefono: "5599999999", correoElectronico: null, contrasena: null,
            estatus: EstatusRegistroEnum.TerminosCondicionesAceptado,
            creationUser: _userId);
        Context.Usuario.Add(entity: usuario);

        var cliente = new Cliente(usuario: usuario, empresa: empresa, creationUser: _userId);
        cliente.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "Bitacora",
            fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Genero.Masculino,
            modificationUser: _userId);
        Context.Cliente.Add(entity: cliente);
        await Context.SaveChangesAsync();

        // Use facade or manual creation? Manual is safer for unit test isolation vs dependencies loop.
        // But implementation requires valid IDs.
        var wallet = new CuentaWallet(idCliente: cliente.Id, moneda: "MXN", cuentaCLABE: "123456789012345678",
            creationUser: _userId);
        Context.CuentaWallet.Add(entity: wallet);
        await Context.SaveChangesAsync();

        return wallet;
    }

    [Fact]
    public async Task GuardarTransaccionAsync_ShouldSaveTransaction_WhenValid()
    {
        // Arrange
        var wallet = await CriarWalletAsync();

        // Act
        var result = await _facade.GuardarTransaccionAsync(idBilletera: wallet.Id, monto: 100.50m, tipo: "DEPOSITO",
            direccion: "Abono", estatus: "Completada",
            creationUser: _userId);

        // Assert
        Assert.NotNull(@object: result);
        Assert.NotEqual(expected: 0, actual: result.Id);
        Assert.Equal(expected: wallet.Id, actual: result.CuentaWalletId);
        Assert.Equal(expected: 100.50m, actual: result.Monto);
    }

    [Fact]
    public async Task ObtenerTodasAsync_ShouldReturnList_WhenTransactionsExist()
    {
        // Arrange
        var wallet = await CriarWalletAsync();
        await _facade.GuardarTransaccionAsync(idBilletera: wallet.Id, monto: 50m, tipo: "TEST", direccion: "Abono",
            estatus: "Completada", creationUser: _userId);

        // Act
        var result = await _facade.ObtenerTodasAsync();

        // Assert
        Assert.NotEmpty(collection: result);
        Assert.Contains(collection: result, filter: t => t.Monto == 50m);
    }

    [Fact]
    public async Task ObtenerPorClienteAsync_ShouldReturnTransactions_ForSpecificClient()
    {
        // Arrange
        var wallet = await CriarWalletAsync();
        await _facade.GuardarTransaccionAsync(idBilletera: wallet.Id, monto: 75m, tipo: "TEST", direccion: "Abono",
            estatus: "Completada", creationUser: _userId);

        // Act
        var result = await _facade.ObtenerPorClienteAsync(idCliente: wallet.IdCliente);

        // Assert
        Assert.Single(collection: result);
        Assert.Equal(expected: 75m, actual: result[index: 0].Monto);
    }
}
