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

public class ClienteFacade(ServiceDbContext context, ITwilioServiceFacade twilioService, IEmpresaFacade empresaFacade, IEstadoFacade estadoFacade) : IClienteFacade
{
    public async Task<Cliente> ObtenerClientePorIdAsync(int idCliente)
    {
        try
        {
            // Obtener cliente
            var cliente = await context.Cliente.Include(x => x.Direccion).
                Include(x => x.DispositivoMovilAutorizados).
                Include(x=>x.Estado).
                Include(x=>x.Empresa).
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
            // Existe pre registro incompleto
            var clienteExiste = await context.Cliente.Include(x=>x.Verificaciones2FA.Where(x=>x.IsActive)).
                FirstOrDefaultAsync(x => x.CodigoPais == codigoPais && x.Telefono == telefono);
            // Existe pero no finalizo la confirmacion, no verificado
            if (clienteExiste != null && clienteExiste.Verificaciones2FA.Any(v => v is { Verificado: false, Tipo: Tipo2FA.Sms or Tipo2FA.Email }))
            {
                // Genera codigo de verificacion y envia por twilio service
                var verificacion2Fa = await GeneraCodigoVerificacion2FASMSyEnviaTwilioServiceAsync(codigoPais: codigoPais, telefono: telefono, creationUser: creationUser, testCase: testCase);
                // El cliente ya existe lo obtenemos
                cliente = clienteExiste;
                // Agrega el codigo de verificacion
                cliente.AgregarVerificacion2FA(verificacion: verificacion2Fa, modificationUser: creationUser);
            }
            else
            {
                // Validamos duplicidad 
                await ValidarDuplicidad(codigoPais: codigoPais, telefono: telefono);
                // Agregar cliente
                await context.AddAsync(cliente);
                // Genera codigo de verificacion y envia por twilio service
                var verificacion2Fa = await GeneraCodigoVerificacion2FASMSyEnviaTwilioServiceAsync(codigoPais: codigoPais, telefono: telefono, creationUser: creationUser, testCase: testCase);
                // Agrega el codigo de verificacion
                cliente.AgregarVerificacion2FA(verificacion: verificacion2Fa, modificationUser: creationUser);
            }
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
            // Confirmacion existosa
            bool confirmado = false;
            // Obtenemos al cliente
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            // Cargar los codigos de verificacion
            await context.Entry(cliente)
                .Collection(c => c.Verificaciones2FA)
                .Query()
                .Where(v => v.Tipo == tipo2FA && v.IsActive)
                .LoadAsync();
            // Resultadod de la verificacion
            VerificacionResult verificacionResult;
            // Es sms
            if (tipo2FA == Tipo2FA.Sms)
                // Llamamos a twilio service
                verificacionResult = await twilioService.ConfirmarVerificacionSMS(codigoPais: cliente.CodigoPais, telefono: cliente.Telefono, codigo: codigoVerificacion);
            else
            {
                if (string.IsNullOrWhiteSpace(cliente.CorreoElectronico))
                {
                    throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.ClienteCorreoElectronicoNoConfigurado,
                    dynamicContent: [cliente.Id],
                    module: this.GetType().Name));
                }
                verificacionResult = await twilioService.ConfirmarVerificacionEmail(correoElectronico: cliente.CorreoElectronico, codigo: codigoVerificacion);
            }
            // Se confrimo ok en twilio
            if (verificacionResult.IsVerified)
            {
                // Confirma la verificacion
                confirmado = cliente.ConfirmarVerificacion2FA(tipo: tipo2FA, codigo: codigoVerificacion, modificationUser: modificationUser);
            }
            // Guardar cambios
            await context.SaveChangesAsync();
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


