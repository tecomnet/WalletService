using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IEmpresaFacade
{
    public Task<Empresa> ObtenerPorIdAsync(int idEmpresa);
    public Task<Empresa> ObtenerPorNombreAsync(string nombre);
    public Task<Empresa> GuardarEmpresaAsync(string nombre, Guid creationUser, string? testCase = null);
    public Task<Empresa> ActualizaEmpresaAsync(int idEmpresa, Guid modificationUser);
    public Task<Empresa> EliminaEmpresaAsync(int idEmpresa, Guid modificationUser);
    public Task<Empresa> ActivaEmpresaAsync(int idEmpresa, Guid modificationUser);
}