using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public class UbicacionGeolocalizacionFacade(IClienteFacade clienteFacade, ServiceDbContext context)
    : IUbicacionGeolocalizacionFacade
{
    public async Task<UbicacionesGeolocalizacion> GuardarUbicacionGeolocalizacionAsync(
        int idCliente,
        decimal latitud,
        decimal longitud,
        Dispositivo dispositivo,
        string tipoEvento,
        string tipoDispositivo,
        string agente,
        string direccionIp,
        Guid creationUser,
        string? testCase = null)
    {
        try
        {
            // Obtiene al cliente
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);
            // Crea nueva ubicacion
            var ubicacionGeolocalizacion = new UbicacionesGeolocalizacion(
                latitud: latitud,
                longitud: longitud,
                dispositivo: dispositivo,
                tipoEvento: tipoEvento,
                tipoDispositivo: tipoDispositivo,
                agente: agente,
                direccionIp: direccionIp,
                creationUser: creationUser,
                testCase: testCase);
            // Agrega ubicacion
            cliente.Usuario.AgregarUbicacionGeolocalizacion(ubicacion: ubicacionGeolocalizacion,
                modificationUser: creationUser);
            // Guarda cambios
            await context.SaveChangesAsync();
            // Retorna ubicacion
            return ubicacionGeolocalizacion;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Throw an aggregate exception
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }
}