using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionUsuario;

namespace Wallet.Funcionalidad.Functionality.ConsentimientosUsuarioFacade;

/// <summary>
/// Interfaz para la fachada de gestión de consentimientos de usuario.
/// </summary>
public interface IConsentimientosUsuarioFacade
{
    /// <summary>
    /// Guarda un nuevo consentimiento para un usuario.
    /// </summary>
    /// <param name="idUsuario">El identificador del usuario.</param>
    /// <param name="tipoDocumento">El tipo de documento de consentimiento.</param>
    /// <param name="version">La versión del documento.</param>
    /// <param name="creationUser">El usuario que crea el registro.</param>
    /// <returns>El consentimiento creado.</returns>
    Task<ConsentimientosUsuario> GuardarConsentimientoAsync(int idUsuario, TipoDocumentoConsentimiento tipoDocumento,
        string version, Guid creationUser);

    /// <summary>
    /// Obtiene los últimos consentimientos aceptados por un usuario para todos los tipos de documentos.
    /// </summary>
    /// <param name="idUsuario">El identificador del usuario.</param>
    /// <returns>Una lista con el último consentimiento de cada tipo.</returns>
    Task<List<ConsentimientosUsuario>> ObtenerUltimosConsentimientosAsync(int idUsuario);
}
