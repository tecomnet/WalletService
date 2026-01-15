namespace Wallet.DOM.Enums;

/// <summary>
/// Proporciona una enumeración para los estados posibles de una tarjeta.
/// </summary>
public enum EstadoTarjeta
{
    /// <summary>
    /// La tarjeta está activa y operativa.
    /// </summary>
    Activa = 1,

    /// <summary>
    /// La tarjeta está inactiva (ej. aún no activada por el usuario).
    /// </summary>
    Inactiva = 2,

    /// <summary>
    /// La tarjeta está bloqueada temporalmente por el usuario.
    /// </summary>
    BloqueadaTemporalmente = 3,

    /// <summary>
    /// La tarjeta ha sido cancelada por robo.
    /// </summary>
    CanceladaPorRobo = 4,

    /// <summary>
    /// La tarjeta ha sido cancelada por extravío.
    /// </summary>
    CanceladaPorExtravio = 5,

    /// <summary>
    /// La tarjeta ha sido cancelada por el banco o sistema.
    /// </summary>
    CanceladaPorSistema = 6,

    /// <summary>
    /// La tarjeta ha expirado por fecha de vencimiento.
    /// </summary>
    Expirada = 7
}
