using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;
using Wallet.Funcionalidad.ServiceClient;

namespace Wallet.Funcionalidad.Functionality.UsuarioFacade;

public class UsuarioFacade(
    ServiceDbContext context,
    ITwilioServiceFacade twilioService) : IUsuarioFacade
{
    public async Task<Usuario> ObtenerUsuarioPorIdAsync(int idUsuario)
    {
        try
        {
            var usuario = await context.Usuario
                .Include(u => u.Empresa)
                .Include(u => u.Cliente)
                .Include(u => u.Verificaciones2FA)
                .Include(u => u.DispositivoMovilAutorizados)
                .Include(u => u.UbicacionesGeolocalizacion)
                .FirstOrDefaultAsync(x => x.Id == idUsuario);

            if (usuario == null)
            {
                throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.UsuarioNoEncontrado,
                    dynamicContent: [idUsuario],
                    module: this.GetType().Name));
            }

            return usuario;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<Usuario> GuardarContrasenaAsync(int idUsuario, string contrasena, Guid modificationUser)
    {
        try
        {
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario);
            usuario.CrearContrasena(contrasena: contrasena, modificationUser: modificationUser);
            await context.SaveChangesAsync();
            return usuario;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<Usuario> ActualizarContrasenaAsync(int idUsuario, string contrasenaActual, string contrasenaNueva,
        string confirmacionContrasenaNueva, Guid modificationUser)
    {
        try
        {
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario);
            usuario.ActualizarContrasena(
                contrasenaActual: contrasenaActual,
                contrasenaNueva: contrasenaNueva,
                confirmacionContrasenaNueva: confirmacionContrasenaNueva,
                modificationUser: modificationUser);
            await context.SaveChangesAsync();
            return usuario;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<Usuario> ActualizarCorreoElectronicoAsync(int idUsuario, string correoElectronico,
        Guid modificationUser, string? testCase = null)
    {
        try
        {
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario);
            usuario.ActualizarCorreoElectronico(correoElectronico: correoElectronico,
                modificationUser: modificationUser);

            await ValidarDuplicidad(correoElectronico: correoElectronico, id: idUsuario);

            var nuevaVerificacion = await GeneraCodigoVerificacionTwilio2FAEmailAsync(
                correoElectronico: correoElectronico,
                nombreCliente: usuario.Cliente?.NombreCompleto ?? "Usuario", // Fallback if Cliente is null
                nombreEmpresa: usuario.Empresa?.Nombre ?? "Tecomnet", // Fallback if Empresa is null
                creationUser: modificationUser,
                testCase: testCase);

            usuario.AgregarVerificacion2FA(verificacion: nuevaVerificacion, modificationUser: modificationUser);
            await context.SaveChangesAsync();
            return usuario;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<Usuario> ActualizarTelefonoAsync(int idUsuario, string codigoPais, string telefono,
        Guid modificationUser, string? testCase = null)
    {
        try
        {
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario);
            usuario.ActualizarTelefono(codigoPais: codigoPais, telefono: telefono,
                modificationUser: modificationUser);

            await ValidarDuplicidad(codigoPais: codigoPais, telefono: telefono, id: idUsuario);

            var nuevaVerificacion = await GeneraCodigoVerificacionTwilio2FASMSAsync(
                codigoPais: codigoPais,
                telefono: telefono,
                creationUser: modificationUser,
                testCase: testCase);

            usuario.AgregarVerificacion2FA(verificacion: nuevaVerificacion, modificationUser: modificationUser);
            await context.SaveChangesAsync();
            return usuario;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<bool> ConfirmarCodigoVerificacion2FAAsync(int idUsuario, Tipo2FA tipo2FA,
        string codigoVerificacion, Guid modificationUser)
    {
        try
        {
            bool confirmado = false;
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario);

            await context.Entry(usuario)
                .Collection(c => c.Verificaciones2FA)
                .Query()
                .Where(v => v.Tipo == tipo2FA && v.IsActive)
                .LoadAsync();

            VerificacionResult verificacionResult;
            if (tipo2FA == Tipo2FA.Sms)
                verificacionResult = await twilioService.ConfirmarVerificacionSMS(
                    codigoPais: usuario.CodigoPais, telefono: usuario.Telefono,
                    codigo: codigoVerificacion);
            else
            {
                if (string.IsNullOrWhiteSpace(usuario.CorreoElectronico))
                {
                    throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                        errorCode: ServiceErrorsBuilder
                            .ClienteCorreoElectronicoNoConfigurado, // Keep error code for now or create new one
                        dynamicContent: [usuario.Id],
                        module: this.GetType().Name));
                }

                verificacionResult =
                    await twilioService.ConfirmarVerificacionEmail(correoElectronico: usuario.CorreoElectronico,
                        codigo: codigoVerificacion);
            }

            if (verificacionResult.IsVerified)
            {
                confirmado = usuario.ConfirmarVerificacion2FA(tipo: tipo2FA, codigo: codigoVerificacion,
                    modificationUser: modificationUser);
            }

            await context.SaveChangesAsync();
            return confirmado;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    #region Metodos privados

    private async Task ValidarDuplicidad(string codigoPais, string telefono, int id = 0)
    {
        var usuarioExistente = await context.Usuario.FirstOrDefaultAsync(x =>
            x.CodigoPais == codigoPais && x.Telefono == telefono && x.Id != id);

        if (usuarioExistente != null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ClienteDuplicado, // Reuse error code
                dynamicContent: [codigoPais, telefono],
                module: this.GetType().Name));
        }
    }

    private async Task ValidarDuplicidad(string correoElectronico, int id = 0)
    {
        var usuarioExistente =
            await context.Usuario.FirstOrDefaultAsync(x =>
                x.CorreoElectronico == correoElectronico && x.Id != id);

        if (usuarioExistente != null)
        {
            throw new EMGeneralAggregateException(DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ClienteDuplicadoPorCorreoElectronico, // Reuse error code
                dynamicContent: [correoElectronico!],
                module: this.GetType().Name));
        }
    }

    private async Task<Verificacion2FA> GeneraCodigoVerificacionTwilio2FASMSAsync(string codigoPais, string telefono,
        Guid creationUser, string? testCase = null)
    {
        try
        {
            var verificacion = await twilioService.VerificacionSMS(codigoPais: codigoPais, telefono: telefono);
            Verificacion2FA verificacion2Fa = new Verificacion2FA(
                twilioSid: verificacion.Sid,
                fechaVencimiento: DateTime.Now.AddMinutes(10),
                tipo: Tipo2FA.Sms,
                creationUser: creationUser,
                testCase: testCase
            );
            return verificacion2Fa;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    private async Task<Verificacion2FA> GeneraCodigoVerificacionTwilio2FAEmailAsync(
        string correoElectronico, string nombreCliente, string nombreEmpresa, Guid creationUser,
        string? testCase = null)
    {
        try
        {
            var verificacion = await twilioService.VerificacionEmail(correoElectronico: correoElectronico,
                nombreEmpresa: nombreEmpresa, nombreCliente: nombreCliente);
            Verificacion2FA verificacion2Fa = new Verificacion2FA(
                twilioSid: verificacion.Sid,
                fechaVencimiento: DateTime.Now.AddMinutes(10),
                tipo: Tipo2FA.Email,
                creationUser: creationUser,
                testCase: testCase
            );
            return verificacion2Fa;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    #endregion
}
