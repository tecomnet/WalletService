using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionUsuario;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

/// <summary>
/// Fachada para gestionar las operaciones relacionadas con la ubicación geolocalizada de los clientes.
/// </summary>
public class UbicacionGeolocalizacionFacade(IClienteFacade clienteFacade, ServiceDbContext context)
    : IUbicacionGeolocalizacionFacade
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UbicacionGeolocalizacionFacade"/>.
    /// </summary>
    /// <param name="clienteFacade">La fachada de cliente para obtener información del cliente.</param>
    /// <param name="context">El contexto de la base de datos para interactuar con los datos.</param>
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
            // Obtiene al cliente por su ID.
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);
            
            // Crea una nueva instancia de UbicacionesGeolocalizacion con los datos proporcionados.
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
            
            // Agrega la ubicación geolocalizada al usuario asociado al cliente.
            cliente.Usuario.AgregarUbicacionGeolocalizacion(ubicacion: ubicacionGeolocalizacion,
                modificationUser: creationUser);
            
            // Guarda los cambios en la base de datos de forma asíncrona.
            await context.SaveChangesAsync();
            
            // Retorna la ubicación geolocalizada recién creada.
            return ubicacionGeolocalizacion;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Captura cualquier excepción que no sea una EMGeneralAggregateException y la envuelve en una nueva excepción agregada.
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }
}