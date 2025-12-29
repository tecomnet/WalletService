using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.ConsentimientosUsuarioFacade;
using Wallet.UnitTest.Functionality.Configuration;

namespace Wallet.UnitTest.Functionality.UsuarioFacadeTest;

public class ConsentimientosUsuarioFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IConsentimientosUsuarioFacade>(setupConfig)
{
    [Fact]
    public async Task GuardarConsentimientoAsync_ShouldCreateConsent()
    {
        // Arrange
        var usuario = await Context.Usuario.FirstAsync();
        var tipoDocumento = TipoDocumentoConsentimiento.Terminos;
        var version = "v1.0";
        var creationUser = Guid.NewGuid();

        // Act
        var result = await Facade.GuardarConsentimientoAsync(usuario.Id, tipoDocumento, version, creationUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(usuario.Id, result.IdUsuario);
        Assert.Equal(tipoDocumento, result.TipoDocumento);
        Assert.Equal(version, result.Version);
        Assert.True(result.FechaAceptacion > DateTime.MinValue);
    }

    [Fact]
    public async Task ObtenerUltimosConsentimientosAsync_ShouldReturnLatestConsents()
    {
        // Arrange
        var usuario = await Context.Usuario.FirstAsync();
        var creationUser = Guid.NewGuid();

        // Save Terminos v1 and v2
        await Facade.GuardarConsentimientoAsync(usuario.Id, TipoDocumentoConsentimiento.Terminos, "v1.0", creationUser);
        await Task.Delay(100);
        var terminosV2 =
            await Facade.GuardarConsentimientoAsync(usuario.Id, TipoDocumentoConsentimiento.Terminos, "v2.0",
                creationUser);

        // Save Privacidad v1
        var privacidadV1 = await Facade.GuardarConsentimientoAsync(usuario.Id, TipoDocumentoConsentimiento.Privacidad,
            "v1.0", creationUser);

        // Act
        var result = await Facade.ObtenerUltimosConsentimientosAsync(usuario.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var terminos = result.FirstOrDefault(c => c.TipoDocumento == TipoDocumentoConsentimiento.Terminos);
        Assert.NotNull(terminos);
        Assert.Equal(terminosV2.Id, terminos!.Id);
        Assert.Equal("v2.0", terminos.Version);

        var privacidad = result.FirstOrDefault(c => c.TipoDocumento == TipoDocumentoConsentimiento.Privacidad);
        Assert.NotNull(privacidad);
        Assert.Equal(privacidadV1.Id, privacidad!.Id);
        Assert.Equal("v1.0", privacidad.Version);
    }
}
