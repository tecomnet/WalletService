using System;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality;

public interface IUbicacionGeolocalizacionFacade
{
    /// <summary>
    /// Guarda la ubicación geolocalización de un cliente
    /// </summary>
    /// <param name="idCliente"></param>
    /// <param name="latitud"></param>
    /// <param name="longitud"></param>
    /// <param name="dispositivo"></param>
    /// <param name="tipoEvento"></param>
    /// <param name="tipoDispositivo"></param>
    /// <param name="agente"></param>
    /// <param name="direccionIp"></param>
    /// <param name="creationUser"></param>
    /// <param name="testCase"></param>
    /// <returns></returns>
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
