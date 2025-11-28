using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IClienteFacade
{
    /// <summary>
    /// Obtiene el cliente por su Id
    /// </summary>
    /// <param name="idCliente"></param>
    /// <returns></returns>
    public Task<Cliente> ObtenerClientePorIdAsync(int idCliente);


    /// <summary>
    /// Actualiza los datos personales del cliente
    /// </summary>
    /// <param name="idCliente"></param>
    /// <param name="nombre"></param>
    /// <param name="primerApellido"></param>
    /// <param name="segundoApellido"></param>
    /// <param name="fechaNacimiento"></param>
    /// <param name="genero"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    public Task<Cliente> ActualizarClienteDatosPersonalesAsync(int idCliente, string nombre, string primerApellido,
        string segundoApellido, string nombreEstado, DateOnly fechaNacimiento, Genero genero, Guid modificationUser,
        string? testCase = null);


    /// <summary>
    /// Elimina un cliente por su Id
    /// </summary>
    /// <param name="idCliente"></param>
    /// <returns></returns>
    public Task<Cliente> EliminarClienteAsync(int idCliente, Guid modificationUser);

    /// <summary>
    /// Activa un cliente por su Id
    /// </summary>
    /// <param name="idCliente"></param>
    /// <returns></returns>
    public Task<Cliente> ActivarClienteAsync(int idCliente, Guid modificationUser);

    /// <summary>
    /// Obtiene la lista de todos los clientes
    /// </summary>
    /// <returns></returns>
    public Task<List<Cliente>> ObtenerClientesAsync();
}
    
