namespace Wallet.DOM.Errors;

/// <summary>
/// Representa un error de servicio genérico con un código de error, mensaje y descripción.
/// </summary>
public class ServiceError : IServiceError
{
    /// <summary>
    /// Descripción detallada del error.
    /// </summary>
    private readonly string _description = "Este es un error de servicio predeterminado emitido cuando el error específico no puede ser encontrado.";

    /// <summary>
    /// Obtiene o establece el código único del error.
    /// </summary>
    public string ErrorCode { get; protected set; }

    /// <summary>
    /// Obtiene o establece el mensaje principal del error.
    /// </summary>
    public string Message { get; protected set; }

    /// <summary>
    /// Obtiene el título del error, que por defecto es el mismo que el mensaje.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Obtiene la descripción del error, formateando argumentos si se proporcionan.
    /// </summary>
    /// <param name="args">Argumentos opcionales para formatear la descripción.</param>
    /// <returns>La descripción formateada del error.</returns>
    public string Description(object[]? args = null)
    {
        return args == null || args.Length == 0 ? this._description : string.Format(format: this._description, args: args);
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ServiceError"/>.
    /// </summary>
    /// <param name="errorCode">El código único del error.</param>
    /// <param name="message">El mensaje principal del error.</param>
    /// <param name="description">La descripción detallada del error.</param>
    public ServiceError(string errorCode, string message, string description)
    {
        this.ErrorCode = errorCode;
        this.Message = message;
        this.Title = message; // El título se establece igual que el mensaje por defecto.
        this._description = description;
    }
}