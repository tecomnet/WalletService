using Microsoft.Extensions.DependencyInjection;
using Template.Funcionalidad;
using Template.Funcionalidad.ApplicationDbContext;

namespace Template.UnitTest.Functionality.Configuration;

[Collection("FunctionalCollection")]
public abstract class BaseFacadeTest<T> : UnitTestTemplate, IClassFixture<SetupDataConfig> where T : class
{
    #region Config

    // Defines setup config
    protected readonly SetupDataConfig SetupConfig;

    // Defines a context for test   
    protected readonly AppDbContext Context;

    // Defines a facade for test
    protected readonly T Facade;

    // Dependency injection container
    private IServiceProvider ServiceProvider { get; }

    protected BaseFacadeTest(SetupDataConfig setupConfig)
    {
        var services = new ServiceCollection();
        // Configure your services as needed, register dependencies
        services.AddEmTestServices();
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