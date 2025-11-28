using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.UsuarioFacade;

public interface IUsuarioFacade
{
    /// <summary>
    /// Obtiene un usuario por su identificador único.
    /// </summary>
    /// <param name="idUsuario">El identificador único del usuario.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Usuario"/> encontrado.</returns>
    Task<Usuario> ObtenerUsuarioPorIdAsync(int idUsuario);

    /// <summary>
    /// Guarda (establece) la contraseña de un usuario.
    /// </summary>
    /// <param name="idUsuario">El identificador del usuario.</param>
    /// <param name="contrasena">La nueva contraseña a establecer.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el token de acceso generado.</returns>
    Task<string> GuardarContrasenaAsync(int idUsuario, string contrasena, Guid modificationUser);

    /// <summary>
    /// Actualiza la contraseña de un usuario, verificando la contraseña actual.
    /// </summary>
    /// <param name="idUsuario">El identificador del usuario.</param>
    /// <param name="contrasenaActual">La contraseña actual del usuario.</param>
    /// <param name="contrasenaNueva">La nueva contraseña a establecer.</param>
    /// <param name="confirmacionContrasenaNueva">La confirmación de la nueva contraseña (debe coincidir con contrasenaNueva).</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Usuario"/> actualizado.</returns>
    Task<Usuario> ActualizarContrasenaAsync(int idUsuario, string contrasenaActual, string contrasenaNueva,
        string confirmacionContrasenaNueva, Guid modificationUser);

    /// <summary>
    /// Actualiza el correo electrónico de un usuario.
    /// </summary>
    /// <param name="idUsuario">El identificador del usuario.</param>
    /// <param name="correoElectronico">El nuevo correo electrónico.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
    /// <param name="testCase">Opcional. Un identificador para casos de prueba.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Usuario"/> actualizado.</returns>
    Task<Usuario> ActualizarCorreoElectronicoAsync(int idUsuario, string correoElectronico,
        Guid modificationUser, string? testCase = null);

    /// <summary>
    /// Actualiza el número de teléfono de un usuario.
    /// </summary>
    /// <param name="idUsuario">El identificador del usuario.</param>
    /// <param name="codigoPais">El código de país del nuevo número de teléfono.</param>
    /// <param name="telefono">El nuevo número de teléfono.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
    /// <param name="testCase">Opcional. Un identificador para casos de prueba.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Usuario"/> actualizado.</returns>
    Task<Usuario> ActualizarTelefonoAsync(int idUsuario, string codigoPais, string telefono,
        Guid modificationUser, string? testCase = null);

    /// <summary>
    /// Confirma un código de verificación de doble factor (2FA).
    /// </summary>
    /// <param name="idUsuario">El identificador del usuario.</param>
    /// <param name="tipo2FA">El tipo de autenticación de doble factor (ej. SMS, Email).</param>
    /// <param name="codigoVerificacion">El código de verificación ingresado por el usuario.</param>
    /// <param name="modificationUser">El identificador del usuario que realiza la confirmación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. Retorna true si la verificación es exitosa, false en caso contrario.</returns>
    Task<bool> ConfirmarCodigoVerificacion2FAAsync(int idUsuario, Tipo2FA tipo2FA,
        string codigoVerificacion, Guid modificationUser);

    /// <summary>
    /// Crea un usuario en estado de pre-registro con su número de teléfono.
    /// </summary>
    /// <param name="codigoPais">El código de país del teléfono.</param>
    /// <param name="telefono">El número de teléfono.</param>
    /// <param name="creationUser">El identificador del usuario que crea el registro.</param>
    /// <param name="testCase">Opcional. Un identificador para casos de prueba.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el objeto <see cref="Usuario"/> creado.</returns>
    Task<Usuario> GuardarUsuarioPreRegistroAsync(string codigoPais, string telefono, Guid creationUser,
        string? testCase = null);
}
