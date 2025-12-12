using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Wallet.DOM.ApplicationDbContext;

namespace Wallet.Funcionalidad;

public class EmServiceContextFactory : IDesignTimeDbContextFactory<ServiceDbContext>
{
    public ServiceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ServiceDbContext>();
        optionsBuilder.UseSqlServer(
            connectionString: EmServiceCollectionExtensions.GetConnectionString(),
            builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        
        return new ServiceDbContext(optionsBuilder.Options);
    }
}