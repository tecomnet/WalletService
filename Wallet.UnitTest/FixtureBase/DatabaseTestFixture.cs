using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Funcionalidad;
using Wallet.DOM.ApplicationDbContext;
using Wallet.RestAPI;

namespace Wallet.UnitTest.FixtureBase
{
    [Collection("FunctionalCollection")]
    public class DatabaseTestFixture : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        protected readonly CustomWebApplicationFactory<Program> Factory;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DatabaseTestFixture()
        {
            // Build configuration with User Secrets
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<DatabaseTestFixture>()
                .AddEnvironmentVariables();
            _configuration = builder.Build();

            _connectionString = EmServiceCollectionExtensions.GetConnectionString(_configuration);

            SetEnvironmentalVariables();
            var factory = new CustomWebApplicationFactory<Program>();
            factory.ConfigureWebApplicationFactory(services => { services.AddSingleton(_configuration); });
            Factory = factory;

            using var context = CreateContext();
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        protected async Task SetupDataAsync(Func<ServiceDbContext, Task> setupDataAction)
        {
            await using var context = CreateContext();
            await setupDataAction(context);
        }

        protected internal ServiceDbContext CreateContext()
            => new(
                new DbContextOptionsBuilder<ServiceDbContext>()
                    .UseSqlServer(_connectionString,
                        optionsBuilder => optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .Options);

        private void SetEnvironmentalVariables()
        {
            Environment.SetEnvironmentVariable(
                "dbConnectionString",
                _connectionString);
        }
    }
}