using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

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
        Assert.Contains(ServiceErrorsBuilder.EstadoNoEncontrado, exception.InnerException!.Code);
        Assert.Contains(nonExistentId.ToString(), exception.InnerException.Description);
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

        Assert.Contains(ServiceErrorsBuilder.EstadoNoEncontrado, exception.InnerException!.Code);
        Assert.Contains(nonExistentName, exception.InnerException.Description);
    }

    // Test para metodo obtener todos
    [Theory(DisplayName = "ObtenerTodosAsync: Debe retornar la cantidad correcta de estados basado en el filtro 'activo'")]
    [InlineData(null)] // Caso 1: null (todos)
    [InlineData(true)]  // Caso 2: true (activos)
    [InlineData(false)] // Caso 3: false (inactivos)
    public async Task ObtenerTodosAsync_ConFiltro_DebeRetornarConteoCorrecto(bool? activo)
    {
        // Act
        var resultado = await Facade.ObtenerTodosAsync(activo: activo);

        // Assert
        Assert.NotNull(resultado);

        // Si se aplicó un filtro (activo no es null), se valida que todos los resultados coincidan.
        if (activo.HasValue)
        {
            Assert.True(resultado.All(x => x.IsActive == activo.Value),
                $"El filtro esperaba IsActive = {activo.Value}, pero se encontraron resultados que no coinciden.");
        }
    }

    // Test para metodo activar estado
    [Theory]
    // Successfully case
    [InlineData("1. Debe activar el estado", 1, true, new string[] { })]
    // Cases with errors
    [InlineData("2. Estado no encontrado", 999, false, new string[] { ServiceErrorsBuilder.EstadoNoEncontrado })]
    public async Task ActivarEstadoAsync(string caseName, int idEstado, 
     bool success, string[] expectedErrors)
    {
        try
        {
            // Act
            var resultado = await Facade.ActivaEstadoAsync(idEstado: idEstado, modificationUser: SetupConfig.UserId);
            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(idEstado, resultado.Id);
            Assert.True(resultado.IsActive);
            // Validdar estado en base de datos
            var estadoContext = await Context.Estado.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idEstado);
            Assert.NotNull(estadoContext);
            Assert.True(estadoContext.IsActive);
        }
        // Catch the managed errors and check them with the expected ones in the case of failures
        catch (EMGeneralAggregateException exception)
        {
            // Treat the raised error
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Catch any non managed errors and display them to understand the root cause
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            // Should not reach for unmanaged errors
            Assert.Fail($"Uncaught exception. {exception.Message}");
        }
    }

    // Test para metodo desactivar estado
    [Theory]
    // Successfully case
    [InlineData("1. Debe desactivar el estado", 1, true, new string[] { })]
    // Cases with errors
    [InlineData("2. Estado no encontrado", 999, false, new string[] { ServiceErrorsBuilder.EstadoNoEncontrado })]
    public async Task DesactivarEstadoAsync(string caseName, int idEstado,
     bool success, string[] expectedErrors)
    {
        try
        {
            // Act
            var resultado = await Facade.EliminaEstadoAsync(idEstado: idEstado, modificationUser: SetupConfig.UserId);
            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(idEstado, resultado.Id);
            Assert.False(resultado.IsActive);
            // Validdar estado en base de datos
            var estadoContext = await Context.Estado.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idEstado);
            Assert.NotNull(estadoContext);
            Assert.False(estadoContext.IsActive);
        }
        // Catch the managed errors and check them with the expected ones in the case of failures
        catch (EMGeneralAggregateException exception)
        {
            // Treat the raised error
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Catch any non managed errors and display them to understand the root cause
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            // Should not reach for unmanaged errors
            Assert.Fail($"Uncaught exception. {exception.Message}");
        }
    }

    // Test para metodo guardar estado
    [Theory]
    // Successfully case
    [InlineData("1. Debe guardar el estado correctamente", "NuevoEstado", true, new string[] { })]
    // Cases with errors
    [InlineData("2. Estado ya existe", "Aguascalientes", false, new string[] { ServiceErrorsBuilder.EstadoDuplicado })]
    public async Task GuardarEstadoAsync(string caseName, string nombre, bool success, string[] expectedErrors)
    {
        try
        {
            // Act
            var resultado = await Facade.GuardarEstadoAsync(nombre: nombre, creationUser: SetupConfig.UserId);
            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(nombre, resultado.Nombre);
            // Validdar estado en base de datos
            var estadoContext = await Context.Estado.AsNoTracking().FirstOrDefaultAsync(x => x.Nombre == nombre);
            Assert.NotNull(estadoContext);
            Assert.True(estadoContext.IsActive);
        }
        // Catch the managed errors and check them with the expected ones in the case of failures
        catch (EMGeneralAggregateException exception)
        {
            // Treat the raised error
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Catch any non managed errors and display them to understand the root cause
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            // Should not reach for unmanaged errors
            Assert.Fail($"Uncaught exception. {exception.Message}");
        }
    }

    // Test para metodo actualizar estado
    [Theory]
    // Successfully case
    [InlineData("1. Debe actualizar el estado correctamente", 1, "NuevoNombre", true, new string[] { })]
    // Cases with errors
    [InlineData("2. Estado no encontrado", 999, "NuevoNombre", false, new string[] { ServiceErrorsBuilder.EstadoNoEncontrado })]
    [InlineData("3. El nuevo nombre ya existe", 1, "Campeche", false, new string[] { ServiceErrorsBuilder.EstadoDuplicado })]
    [InlineData("4. Estado inactivo", 5, "EstadoInactivo", false, new string[] { ServiceErrorsBuilder.EstadoInactivo })]
    public async Task ActualizarEstadoAsync(string caseName, int idEstado, string nombre, bool success, string[] expectedErrors)
    {
        try
        {
            // Act
            var resultado = await Facade.ActualizaEstadoAsync(idEstado: idEstado, nombre: nombre, modificationUser: SetupConfig.UserId);
            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(idEstado, resultado.Id);
            Assert.Equal(nombre, resultado.Nombre);
            // Validdar estado en base de datos
            var estadoContext = await Context.Estado.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idEstado);
            Assert.NotNull(estadoContext);
            Assert.Equal(nombre, estadoContext.Nombre);
        }
        // Catch the managed errors and check them with the expected ones in the case of failures
        catch (EMGeneralAggregateException exception)
        {
            // Treat the raised error
            CatchErrors(caseName: caseName, success: success, expectedErrors: expectedErrors, exception: exception);
        }
        // Catch any non managed errors and display them to understand the root cause
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not TrueException && exception is not FalseException)
        {
            // Should not reach for unmanaged errors
            Assert.Fail($"Uncaught exception. {exception.Message}");
        }
    }
}
