using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.DOM.ApplicationDbContext;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;
using Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;
using Wallet.Funcionalidad.Helper;
using Wallet.Funcionalidad.ServiceClient;
using Wallet.Funcionalidad.Functionality.AuthFacade;
using Wallet.Funcionalidad.Services.TokenService;

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
			// Configure internal services
			ConfigureInternalServices(services: services);
		}

		public static IServiceCollection AddEmServices(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddDbContext<ServiceDbContext>(optionsAction: options =>
				{
					var connString = BuildConnectionString(configuration: configuration);
					options.UseSqlServer(connectionString: connString,
						sqlServerOptionsAction: optionsBuilder => optionsBuilder.UseQuerySplittingBehavior(querySplittingBehavior: QuerySplittingBehavior.SplitQuery));
				}
			);
			ConfigureServices(services: services);
			return services;
		}

		public static string GetConnectionString(IConfiguration configuration)
		{
			// Try to get connection string from configuration (User Secrets or appsettings)
			var configConnectionString = configuration[key: "dbConnectionString"];
			if (!string.IsNullOrWhiteSpace(value: configConnectionString))
			{
				return configConnectionString;
			}

			// Try to get test connection string from environment variables (Azure style)
			if (!string.IsNullOrWhiteSpace(value: Environment.GetEnvironmentVariable(variable: "DbServer")) &&
			    !string.IsNullOrWhiteSpace(value: Environment.GetEnvironmentVariable(variable: "Database")) &&
			    !string.IsNullOrWhiteSpace(value: Environment.GetEnvironmentVariable(variable: "DbUser")) &&
			    !string.IsNullOrWhiteSpace(value: Environment.GetEnvironmentVariable(variable: "DbPassword")))
			{
				return $"Server=tcp:{Environment.GetEnvironmentVariable(variable: "DbServer")};" +
				       $"Initial Catalog={Environment.GetEnvironmentVariable(variable: "Database")};" +
				       $"User Id={Environment.GetEnvironmentVariable(variable: "DbUser")};" +
				       $"password={Environment.GetEnvironmentVariable(variable: "DbPassword")}; TrustServerCertificate=true;";
			}

			return string.Empty;
		}

		public static IServiceCollection AddEmTestServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ServiceDbContext>(optionsAction: options => options.UseSqlServer(connectionString: GetConnectionString(configuration: configuration),
				sqlServerOptionsAction: optionsBuilder => optionsBuilder.UseQuerySplittingBehavior(querySplittingBehavior: QuerySplittingBehavior.SplitQuery)));
			ConfigureServices(services: services);
			return services;
		}

		public static string BuildConnectionString(IConfiguration configuration)
		{
			// try to build a connection string from configuration
			var connectionString = GetConnectionString(configuration: configuration);
			// if we have a connection string, return it
			if (!string.IsNullOrWhiteSpace(value: connectionString)) return connectionString;

			throw new Exception(message: "No configuration detected for the service database.");
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
			services.AddScoped<IProveedorServicioFacade, ProveedorServicioFacade>();
			services.AddScoped<IServicioFavoritoFacade, ServicioFavoritoFacade>();
			services.AddScoped<IUsuarioFacade, UsuarioFacade>();
			services.AddScoped<IAuthFacade, AuthFacade>();
		}

		private static void ConfigureInternalServices(IServiceCollection services)
		{
			services.AddScoped<ITokenService, TokenService>();
		}
	}
}

