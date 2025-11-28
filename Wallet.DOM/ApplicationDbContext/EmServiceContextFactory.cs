using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Wallet.DOM.Helper;

namespace Wallet.DOM.ApplicationDbContext
{
    /// <summary>
    /// Factoría para crear instancias de ServiceDbContext en tiempo de diseño.
    /// Implementa <see cref="IDesignTimeDbContextFactory{TContext}"/> para permitir
    /// que las herramientas de Entity Framework Core (como las migraciones) puedan crear
    /// una instancia del contexto de base de datos.
    /// </summary>
    public class EmServiceContextFactory : IDesignTimeDbContextFactory<ServiceDbContext>
    {
        /// <summary>
        /// Crea una nueva instancia de <see cref="ServiceDbContext"/>.
        /// Este método es utilizado por las herramientas de Entity Framework Core en tiempo de diseño.
        /// </summary>
        /// <param name="args">Argumentos de línea de comandos (no utilizados en esta implementación).</param>
        /// <returns>Una nueva instancia de <see cref="ServiceDbContext"/> configurada.</returns>
        public ServiceDbContext CreateDbContext(string[] args)
        {
            // Construye la configuración para obtener variables de entorno.
            // Esto permite que la cadena de conexión se defina fuera del código, por ejemplo, en variables de entorno.
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            // Crea un constructor de opciones para ServiceDbContext.
            var optionsBuilder = new DbContextOptionsBuilder<ServiceDbContext>();

            // Configura el contexto para usar SQL Server.
            // La cadena de conexión se obtiene a través de un método auxiliar.
            // Se configura el comportamiento de división de consultas para optimizar el rendimiento.
            optionsBuilder.UseSqlServer(
                connectionString: DbConnectionHelper.GetConnectionString(configuration),
                sqlServerOptionsAction: builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

            // Retorna una nueva instancia de ServiceDbContext con las opciones configuradas.
            return new ServiceDbContext(optionsBuilder.Options);
        }
    }
}
