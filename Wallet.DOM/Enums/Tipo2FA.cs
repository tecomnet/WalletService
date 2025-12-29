namespace Wallet.DOM.Enums;

/// <summary>
/// Define los tipos de autenticación de dos factores (2FA) disponibles.
/// </summary>
public enum Tipo2FA
{
    /// <summary>
    /// Autenticación de dos factores a través de SMS.
    /// </summary>
    Sms = 0,
    /// <summary>
    /// Autenticación de dos factores a través de correo electrónico.
    /// </summary>
    Email = 1,
}