namespace Wallet.DOM.Enums;

/// <summary>
/// Define los estados del proceso de registro de usuario.
/// </summary>
public enum EstatusRegistroEnum
{
    /// <summary>
    /// Estado cuando el usuario ingresa su numero de teléfono.
    /// </summary>
    PreRegistro = 1,

    /// <summary>
    /// Estado cuando el usuario confirma su número de teléfono con el código recibido.
    /// </summary>
    NumeroConfirmado = 2,

    /// <summary>
    /// Estado cuando el cliente rellenó sus datos básicos como nombre, fecha de nacimiento.
    /// </summary>
    DatosClienteCompletado = 3,

    /// <summary>
    /// Estado cuando el cliente ingresa su correo y está en espera de confirmación.
    /// </summary>
    CorreoRegistrado = 4,

    /// <summary>
    /// Estado cuando el cliente ha verificado su correo con el código que recibió.
    /// </summary>
    CorreoVerificado = 5,

    /// <summary>
    /// Estado cuando el cliente ha generado un dispositivo autorizado.
    /// </summary>
    DatosBiometricosRegistrado = 6,

    /// <summary>
    /// Estado cuando el cliente ha aceptado todos los términos y condiciones.
    /// </summary>
    TerminosCondicionesAceptado = 7,

    /// <summary>
    /// Este estado se registra cuando el cliente ha establecido su contraseña del sistema.
    /// </summary>
    RegistroCompletado = 8
}
