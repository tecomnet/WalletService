using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.UsuarioFacade;

public interface IUsuarioFacade
{
    Task<Usuario> ObtenerUsuarioPorIdAsync(int idUsuario);
    
    Task<Usuario> GuardarContrasenaAsync(int idUsuario, string contrasena, Guid modificationUser);
    
    Task<Usuario> ActualizarContrasenaAsync(int idUsuario, string contrasenaActual, string contrasenaNueva,
        string confirmacionContrasenaNueva, Guid modificationUser);
    
    Task<Usuario> ActualizarCorreoElectronicoAsync(int idUsuario, string correoElectronico,
        Guid modificationUser, string? testCase = null);
    
    Task<Usuario> ActualizarTelefonoAsync(int idUsuario, string codigoPais, string telefono,
        Guid modificationUser, string? testCase = null);
        
    Task<bool> ConfirmarCodigoVerificacion2FAAsync(int idUsuario, Tipo2FA tipo2FA,
        string codigoVerificacion, Guid modificationUser);
}
