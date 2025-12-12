using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IEstadoFacade
{
    public Task<Estado> ObtenerEstadoPorIdAsync(int idEstado);
    public Task<Estado> ObtenerEstadoPorNombreAsync(string nombre);
    public Task<List<Estado>> ObtenerTodosAsync(bool? activo = null);
    public Task<Estado> GuardarEstadoAsync(string nombre, Guid creationUser, string? testCase = null);
    public Task<Estado> ActualizaEstadoAsync(int idEstado, string nombre, Guid modificationUser);
    public Task<Estado> EliminaEstadoAsync(int idEstado, Guid modificationUser);
    public Task<Estado> ActivaEstadoAsync(int idEstado, Guid modificationUser);
    
    
}