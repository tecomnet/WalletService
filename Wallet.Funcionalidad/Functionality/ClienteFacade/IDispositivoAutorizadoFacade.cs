using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IDispositivoAutorizado
{
    /// <summary>
    /// Guarda un dispositivo autorizado para un cliente
    /// </summary>
    /// <param name="idCliente"></param>
    /// <param name="token"></param>
    /// <param name="idDispositivo"></param>
    /// <param name="nombre"></param>
    /// <param name="caracteristicas"></param>
    /// <param name="creationUser"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    public Task<DispositivoMovilAutorizado> GuardarDispositivoAutorizadoAsync(
        int idCliente,
        string token,
        string idDispositivo,
        string nombre,
        string caracteristicas,
        Guid creationUser,
        string? testCase = null);

    /// <summary>
    /// Verifica si un dispositivo está autorizado para un cliente
    /// </summary>
    /// <param name="idCliente"></param>
    /// <param name="idDispositivo"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<bool> EsDispositivoAutorizadoAsync(int idCliente, string idDispositivo, string token);
}
