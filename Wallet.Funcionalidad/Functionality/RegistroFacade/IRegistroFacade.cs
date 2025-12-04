using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.RegistroFacade;

/// <summary>
/// Proporciona una interfaz para la fachada del proceso de registro de usuarios,
/// orquestando las diferentes etapas del registro.
/// </summary>
public interface IRegistroFacade
{
    /// <summary>
    /// Realiza el pre-registro de un usuario, asociando un número de teléfono y un código de país.
    /// </summary>
    /// <param name="codigoPais">El código del país del número de teléfono.</param>
    /// <param name="telefono">El número de teléfono del usuario.</param>
    /// <param name="creationUser">El GUID del usuario que realiza la creación.</param>
    /// <returns>Un objeto <see cref="Usuario"/> que representa el usuario pre-registrado.</returns>
    Task<Usuario> PreRegistroAsync(string codigoPais, string telefono, Guid creationUser);

    /// <summary>
    /// Confirma el número de teléfono de un usuario utilizando un código de verificación.
    /// </summary>
    /// <param name="idUsuario">El identificador único del usuario.</param>
    /// <param name="codigo">El código de verificación enviado al número de teléfono.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <returns>True si la confirmación fue exitosa, de lo contrario, false.</returns>
    Task<bool> ConfirmarNumeroAsync(int idUsuario, string codigo, Guid modificationUser);

    /// <summary>
    /// Completa los datos personales básicos del cliente.
    /// </summary>
    /// <param name="idUsuario">El identificador único del usuario.</param>
    /// <param name="nombre">El nombre del usuario.</param>
    /// <param name="apellidoPaterno">El apellido paterno del usuario.</param>
    /// <param name="apellidoMaterno">El apellido materno del usuario.</param>
    /// <param name="nombreEstado">El nombre del estado de residencia del usuario.</param>
    /// <param name="fechaNacimiento">La fecha de nacimiento del usuario.</param>
    /// <param name="genero">El género del usuario.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <returns>Un objeto <see cref="Usuario"/> con los datos actualizados.</returns>
    Task<Usuario> CompletarDatosClienteAsync(int idUsuario, string nombre, string apellidoPaterno,
        string apellidoMaterno, string nombreEstado, DateOnly fechaNacimiento, Genero genero, Guid modificationUser);

    /// <summary>
    /// Registra la dirección de correo electrónico de un usuario.
    /// </summary>
    /// <param name="idUsuario">El identificador único del usuario.</param>
    /// <param name="correo">La dirección de correo electrónico a registrar.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <returns>Un objeto <see cref="Usuario"/> con el correo electrónico registrado.</returns>
    Task<Usuario> RegistrarCorreoAsync(int idUsuario, string correo, Guid modificationUser);

    /// <summary>
    /// Verifica la dirección de correo electrónico de un usuario utilizando un código.
    /// </summary>
    /// <param name="idUsuario">El identificador único del usuario.</param>
    /// <param name="codigo">El código de verificación enviado al correo electrónico.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <returns>True si la verificación fue exitosa, de lo contrario, false.</returns>
    Task<bool> VerificarCorreoAsync(int idUsuario, string codigo, Guid modificationUser);

    /// <summary>
    /// Registra los datos biométricos de un usuario asociados a un dispositivo.
    /// </summary>
    /// <param name="idUsuario">El identificador único del usuario.</param>
    /// <param name="idDispositivo">El identificador del dispositivo.</param>
    /// <param name="token">El token biométrico o de autenticación del dispositivo.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <returns>Un objeto <see cref="Usuario"/> con los datos biométricos registrados.</returns>
    Task<Usuario> RegistrarDatosBiometricosAsync(int idUsuario, string idDispositivo, string token,
        Guid modificationUser);

    /// <summary>
    /// Registra la aceptación de los términos y condiciones, política de privacidad y política de PLD por parte del usuario.
    /// </summary>
    /// <param name="idUsuario">El identificador único del usuario.</param>
    /// <param name="version">La versión de los términos y condiciones aceptados.</param>
    /// <param name="aceptoTerminos">Indica si el usuario aceptó los términos y condiciones.</param>
    /// <param name="aceptoPrivacidad">Indica si el usuario aceptó la política de privacidad.</param>
    /// <param name="aceptoPld">Indica si el usuario aceptó la política de prevención de lavado de dinero (PLD).</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <returns>Un objeto <see cref="Usuario"/> con el estado de aceptación actualizado.</returns>
    Task<Usuario> AceptarTerminosCondicionesAsync(int idUsuario, string version, bool aceptoTerminos,
        bool aceptoPrivacidad, bool aceptoPld, Guid modificationUser);

    /// <summary>
    /// Completa el proceso de registro del usuario estableciendo una contraseña.
    /// </summary>
    /// <param name="idUsuario">El identificador único del usuario.</param>
    /// <param name="contrasena">La contraseña que el usuario desea establecer.</param>
    /// <param name="confirmacionContrasena">La confirmación de la contraseña.</param>
    /// <param name="modificationUser">El GUID del usuario que realiza la modificación.</param>
    /// <returns>Un objeto <see cref="Usuario"/> con la contraseña establecida.</returns>
    Task<Usuario> CompletarRegistroAsync(int idUsuario, string contrasena, string confirmacionContrasena,
        Guid modificationUser);
}
