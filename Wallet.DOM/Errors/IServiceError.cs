namespace Wallet.DOM.Errors;

/// <summary>
/// Define una interfaz para errores de servicio, proporcionando detalles estandarizados.
/// </summary>
public interface IServiceError
{
    /// <summary>
    /// Obtiene el código de error único que identifica el tipo de error.
    /// </summary>
    string ErrorCode { get; }

    /// <summary>
    /// Obtiene un mensaje conciso que describe el error.
    /// </summary>
    string Message { get; }
    
    /// <summary>
    /// Obtiene el título del error, que puede ser utilizado para encabezados de mensajes.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Obtiene una descripción detallada del error, que puede incluir argumentos para formateo.
    /// </summary>
    /// <param name="args">Argumentos opcionales para formatear la descripción del error.</param>
    /// <returns>Una cadena que contiene la descripción detallada del error.</returns>
    string Description(object[]? args = null);
}