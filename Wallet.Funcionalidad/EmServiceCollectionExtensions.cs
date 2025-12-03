using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Helper;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;
using Wallet.Funcionalidad.Functionality.ServicioFavoritoFacade;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;
using Wallet.Funcionalidad.Functionality.ConsentimientosUsuarioFacade;
using Wallet.Funcionalidad.Helper;
using Wallet.Funcionalidad.ServiceClient;
using Wallet.Funcionalidad.Functionality.AuthFacade;
using Wallet.Funcionalidad.Services.TokenService;

namespace Wallet.Funcionalidad
{
	/// <summary>
	/// Extensiones especiales para la colección de servicios de EM.
	/// Proporciona métodos para configurar y registrar los servicios y fachadas de la aplicación.
	/// </summary>
	public static partial class EmServiceCollectionExtensions
	{
		/// <summary>
		/// Configura los servicios internos de la aplicación, incluyendo logging, constructores de URL y otros servicios específicos.
		/// </summary>
		/// <param name="services">La colección de servicios para registrar las dependencias.</param>
		private static void ConfigureServices(IServiceCollection services)
		{
			// Habilita el servicio de logging.
			services.AddLogging();
			// Registra el constructor de URLs como un servicio con ámbito.
			services.AddScoped<UrlBuilder>();
			// Configuración de servicios externos.
			ConfigureExternalConnectionServices(services: services);
			// Configuración de servicios de fachada.
			ConfigureFacadeServices(services: services);
			// Configuración de servicios internos.
			ConfigureInternalServices(services: services);
		}

		/// <summary>
		/// Agrega los servicios de la aplicación al contenedor de inyección de dependencias.
		/// Configura el DbContext y otros servicios esenciales.
		/// </summary>
		/// <param name="services">La colección de servicios a la que se añadirán los servicios de EM.</param>
		/// <param name="configuration">La configuración de la aplicación.</param>
		/// <returns>La colección de servicios actualizada.</returns>
		public static IServiceCollection AddEmServices(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			// Configura el contexto de base de datos para usar SQL Server.
			services.AddDbContext<ServiceDbContext>(optionsAction: options =>
				{
					// Construye la cadena de conexión a la base de datos.
					var connString = DbConnectionHelper.BuildConnectionString(configuration: configuration);
					options.UseSqlServer(connectionString: connString,
						sqlServerOptionsAction: optionsBuilder =>
							optionsBuilder.UseQuerySplittingBehavior(querySplittingBehavior: QuerySplittingBehavior.SplitQuery));
				}
			);
			// Llama al método para configurar servicios adicionales.
			ConfigureServices(services: services);
			return services;
		}


		/// <summary>
		/// Agrega los servicios de la aplicación para entornos de prueba.
		/// Configura el DbContext con la cadena de conexión obtenida de la configuración.
		/// </summary>
		/// <param name="services">La colección de servicios a la que se añadirán los servicios de prueba de EM.</param>
		/// <param name="configuration">La configuración de la aplicación.</param>
		/// <returns>La colección de servicios actualizada.</returns>
		public static IServiceCollection AddEmTestServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ServiceDbContext>(optionsAction: options => options.UseSqlServer(
				connectionString: DbConnectionHelper.GetConnectionString(configuration: configuration),
				sqlServerOptionsAction: builder =>
					builder.UseQuerySplittingBehavior(querySplittingBehavior: QuerySplittingBehavior.SplitQuery)));
			// Llama al método para configurar servicios adicionales.
			ConfigureServices(services: services);
			return services;
		}


		/// <summary>
		/// Configura los servicios de conexión a proveedores externos.
		/// </summary>
		/// <param name="services">La colección de servicios para registrar las dependencias.</param>
		private static void ConfigureExternalConnectionServices(IServiceCollection services)
		{
			// Registra la fachada del servicio Twilio.
			services.AddScoped<ITwilioServiceFacade, TwilioServiceFacade>();
			// Registra la fachada del servicio Checkton PLD.
			services.AddScoped<IChecktonPldServiceFacade, ChecktonPldServiceFacade>();
		}

		/// <summary>
		/// Configura y registra las fachadas de la aplicación.
		/// </summary>
		/// <param name="services">La colección de servicios para registrar las dependencias.</param>
		private static void ConfigureFacadeServices(IServiceCollection services)
		{
			// Fachadas de gestión de datos.
			services.AddScoped<IClienteFacade, ClienteFacade>();
			services.AddScoped<IDireccionFacade, DireccionFacade>();
			services.AddScoped<IUbicacionGeolocalizacionFacade, UbicacionGeolocalizacionFacade>();
			services.AddScoped<IDispositivoMovilAutorizadoFacade, DispositivoMovilAutorizadoFacadeFacade>();
			services.AddScoped<IEmpresaFacade, EmpresaFacade>();
			services.AddScoped<IEstadoFacade, EstadoFacade>();
			services.AddScoped<IProveedorServicioFacade, ProveedorServicioFacade>();
			services.AddScoped<IServicioFavoritoFacade, ServicioFavoritoFacade>();
			services.AddScoped<IUsuarioFacade, UsuarioFacade>();
			services.AddScoped<IConsentimientosUsuarioFacade, ConsentimientosUsuarioFacade>();
			// Fachada de autenticación.
			services.AddScoped<IAuthFacade, AuthFacade>();
		}

		/// <summary>
		/// Configura y registra los servicios internos de la aplicación.
		/// </summary>
		/// <param name="services">La colección de servicios para registrar las dependencias.</param>
		private static void ConfigureInternalServices(IServiceCollection services)
		{
			// Servicio para la gestión de tokens.
			services.AddScoped<ITokenService, TokenService>();
		}
	}
}
