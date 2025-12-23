using System.Net.Http.Headers;

namespace Wallet.Funcionalidad.ServiceClient
{
	public class ServiceClient<T>
	{
		#region Internal backing variables

		// Contenedor del cliente HTTP
		private readonly HttpClient _httpClient;

		// Contenedor genérico para el cliente del servicio
		private T _serviceClient;

		// URL base
		private readonly string _baseUrl;

		// Versión del servicio a utilizar
		private readonly string _version;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor para configurar el cliente HTTP.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="version">Versión del servicio a utilizar.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public ServiceClient(string baseUrl, string version)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			// Instancia el cliente HTTP
			_httpClient = new();
			// Establece la URL base
			_baseUrl = baseUrl;
			// Establece la versión
			_version = version;
		}

		/// <summary>
		/// Constructor para configurar el cliente HTTP con una instancia existente.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="version">Versión del servicio a utilizar.</param>
		/// <param name="httpClient">Instancia del cliente HTTP.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public ServiceClient(string baseUrl, string version, HttpClient httpClient)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			// Establece el cliente HTTP
			_httpClient = httpClient;
			// Establece la URL base
			_baseUrl = baseUrl;
			// Establece la versión
			_version = version;
		}

		/// <summary>
		/// Constructor para configurar el cliente HTTP solo con URL base.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		public ServiceClient(string baseUrl)
		{
			// Instancia el cliente HTTP
			_httpClient = new();
			// Establece la URL base
			_baseUrl = baseUrl;
		}

		#endregion

		#region Factories

		/// <summary>
		/// Fábrica para clientes con clave API de Postman.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="version">Versión del servicio a utilizar.</param>
		/// <param name="postmanApiKey">Clave API de Postman para servidores mock privados.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreatePostmanServiceClientFacade(
			string baseUrl,
			string version,
			string postmanApiKey)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version);
			// Agrega la clave API de Postman
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-Api-Key",
				value: postmanApiKey);
			// Retorna la instancia
			return clientFacade;
		}

		/// <summary>
		/// Fábrica para clientes con API Key y GUID de usuario.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="version">Versión del servicio a utilizar.</param>
		/// <param name="apiKey">Clave API.</param>
		/// <param name="userGuid">GUID del usuario.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreateApiKeyServiceClientFacade(
			string baseUrl,
			string version,
			string apiKey,
			string userGuid)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version);
			// Agrega la clave API
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-API-Key",
				value: apiKey);
			// Agrega el GUID del usuario
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "user",
				value: userGuid);
			// Retorna la instancia
			return clientFacade;
		}

		/// <summary>
		/// Fábrica para clientes con API Key, GUID de usuario y satélite.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="version">Versión del servicio a utilizar.</param>
		/// <param name="apiKey">Clave API.</param>
		/// <param name="userGuid">GUID del usuario.</param>
		/// <param name="satellite">Identificador del satélite.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreateApiKeyServiceClientFacade(
			string baseUrl,
			string version,
			string apiKey,
			string userGuid,
			string satellite)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version);
			// Agrega la clave API
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-API-Key",
				value: apiKey);
			// Agrega el GUID del usuario
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "user",
				value: userGuid);
			// Agrega el satélite
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "satelite",
				value: satellite);
			// Retorna la instancia
			return clientFacade;
		}

		/// <summary>
		/// Fábrica para clientes con clave API de Postman e instancia de HttpClient.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="version">Versión del servicio a utilizar.</param>
		/// <param name="postmanApiKey">Clave API de Postman para servidores mock privados.</param>
		/// <param name="httpClient">Instancia del cliente HTTP.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreatePostmanServiceClientFacade(
			string baseUrl,
			string version,
			string postmanApiKey,
			HttpClient httpClient)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version, httpClient: httpClient);
			// Agrega la clave API de Postman
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-Api-Key",
				value: postmanApiKey);
			// Retorna la instancia
			return clientFacade;
		}

		/// <summary>
		/// Fábrica para clientes con API Key, GUID de usuario e instancia de HttpClient.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="version">Versión del servicio a utilizar.</param>
		/// <param name="apiKey">Clave API.</param>
		/// <param name="userGuid">GUID del usuario.</param>
		/// <param name="httpClient">Instancia del cliente HTTP.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreateApiKeyServiceClientFacade(
			string baseUrl,
			string version,
			string apiKey,
			string userGuid,
			HttpClient httpClient)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version, httpClient: httpClient);
			// Agrega la clave API
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-Api-Key",
				value: apiKey);
			// Agrega el GUID del usuario
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "user",
				value: userGuid);
			// Retorna la instancia
			return clientFacade;
		}

		/// <summary>
		/// Fábrica para clientes usando token Bearer.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="version">Versión del servicio a utilizar.</param>
		/// <param name="bearerToken">Token de inicio de sesión para el cliente.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreateBearerServiceClientFacade(
			string baseUrl,
			string version,
			string bearerToken)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version);
			// Establece el token Bearer
			AuthenticationHeaderValue authenticationHeaderValue = new(scheme: "Bearer", parameter: bearerToken);
			clientFacade.HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			// Retorna la instancia del cliente
			return clientFacade;
		}

		/// <summary>
		/// Fábrica para clientes usando token Bearer (sin versión explícita).
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="bearerToken">Token de inicio de sesión para el cliente.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreateServiceClientFacade(
			string baseUrl,
			string bearerToken)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl);
			// Establece el token Bearer
			AuthenticationHeaderValue authenticationHeaderValue = new(scheme: "Bearer", parameter: bearerToken);
			clientFacade.HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			// Retorna la instancia del cliente
			return clientFacade;
		}

		/// <summary>
		/// Fábrica para clientes de acceso libre (sin token).
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="version">Versión del servicio a utilizar.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreateServiceClientFacadeFreeAccess(string baseUrl, string version)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version);
			// Retorna la instancia del cliente
			return clientFacade;
		}

		/// <summary>
		/// Fábrica para clientes con cabecera X-Id-Distribuidor.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="bearerToken">Token de inicio de sesión para el cliente.</param>
		/// <param name="xIdDistribuidor">ID del distribuidor.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreateXDistribuidorClientFacade(
			string baseUrl,
			string bearerToken,
			string xIdDistribuidor)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl);
			// Establece el token Bearer
			AuthenticationHeaderValue authenticationHeaderValue = new(scheme: "Bearer", parameter: bearerToken);
			clientFacade.HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			// Agrega la cabecera X-Id-Distribuidor
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-Id-Distribuidor",
				value: xIdDistribuidor);
			// Retorna la instancia del cliente
			return clientFacade;
		}

		/// <summary>
		/// Fábrica para clientes con cabecera X-Id-Acceso.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="bearerToken">Token de inicio de sesión para el cliente.</param>
		/// <param name="xIdAcceso">ID de acceso.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreateXAccesoClientFacade(
			string baseUrl,
			string bearerToken,
			string xIdAcceso)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl);
			// Establece el token Bearer
			AuthenticationHeaderValue authenticationHeaderValue = new(scheme: "Bearer", parameter: bearerToken);
			clientFacade.HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			// Agrega la cabecera X-Id-Acceso
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-Id-Acceso",
				value: xIdAcceso);
			// Retorna la instancia del cliente
			return clientFacade;
		}

		/// <summary>
		/// Fábrica para clientes usando token Bearer e instancia de HttpClient.
		/// </summary>
		/// <param name="baseUrl">URL base para el servicio.</param>
		/// <param name="version">Versión del servicio a utilizar.</param>
		/// <param name="bearerToken">Token de inicio de sesión para el cliente.</param>
		/// <param name="httpClient">Instancia del cliente HTTP.</param>
		/// <returns>Instancia de la fachada del cliente de servicio.</returns>
		public static ServiceClient<T> CreateBearerServiceClientFacade(
			string baseUrl,
			string version,
			string bearerToken,
			HttpClient httpClient)
		{
			// Crea una instancia de la clase
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version, httpClient: httpClient);
			// Establece el token Bearer
			AuthenticationHeaderValue authenticationHeaderValue = new(scheme: "Bearer", parameter: bearerToken);
			clientFacade.HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			// Retorna la instancia del cliente
			return clientFacade;
		}

		#endregion

		#region Properties

		// Propiedad para la URL base (solo lectura)
		public string BaseUrl => _baseUrl;

		// Propiedad para el cliente HTTP (solo lectura)
		public HttpClient HttpClient => _httpClient;

		// Propiedad para el cliente del servicio
		public T Client
		{
			get => _serviceClient;
			set { _serviceClient = value; }
		}

		// Versión
		public string Version => _version;

		#endregion
	}
}
