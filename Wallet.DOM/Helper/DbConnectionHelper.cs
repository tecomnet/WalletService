using Microsoft.Extensions.Configuration;

namespace Wallet.DOM.Helper
{
    /// <summary>
    /// Helper para la gestión de cadenas de conexión a la base de datos.
    /// </summary>
    public static class DbConnectionHelper
    {
        /// <summary>
        /// Obtiene la cadena de conexión a la base de datos de la configuración o de las variables de entorno.
        /// </summary>
        /// <param name="configuration">La configuración de la aplicación.</param>
        /// <returns>La cadena de conexión a la base de datos.</returns>
        public static string GetConnectionString(IConfiguration configuration)
        {
            // Intenta obtener la cadena de conexión de la configuración (Secrets de usuario o appsettings).
            var configConnectionString = configuration["dbConnectionString"] ??
                                         configuration.GetConnectionString("DefaultConnectionString");
            if (!string.IsNullOrWhiteSpace(configConnectionString))
            {
                return configConnectionString;
            }

            // Intenta obtener la cadena de conexión de prueba de las variables de entorno (estilo Azure).
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DbServer")) &&
                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("Database")) &&
                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DbUser")) &&
                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DbPassword")))
            {
                return $"Server={Environment.GetEnvironmentVariable("DbServer")};" +
                       $"Initial Catalog={Environment.GetEnvironmentVariable("Database")};" +
                       $"Persist Security Info=False;" +
                       $"User ID={Environment.GetEnvironmentVariable("DbUser")};" +
                       $"password={Environment.GetEnvironmentVariable("DbPassword")}; TrustServerCertificate=true;";
            }

            // Si no se encuentra ninguna cadena de conexión, devuelve una cadena vacía.
            return string.Empty;
        }

        /// <summary>
        /// Construye la cadena de conexión a la base de datos, lanzando una excepción si no se encuentra ninguna configuración.
        /// </summary>
        /// <param name="configuration">La configuración de la aplicación.</param>
        /// <returns>La cadena de conexión a la base de datos.</returns>
        /// <exception cref="Exception">Se lanza si no se detecta ninguna configuración para la base de datos del servicio.</exception>
        public static string BuildConnectionString(IConfiguration configuration)
        {
            // Intenta construir la cadena de conexión desde la configuración.
            var connectionString = GetConnectionString(configuration);
            // Si tenemos una cadena de conexión, la devolvemos.
            if (!string.IsNullOrWhiteSpace(connectionString)) return connectionString;

            // Si no se encuentra ninguna cadena de conexión, lanza una excepción.
            throw new Exception("No se detectó ninguna configuración para la base de datos del servicio.");
        }
    }
}
