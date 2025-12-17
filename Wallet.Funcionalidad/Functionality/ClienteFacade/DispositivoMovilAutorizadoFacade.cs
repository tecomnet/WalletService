using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionUsuario;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

/// <summary>
/// Fachada para gestionar las operaciones relacionadas con los dispositivos móviles autorizados de un cliente.
/// </summary>
public class DispositivoMovilAutorizadoFacadeFacade(ServiceDbContext context, IClienteFacade clienteFacade)
    : IDispositivoMovilAutorizadoFacade
{
    /// <summary>
    /// Guarda un nuevo dispositivo móvil autorizado para un cliente.
    /// </summary>
    /// <param name="idCliente">El ID del cliente al que se le agregará el dispositivo móvil autorizado.</param>
    /// <param name="token">El token de autenticación del dispositivo móvil autorizado.</param>
    /// <param name="idDispositivo">El ID único del dispositivo móvil autorizado.</param>
    /// <param name="nombre">El nombre del dispositivo móvil autorizado.</param>
    /// <param name="caracteristicas">Características del dispositivo móvil autorizado.</param>
    /// <param name="creationUser">El ID del usuario que creó el dispositivo móvil autorizado.</param>
    /// <param name="testCase">El caso de prueba asociado al dispositivo móvil autorizado (opcional).</param>
    /// <returns>El dispositivo móvil autorizado recién creado y guardado.</returns>
    /// <exception cref="EMGeneralAggregateException">Excepción general que se lanza si ocurre un error durante la operación.</exception>
    public async Task<DispositivoMovilAutorizado> GuardarDispositivoAutorizadoAsync(
        int idCliente,
        string token,
        string idDispositivo,
        string nombre,
        string caracteristicas,
        Guid creationUser,
        string? testCase = null)
    {
        try
        {
            // Obtenemos el cliente por su ID.
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);
            // Creamos una nueva instancia de DispositivoMovilAutorizado con los datos proporcionados.
            var dispositivo = new DispositivoMovilAutorizado(
                token: token,
                idDispositivo: idDispositivo,
                nombre: nombre,
                caracteristicas: caracteristicas,
                creationUser: creationUser,
                testCase: testCase);
            // Agregamos el dispositivo móvil autorizado al usuario del cliente.
            cliente.Usuario.AgregarDispositivoMovilAutorizado(dispositivo: dispositivo, modificationUser: creationUser);
            // Guardamos los cambios en la base de datos de forma asíncrona.
            await context.SaveChangesAsync();
            // Retornamos el dispositivo móvil autorizado recién creado y guardado.
            return dispositivo;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // En caso de una excepción no controlada previamente, la encapsulamos en una EMGeneralAggregateException
            // y la lanzamos para una gestión centralizada de errores.
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <summary>
    /// Verifica si un dispositivo móvil específico está autorizado para un cliente dado.
    /// </summary>
    /// <param name="idCliente">El ID del cliente.</param>
    /// <param name="idDispositivo">El ID único del dispositivo.</param>
    /// <param name="token">El token de autorización del dispositivo.</param>
    /// <returns>
    /// Una tarea que representa la operación asíncrona. El resultado de la tarea es <c>true</c>
    /// si el dispositivo está autorizado; de lo contrario, <c>false</c>.
    /// </returns>
    public async Task<bool> EsDispositivoAutorizadoAsync(int idCliente, string idDispositivo, string token)
    {
        try
        {
            // Obtenemos el cliente por su ID.
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);
            // Verificamos si el dispositivo está autorizado para el usuario del cliente.
            return cliente.Usuario.EsDispositivoAutorizado(idDispositivo: idDispositivo, token: token);
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // En caso de una excepción no controlada previamente, la encapsulamos en una EMGeneralAggregateException
            // y la lanzamos para una gestión centralizada de errores.
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }
}