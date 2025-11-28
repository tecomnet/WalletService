using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IDireccionFacade
{
    /// <summary>
    /// Actualiza una direccion para un cliente
    /// </summary>
    /// <param name="idCliente"></param>
    /// <param name="codigoPostal"></param>
    /// <param name="municipio"></param>
    /// <param name="colonia"></param>
    /// <param name="calle"></param>
    /// <param name="numeroExterior"></param>
    /// <param name="numeroInterior"></param>
    /// <param name="referencia"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    public Task<Direccion> ActualizarDireccionCliente(
        int idCliente,
        string codigoPostal,
        string municipio,
        string colonia,
        string calle,
        string numeroExterior,
        string numeroInterior,
        string referencia,
        Guid modificationUser);
}
