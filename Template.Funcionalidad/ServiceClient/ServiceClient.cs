using System.Net.Http.Headers;

namespace Template.Funcionalidad.ServiceClient
{
	public class ServiceClient<T>
	{
		#region Internal backing variables
		// Holder of the HTTP client
		private readonly HttpClient _httpClient;
		// Generic holder for the service client
		private T _serviceClient;
		// Base URL
		private readonly string _baseUrl;
		// Version of the service to use
		private readonly string _version;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor to set up the HTTP client for Postman
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="version">Version of the service to use</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public ServiceClient(string baseUrl, string version)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			// Instantiate the HTTP client
			_httpClient = new();
			// Set the base URL
			_baseUrl = baseUrl;
			// Set the version
			_version = version;
		}
		/// <summary>
		/// Constructor to set up the HTTP client for Postman
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="version">Version of the service to use</param>
		/// <param name="httpClient">Instance of the http client</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public ServiceClient(string baseUrl, string version, HttpClient httpClient)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			// Set the HTTP client
			_httpClient = httpClient;
			// Set the base URL
			_baseUrl = baseUrl;
			// Set the version
			_version = version;
		}

		public ServiceClient(string baseUrl)
		{
			// Instantiate the HTTP client
			_httpClient = new();
			// Set the base URL
			_baseUrl = baseUrl;
		}
		#endregion

		#region Factories
		/// <summary>
		/// Factory for Postman API keys
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="version">Version of the service to use</param>
		/// <param name="postmanApiKey">Postman API key for private mock servers</param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreatePostmanServiceClientFacade(
			string baseUrl,
			string version,
			string postmanApiKey)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version);
			// Add the Postman API key
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-Api-Key",
				value: postmanApiKey);
			// Return the instance
			return clientFacade;

		}
		/// <summary>
		/// Factory for API Key
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="version">Version of the service to use</param>
		/// <param name="apiKey">API key for private mock servers</param>
		/// <param name="userGuid">User GUID</param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreateApiKeyServiceClientFacade(
			string baseUrl,
			string version,
			string apiKey,
			string userGuid)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version);
			// Add the API key
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-API-Key",
				value: apiKey);
			// Add the user Guid
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "user",
				value: userGuid);
			// Return the instance
			return clientFacade;
		}

		/// <summary>
		/// Factory for API Key
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="version">Version of the service to use</param>
		/// <param name="apiKey">API key for private mock servers</param>
		/// <param name="userGuid">User GUID</param>
		/// <param name="satellite"></param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreateApiKeyServiceClientFacade(
			string baseUrl,
			string version,
			string apiKey,
			string userGuid,
			string satellite)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version);
			// Add the API key
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-API-Key",
				value: apiKey);
			// Add the user Guid
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "user",
				value: userGuid);
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "satelite",
				value: satellite);
			// Return the instance
			return clientFacade;
		}
		// <summary>
		/// Factory for Postman API keys
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="version">Version of the service to use</param>
		/// <param name="postmanApiKey">Postman API key for private mock servers</param>
		/// <param name="httpClient">Instance of the http client</param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreatePostmanServiceClientFacade(
			string baseUrl,
			string version,
			string postmanApiKey,
			HttpClient httpClient)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version, httpClient: httpClient);
			// Add the Postman API key
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-Api-Key",
				value: postmanApiKey);
			// Return the instance
			return clientFacade;

		}
		// <summary>
		/// Factory for API key
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="version">Version of the service to use</param>
		/// <param name="postmanApiKey">API key</param>
		/// <param name="userGuid">User GUID</param>
		/// <param name="httpClient">Instance of the http client</param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreateApiKeyServiceClientFacade(
			string baseUrl,
			string version,
			string apiKey,
			string userGuid,
			HttpClient httpClient)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version, httpClient: httpClient);
			// Add the Postman API key
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-Api-Key",
				value: apiKey);
			// Add the user Guid
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "user",
				value: userGuid);
			// Return the instance
			return clientFacade;

		}
		/// <summary>
		/// Factory using a bearer token
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="version">Version of the service to use</param>
		/// <param name="bearerToken">Login token for the client</param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreateBearerServiceClientFacade(
			string baseUrl,
			string version,
			string bearerToken)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version);
			// Set the beare token
			AuthenticationHeaderValue authenticationHeaderValue = new("Bearer", bearerToken);
			clientFacade.HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			// Return the client instance
			return clientFacade;
		}

		/// <summary>
		/// Factory using a bearer token
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="bearerToken">Login token for the client</param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreateServiceClientFacade(
			string baseUrl,
			string bearerToken)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl);
			// Set the beare token
			AuthenticationHeaderValue authenticationHeaderValue = new("Bearer", bearerToken);
			clientFacade.HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			// Return the client instance
			return clientFacade;
		}
		
		/// <summary>
		/// Factory using a bearer token
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreateServiceClientFacade(string baseUrl)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl);
			// Return the client instance
			return clientFacade;
		}
		
		/// <summary>
		/// Factory using a bearer token
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="bearerToken">Login token for the client</param>
		/// <param name="xIdDistribuidor"></param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreateXDistribuidorClientFacade(
			string baseUrl,
			string bearerToken,
			string xIdDistribuidor)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl);
			// Set the beare token
			AuthenticationHeaderValue authenticationHeaderValue = new("Bearer", bearerToken);
			clientFacade.HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			// Add the Postman API key
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-Id-Distribuidor",
				value: xIdDistribuidor);
			// Return the client instance
			return clientFacade;
		}
		/// <summary>
		/// Factory using a bearer token
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="bearerToken">Login token for the client</param>
		/// <param name="xIdAcceso"></param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreateXAccesoClientFacade(
			string baseUrl,
			string bearerToken,
			string xIdAcceso)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl);
			// Set the beare token
			AuthenticationHeaderValue authenticationHeaderValue = new("Bearer", bearerToken);
			clientFacade.HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			// Add the Postman API key
			clientFacade.HttpClient.DefaultRequestHeaders.Add(
				name: "X-Id-Acceso",
				value: xIdAcceso);
			// Return the client instance
			return clientFacade;
		}
		/// <summary>
		/// Factory using a bearer token
		/// </summary>
		/// <param name="baseUrl">Base URL for the service</param>
		/// <param name="version">Version of the service to use</param>
		/// <param name="bearerToken">Login token for the client</param>
		/// <param name="httpClient">Instance of the http client</param>
		/// <returns>Instance of the client service facade</returns>
		public static ServiceClient<T> CreateBearerServiceClientFacade(
			string baseUrl,
			string version,
			string bearerToken,
			HttpClient httpClient)
		{
			// Create an instance of the class
			ServiceClient<T> clientFacade = new(baseUrl: baseUrl, version: version, httpClient: httpClient);
			// Set the beare token
			AuthenticationHeaderValue authenticationHeaderValue = new("Bearer", bearerToken);
			clientFacade.HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			// Return the client instance
			return clientFacade;
		}
		#endregion

		#region Properties
		// Porperty for the base URL readonly
		public string BaseUrl => _baseUrl;
		// Porperty for the HTTP client readonly
		public HttpClient HttpClient => _httpClient;
		// Property for the service client
		public T Client
		{
			get => _serviceClient;
			set
			{
				_serviceClient = value;
			}
		}
		// Version
		public string Version => _version;
		#endregion
	}
}
