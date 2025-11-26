using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Wallet.DOM.ApplicationDbContext;
using Microsoft.Extensions.Configuration;

namespace Wallet.Funcionalidad;

public class EmServiceContextFactory : IDesignTimeDbContextFactory<ServiceDbContext>
{
    public ServiceDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ServiceDbContext>();
        optionsBuilder.UseSqlServer(
            connectionString: EmServiceCollectionExtensions.GetConnectionString(configuration),
            builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

        return new ServiceDbContext(optionsBuilder.Options);
    }
}