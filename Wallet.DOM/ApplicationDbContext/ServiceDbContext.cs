using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Modelos;

namespace Wallet.DOM.ApplicationDbContext;

public class ServiceDbContext : DbContext
{

    public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Cliente { get; set; }
    public DbSet<Direccion> Direccion { get; set; }
    public DbSet<Estado> Estado { get; set; }
    public DbSet<Empresa> Empresa { get; set; }



    // Sobrescribimos este método para configurar el modelo y agregar los datos iniciales
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -----------------------------------------------------
        // 1. CONFIGURACIÓN DE RELACIONES (Opcional, pero buena práctica)
        // -----------------------------------------------------
        /*modelBuilder.Entity<Cliente>()
            .HasMany(c => c.Direcciones) // Cliente tiene muchas Direcciones
            .WithOne(d => d.Cliente) // Direccion pertenece a un Cliente
            .HasForeignKey(d => d.ClienteId); // La clave foránea es ClienteId*/


        // -----------------------------------------------------
        // 2. SEEDING DE DATOS (Llenado de datos iniciales)
        // -----------------------------------------------------

        // A. Seed para la entidad Empresa
        /*modelBuilder.Entity<Empresa>().HasData(
            new Empresa
            {
                Id = 1, 
                Nombre = "Tecomnet", 
                CreationUser = Guid.Empty,
                TestCaseID = "Seeding",
                IsActive = true
            },
            new Empresa
            {
                Id = 2, 
                Nombre = "NuevaEmpresa", 
                CreationUser = Guid.Empty,
                TestCaseID = "Seeding",
                IsActive = true
            }
        );

        // B. Seed para la entidad Estados de Mexico 
        modelBuilder.Entity<Estado>().HasData(
            new Estado { Id = 1, Nombre = "Aguascalientes", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true},
            new Estado { Id = 2, Nombre = "Baja California", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 3, Nombre = "Baja California Sur", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 4, Nombre = "Campeche", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 5, Nombre = "Chiapas", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 6, Nombre = "Chihuahua", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 7, Nombre = "Coahuila", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 8, Nombre = "Colima", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 9, Nombre = "Ciudad de México", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 10, Nombre = "Durango", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 11, Nombre = "Guanajuato", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 12, Nombre = "Guerrero", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 13, Nombre = "Hidalgo", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 14, Nombre = "Jalisco", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 15, Nombre = "México", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true }, // Estado de México
            new Estado { Id = 16, Nombre = "Michoacán", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 17, Nombre = "Morelos", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 18, Nombre = "Nayarit", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 19, Nombre = "Nuevo León", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 20, Nombre = "Oaxaca", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 21, Nombre = "Puebla", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 22, Nombre = "Querétaro", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 23, Nombre = "Quintana Roo", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 24, Nombre = "San Luis Potosí", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 25, Nombre = "Sinaloa", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 26, Nombre = "Sonora", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 27, Nombre = "Tabasco", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 28, Nombre = "Tamaulipas", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 29, Nombre = "Tlaxcala", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 30, Nombre = "Veracruz", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 31, Nombre = "Yucatán", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true },
            new Estado { Id = 32, Nombre = "Zacatecas", CreationUser = Guid.Empty, TestCaseID = "Seeding", IsActive = true }
        );*/
    }
}
    