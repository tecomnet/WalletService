using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq; // Add Moq namespace
using Wallet.DOM.ApplicationDbContext;
using Wallet.Funcionalidad;
using Wallet.Funcionalidad.ServiceClient; // Add namespace for facades
using Wallet.Funcionalidad.Services.TokenService;

namespace Wallet.UnitTest.Functionality.Configuration;

[Collection(name: "FunctionalCollection")]
public abstract class BaseFacadeTest<T> : UnitTestTemplate, IClassFixture<SetupDataConfig> where T : class
{
    #region Config

    // Defines setup config
    protected readonly SetupDataConfig SetupConfig;

    // Defines a context for test   
    protected readonly ServiceDbContext Context;

    // Defines a facade for test
    protected readonly T Facade;

    // Mocks for external services
    public Mock<ITwilioServiceFacade> TwilioServiceFacadeMock { get; }
    public Mock<IChecktonPldServiceFacade> ChecktonPldServiceFacadeMock { get; }
    public Mock<ITokenService> TokenServiceMock { get; }

    // Dependency injection container
    private IServiceProvider ServiceProvider { get; }

    protected BaseFacadeTest(SetupDataConfig setupConfig)
    {
        var services = new ServiceCollection();
        // Configure your services as needed, register dependencies
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SetupDataConfig>()
            .AddEnvironmentVariables()
            .Build();

        services.AddEmTestServices(configuration: configuration);

        // Instantiate mocks
        TwilioServiceFacadeMock = new Mock<ITwilioServiceFacade>();
        ChecktonPldServiceFacadeMock = new Mock<IChecktonPldServiceFacade>();
        TokenServiceMock = new Mock<ITokenService>();

        // Override with mocked services
        services.AddSingleton(implementationInstance: TwilioServiceFacadeMock.Object);
        services.AddSingleton(implementationInstance: ChecktonPldServiceFacadeMock.Object);
        services.AddSingleton(implementationInstance: TokenServiceMock.Object);

        // Build the service provider
        ServiceProvider = services.BuildServiceProvider();
        // Context for test
        Context = setupConfig.CreateContext();
        // Define facade
        Facade = ServiceProvider.GetRequiredService<T>();
        // Instance of config
        SetupConfig = new SetupDataConfig();
    }

    public IServiceProvider GetServiceProvider()
    {
        return ServiceProvider;
    }

    #endregion
}