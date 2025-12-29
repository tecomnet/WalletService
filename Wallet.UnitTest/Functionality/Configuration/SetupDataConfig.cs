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
			await context.AddRangeAsync(entities: _commonSettings.Brokers);
			await context.AddRangeAsync(entities: _commonSettings.Proveedores);
			await context.AddRangeAsync(entities: _commonSettings.Productos);
			await context.SaveChangesAsync();

			// After SaveChangesAsync, IDs are assigned to Clientes and Proveedores
			var primerCliente = _commonSettings.Clientes.First();
			var primerProveedor = _commonSettings.Proveedores.First();
			_commonSettings.CrearServiciosFavoritos(primerCliente: primerCliente, primerProveedor: primerProveedor);

			// Create Wallet Accounts and Cards
			_commonSettings.CrearCuentas(primerCliente);
			await context.AddRangeAsync(_commonSettings.Cuentas);
			await context.SaveChangesAsync();

			var primerCuenta = _commonSettings.Cuentas.First();
			_commonSettings.CrearTarjetasEmitidas(primerCuenta);
			await context.AddRangeAsync(_commonSettings.TarjetasEmitidas);

			_commonSettings.CrearTarjetasVinculadas(primerCuenta);
			await context.AddRangeAsync(_commonSettings.TarjetasVinculadas);

			await context.AddRangeAsync(entities: _commonSettings.ServiciosFavoritos);
			await context.SaveChangesAsync();
		}).GetAwaiter().GetResult();
	}
}