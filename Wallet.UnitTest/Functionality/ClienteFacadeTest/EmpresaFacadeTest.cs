using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Errors;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.UnitTest.Functionality.Configuration;

namespace Wallet.UnitTest.Functionality.ClienteFacadeTest;

public class EmpresaFacadeTest(SetupDataConfig setupConfig)
    : BaseFacadeTest<IEmpresaFacade>(setupConfig)
{
    // =============================
    // --- OBTENER POR ID ---
    // =============================

    [Theory(DisplayName = "ObtenerPorIdAsync: Retorna la empresa existente por ID")]
    [InlineData(1, "Tecomnet")]
    [InlineData(2, "EmpresaInactiva")]
    public async Task ObtenerPorIdAsync_Existente_RetornaEmpresa(int id, string nombreEsperado)
    {
        // Act
        var result = await Facade.ObtenerPorIdAsync(id);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(nombreEsperado, result.Nombre);
    }

    [Theory(DisplayName = "ObtenerPorIdAsync: Lanza excepción si la empresa no existe")]
    [InlineData(100)]
    [InlineData(0)]
    public async Task ObtenerPorIdAsync_NoExistente_LanzaEmpresaNoEncontrada(int id)
    {
        // Act & Assert
        await Assert.ThrowsAsync<EMGeneralAggregateException>(() => Facade.ObtenerPorIdAsync(id));
    }

    // =============================
    // --- OBTENER POR NOMBRE ---
    // =============================

    [Theory(DisplayName = "ObtenerPorNombreAsync: Retorna la empresa existente por Nombre")]
    [InlineData("Tecomnet")]
    [InlineData("EmpresaInactiva")]
    public async Task ObtenerPorNombreAsync_Existente_RetornaEmpresa(string nombre)
    {
        // Act
        var result = await Facade.ObtenerPorNombreAsync(nombre);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nombre, result.Nombre);
    }

    [Theory(DisplayName = "ObtenerPorNombreAsync: Lanza excepción si la empresa no existe")]
    [InlineData("EmpresaInexistente")]
    [InlineData("OTRA_EMPRESA")]
    public async Task ObtenerPorNombreAsync_NoExistente_LanzaEmpresaNoEncontrada(string nombre)
    {
        // Act & Assert
        await Assert.ThrowsAsync<EMGeneralAggregateException>(() => Facade.ObtenerPorNombreAsync(nombre));
    }

    // =============================
    // --- GUARDAR EMPRESA ---
    // =============================

    [Fact(DisplayName = "GuardarEmpresaAsync: Guarda una empresa nueva exitosamente")]
    public async Task GuardarEmpresaAsync_Nuevo_GuardaYRetornaEmpresa()
    {
        // Arrange
        const string nombreNuevo = "EmpresaTestGuardar";

        // Act
        var result = await Facade.GuardarEmpresaAsync(nombre: nombreNuevo, creationUser: SetupConfig.UserId,
            testCase: SetupConfig.TestCaseId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0); // EF Core asignó un ID
        Assert.Equal(nombreNuevo, result.Nombre);

        // Verifica que se guardó en la DB
        var savedEntity = await Context.Empresa.AsNoTracking().FirstAsync(x => x.Id == result.Id);
        Assert.NotNull(savedEntity);
    }

    [Theory(DisplayName = "GuardarEmpresaAsync: Lanza excepción por duplicidad de nombre")]
    [InlineData("Tecomnet")]
    [InlineData("EmpresaInactiva")]
    public async Task GuardarEmpresaAsync_Duplicado_LanzaEmpresaDuplicada(string nombreDuplicado)
    {
        // Act & Assert
        await Assert.ThrowsAsync<EMGeneralAggregateException>(() =>
            Facade.GuardarEmpresaAsync(nombre: nombreDuplicado, creationUser: SetupConfig.UserId,
                testCase: SetupConfig.TestCaseId));
    }

    // =============================
    // --- ACTUALIZAR EMPRESA ---
    // =============================

    [Fact(DisplayName = "ActualizaEmpresaAsync: Actualiza nombre exitosamente")]
    public async Task ActualizaEmpresaAsync_Valido_ActualizaNombre()
    {
        // Arrange
        const int idAActualizar = 1; // Tecomnet
        const string nuevoNombre = "TecomnetActualizada";

        // Act
        var result = await Facade.ActualizaEmpresaAsync(idEmpresa: idAActualizar, nombre: nuevoNombre,
            modificationUser: SetupConfig.UserId);

        // Assert
        Assert.Equal(nuevoNombre, result.Nombre);

        // Verifica el cambio en la DB
        var savedEntity = await Context.Empresa.AsNoTracking().FirstAsync(x => x.Id == idAActualizar);
        Assert.Equal(nuevoNombre, savedEntity.Nombre);
    }

    [Fact(DisplayName = "ActualizaEmpresaAsync: Lanza excepción si el nuevo nombre es duplicado")]
    public async Task ActualizaEmpresaAsync_NombreDuplicado_LanzaEmpresaDuplicada()
    {
        // Arrange
        const int idAActualizar = 1; // Tecomnet
        const string nombreDuplicado = "EmpresaInactiva"; // Ya existe

        // Act & Assert
        await Assert.ThrowsAsync<EMGeneralAggregateException>(() =>
            Facade.ActualizaEmpresaAsync(idEmpresa: idAActualizar, nombre: nombreDuplicado,
                modificationUser: SetupConfig.UserId));
    }

    [Fact(DisplayName = "ActualizaEmpresaAsync: Lanza excepción si la empresa está inactiva")]
    public async Task ActualizaEmpresaAsync_Inactiva_LanzaEmpresaInactiva()
    {
        // Arrange
        const int idInactiva = 2;

        // Inactivar la entidad en la DB antes de la prueba (Simulación)
        var empresaToDeactivate = await Context.Empresa.FindAsync(idInactiva);
        empresaToDeactivate!.Deactivate(SetupConfig.UserId);
        await Context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<EMGeneralAggregateException>(() =>
            Facade.ActualizaEmpresaAsync(idEmpresa: idInactiva, nombre: "NombreNoImporta",
                modificationUser: SetupConfig.UserId));
    }


    // =============================
    // --- ELIMINAR / ACTIVAR ---
    // =============================

    [Fact(DisplayName = "EliminaEmpresaAsync: Desactiva la empresa exitosamente")]
    public async Task EliminaEmpresaAsync_Valido_DesactivaEmpresa()
    {
        // Arrange
        const int idAEliminar = 1;

        // Act
        var result = await Facade.EliminaEmpresaAsync(idAEliminar, SetupConfig.UserId);

        // Assert
        Assert.False(result.IsActive);

        // Verifica el cambio en la DB
        var savedEntity = await Context.Empresa.AsNoTracking().FirstAsync(x => x.Id == idAEliminar);
        Assert.False(savedEntity.IsActive);
    }

    [Fact(DisplayName = "ActivaEmpresaAsync: Activa la empresa inactiva exitosamente")]
    public async Task ActivaEmpresaAsync_Inactiva_ActivaEmpresa()
    {
        // Arrange
        const int idAActivar = 2;

        // 1. Desactivar la entidad primero (Simulación)
        var empresaToDeactivate = await Context.Empresa.FindAsync(idAActivar);
        empresaToDeactivate!.Deactivate(SetupConfig.UserId);
        await Context.SaveChangesAsync();

        // Act
        var result = await Facade.ActivaEmpresaAsync(idAActivar, SetupConfig.UserId);

        // Assert
        Assert.True(result.IsActive);

        // Verifica el cambio en la DB
        var savedEntity = await Context.Empresa.AsNoTracking().FirstAsync(x => x.Id == idAActivar);
        Assert.True(savedEntity.IsActive);
    }
}
