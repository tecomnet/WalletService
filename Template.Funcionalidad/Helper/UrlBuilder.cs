namespace Template.Funcionalidad.Helper
{
	/// <summary>
	/// URL builder for the services
	/// </summary>
	public class UrlBuilder
	{
		/// <summary>
		/// Build the URL for the service
		/// </summary>
		/// <param name="serviceName">Name of the service</param>
		/// <returns>URL of the service</returns>
		public string BuildUrl(string serviceName)
		{
			var clusterNamespace = Environment.GetEnvironmentVariable("namespace");
			if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable($"{serviceName}")))
			{
				// Build the URL for the service
				var serviceUrl = $"http://{serviceName}-service.{clusterNamespace}.svc.cluster.local:80";
				return serviceUrl;
			}
			else
			{
				// Get the service URL from the environment variables
				var serviceUrl = Environment.GetEnvironmentVariable($"{serviceName}");
				return serviceUrl;
			}
		}
	}
}