namespace Wallet.Funcionalidad.Models;

/// <summary>
/// Representa el resultado de una operación de autenticación.
/// Contiene tokens de acceso y refresco, un indicador de éxito y una lista de errores si los hubiera.
/// </summary>
public class AuthResultDto
{
    /// <summary>
    /// Obtiene o establece el token de acceso JWT (JSON Web Token).
    /// Este token se utiliza para autenticar solicitudes a recursos protegidos.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Obtiene o establece el token de refresco.
    /// Se utiliza para obtener un nuevo token de acceso cuando el actual expira, sin necesidad de que el usuario se autentique de nuevo.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica si la operación de autenticación fue exitosa.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Obtiene o establece una lista de mensajes de error si la operación de autenticación falló.
    /// Se inicializa como una lista vacía para evitar referencias nulas.
    /// </summary>
    public List<string> Errors { get; set; } = new();
}
