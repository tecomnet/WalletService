using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;
using Wallet.Funcionalidad.ServiceClient;
using Wallet.Funcionalidad.Services.TokenService;
using System.Security.Claims;

namespace Wallet.Funcionalidad.Functionality.UsuarioFacade;

public class UsuarioFacade(
    ServiceDbContext context,
    ITwilioServiceFacade twilioService,
    ITokenService tokenService) : IUsuarioFacade
{
    public async Task<Usuario> ObtenerUsuarioPorIdAsync(int idUsuario)
    {
        try
        {
            var usuario = await context.Usuario
                .Include(navigationPropertyPath: u => u.Empresa)
                .Include(navigationPropertyPath: u => u.Cliente)
                .Include(navigationPropertyPath: u => u.Verificaciones2Fa)
                .Include(navigationPropertyPath: u => u.DispositivoMovilAutorizados)
                .Include(navigationPropertyPath: u => u.UbicacionesGeolocalizacion)
                .FirstOrDefaultAsync(predicate: x => x.Id == idUsuario);

            if (usuario == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
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
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);
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
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);
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
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);
            usuario.ActualizarCorreoElectronico(correoElectronico: correoElectronico,
                modificationUser: modificationUser);

            await ValidarDuplicidad(correoElectronico: correoElectronico, id: idUsuario);

            var nuevaVerificacion = await GeneraCodigoVerificacionTwilio2FAEmailAsync(
                correoElectronico: correoElectronico,
                nombreCliente: usuario.Cliente?.NombreCompleto ?? "Usuario", // Fallback if Cliente is null
                nombreEmpresa: usuario.Empresa?.Nombre ?? "Tecomnet", // Fallback if Empresa is null
                creationUser: modificationUser,
                testCase: testCase);

            usuario.AgregarVerificacion2Fa(verificacion: nuevaVerificacion, modificationUser: modificationUser);
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
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);
            usuario.ActualizarTelefono(codigoPais: codigoPais, telefono: telefono,
                modificationUser: modificationUser);

            await ValidarDuplicidad(codigoPais: codigoPais, telefono: telefono, id: idUsuario);

            var nuevaVerificacion = await GeneraCodigoVerificacionTwilio2FASMSAsync(
                codigoPais: codigoPais,
                telefono: telefono,
                creationUser: modificationUser,
                testCase: testCase);

            usuario.AgregarVerificacion2Fa(verificacion: nuevaVerificacion, modificationUser: modificationUser);
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

    public async Task<string?> ConfirmarCodigoVerificacion2FAAsync(int idUsuario, Tipo2FA tipo2FA,
        string codigoVerificacion, Guid modificationUser)
    {
        try
        {
            bool confirmado = false;
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);

            await context.Entry(entity: usuario)
                .Collection(propertyExpression: c => c.Verificaciones2Fa)
                .Query()
                .Where(predicate: v => v.Tipo == tipo2FA && v.IsActive)
                .LoadAsync();

            VerificacionResult verificacionResult;
            if (tipo2FA == Tipo2FA.Sms)
                verificacionResult = await twilioService.ConfirmarVerificacionSMS(
                    codigoPais: usuario.CodigoPais, telefono: usuario.Telefono,
                    codigo: codigoVerificacion);
            else
            {
                if (string.IsNullOrWhiteSpace(value: usuario.CorreoElectronico))
                {
                    throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
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
                confirmado = usuario.ConfirmarVerificacion2Fa(tipo: tipo2FA, codigo: codigoVerificacion,
                    modificationUser: modificationUser);
            }

            await context.SaveChangesAsync();

            if (confirmado)
            {
                var claims = new List<Claim>
                {
                    new Claim(type: ClaimTypes.NameIdentifier, value: usuario.Id.ToString()),
                    new Claim(type: ClaimTypes.Name, value: usuario.Cliente?.NombreCompleto ?? "Usuario"),
                    new Claim(type: "IdUsuario", value: usuario.Id.ToString())
                };

                if (usuario.Cliente != null)
                {
                    claims.Add(new Claim(type: "IdCliente", value: usuario.Cliente.Id.ToString()));
                }

                return tokenService.GenerateAccessToken(claims: claims);
            }

            return null;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<Usuario> GuardarUsuarioPreRegistroAsync(string codigoPais, string telefono, Guid creationUser,
        string? testCase = null)
    {
        try
        {
            // Existe pre registro incompleto
            var usuario = await context.Usuario
                .Include(navigationPropertyPath: x => x.Verificaciones2Fa.Where(x => x.IsActive))
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(predicate: x => x.CodigoPais == codigoPais && x.Telefono == telefono);

            if (usuario != null)
            {
                // Existe, pero no finalizo la confirmacion, ya sea por sms o email, iniciar el proceso de verificacion con sms
                if (usuario.Verificaciones2Fa.Any(predicate: v => v is { Verificado: false, Tipo: Tipo2FA.Sms }) ||
                    usuario.Verificaciones2Fa.Any(predicate: v => v is { Verificado: false, Tipo: Tipo2FA.Email }))
                {
                    // Genera codigo de verificacion y envia por twilio service
                    var verificacion2Fa = await GeneraCodigoVerificacionTwilio2FASMSAsync(codigoPais: codigoPais,
                        telefono: telefono, creationUser: creationUser, testCase: testCase);
                    // Agrega el codigo de verificacion
                    usuario.AgregarVerificacion2Fa(verificacion: verificacion2Fa, modificationUser: creationUser);
                }
                else
                {
                    throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                        errorCode: ServiceErrorsBuilder.ClienteDuplicado,
                        dynamicContent: [codigoPais, telefono],
                        module: this.GetType().Name));
                }
            }
            else
            {
                // Validamos duplicidad 
                await ValidarDuplicidad(codigoPais: codigoPais, telefono: telefono);

                // Crea un nuevo usuario
                usuario = new Usuario(codigoPais: codigoPais, telefono: telefono, correoElectronico: null,
                    contrasena: null, estatus: "PreRegistrado", creationUser: creationUser, testCase: testCase);
                await context.Usuario.AddAsync(entity: usuario);

                // Crea un nuevo cliente vinculado al usuario
                var cliente = new Cliente(usuario: usuario, creationUser: creationUser, testCase: testCase);
                await context.Cliente.AddAsync(entity: cliente);

                // Genera codigo de verificacion y envia por twilio service
                var verificacion2Fa = await GeneraCodigoVerificacionTwilio2FASMSAsync(codigoPais: codigoPais,
                    telefono: telefono, creationUser: creationUser, testCase: testCase);
                // Agrega el codigo de verificacion
                usuario.AgregarVerificacion2Fa(verificacion: verificacion2Fa, modificationUser: creationUser);
            }

            // Guardar cambios
            await context.SaveChangesAsync();
            // Retornar usuario
            return usuario;
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
        var usuarioExistente = await context.Usuario.FirstOrDefaultAsync(predicate: x =>
            x.CodigoPais == codigoPais && x.Telefono == telefono && x.Id != id);

        if (usuarioExistente != null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ClienteDuplicado, // Reuse error code
                dynamicContent: [codigoPais, telefono],
                module: this.GetType().Name));
        }
    }

    private async Task ValidarDuplicidad(string correoElectronico, int id = 0)
    {
        var usuarioExistente =
            await context.Usuario.FirstOrDefaultAsync(predicate: x =>
                x.CorreoElectronico == correoElectronico && x.Id != id);

        if (usuarioExistente != null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
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
                fechaVencimiento: DateTime.Now.AddMinutes(value: 10),
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
                fechaVencimiento: DateTime.Now.AddMinutes(value: 10),
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
