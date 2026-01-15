using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionUsuario;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

/// <summary>
/// Define la interfaz para la fachada de geolocalización de clientes.
/// </summary>
public interface IUbicacionGeolocalizacionFacade
{
    /// <summary>
    /// Guarda la ubicación geolocalización de un cliente de forma asíncrona.
    /// </summary>
    /// <param name="idCliente">El identificador único del cliente.</param>
    /// <param name="latitud">La latitud de la ubicación geográfica.</param>
    /// <param name="longitud">La longitud de la ubicación geográfica.</param>
    /// <param name="dispositivo">El tipo de dispositivo desde el cual se registra la ubicación.</param>
    /// <param name="tipoEvento">El tipo de evento que generó el registro de la ubicación (ej. "Login", "Transaccion").</param>
    /// <param name="tipoDispositivo">Una descripción del tipo de dispositivo (ej. "Mobile", "Web").</param>
    /// <param name="agente">El agente de usuario (user-agent) del navegador o aplicación.</param>
    /// <param name="direccionIp">La dirección IP desde la cual se realizó la acción.</param>
    /// <param name="creationUser">El identificador único del usuario que realiza la creación del registro.</param>
    /// <param name="testCase">Opcional. Un identificador de caso de prueba para propósitos de desarrollo o testing.</param>
    /// <returns>Un objeto <see cref="UbicacionesGeolocalizacion"/> que representa la ubicación guardada.</returns>
    public Task<UbicacionesGeolocalizacion> GuardarUbicacionGeolocalizacionAsync(
        int idCliente,
        decimal latitud,
        decimal longitud,
        Dispositivo dispositivo,
        string tipoEvento,
        string tipoDispositivo,
        string agente,
        string direccionIp,
        Guid creationUser,
        string? testCase = null);
}
