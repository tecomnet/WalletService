using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.Funcionalidad.Functionality.ConsentimientosUsuarioFacade;
using Wallet.UnitTest.Functionality.Configuration;

namespace Wallet.UnitTest.Functionality.UsuarioFacadeTest;

public class ConsentimientosUsuarioFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IConsentimientosUsuarioFacade>(setupConfig: setupConfig)
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
        var result = await Facade.GuardarConsentimientoAsync(idUsuario: usuario.Id, tipoDocumento: tipoDocumento, version: version, creationUser: creationUser);

        // Assert
        Assert.NotNull(@object: result);
        Assert.Equal(expected: usuario.Id, actual: result.IdUsuario);
        Assert.Equal(expected: tipoDocumento, actual: result.TipoDocumento);
        Assert.Equal(expected: version, actual: result.Version);
        Assert.True(condition: result.FechaAceptacion > DateTime.MinValue);
    }

    [Fact]
    public async Task ObtenerUltimosConsentimientosAsync_ShouldReturnLatestConsents()
    {
        // Arrange
        var usuario = await Context.Usuario.FirstAsync();
        var creationUser = Guid.NewGuid();

        // Save Terminos v1 and v2
        await Facade.GuardarConsentimientoAsync(idUsuario: usuario.Id, tipoDocumento: TipoDocumentoConsentimiento.Terminos, version: "v1.0", creationUser: creationUser);
        await Task.Delay(millisecondsDelay: 100);
        var terminosV2 =
            await Facade.GuardarConsentimientoAsync(idUsuario: usuario.Id, tipoDocumento: TipoDocumentoConsentimiento.Terminos, version: "v2.0",
                creationUser: creationUser);

        // Save Privacidad v1
        var privacidadV1 = await Facade.GuardarConsentimientoAsync(idUsuario: usuario.Id, tipoDocumento: TipoDocumentoConsentimiento.Privacidad,
            version: "v1.0", creationUser: creationUser);

        // Act
        var result = await Facade.ObtenerUltimosConsentimientosAsync(idUsuario: usuario.Id);

        // Assert
        Assert.NotNull(@object: result);
        Assert.Equal(expected: 2, actual: result.Count);

        var terminos = result.FirstOrDefault(predicate: c => c.TipoDocumento == TipoDocumentoConsentimiento.Terminos);
        Assert.NotNull(@object: terminos);
        Assert.Equal(expected: terminosV2.Id, actual: terminos!.Id);
        Assert.Equal(expected: "v2.0", actual: terminos.Version);

        var privacidad = result.FirstOrDefault(predicate: c => c.TipoDocumento == TipoDocumentoConsentimiento.Privacidad);
        Assert.NotNull(@object: privacidad);
        Assert.Equal(expected: privacidadV1.Id, actual: privacidad!.Id);
        Assert.Equal(expected: "v1.0", actual: privacidad.Version);
    }
}
