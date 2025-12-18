using Microsoft.EntityFrameworkCore;
using Moq;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.Funcionalidad.Functionality.ConsentimientosUsuarioFacade;
using Wallet.Funcionalidad.Functionality.RegistroFacade;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;
using Wallet.Funcionalidad.Functionality.CuentaWalletFacade;
using Wallet.UnitTest.Functionality.Configuration;

namespace Wallet.UnitTest.Functionality;

using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionCliente;

public class RegistroFacadeTest : BaseFacadeTest<IRegistroFacade>, IDisposable
{
    private readonly Mock<IUsuarioFacade> _usuarioFacadeMock = new();
    private readonly Mock<IClienteFacade> _clienteFacadeMock = new();
    private readonly Mock<IConsentimientosUsuarioFacade> _consentimientosFacadeMock = new();
    private readonly Mock<ICuentaWalletFacade> _cuentaWalletFacadeMock = new();
    private readonly IRegistroFacade _registroFacade;
    private readonly Guid _userId = Guid.NewGuid();

    public RegistroFacadeTest() : base(new SetupDataConfig())
    {
        _registroFacade = new RegistroFacade(Context, _usuarioFacadeMock.Object, _clienteFacadeMock.Object,
            _consentimientosFacadeMock.Object, _cuentaWalletFacadeMock.Object);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    [Fact]
    public async Task ConfirmarNumeroAsync_ShouldSucceed_WhenStateIsPreRegistro()
    {
        // Arrange
        var usuario = new Usuario("+52", "5512345678", null, null, EstatusRegistroEnum.PreRegistro, _userId);
        // Mock 2FA verification
        var verificacion = new Verificacion2FA("sid123", DateTime.Now.AddMinutes(10), Tipo2FA.Sms, _userId);
        usuario.AgregarVerificacion2Fa(verificacion, _userId);

        Context.Usuario.Add(usuario);
        await Context.SaveChangesAsync();

        _usuarioFacadeMock.Setup(x => x.ObtenerUsuarioPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        _usuarioFacadeMock.Setup(x =>
                x.ConfirmarCodigoVerificacion2FAAsync(It.IsAny<int>(), It.IsAny<Tipo2FA>(), It.IsAny<string>(),
                    It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        var result = await _registroFacade.ConfirmarNumeroAsync(usuario.Id, "123456");

        // Assert
        Assert.True(result);
        var updatedUser = await Context.Usuario.FindAsync(usuario.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(EstatusRegistroEnum.NumeroConfirmado, updatedUser.Estatus);
    }

    [Fact]
    public async Task ConfirmarNumeroAsync_ShouldFail_WhenStateIsInvalid()
    {
        // Arrange
        var usuario = new Usuario("+52", "5512345678", null, null, EstatusRegistroEnum.CorreoRegistrado, _userId);
        Context.Usuario.Add(usuario);
        await Context.SaveChangesAsync();

        // Act & Assert
        _usuarioFacadeMock.Setup(x => x.ObtenerUsuarioPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        await Assert.ThrowsAsync<EMGeneralAggregateException>(() =>
            _registroFacade.ConfirmarNumeroAsync(usuario.Id, "123456"));
    }

    [Fact]
    public async Task CompletarDatosClienteAsync_ShouldSucceed_WhenStateIsNumeroConfirmado()
    {
        // Arrange
        var usuario = new Usuario("+52", "5512345678", null, null, EstatusRegistroEnum.NumeroConfirmado, _userId);
        Context.Usuario.Add(usuario);
        await Context.SaveChangesAsync();

        _usuarioFacadeMock.Setup(x => x.ObtenerUsuarioPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        // Act
        var result = await _registroFacade.CompletarDatosClienteAsync(
            usuario.Id,
            "Juan",
            "Perez",
            "Lopez",
            "Campeche",
            new DateOnly(1990, 1, 1),
            Genero.Masculino);

        // Assert
        Assert.NotNull(result);
        var updatedUser = await Context.Usuario.FindAsync(usuario.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(EstatusRegistroEnum.DatosClienteCompletado, updatedUser.Estatus);

        _clienteFacadeMock.Verify(x => x.ActualizarClienteDatosPersonalesAsync(
            usuario.Id,
            "Juan",
            "Perez",
            "Lopez",
            "Campeche",
            new DateOnly(1990, 1, 1),
            Genero.Masculino,
            It.IsAny<string>(),
            It.IsAny<Guid>(),
            false,
            null), Times.Once);
    }

    [Fact]
    public async Task CompletarRegistroAsync_ShouldCreateWallet_WhenSuccessful()
    {
        // Arrange
        var usuario = new Usuario("+52", "5512345678", null, null, EstatusRegistroEnum.TerminosCondicionesAceptado,
            _userId);
        Context.Usuario.Add(usuario);
        await Context.SaveChangesAsync();

        var empresa = new Empresa("Tecomnet", _userId);
        Context.Empresa.Add(empresa);

        var cliente = new Cliente(usuario, empresa, _userId);
        Context.Cliente.Add(cliente);
        await Context.SaveChangesAsync();

        _usuarioFacadeMock.Setup(x => x.ObtenerUsuarioPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        // Act
        var result = await _registroFacade.CompletarRegistroAsync(usuario.Id, "Password123", "Password123");

        // Assert
        Assert.NotNull(result);
        var updatedUser = await Context.Usuario.FindAsync(usuario.Id);
        Assert.Equal(EstatusRegistroEnum.RegistroCompletado, updatedUser!.Estatus);

        _cuentaWalletFacadeMock.Verify(x => x.CrearCuentaWalletAsync(cliente.Guid, usuario.CreationUser, "MXN"),
            Times.Once);
    }
}
