namespace Wallet.Funcionalidad.Helper
{
	/// <summary>
	/// Constructor de URLs para los servicios
	/// </summary>
	public class UrlBuilder
	{
		/// <summary>
		/// Construye la URL para el servicio dado.
		/// </summary>
		/// <param name="serviceName">Nombre del servicio.</param>
		/// <returns>URL completa del servicio.</returns>
		public string BuildUrl(string serviceName)
		{
			// Obtiene el namespace del clúster desde las variables de entorno.
			var clusterNamespace = Environment.GetEnvironmentVariable(variable: "namespace");

			// Verifica si la URL del servicio está definida directamente en una variable de entorno.
			if (string.IsNullOrEmpty(value: Environment.GetEnvironmentVariable(variable: $"{serviceName}")))
			{
				// Si no está definida, construye la URL asumiendo un entorno de Kubernetes.
				// Esto es para entornos de Kubernetes donde los servicios se descubren por nombre y namespace.
				var serviceUrl = $"http://{serviceName}-service.{clusterNamespace}.svc.cluster.local:80";
				return serviceUrl;
			}
			else
			{
				// Si la URL del servicio está definida en una variable de entorno, la utiliza directamente.
				// Esto permite sobrescribir la URL por defecto o usarla en entornos no-Kubernetes.
				var serviceUrl = Environment.GetEnvironmentVariable(variable: $"{serviceName}");
				return serviceUrl;
			}
		}
	}
}