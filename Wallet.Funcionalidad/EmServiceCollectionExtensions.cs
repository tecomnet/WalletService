using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.DOM.ApplicationDbContext;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.Funcionalidad.Helper;
using Wallet.Funcionalidad.ServiceClient;

namespace Wallet.Funcionalidad
{
	/// <summary>
	/// Special EM service extensions 602 385
	/// </summary>
	public static partial class EmServiceCollectionExtensions
	{
		private static void ConfigureServices(IServiceCollection services)
		{
            services.AddLogging();
            services.AddScoped<UrlBuilder>();
			// Setup for external services
			ConfigureExternalConnectionServices(services: services);
			// Configure facades
			ConfigureFacadeServices(services: services);
		}

		public static IServiceCollection AddEmServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ServiceDbContext>(
                options =>
                {
                    var connString = BuildConnectionString(configuration);
                    options.UseSqlServer(connString,
                        optionsBuilder => optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                }
            );
            ConfigureServices(services);
            return services;
        }

		public static string GetConnectionString()
		{
			// Try to get test connection string from environment variable
			if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("testDbConnectionString")))
			{
				// Retorna la cadena de conexion de la variable de ambiente del windows
				//return Environment.GetEnvironmentVariable("testDbConnectionString")!;
				// Retorna la conexion espaecificada
				return "Server=.;Initial Catalog=WallerService;User Id=sa; password=123;TrustServerCertificate=True;";
				// Retorna la conexion del server dev www.winsefweb.net
				//return "Server=www.winsefweb.net;Initial Catalog=TECOMNET_WALLET;User Id=DesarrolloTecomnet; password=t3comn3t2025*iu;TrustServerCertificate=True;";
			}
			// Try to get test connection string from environment variables
			if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DbServer")) &&
			    !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("Database")) &&
			    !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DbUser")) &&
			    !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DbPassword")))
			{
				return $"Server=tcp:{Environment.GetEnvironmentVariable("DbServer")};" +
				       $"Initial Catalog={Environment.GetEnvironmentVariable("Database")};" +
				       $"User Id={Environment.GetEnvironmentVariable("DbUser")};" +
				       $"password={Environment.GetEnvironmentVariable("DbPassword")}; TrustServerCertificate=true;";
			}
			// return empty string
			return string.Empty;
		}

		public static IServiceCollection AddEmTestServices(this IServiceCollection services)
        {
            services.AddDbContext<ServiceDbContext>(options => options.UseSqlServer(GetConnectionString(),
                optionsBuilder => optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            ConfigureServices(services);
            return services;
        }
        
        public static string BuildConnectionString(IConfiguration configuration)
        {
            // try to build a connection string from environment variables
            var connectionString = GetConnectionString();
            // if we have a connection string, return it
            if (!string.IsNullOrWhiteSpace(connectionString)) return connectionString;
            // try to build a connection string from configuration
            if (!string.IsNullOrWhiteSpace(configuration["dbConnectionString"])) 
                return configuration["dbConnectionString"]!;
            
            throw new Exception("No configuration detected for the service database.");
        }

		/// <summary>
		/// Setup external connection services
		/// </summary>
		/// <param name="services"></param>
		private static void ConfigureExternalConnectionServices(IServiceCollection services)
		{
			services.AddScoped<ITwilioServiceFacade, TwilioServiceFacade>();
			services.AddScoped<IChecktonPldServiceFacade, ChecktonPldServiceFacade>();
		}

		private static void ConfigureFacadeServices(IServiceCollection services)
		{
			services.AddScoped<IClienteFacade, ClienteFacade>();
			services.AddScoped<IDireccionFacade, DireccionFacade>();
            services.AddScoped<IUbicacionGeolocalizacionFacade, UbicacionGeolocalizacionFacade>();
            services.AddScoped<IDispositivoMovilAutorizadoFacade, DispositivoMovilAutorizadoFacadeFacade>();
            services.AddScoped<IEmpresaFacade, EmpresaFacade>();
            services.AddScoped<IEstadoFacade, EstadoFacade>();
		}
	}
}

