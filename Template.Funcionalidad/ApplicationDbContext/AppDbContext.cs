using Microsoft.EntityFrameworkCore;

namespace Template.Funcionalidad.ApplicationDbContext;

public class AppDbContext : DbContext
{
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    
    
    
}