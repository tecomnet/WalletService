namespace Template.UnitTest;

public class CommonSettings
{
	// Test line for Gitnuro application
	private const string TestCaseId = "FunctionalTest";
	private static readonly Guid UserId = Guid.NewGuid();
	//public readonly List<User> Users = [];

	public CommonSettings()
	{

        // Create user data
		CreateUserData();

	}



	private void CreateUserData()
	{
		// User
		/*var user = new User(
			username: "userTest",
			creationUser: UserId,
			testCase: TestCaseId);
		Users.Add(user);*/
	}


}