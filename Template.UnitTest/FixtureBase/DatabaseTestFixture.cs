using Microsoft.EntityFrameworkCore;
using Template.Funcionalidad;
using Template.Funcionalidad.ApplicationDbContext;
using Template.RestAPI;

namespace Template.UnitTest.FixtureBase
{
    [Collection("FunctionalCollection")]
    public class DatabaseTestFixture : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        protected readonly CustomWebApplicationFactory<Program> Factory;

        private readonly string _connectionString = EmServiceCollectionExtensions.GetConnectionString();

        public DatabaseTestFixture()
        {
            SetEnvironmentalVariables();
            var factory = new CustomWebApplicationFactory<Program>();
            Factory = factory;
         
            using var context = CreateContext();
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        protected async Task SetupDataAsync(Func<AppDbContext, Task> setupDataAction)
        {
            await using var context = CreateContext();
            await setupDataAction(context);
        }

        protected internal AppDbContext CreateContext()
            => new(
                new DbContextOptionsBuilder<AppDbContext>()
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