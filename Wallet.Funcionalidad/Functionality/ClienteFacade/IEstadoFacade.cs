using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IEstadoFacade
{
    public Task<Estado> ObtenerEstadoPorId(int idEstado);
    public Task<Estado> ObtenerEstadoPorNombre(string nombre);
    public Task<List<Estado>> ObtenerTodos(bool? activo = null);
    public Task<Estado> GuardarEstado(string nombre, Guid creationUser, string? testCase = null);
    public Task<Estado> ActualizaEstado(int idEstado, Guid modificationUser);
    public Task<Estado> EliminaEstado(int idEstado, Guid modificationUser);
    public Task<Estado> ActivaEstado(int idEstado, Guid modificationUser);
    
    
}