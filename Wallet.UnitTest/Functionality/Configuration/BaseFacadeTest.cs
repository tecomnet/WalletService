using Microsoft.Extensions.DependencyInjection;
using Wallet.Funcionalidad;
using Wallet.DOM.ApplicationDbContext;
using Microsoft.Extensions.Configuration;
using Moq; // Add Moq namespace
using Wallet.Funcionalidad.ServiceClient; // Add namespace for facades

namespace Wallet.UnitTest.Functionality.Configuration;

[Collection("FunctionalCollection")]
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

        services.AddEmTestServices(configuration);

        // Instantiate mocks
        TwilioServiceFacadeMock = new Mock<ITwilioServiceFacade>();
        ChecktonPldServiceFacadeMock = new Mock<IChecktonPldServiceFacade>();

        // Override with mocked services
        services.AddSingleton(TwilioServiceFacadeMock.Object);
        services.AddSingleton(ChecktonPldServiceFacadeMock.Object);

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