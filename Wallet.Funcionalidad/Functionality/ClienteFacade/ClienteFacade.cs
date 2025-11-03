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
            // Genera codigo de verificacion y envia por twilio service
            var verificacion2Fa = await GeneraCodigoVerificacion2FASMSyEnviaTwilioServiceAsync(codigoPais: codigoPais, telefono: telefono, creationUser: creationUser, testCase: testCase);
            // Agrega el codigo de verificacion
            cliente.AgregarVerificacion2FA(verificacion: verificacion2Fa, modificationUser: creationUser);
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
            // Validar que ya haya confirmado el código de verificación por SMS
            await ValidarConfirmacionCodigoVerificacionSMS2FA(cliente: cliente);
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

    private async Task<Verificacion2FA> GeneraCodigoVerificacion2FASMSyEnviaTwilioServiceAsync(string codigoPais, string telefono, Guid creationUser, string? testCase = null)
    {
        try
        {
            // Llamamos a twilio service
            //var verificacion = await twilioService.VerificacionSMS(codigoPais: codigoPais, telefono: telefono);
            // Creamos la verificacion 2fa
            Verificacion2FA verificacion2Fa = new Verificacion2FA(
                twilioSid: "sid test", //verificacion.Sid,
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
                fechaVencimiento: DateTime.UtcNow.AddMinutes(10),
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

    public async Task<Direccion> CrearDireccionPreRegistro(string pais, string estado, Guid creationUser, string? testCase = null)
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

    private async Task ValidarConfirmacionCodigoVerificacionSMS2FA(Cliente cliente)
    {
        // Obtiene el código de verificación SMS
        var confirmacionSMSCode = await context.Verificacion2FA.FirstOrDefaultAsync(x => x.Tipo == Tipo2FA.Sms && x.Verificado && x.ClienteId == cliente.Id);
        // Valida que exista el código de verificación SMS
        if (confirmacionSMSCode is null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.Verificacion2FASMSNoConfirmado,
                dynamicContent: [],
                module: this.GetType().Name));
        }
    }

    #endregion
}
