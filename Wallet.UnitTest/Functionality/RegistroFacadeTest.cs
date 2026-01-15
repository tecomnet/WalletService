using Moq;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
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

    public RegistroFacadeTest() : base(setupConfig: new SetupDataConfig())
    {
        _registroFacade = new RegistroFacade(context: Context, usuarioFacade: _usuarioFacadeMock.Object, clienteFacade: _clienteFacadeMock.Object,
            consentimientosUsuarioFacade: _consentimientosFacadeMock.Object, cuentaWalletFacade: _cuentaWalletFacadeMock.Object);
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    [Fact]
    public async Task ConfirmarNumeroAsync_ShouldSucceed_WhenStateIsPreRegistro()
    {
        // Arrange
        var usuario = new Usuario(codigoPais: "+52", telefono: "5512345678", correoElectronico: null, contrasena: null, estatus: EstatusRegistroEnum.PreRegistro, creationUser: _userId);
        // Mock 2FA verification
        var verificacion = new Verificacion2FA(twilioSid: "sid123", fechaVencimiento: DateTime.Now.AddMinutes(value: 10), tipo: Tipo2FA.Sms, creationUser: _userId);
        usuario.AgregarVerificacion2Fa(verificacion: verificacion, modificationUser: _userId);

        Context.Usuario.Add(entity: usuario);
        await Context.SaveChangesAsync();

        _usuarioFacadeMock.Setup(expression: x => x.ObtenerUsuarioPorIdAsync(usuario.Id))
            .ReturnsAsync(value: usuario);

        _usuarioFacadeMock.Setup(expression: x =>
                x.ConfirmarCodigoVerificacion2FAAsync(It.IsAny<int>(), It.IsAny<Tipo2FA>(), It.IsAny<string>(),
                    It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(value: true);

        // Act
        var result = await _registroFacade.ConfirmarNumeroAsync(idUsuario: usuario.Id, codigo: "123456");

        // Assert
        Assert.True(condition: result);
        var updatedUser = await Context.Usuario.FindAsync(keyValues: usuario.Id);
        Assert.NotNull(@object: updatedUser);
        Assert.Equal(expected: EstatusRegistroEnum.NumeroConfirmado, actual: updatedUser.Estatus);
    }

    [Fact]
    public async Task ConfirmarNumeroAsync_ShouldFail_WhenStateIsInvalid()
    {
        // Arrange
        var usuario = new Usuario(codigoPais: "+52", telefono: "5512345678", correoElectronico: null, contrasena: null, estatus: EstatusRegistroEnum.CorreoRegistrado, creationUser: _userId);
        Context.Usuario.Add(entity: usuario);
        await Context.SaveChangesAsync();

        // Act & Assert
        _usuarioFacadeMock.Setup(expression: x => x.ObtenerUsuarioPorIdAsync(usuario.Id))
            .ReturnsAsync(value: usuario);

        await Assert.ThrowsAsync<EMGeneralAggregateException>(testCode: () =>
            _registroFacade.ConfirmarNumeroAsync(idUsuario: usuario.Id, codigo: "123456"));
    }

    [Fact]
    public async Task CompletarDatosClienteAsync_ShouldSucceed_WhenStateIsNumeroConfirmado()
    {
        // Arrange
        var usuario = new Usuario(codigoPais: "+52", telefono: "5512345678", correoElectronico: null, contrasena: null, estatus: EstatusRegistroEnum.NumeroConfirmado, creationUser: _userId);
        Context.Usuario.Add(entity: usuario);
        await Context.SaveChangesAsync();

        _usuarioFacadeMock.Setup(expression: x => x.ObtenerUsuarioPorIdAsync(usuario.Id))
            .ReturnsAsync(value: usuario);

        // Act
        var result = await _registroFacade.CompletarDatosClienteAsync(
            idUsuario: usuario.Id,
            nombre: "Juan",
            apellidoPaterno: "Perez",
            apellidoMaterno: "Lopez",
            nombreEstado: "Campeche",
            fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1),
            genero: Genero.Masculino);

        // Assert
        Assert.NotNull(@object: result);
        var updatedUser = await Context.Usuario.FindAsync(keyValues: usuario.Id);
        Assert.NotNull(@object: updatedUser);
        Assert.Equal(expected: EstatusRegistroEnum.DatosClienteCompletado, actual: updatedUser.Estatus);

        _clienteFacadeMock.Verify(expression: x => x.ActualizarClienteDatosPersonalesAsync(
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
            null), times: Times.Once);
    }

    [Fact]
    public async Task CompletarRegistroAsync_ShouldCreateWallet_WhenSuccessful()
    {
        // Arrange
        var usuario = new Usuario(codigoPais: "+52", telefono: "5512345678", correoElectronico: null, contrasena: null, estatus: EstatusRegistroEnum.TerminosCondicionesAceptado,
            creationUser: _userId);
        Context.Usuario.Add(entity: usuario);
        await Context.SaveChangesAsync();

        var empresa = new Empresa(nombre: "Tecomnet", creationUser: _userId);
        Context.Empresa.Add(entity: empresa);

        var cliente = new Cliente(usuario: usuario, empresa: empresa, creationUser: _userId);
        Context.Cliente.Add(entity: cliente);
        await Context.SaveChangesAsync();

        _usuarioFacadeMock.Setup(expression: x => x.ObtenerUsuarioPorIdAsync(usuario.Id))
            .ReturnsAsync(value: usuario);

        // Act
        var result = await _registroFacade.CompletarRegistroAsync(idUsuario: usuario.Id, contrasena: "Password123", confirmacionContrasena: "Password123");

        // Assert
        Assert.NotNull(@object: result);
        var updatedUser = await Context.Usuario.FindAsync(keyValues: usuario.Id);
        Assert.Equal(expected: EstatusRegistroEnum.RegistroCompletado, actual: updatedUser!.Estatus);

        _cuentaWalletFacadeMock.Verify(expression: x => x.CrearCuentaWalletAsync(cliente.Id, usuario.CreationUser, "MXN"),
            times: Times.Once);
    }
}