    public async Task<Cliente> ActualizarClienteDatosPersonalesAsync(
        int idCliente,
        string nombre,
        string primerApellido,
        string segundoApellido,
        string nombreEstado,
        DateOnly fechaNacimiento,
        Genero genero,
        string correoElectronico,
        Guid modificationUser,
        string? testCase = null)
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
            // Cargar los codigos de verificacion
            await context.Entry(cliente)
                .Collection(c => c.Verificaciones2FA)
                .LoadAsync();
            // Validar que ya haya confirmado el código de verificación por SMS
            ValidarConfirmacionCodigoVerificacionSMS2FA(cliente: cliente);
            // TODO EMD: UBICARLO EN LA EMPRESA TECOMNET
            var empresa = await empresaFacade.ObtenerPorNombreAsync("Tecomnet");
            cliente.AgregarEmpresa(empresa: empresa, modificationUser: modificationUser);
            // Agregar estado
            var estado = await estadoFacade.ObtenerEstadoPorNombreAsync(nombreEstado);
            cliente.AgregarEstado(estado: estado, modificationUser: modificationUser);
            // Agregar direccion pre-registro, TODO EMD: PENDIENTE RECIBIR EL PAIS O IMPLEMENTAR EL CATALOGO PAIS
            var preDireccion = await CrearDireccionPreRegistro(pais: "México", estado: nombreEstado, creationUser: modificationUser, testCase: testCase);
            cliente.AgregarDireccion(direccion: preDireccion, creationUser: modificationUser);
            // Generar nuevo codigo de verificacion y envia a twilio service
            var nuevaVerificacion = await GeneraCodigoVerificacion2FAEmailyEnviaTwilioServiceAsync(
                correoElectronico: correoElectronico,
                nombreCliente: cliente.NombreCompleto!,
                nombreEmpresa: cliente.Empresa!.Nombre,
                creationUser: modificationUser,
                testCase: testCase);
            // Se agrega nuevo codigo para luego confrimar
            cliente.AgregarVerificacion2FA(verificacion: nuevaVerificacion, modificationUser: modificationUser);
            // TODO EMD: LLAMAR A API DE CHECKTON PARA VALIDACION RENAPO
            // Actualizar en db
            context.Update(cliente);
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
            // Cargar empresa 
            var empresa = context.Entry(cliente).Reference(x => x.Empresa);
            // Actualizar datos del correo electronico
            cliente.ActualizarCorreoElectronico(correoElectronico: correoElectronico, modificationUser: modificationUser);
            // Se valida la duplicidad, despues de la actualizacion
            await ValidarDuplicidad(correoElectronico: correoElectronico, id: idCliente);
            // Generar nuevo codigo de verificacion y envia a twilio service
            var nuevaVerificacion = await GeneraCodigoVerificacion2FAEmailyEnviaTwilioServiceAsync(
                correoElectronico: correoElectronico,
                nombreCliente: cliente.NombreCompleto!,
                nombreEmpresa: cliente.Empresa!.Nombre,
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
            var nuevaVerificacion = await GeneraCodigoVerificacion2FASMSyEnviaTwilioServiceAsync(
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

    public async Task<Cliente> EliminarClienteAsync(int idCliente, Guid modificationUser)
    {
        try
        {
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            cliente.Deactivate(modificationUser: modificationUser);
            await context.SaveChangesAsync();
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

    public async Task<Cliente> ActivarClienteAsync(int idCliente, Guid modificationUser)
    {
        try
        {
            var cliente = await ObtenerClientePorIdAsync(idCliente: idCliente);
            cliente.Activate(modificationUser: modificationUser);
            await context.SaveChangesAsync();
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

    public async Task<List<Cliente>> ObtenerClientesAsync()
    {
        try
        {
            // Obtener lista de clientes
            var clientes = await context.Cliente.ToListAsync();
            // Retornar lista de clientes
            return clientes;
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
        var clienteExistente = await context.Cliente.FirstOrDefaultAsync(x => x.CodigoPais == codigoPais && x.Telefono == telefono && x.Id != id);
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

    private async Task<Verificacion2FA> GeneraCodigoVerificacion2FASMSyEnviaTwilioServiceAsync(string codigoPais, string telefono, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Llamamos a twilio service
            var verificacion = await twilioService.VerificacionSMS(codigoPais: codigoPais, telefono: telefono);
            // Creamos la verificacion 2fa
            Verificacion2FA verificacion2Fa = new Verificacion2FA(
                twilioSid: verificacion.Sid,//"sid test",
                fechaVencimiento: DateTime.Now.AddMinutes(10),
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

    private async Task<Verificacion2FA> GeneraCodigoVerificacion2FAEmailyEnviaTwilioServiceAsync(
        string correoElectronico, string nombreCliente, string nombreEmpresa, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Llamamos a twilio service
            var verificacion = await twilioService.VerificacionEmail(correoElectronico: correoElectronico, nombreEmpresa: nombreEmpresa, nombreCliente: nombreCliente);
            // Creamos la verificacion 2fa
            Verificacion2FA verificacion2Fa = new Verificacion2FA(
                twilioSid: verificacion.Sid,
                fechaVencimiento: DateTime.Now.AddMinutes(10),
                tipo: Tipo2FA.Email,
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

    private async Task<Direccion> CrearDireccionPreRegistro(string pais, string estado, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Localiza estado 
            var estadoExiste = await estadoFacade.ObtenerEstadoPorNombreAsync(nombre: estado);
            // Crea la direccion
            var direccion = new Direccion(
                pais: pais,
                estado: estadoExiste.Nombre,
                creationUser: creationUser,
                testCase: testCase);
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

    private void ValidarClienteActivo(Cliente cliente)
    {
        if (!cliente.IsActive)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ClienteInactivo,
                dynamicContent: [cliente.NombreCompleto!],
                module: this.GetType().Name));
        }
    }
    /// <summary>
    /// Valida que el cliente tenga confirmado el codigo de verificacion por SMS
    /// </summary>
    /// <param name="cliente"></param>
    /// <exception cref="EMGeneralAggregateException"></exception>

    private void ValidarConfirmacionCodigoVerificacionSMS2FA(Cliente cliente)
    {
        // Obtiene el código de verificación SMS
        var confirmacionSmsCode = cliente.Verificaciones2FA.FirstOrDefault(x => x is { Tipo: Tipo2FA.Sms, Verificado: true });
        // Valida que exista el código de verificación SMS
        if (confirmacionSmsCode is null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.Verificacion2FASMSNoConfirmado,
                dynamicContent: [],
                module: this.GetType().Name));
        }
    }
    /// <summary>
    /// Valida que el cliente tenga confirmado el codigo de verificacion por SMS
    /// </summary>
    /// <param name="codigoPais"></param>
    /// <param name="telefono"></param>
    /// <returns></returns>
    private async Task<bool> ClienteYaVerificoPorSmsAsync(string codigoPais, string telefono)
    {
        var estaVerificado = await context.Cliente
            .AnyAsync(c =>
                c.CodigoPais == codigoPais &&
                c.Telefono == telefono &&
                c.Verificaciones2FA.Any(v => v.Tipo == Tipo2FA.Sms && v.Verificado));

        return estaVerificado;
    }
    #endregion
}
