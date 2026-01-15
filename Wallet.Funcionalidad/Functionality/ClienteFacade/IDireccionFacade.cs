using Wallet.DOM.Modelos.GestionCliente;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

/// <summary>
/// Define la interfaz para las operaciones de fachada relacionadas con la gestión de direcciones de clientes.
/// </summary>
public interface IDireccionFacade
{
    /// <summary>
    /// Actualiza o crea una dirección para un cliente específico.
    /// </summary>
    /// <param name="idCliente">Identificador único del cliente.</param>
    /// <param name="codigoPostal">El código postal de la dirección.</param>
    /// <param name="municipio">El municipio o delegación de la dirección.</param>
    /// <param name="colonia">La colonia de la dirección.</param>
    /// <param name="calle">La calle de la dirección.</param>
    /// <param name="numeroExterior">El número exterior del domicilio.</param>
    /// <param name="numeroInterior">El número interior del domicilio (opcional).</param>
    /// <param name="referencia">Una referencia adicional para ubicar la dirección.</param>
    /// <param name="modificationUser">Identificador del usuario que realiza la modificación.</param>
    /// <returns>Una tarea que representa la operación asíncrona, cuyo resultado es la entidad <see cref="Direccion"/> actualizada.</returns>
    public Task<Direccion> ActualizarDireccionCliente(
        int idCliente,
        string codigoPostal,
        string municipio,
        string colonia,
        string calle,
        string numeroExterior,
        string numeroInterior,
        string referencia,
        string? concurrencyToken,
        Guid modificationUser);

    /// <summary>
    /// Obtiene la dirección de un cliente específico.
    /// </summary>
    /// <param name="idCliente">Identificador único del cliente.</param>
    /// <returns>Una tarea que representa la operación asíncrona, cuyo resultado es la entidad <see cref="Direccion"/> encontrada.</returns>
    public Task<Direccion> ObtenerDireccionPorClienteIdAsync(int idCliente);
}
