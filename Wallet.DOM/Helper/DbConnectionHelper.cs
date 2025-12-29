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
            //return "Server=.;Database=WalletService02;User Id=sa;Password=123;TrustServerCertificate=True;";
            // Intenta obtener la cadena de conexión de la configuración (Secrets de usuario o appsettings).
            var configConnectionString = configuration[key: "dbConnectionString"] ??
                                         configuration.GetConnectionString(name: "DefaultConnectionString");
            if (!string.IsNullOrWhiteSpace(value: configConnectionString))
            {
                return configConnectionString;
            }

            // Intenta obtener la cadena de conexión de prueba de las variables de entorno (estilo Azure).
            if (!string.IsNullOrWhiteSpace(value: Environment.GetEnvironmentVariable(variable: "DbServer")) &&
                !string.IsNullOrWhiteSpace(value: Environment.GetEnvironmentVariable(variable: "Database")) &&
                !string.IsNullOrWhiteSpace(value: Environment.GetEnvironmentVariable(variable: "DbUser")) &&
                !string.IsNullOrWhiteSpace(value: Environment.GetEnvironmentVariable(variable: "DbPassword")))
            {
                return $"Server={Environment.GetEnvironmentVariable(variable: "DbServer")};" +
                       $"Initial Catalog={Environment.GetEnvironmentVariable(variable: "Database")};" +
                       $"Persist Security Info=False;" +
                       $"User ID={Environment.GetEnvironmentVariable(variable: "DbUser")};" +
                       $"password={Environment.GetEnvironmentVariable(variable: "DbPassword")}; TrustServerCertificate=true;";
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
            var connectionString = GetConnectionString(configuration: configuration);
            // Si tenemos una cadena de conexión, la devolvemos.
            if (!string.IsNullOrWhiteSpace(value: connectionString)) return connectionString;

            // Si no se encuentra ninguna cadena de conexión, lanza una excepción.
            throw new Exception(message: "No se detectó ninguna configuración para la base de datos del servicio.");
        }
    }
}
