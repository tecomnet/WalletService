using System;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;

namespace Wallet.UnitTest.Functionality.ClienteFacadeTest;

public class EstadoFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IEstadoFacade>(setupConfig) 
{
     // -------------------------------------------------------------------
    // Tests para ObtenerEstadoPorIdAsync
    // -------------------------------------------------------------------

    [Fact]
    public async Task ObtenerEstadoPorIdAsync_ExisteId_DebeRetornarEstado()
    {
        // Arrange
        int idValido = 1;
        string nombreEsperado = "Aguascalientes";
        // Act
        var result = await Facade.ObtenerEstadoPorIdAsync(idValido);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idValido, result.Id);
        Assert.Equal(nombreEsperado, result.Nombre);
    }

    [Fact]
    public async Task ObtenerEstadoPorIdAsync_NoExisteId_DebeLanzarExcepcionNoEncontrado()
    {
        // Arrange
        int nonExistentId = 999;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EMGeneralAggregateException>(
            () => Facade.ObtenerEstadoPorIdAsync(nonExistentId));
        
        // Se puede añadir una aserción sobre el mensaje de error si se pudiera deserializar la respuesta del helper
        // Por ahora, comprobamos que es la excepción correcta.
        Assert.Contains(ServiceErrorsBuilder.EstadoNoEncontrado, exception.Message);
        Assert.Contains(nonExistentId.ToString(), exception.Message);
    }

    // -------------------------------------------------------------------
    // Tests para ObtenerEstadoPorNombreAsync
    // -------------------------------------------------------------------

    [Fact]
    public async Task ObtenerEstadoPorNombreAsync_ExisteNombre_DebeRetornarEstado()
    {
        // Arrange
        string nombre = "Campeche";

        // Act
        var result = await Facade.ObtenerEstadoPorNombreAsync(nombre);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nombre, result.Nombre);
    }

    [Fact]
    public async Task ObtenerEstadoPorNombreAsync_NoExisteNombre_DebeLanzarExcepcionNoEncontrado()
    {
        // Arrange
        string nonExistentName = "Inexistente";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EMGeneralAggregateException>(
            () => Facade.ObtenerEstadoPorNombreAsync(nonExistentName));

        Assert.Contains(ServiceErrorsBuilder.EstadoNoEncontrado, exception.Message);
        Assert.Contains(nonExistentName, exception.Message);
    }
}
