using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.Functionality.Configuration;

public class SetupDataConfig : DatabaseTestFixture
{
	// Defines test case id
	public readonly string? TestCaseId = "FunctionalTest";

	// Defines user Guid
	public readonly Guid UserId = new(g: "00000000-0000-0000-0000-000000000000");

	private readonly CommonSettings _commonSettings = new();

	/// <summary>
	/// Initialize data for test
	/// </summary>
	public SetupDataConfig()
	{
		SetupDataAsync(async context =>
		{
			await context.AddRangeAsync(_commonSettings.Empresas);
			await context.AddRangeAsync(_commonSettings.Estados);
			await context.AddRangeAsync(_commonSettings.Clientes);
			await context.AddRangeAsync(_commonSettings.ProveedoresServicios);
			// Guardamos cambios para generar IDs de Clientes y Proveedores antes de guardar ServiciosFavoritos?
			// EF Core debería manejar las relaciones si los objetos estuvieran vinculados por navegación,
			// pero ServicioFavorito usa IDs (FKs) y no estamos estableciendo las propiedades de navegación con los objetos de las listas anteriores.
			// Así que necesitamos que los IDs existan.
			await context.SaveChangesAsync();

			// Ahora agregamos ServiciosFavoritos. 
			// Nota: CommonSettings.CrearServiciosFavoritos usa IDs hardcoded (1). 
			// Si los IDs generados no son 1, fallará la FK.
			// En pruebas con base de datos en memoria o limpia, debería ser determinista.
			await context.AddRangeAsync(_commonSettings.ServiciosFavoritos);
			await context.SaveChangesAsync();
		}).GetAwaiter().GetResult();
	}
}