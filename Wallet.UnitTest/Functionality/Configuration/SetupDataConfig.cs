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
		SetupDataAsync(setupDataAction: async context =>
		{
			await context.AddRangeAsync(entities: _commonSettings.Empresas);
			await context.AddRangeAsync(entities: _commonSettings.Estados);
			await context.AddRangeAsync(entities: _commonSettings.Usuarios);
			await context.AddRangeAsync(entities: _commonSettings.Clientes);
			await context.AddRangeAsync(entities: _commonSettings.ProveedoresServicios);
			await context.SaveChangesAsync();

			// After SaveChangesAsync, IDs are assigned to Clientes and ProveedoresServicios
			var primerCliente = _commonSettings.Clientes.First();
			var primerProveedor = _commonSettings.ProveedoresServicios.First();
			_commonSettings.CrearServiciosFavoritos(primerCliente: primerCliente, primerProveedor: primerProveedor); // Call new method

			await context.AddRangeAsync(entities: _commonSettings.ServiciosFavoritos);
			await context.SaveChangesAsync();
		}).GetAwaiter().GetResult();
	}
}