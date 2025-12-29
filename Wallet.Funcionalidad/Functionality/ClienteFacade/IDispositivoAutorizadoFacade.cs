using Wallet.DOM.Modelos.GestionUsuario;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

/// <summary>
/// Define la interfaz para la fachada de gestión de dispositivos móviles autorizados.
/// </summary>
public interface IDispositivoMovilAutorizadoFacade
{
    /// <summary>
    /// Guarda un nuevo dispositivo móvil como autorizado para un cliente específico.
    /// </summary>
    /// <param name="idCliente">Identificador único del cliente.</param>
    /// <param name="token">Token de autorización asociado al dispositivo.</param>
    /// <param name="idDispositivo">Identificador único del dispositivo.</param>
    /// <param name="nombre">Nombre descriptivo del dispositivo.</param>
    /// <param name="caracteristicas">Características o descripción adicional del dispositivo.</param>
    /// <param name="creationUser">Identificador del usuario que realiza la creación.</param>
    /// <param name="testCase">Opcional. Caso de prueba para escenarios específicos.</param>
    /// <returns>Una tarea que representa la operación asíncrona, devolviendo el <see cref="DispositivoMovilAutorizado"/> guardado.</returns>
    public Task<DispositivoMovilAutorizado> GuardarDispositivoAutorizadoAsync(
        int idCliente,
        string token,
        string idDispositivo,
        string nombre,
        string caracteristicas,
        Guid creationUser,
        string? testCase = null);

    /// <summary>
    /// Verifica si un dispositivo móvil específico está autorizado para un cliente.
    /// </summary>
    /// <param name="idCliente">Identificador único del cliente.</param>
    /// <param name="idDispositivo">Identificador único del dispositivo a verificar.</param>
    /// <param name="token">Token de autorización asociado al dispositivo.</param>
    /// <returns>Una tarea que representa la operación asíncrona, devolviendo <c>true</c> si el dispositivo está autorizado; de lo contrario, <c>false</c>.</returns>
    public Task<bool> EsDispositivoAutorizadoAsync(int idCliente, string idDispositivo, string token);
}
