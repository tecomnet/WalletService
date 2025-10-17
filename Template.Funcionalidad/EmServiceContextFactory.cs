using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Template.Funcionalidad.ApplicationDbContext;

namespace Template.Funcionalidad;

public class EmServiceContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(
            connectionString: EmServiceCollectionExtensions.GetConnectionString(),
            builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        
        return new AppDbContext(optionsBuilder.Options);
    }
}