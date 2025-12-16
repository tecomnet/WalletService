using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

/// <summary>
/// Fachada para la gestión de direcciones de clientes.
/// Proporciona una interfaz simplificada para interactuar con la lógica de negocio de direcciones.
/// </summary>
public class DireccionFacade(IClienteFacade clienteFacade, ServiceDbContext context) : IDireccionFacade
{
    /// <summary>
    /// Actualiza la dirección de un cliente existente o la agrega si no tiene una configurada.
    /// </summary>
    /// <param name="idCliente">Identificador único del cliente cuya dirección se va a actualizar.</param>
    /// <param name="codigoPostal">Nuevo código postal de la dirección.</param>
    /// <param name="municipio">Nuevo municipio de la dirección.</param>
    /// <param name="colonia">Nueva colonia de la dirección.</param>
    /// <param name="calle">Nueva calle de la dirección.</param>
    /// <param name="numeroExterior">Nuevo número exterior de la dirección.</param>
    /// <param name="numeroInterior">Nuevo número interior de la dirección (opcional).</param>
    /// <param name="referencia">Nueva referencia o descripción adicional de la dirección (opcional).</param>
    /// <param name="modificationUser">Identificador del usuario que realiza la modificación.</param>
    /// <returns>La entidad <see cref="Direccion"/> actualizada.</returns>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el cliente no existe, la dirección no está configurada o si ocurre un error durante la actualización.</exception>
    public async Task<Direccion> ActualizarDireccionCliente(int idCliente, string codigoPostal, string municipio,
        string colonia, string calle, string numeroExterior, string numeroInterior, string referencia,
        string? concurrencyToken, Guid modificationUser)
    {
        try
        {
            // Obtiene al cliente por su ID.
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);

            // Obtiene la dirección asociada al cliente.
            var direccion = cliente.Direccion;

            // Validamos que la dirección no sea nula. Si es nula, significa que el cliente no tiene una dirección configurada.
            if (direccion is null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.DireccionNoConfigurada,
                    dynamicContent: []));
            }

            // Manejo de ConcurrencyToken
            if (!string.IsNullOrEmpty(concurrencyToken))
            {
                context.Entry(direccion).Property(x => x.ConcurrencyToken).OriginalValue =
                    Convert.FromBase64String(concurrencyToken);
            }

            // Actualiza los datos de la dirección con los valores proporcionados.
            direccion.ActualizarDireccion(
                codigoPostal: codigoPostal,
                municipio: municipio,
                colonia: colonia,
                calle: calle,
                numeroExterior: numeroExterior,
                numeroInterior: numeroInterior,
                referencia: referencia,
                modificationUser: modificationUser);

            // Guarda los cambios realizados en la base de datos.
            await context.SaveChangesAsync();

            // Retorna la entidad de dirección actualizada.
            return direccion;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ConcurrencyError,
                dynamicContent: []));
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            // Captura cualquier excepción no controlada y la envuelve en una EMGeneralAggregateException.
            // Esto asegura que todas las excepciones de servicio sean del tipo esperado.
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }


    /// <inheritdoc />
    public async Task<Direccion> ObtenerDireccionPorClienteIdAsync(int idCliente)
    {
        try
        {
            // Obtiene al cliente por su ID.
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);

            // Obtiene la dirección asociada al cliente.
            var direccion = cliente.Direccion;

            // Validamos que la dirección no sea nula. Si es nula, significa que el cliente no tiene una dirección configurada.
            if (direccion is null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.DireccionNoConfigurada,
                    dynamicContent: []));
            }

            return direccion;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }
}