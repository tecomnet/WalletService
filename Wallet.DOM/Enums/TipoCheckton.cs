namespace Wallet.DOM.Enums;

/// <summary>
/// Define los tipos de "Checkton" utilizados para la validación o categorización de entidades.
/// Estos tipos pueden representar diferentes listas de control o criterios de verificación.
/// </summary>
public enum TipoCheckton
{
    /// <summary>
    /// Indica que la entidad está en una lista negra, lo que generalmente implica restricciones o prohibiciones.
    /// </summary>
    ListaNegra,
    /// <summary>
    /// Indica que la entidad está en una lista restrictiva específica de Estados Unidos (USA),
    /// lo que puede implicar regulaciones o sanciones.
    /// </summary>
    ListaRestrictivaUSA,
    /// <summary>
    /// Representa el tipo de verificación relacionado con la Clave Única de Registro de Población (CURP) en México.
    /// </summary>
    Curp
}