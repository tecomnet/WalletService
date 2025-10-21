using Microsoft.EntityFrameworkCore;

namespace Wallet.DOM.ApplicationDbContext;

public class ServiceDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    
    public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options)
    {
    }
    
    
    
    
}