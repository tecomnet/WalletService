namespace Wallet.DOM.Enums;

/// <summary>
/// Proporciona una enumeración para el estado logístico de entrega de una tarjeta física.
/// </summary>
public enum EstadoEntrega
{
    /// <summary>
    /// La tarjeta ha sido solicitada pero no procesada.
    /// </summary>
    Solicitada = 1,

    /// <summary>
    /// La tarjeta está en proceso de producción/impresión.
    /// </summary>
    EnProduccion = 2,

    /// <summary>
    /// La tarjeta ha sido enviada a la paquetería.
    /// </summary>
    Enviada = 3,

    /// <summary>
    /// La tarjeta ha sido entregada al usuario.
    /// </summary>
    Entregada = 4,

    /// <summary>
    /// La tarjeta fue devuelta por fallo en la entrega.
    /// </summary>
    Devuelta = 5
}
