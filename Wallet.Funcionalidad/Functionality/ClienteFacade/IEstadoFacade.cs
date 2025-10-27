using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IEstadoFacade
{
    public Task<Estado> ObtenerEstado(string nombre);
    public Task<List<Estado>> ObtenerEstados(bool? activo = null);
}