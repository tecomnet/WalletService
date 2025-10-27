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
			await context.AddRangeAsync(_commonSettings.Clientes);
			await context.AddRangeAsync(_commonSettings.Estados);
			await context.AddRangeAsync(_commonSettings.Empresas);
			await context.SaveChangesAsync();
		}).GetAwaiter().GetResult();
	}
}