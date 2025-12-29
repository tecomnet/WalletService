using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;
using Xunit.Sdk;

namespace Wallet.UnitTest.Functionality.ClienteFacadeTest;

public class EstadoFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IEstadoFacade>(setupConfig: setupConfig)
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
        var result = await Facade.ObtenerEstadoPorIdAsync(idEstado: idValido);

        // Assert
        Assert.NotNull(@object: result);
        Assert.Equal(expected: idValido, actual: result.Id);
        Assert.Equal(expected: nombreEsperado, actual: result.Nombre);
    }

    [Fact]
    public async Task ObtenerEstadoPorIdAsync_NoExisteId_DebeLanzarExcepcionNoEncontrado()
    {
        // Arrange
        int nonExistentId = 999;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EMGeneralAggregateException>(
            testCode: () => Facade.ObtenerEstadoPorIdAsync(idEstado: nonExistentId));

        // Se puede añadir una aserción sobre el mensaje de error si se pudiera deserializar la respuesta del helper
        // Por ahora, comprobamos que es la excepción correcta.
        Assert.Contains(expectedSubstring: ServiceErrorsBuilder.EstadoNoEncontrado,
            actualString: exception.InnerException!.Code);
        Assert.Contains(expectedSubstring: nonExistentId.ToString(),
            actualString: exception.InnerException.Description);
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
        var result = await Facade.ObtenerEstadoPorNombreAsync(nombre: nombre);

        // Assert
        Assert.NotNull(@object: result);
        Assert.Equal(expected: nombre, actual: result.Nombre);
    }

    [Fact]
    public async Task ObtenerEstadoPorNombreAsync_NoExisteNombre_DebeLanzarExcepcionNoEncontrado()
    {
        // Arrange
        string nonExistentName = "Inexistente";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EMGeneralAggregateException>(
            testCode: () => Facade.ObtenerEstadoPorNombreAsync(nombre: nonExistentName));

        Assert.Contains(expectedSubstring: ServiceErrorsBuilder.EstadoNoEncontrado,
            actualString: exception.InnerException!.Code);
        Assert.Contains(expectedSubstring: nonExistentName, actualString: exception.InnerException.Description);
    }

    // Test para metodo obtener todos
    [Theory(DisplayName =
        "ObtenerTodosAsync: Debe retornar la cantidad correcta de estados basado en el filtro 'activo'")]
    [InlineData(data: null)] // Caso 1: null (todos)
    [InlineData(data: true)] // Caso 2: true (activos)
    [InlineData(data: false)] // Caso 3: false (inactivos)
    public async Task ObtenerTodosAsync_ConFiltro_DebeRetornarConteoCorrecto(bool? activo)
    {
        // Act
        var resultado = await Facade.ObtenerTodosAsync(activo: activo);

        // Assert
        Assert.NotNull(@object: resultado);

        // Si se aplicó un filtro (activo no es null), se valida que todos los resultados coincidan.
        if (activo.HasValue)
        {
            Assert.True(condition: resultado.All(predicate: x => x.IsActive == activo.Value),
                userMessage:
                $"El filtro esperaba IsActive = {activo.Value}, pero se encontraron resultados que no coinciden.");
        }
    }

    // Test para metodo activar estado
    [Theory]
    // Successfully case
    [InlineData(data: ["1. Debe activar el estado", 1, true, new string[] { }])]
    // Cases with errors
    [InlineData(data:
        ["2. Estado no encontrado", 999, false, new string[] { ServiceErrorsBuilder.EstadoNoEncontrado }])]
    public async Task ActivarEstadoAsync(string caseName, int idEstado,
        bool success, string[] expectedErrors)
    {
        try
        {
            // Act
            var resultado = await Facade.ActivaEstadoAsync(idEstado: idEstado, modificationUser: SetupConfig.UserId);
            // Assert
            Assert.NotNull(@object: resultado);
            Assert.Equal(expected: idEstado, actual: resultado.Id);
            Assert.True(condition: resultado.IsActive);
            // Validdar estado en base de datos
            var estadoContext =
                await Context.Estado.AsNoTracking().FirstOrDefaultAsync(predicate: x => x.Id == idEstado);
            Assert.NotNull(@object: estadoContext);
            Assert.True(condition: estadoContext.IsActive);
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
            Assert.Fail(message: $"Uncaught exception. {exception.Message}");
        }
    }

    // Test para metodo desactivar estado
    [Theory]
    // Successfully case
    [InlineData(data: ["1. Debe desactivar el estado", 1, true, new string[] { }])]
    // Cases with errors
    [InlineData(data:
        ["2. Estado no encontrado", 999, false, new string[] { ServiceErrorsBuilder.EstadoNoEncontrado }])]
    public async Task DesactivarEstadoAsync(string caseName, int idEstado,
        bool success, string[] expectedErrors)
    {
        try
        {
            // Act
            var resultado = await Facade.EliminaEstadoAsync(idEstado: idEstado, modificationUser: SetupConfig.UserId);
            // Assert
            Assert.NotNull(@object: resultado);
            Assert.Equal(expected: idEstado, actual: resultado.Id);
            Assert.False(condition: resultado.IsActive);
            // Validdar estado en base de datos
            var estadoContext =
                await Context.Estado.AsNoTracking().FirstOrDefaultAsync(predicate: x => x.Id == idEstado);
            Assert.NotNull(@object: estadoContext);
            Assert.False(condition: estadoContext.IsActive);
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
            Assert.Fail(message: $"Uncaught exception. {exception.Message}");
        }
    }

    // Test para metodo guardar estado
    [Theory]
    // Successfully case
    [InlineData(data: ["1. Debe guardar el estado correctamente", "NuevoEstado", true, new string[] { }])]
    // Cases with errors
    [InlineData(data:
        ["2. Estado ya existe", "Aguascalientes", false, new string[] { ServiceErrorsBuilder.EstadoDuplicado }])]
    public async Task GuardarEstadoAsync(string caseName, string nombre, bool success, string[] expectedErrors)
    {
        try
        {
            // Act
            var resultado = await Facade.GuardarEstadoAsync(nombre: nombre, creationUser: SetupConfig.UserId);
            // Assert
            Assert.NotNull(@object: resultado);
            Assert.Equal(expected: nombre, actual: resultado.Nombre);
            // Validdar estado en base de datos
            var estadoContext =
                await Context.Estado.AsNoTracking().FirstOrDefaultAsync(predicate: x => x.Nombre == nombre);
            Assert.NotNull(@object: estadoContext);
            Assert.True(condition: estadoContext.IsActive);
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
            Assert.Fail(message: $"Uncaught exception. {exception.Message}");
        }
    }

    // Test para metodo actualizar estado
    [Theory]
    // Successfully case
    [InlineData(data: ["1. Debe actualizar el estado correctamente", 1, "NuevoNombre", true, new string[] { }])]
    // Cases with errors
    [InlineData(data:
    [
        "2. Estado no encontrado", 999, "NuevoNombre", false, new string[] { ServiceErrorsBuilder.EstadoNoEncontrado }
    ])]
    [InlineData(data:
        ["3. El nuevo nombre ya existe", 1, "Campeche", false, new string[] { ServiceErrorsBuilder.EstadoDuplicado }])]
    [InlineData(data:
        ["4. Estado inactivo", 5, "EstadoInactivo", false, new string[] { ServiceErrorsBuilder.EstadoInactivo }])]
    public async Task ActualizarEstadoAsync(string caseName, int idEstado, string nombre, bool success,
        string[] expectedErrors)
    {
        try
        {
            // Act
            var resultado = await Facade.ActualizaEstadoAsync(idEstado: idEstado, nombre: nombre,
                concurrencyToken: null, modificationUser: SetupConfig.UserId);
            // Assert
            Assert.NotNull(@object: resultado);
            Assert.Equal(expected: idEstado, actual: resultado.Id);
            Assert.Equal(expected: nombre, actual: resultado.Nombre);
            // Validdar estado en base de datos
            var estadoContext =
                await Context.Estado.AsNoTracking().FirstOrDefaultAsync(predicate: x => x.Id == idEstado);
            Assert.NotNull(@object: estadoContext);
            Assert.Equal(expected: nombre, actual: estadoContext.Nombre);
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
            Assert.Fail(message: $"Uncaught exception. {exception.Message}");
        }
    }
}
