using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public class DireccionFacade(IClienteFacade clienteFacade, ServiceDbContext context) : IDireccionFacade
{
    public async Task<Direccion> AgregarDireccionClientePreRegistro(int idCliente, string pais, string estado, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Obtiene al cliente
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);
            // Crea la direccion
            var direccion = new Direccion(
                pais: pais,
                estado: estado,
                creationUser: creationUser,
                testCase: testCase);
            // Agrega la direccion
            cliente.AgregarDireccion(direccion: direccion, creationUser: creationUser);
            // Agregamos al contexto
            await context.AddAsync(direccion);
            // Guardamos cambios
            await context.SaveChangesAsync();
            // Return direccion
            return direccion;
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

    public async Task<Direccion> ActualizarDireccionCliente(int idCliente, string codigoPostal, string municipio, string colonia, string calle, string numeroExterior, string numeroInterior, string? referencia, Guid modificationUser, string? testCase = null)
    {
        try
        {
            // Obtiene al cliente
            var cliente = await clienteFacade.ObtenerClientePorIdAsync(idCliente: idCliente);
            // Obtiene la direccion
            var direccion = cliente.Direccion;
            // Validamos que la direccion no sea nula
            if (direccion is null) throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.DireccionNoConfigurada,
                dynamicContent: []));
            // Actualiza la direccion
            direccion.ActualizarDireccion(
                codigoPostal: codigoPostal,
                municipio: municipio,
                colonia: colonia,
                calle: calle,
                numeroExterior: numeroExterior,
                numeroInterior: numeroInterior,
                referencia: referencia,
                modificationUser: modificationUser);
            // Guardamos cambios
            await context.SaveChangesAsync();
            // Retorna direccion
            return direccion;
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