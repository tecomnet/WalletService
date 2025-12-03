using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.RegistroFacade;

public interface IRegistroFacade
{
    Task<Usuario> PreRegistroAsync(string codigoPais, string telefono, Guid creationUser);
    Task<bool> ConfirmarNumeroAsync(int idUsuario, string codigo, Guid modificationUser);
    Task<Usuario> CompletarDatosClienteAsync(int idUsuario, string nombre, string apellidoPaterno, string apellidoMaterno, DateOnly fechaNacimiento, Guid modificationUser);
    Task<Usuario> RegistrarCorreoAsync(int idUsuario, string correo, Guid modificationUser);
    Task<bool> VerificarCorreoAsync(int idUsuario, string codigo, Guid modificationUser);
    Task<Usuario> RegistrarDatosBiometricosAsync(int idUsuario, string idDispositivo, string token, Guid modificationUser);
    Task<Usuario> AceptarTerminosCondicionesAsync(int idUsuario, string version, Guid modificationUser);
    Task<Usuario> CompletarRegistroAsync(int idUsuario, string contrasena, string confirmacionContrasena, Guid modificationUser);
}
