using Template.UnitTest.FixtureBase;

namespace Template.UnitTest.Functionality.Configuration;

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
			
			//await context.AddRangeAsync(_commonSettings.Users);

			await context.SaveChangesAsync();
		}).GetAwaiter().GetResult();
	}
}