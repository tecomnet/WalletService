using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public interface IClienteFacade
{
    /// <summary>
    /// Guarda un nuevo cliente, es un preregistro por codigo de país y teléfono
    /// </summary>
    /// <param name="codigoPais"></param>
    /// <param name="telefono"></param>
    /// <param name="creationUser"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
    public Task<Cliente> GuardarClientePreRegistroAsync(string codigoPais, string telefono, Guid creationUser, string? testCase = null);
    /// <summary>
    /// Obtiene el cliente por su Id
    /// </summary>
    /// <param name="idCliente"></param>
    /// <returns></returns>
    public Task<Cliente> ObtenerClientePorIdAsync(int idCliente);
    
    /// <summary>
    /// Confirma el código 2FA enviado al cliente por sms o email
    /// </summary>
    /// <param name="idCliente"></param>
    /// <param name="tipo2FA"></param>
    /// <param name="codigoVerificacion"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    public Task<bool> ConfirmarCodigoVerificacion2FAAsync(int idCliente, Tipo2FA tipo2FA, string codigoVerificacion, Guid modificationUser);
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
    public Task<Cliente> ActualizarClienteDatosPersonalesAsync(int idCliente, string nombre, string primerApellido, string segundoApellido, string nombreEstado, DateOnly fechaNacimiento, Genero genero, Guid modificationUser, string? testCase = null);
    
    
    /// <summary>
    /// Guarda la contraseña del cliente
    /// </summary>
    /// <param name="idCliente"></param>
    /// <param name="contrasena"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    public Task<Cliente> GuardarContrasenaAsync(int idCliente, string contrasena, Guid modificationUser);

    /// <summary>
    /// Actualiza la contraseña del cliente
    /// </summary>
    /// <param name="idCliente"></param>
    /// <param name="contrasenaActual"></param>
    /// <param name="contrasenaNueva"></param>
    /// <param name="confirmacionContrasenaNueva"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    public Task<Cliente> ActualizarContrasenaAsync(int idCliente, string contrasenaActual, string contrasenaNueva, string confirmacionContrasenaNueva, Guid modificationUser);

    /// <summary>
    /// Actualiza el teléfono del cliente, este generara nueva verificación 2FA
    /// </summary>
    /// <param name="idCliente"></param>
    /// <param name="codigoPais"></param>
    /// <param name="telefono"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    public Task<Cliente> ActualizarTelefonoAsync(int idCliente, string codigoPais, string telefono, Guid modificationUser, string? testCase = null);
    /// <summary>
    /// Actualiza el correo electrónico del cliente, este generara nueva verificación 2FA
    /// </summary>
    /// <param name="idCliente"></param>
    /// <param name="correoElectronico"></param>
    /// <param name="modificationUser"></param>
    /// <returns></returns>
    public Task<Cliente> ActualizarCorreoElectronicoAsync(int idCliente, string correoElectronico, Guid modificationUser, string? testCase = null);

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
    
