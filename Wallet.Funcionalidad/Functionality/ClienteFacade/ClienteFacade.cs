using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.Helper;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagemet;
using Wallet.Funcionalidad.ServiceClient;

namespace Wallet.Funcionalidad.Functionality.ClienteFacade;

public class ClienteFacade(ServiceDbContext context, ITwilioServiceFacade twilioService, IEmpresaFacade empresaFacade) : IClienteFacade
{
    public async Task<Cliente> ObtenerClientePorIdAsync(int idCliente)
    {
        try
        {
            // Obtener cliente
            var cliente = await context.Cliente.Include(x=>x.Direccion).
                Include(x=>x.DispositivoMovilAutorizados).
                FirstOrDefaultAsync(x => x.Id == idCliente);
            // Validar cliente
            if (cliente == null)
            {
                throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ClienteNoEncontrado,
                    dynamicContent: [idCliente],
                    module: this.GetType().Name));
            }
            // Retornar cliente
            return cliente;
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
    public async Task<Cliente> GuardarClientePreRegistroAsync(string codigoPais, string telefono, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Crea un nuevo cliente
            var cliente = new Cliente(codigoPais, telefono, creationUser, testCase);
            // Validamos duplicidad 
            await ValidarDuplicidad(codigoPais: codigoPais, telefono: telefono);
            // Agregar cliente
            await context.AddAsync(cliente);
            // Genera codigo de verificacion
            var verificacion2Fa = await GeneraCodigoVerificacion2FASMSsync(codigoPais: codigoPais, telefono: telefono, creationUser: creationUser, testCase: testCase);
            // Agrega el codigo de verificacion
            cliente.AgregarVerificacion2FA(verificacion:verificacion2Fa, modificationUser: creationUser);
            // Guardar cambios
            await context.SaveChangesAsync();
            // Retornar cliente
            return cliente;
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
    public async Task<bool> ConfirmarCodigoVerificacion2FAAsync(int idCliente, Tipo2FA tipo2FA, string codigoVerificacion, Guid modificationUser)
    {
        try
        {
            // Obtenemos al cliente
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            // Cargar los codigos de verificacion
            await context.Entry(cliente)
                .Collection(c => c.Verificaciones2FA) 
                .LoadAsync();
            // Confirma la verificacion
            var confirmado = cliente.ConfirmarVerificacion2FA(tipo: tipo2FA, codigo: codigoVerificacion, modificationUser: modificationUser);
            // Return confrimacion
            return confirmado;
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

  
    public async Task<Cliente> ActualizarClienteDatosPersonalesAsync(int idCliente, string nombre, string primerApellido, string segundoApellido, DateOnly fechaNacimiento, Genero genero, string correoElectronico, Guid modificationUser)
    {
        try
        {
            // Obtener cliente
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            // Actualizar datos personales
            cliente.AgregarDatosPersonales(
                nombre: nombre, 
                primerApellido: primerApellido,
                segundoApellido: segundoApellido,
                fechaNacimiento: fechaNacimiento,
                genero: genero,
                correoElectronico: correoElectronico,
                modificationUser: modificationUser);
            // Se valida la duplicidad, despues de la actualizacion
            await ValidarDuplicidad(correoElectronico: correoElectronico, id: idCliente);
            // TODO EMD: UBICARLO EN LA EMPRESA TECOMNET
            var empresa = await empresaFacade.ObtenerPorNombreAsync("TECOMNET");
            // Actualizar en db
            context.Update(cliente);
            // TODO EMD: LLAMAR A API DE Tilwio PARA EL PROCESO DE VALIDACION 2FA
            // Guardar cambios
            await context.SaveChangesAsync();
            // Retornar cliente
            return cliente;
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

    public async Task<Cliente> GuardarContrasenaAsync(int idCliente, string contrasena, Guid modificationUser)
    {
        try
        {
            // Obtener cliente
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            // Agregar cliente
            cliente.CrearContrasena(contrasena: contrasena, modificationUser: modificationUser);
            // Guardar cambios
            await context.SaveChangesAsync();
            // Retornar cliente
            return cliente;
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

    public async Task<Cliente> ActualizarContrasenaAsync(int idCliente, string contrasenaActual, string contrasenaNueva, string confirmacionContrasenaNueva, Guid modificationUser)
    {
        try
        {
            // Obtener el cliente 
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            // Actualizar contrasenas
            cliente.ActualizarContrasena(
                contrasenaActual: contrasenaActual, 
                contrasenaNueva: contrasenaNueva,
                confirmacionContrasenaNueva: confirmacionContrasenaNueva, 
                modificationUser: modificationUser);
            // Guardar cambios
            await context.SaveChangesAsync();
            // Retornar cliente
            return cliente;
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

    public async Task<Cliente> ActualizarCorreoElectronicoAsync(int idCliente, string correoElectronico, Guid modificationUser, string? testCase = null)
    {
        try
        {
            // Obtener cliente
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            // Actualizar datos del correo electronico
            cliente.ActualizarCorreoElectronico(correoElectronico: correoElectronico, modificationUser: modificationUser);
            // Se valida la duplicidad, despues de la actualizacion
            await ValidarDuplicidad(correoElectronico: correoElectronico, id: idCliente);
            // TODO EMD: LLAMAR A API DE Tilwio PARA OTRO PROCESO DE VALIDACION 2FA
            // Guardar cambios
            await context.SaveChangesAsync();
            // Retornar cliente
            return cliente;
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

    public async Task<Cliente> ActualizarTelefonoAsync(int idCliente, string codigoPais, string telefono, Guid modificationUser, string? testCase = null)
    {
        try
        {
            // Obtener cliente
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            // Actualizar datos del telefono
            cliente.ActualizarTelefono(codigoPais: codigoPais, telefono: telefono, modificationUser: modificationUser);
            // Se valida la duplicidad, despues de la actualizacion
            await ValidarDuplicidad(codigoPais: codigoPais, telefono: telefono, id: idCliente);
            // Generar nuevo codigo de verificacion 
            var nuevaVerificacion = await GeneraCodigoVerificacion2FASMSsync(
                codigoPais: codigoPais,
                telefono: telefono,
                creationUser: modificationUser,
                testCase: testCase);
            // Se agrega nuevo codigo para luego confrimar
            cliente.AgregarVerificacion2FA(verificacion: nuevaVerificacion, modificationUser: modificationUser);
            // Guardar cambios
            await context.SaveChangesAsync();
            // Retornar cliente
            return cliente;
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


   
  


    #region Metodos privados

    private async Task ValidarDuplicidad(string codigoPais, string telefono, int id = 0)
    {
        // Obtiene cliente existente
        var clienteExistente = await context.Cliente.FirstOrDefaultAsync(
            x => x.CodigoPais == codigoPais && x.Telefono == telefono && x.Id != id);
        
        // Duplicado por telefono
        if (clienteExistente != null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ClienteDuplicado,
                dynamicContent: [codigoPais, telefono],
                module: this.GetType().Name));
        }
    }
    private async Task ValidarDuplicidad(string correoElectronico, int id = 0)
    {
        // Obtiene cliente existente
        var clienteExistente = await context.Cliente.FirstOrDefaultAsync(
            x => x.CorreoElectronico == correoElectronico && x.Id != id);
        // Duplicado por correo electronico
        if (clienteExistente != null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ClienteDuplicadoPorCorreoElectronico,
                dynamicContent: [correoElectronico!],
                module: this.GetType().Name));
        }
    }

    private async Task<Verificacion2FA> GeneraCodigoVerificacion2FASMSsync(string codigoPais, string telefono, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Llamamos a twilio service
            var verificacion = await twilioService.VerificacionSMS(codigoPais: codigoPais, telefono: telefono);
            // Creamos la verificacion 2fa
            Verificacion2FA verificacion2Fa = new Verificacion2FA(
                twilioSid: verificacion.Sid,
                fechaVencimiento: DateTime.UtcNow.AddMinutes(10),
                tipo: Tipo2FA.Sms,
                creationUser: creationUser,
                testCase: testCase
            );
            // Retorna codigo de verificacion
            return verificacion2Fa;
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
    #endregion
}
