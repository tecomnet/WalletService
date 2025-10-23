using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Modelos;

namespace Wallet.DOM.ApplicationDbContext;

public class ServiceDbContext : DbContext
{
    
    public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options)
    {
    }
    
    public DbSet<Cliente> Cliente { get; set; }   
    
}