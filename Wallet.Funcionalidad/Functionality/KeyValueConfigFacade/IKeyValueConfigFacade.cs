using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.KeyValueConfigFacade;

/// <summary>
/// Interfaz para la fachada de gestión de configuraciones clave-valor.
/// </summary>
public interface IKeyValueConfigFacade
{
    /// <summary>
    /// Guarda o crea una nueva configuración clave-valor.
    /// </summary>
    /// <param name="key">La clave de la configuración.</param>
    /// <param name="value">El valor de la configuración.</param>
    /// <param name="creationUser">El GUID del usuario que crea la configuración.</param>
    /// <param name="testCase">Opcional. Identificador para casos de prueba.</param>
    /// <returns>La entidad KeyValueConfig creada.</returns>
    Task<KeyValueConfig> GuardarKeyValueConfigAsync(string key, string value, Guid creationUser,
        string? testCase = null);

    /// <summary>
    /// Obtiene una configuración por su clave.
    /// </summary>
    /// <param name="key">La clave de la configuración a buscar.</param>
    /// <returns>La entidad KeyValueConfig encontrada.</returns>
    Task<KeyValueConfig> ObtenerKeyValueConfigPorKeyAsync(string key);

    /// <summary>
    /// Obtiene todas las configuraciones almacenadas.
    /// </summary>
    /// <returns>Una lista de todas las configuraciones.</returns>
    Task<List<KeyValueConfig>> ObtenerTodasLasConfiguracionesAsync();

    /// <summary>
    /// Actualiza el valor de una configuración existente.
    /// </summary>
    /// <param name="key">La clave de la configuración a actualizar.</param>
    /// <param name="value">El nuevo valor de la configuración.</param>
    /// <param name="concurrencyToken">Opcional. Token de concurrencia para control de versiones.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <param name="testCase">Opcional. Identificador para casos de prueba.</param>
    /// <returns>La entidad KeyValueConfig actualizada.</returns>
    Task<KeyValueConfig> ActualizarKeyValueConfigAsync(string key, string value, string? concurrencyToken,
        Guid modificationUser,
        string? testCase = null);

    /// <summary>
    /// Elimina (desactivación lógica) una configuración por su clave.
    /// </summary>
    /// <param name="key">La clave de la configuración a eliminar.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la eliminación.</param>
    /// <returns>La entidad KeyValueConfig eliminada.</returns>
    Task<KeyValueConfig> EliminarKeyValueConfigAsync(string key, Guid modificationUser);
}
