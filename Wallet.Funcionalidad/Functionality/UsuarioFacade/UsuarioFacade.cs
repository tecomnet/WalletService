using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;
using Wallet.Funcionalidad.ServiceClient;
using Wallet.Funcionalidad.Services.TokenService;

namespace Wallet.Funcionalidad.Functionality.UsuarioFacade;

/// <summary>
/// Fachada para la gestión de usuarios.
/// Implementa la lógica de negocio para operaciones relacionadas con usuarios, como autenticación, gestión de contraseñas y datos de contacto.
/// </summary>
public class UsuarioFacade(
    ServiceDbContext context,
    ITwilioServiceFacade twilioService,
    ITokenService tokenService) : IUsuarioFacade
{
    /// <inheritdoc />
    public async Task<Usuario> ObtenerUsuarioPorIdAsync(int idUsuario)
    {
        try
        {
            // Busca el usuario por su ID, incluyendo todas las relaciones necesarias.
            // Busca el usuario por su ID, incluyendo todas las relaciones necesarias.
            var usuario = await context.Usuario
                .Include(navigationPropertyPath: u => u.Cliente)
                .ThenInclude(navigationPropertyPath: c => c.Empresa)
                .FirstOrDefaultAsync(predicate: x => x.Id == idUsuario);

            // Si no se encuentra, lanza una excepción.
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

    /// <inheritdoc />
    public async Task<string> GuardarContrasenaAsync(int idUsuario, string contrasena, Guid modificationUser)
    {
        try
        {
            // Obtiene el usuario existente.
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);
            // Crea (establece) la nueva contraseña.
            usuario.CrearContrasena(contrasena: contrasena, modificationUser: modificationUser);


            // Genera el token de acceso.
            // Genera el token de acceso.
            var claims = new List<Claim>
            {
                new Claim(type: ClaimTypes.NameIdentifier, value: usuario.Id.ToString()),
                new Claim(type: ClaimTypes.Name, value: usuario.Cliente?.NombreCompleto ?? "Usuario"),
                new Claim(type: "IdUsuario", value: usuario.Id.ToString())
            };

            if (usuario.Cliente != null)
            {
                claims.Add(item: new Claim(type: "IdCliente", value: usuario.Cliente.Id.ToString()));
            }

            var accesToken = tokenService.GenerateAccessToken(claims: claims);
            // Guarda los cambios.
            await context.SaveChangesAsync();
            return accesToken;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Usuario> ActualizarContrasenaAsync(int idUsuario, string contrasenaActual, string contrasenaNueva,
        string confirmacionContrasenaNueva, string concurrencyToken, Guid modificationUser)
    {
        try
        {
            // Obtiene el usuario existente.
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);
            // Valida que el usuario tenga el registro completado.
            ValidarUsuarioRegistroCompletado(usuario: usuario);
            ValidarUsuarioIsActive(usuario: usuario);
            // TODO: ValidarConfirmacionCodigoVerificacionSms2Fa(usuario: usuario); // Metodo no existe
            // Establece el token original para la validación de concurrencia optimista
            context.Entry(entity: usuario).Property(propertyExpression: x => x.ConcurrencyToken).OriginalValue =
                DomCommon.SafeParseConcurrencyToken(token: concurrencyToken, module: this.GetType().Name);
            // Actualiza la contraseña, validando la actual.
            usuario.ActualizarContrasena(
                contrasenaActual: contrasenaActual,
                contrasenaNueva: contrasenaNueva,
                confirmacionContrasenaNueva: confirmacionContrasenaNueva,
                modificationUser: modificationUser);
            // Guarda los cambios.
            await context.SaveChangesAsync();
            return usuario;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not DbUpdateConcurrencyException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Usuario> ActualizarCorreoElectronicoAsync(int idUsuario, string correoElectronico,
        string concurrencyToken, Guid modificationUser, string? testCase = null, bool validarEstatus = true)
    {
        try
        {
            // Obtiene el usuario existente.
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);

            // Valida que el usuario tenga el registro completado.
            if (validarEstatus)
            {
                ValidarUsuarioRegistroCompletado(usuario: usuario);
                ValidarUsuarioIsActive(usuario: usuario);
            }

            // Establece el token original para la validación de concurrencia optimista
            context.Entry(entity: usuario).Property(propertyExpression: x => x.ConcurrencyToken).OriginalValue =
                DomCommon.SafeParseConcurrencyToken(token: concurrencyToken, module: this.GetType().Name);
            // Actualiza el correo electrónico en la entidad.
            usuario.ActualizarCorreoElectronico(correoElectronico: correoElectronico,
                modificationUser: modificationUser);

            // Carga explícitamente las verificaciones 2FA activas de Email.
            await context.Entry(entity: usuario)
                .Collection(propertyExpression: u => u.Verificaciones2Fa)
                .Query()
                .Where(predicate: v => v.Tipo == Tipo2FA.Email && v.IsActive && !v.Verificado)
                .LoadAsync();

            // Valida que el nuevo correo no esté duplicado.
            await ValidarDuplicidad(correoElectronico: correoElectronico, id: idUsuario);

            // Genera una nueva verificación de 2FA por correo electrónico.
            var nuevaVerificacion = await GeneraCodigoVerificacionTwilio2FAEmailAsync(
                correoElectronico: correoElectronico,
                nombreCliente: usuario.Cliente?.NombreCompleto ?? "Usuario", // Fallback si Cliente es nulo
                nombreEmpresa: usuario.Cliente?.Empresa?.Nombre ?? "Tecomnet", // Fallback si Empresa es nulo
                creationUser: modificationUser,
                testCase: testCase);

            // Agrega la verificación al usuario.
            usuario.AgregarVerificacion2Fa(verificacion: nuevaVerificacion, modificationUser: modificationUser);
            // Guarda los cambios.
            await context.SaveChangesAsync();
            return usuario;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not DbUpdateConcurrencyException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<Usuario> ActualizarTelefonoAsync(int idUsuario, string codigoPais, string telefono,
        string concurrencyToken, Guid modificationUser, string? testCase = null, bool validarEstatus = true)
    {
        try
        {
            // Obtiene el usuario existente.
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);

            // Valida que el usuario tenga el registro completado.
            if (validarEstatus)
            {
                ValidarUsuarioRegistroCompletado(usuario: usuario);
                ValidarUsuarioIsActive(usuario: usuario);
            }

            // Establece el token original para la validación de concurrencia optimista
            context.Entry(entity: usuario).Property(propertyExpression: x => x.ConcurrencyToken).OriginalValue =
                DomCommon.SafeParseConcurrencyToken(token: concurrencyToken, module: this.GetType().Name);
            // Actualiza el teléfono en la entidad.
            usuario.ActualizarTelefono(codigoPais: codigoPais, telefono: telefono,
                modificationUser: modificationUser);

            // Carga explícitamente las verificaciones 2FA activas de SMS.
            await context.Entry(entity: usuario)
                .Collection(propertyExpression: u => u.Verificaciones2Fa)
                .Query()
                .Where(predicate: v => v.Tipo == Tipo2FA.Sms && v.IsActive && !v.Verificado)
                .LoadAsync();

            // Valida que el nuevo teléfono no esté duplicado.
            await ValidarDuplicidad(codigoPais: codigoPais, telefono: telefono, id: idUsuario);

            // Genera una nueva verificación de 2FA por SMS.
            var nuevaVerificacion = await GeneraCodigoVerificacionTwilio2FASMSAsync(
                codigoPais: codigoPais,
                telefono: telefono,
                creationUser: modificationUser,
                testCase: testCase);

            // Agrega la verificación al usuario.
            usuario.AgregarVerificacion2Fa(verificacion: nuevaVerificacion, modificationUser: modificationUser);
            // Guarda los cambios.
            await context.SaveChangesAsync();
            return usuario;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not DbUpdateConcurrencyException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ConfirmarCodigoVerificacion2FAAsync(int idUsuario, Tipo2FA tipo2FA,
        string codigoVerificacion, Guid modificationUser, bool validarEstatus = true)
    {
        try
        {
            bool confirmado = false;
            // Obtiene el usuario.
            var usuario = await context.Usuario.FindAsync(keyValues: idUsuario);

            if (usuario == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.UsuarioNoEncontrado,
                    dynamicContent: [idUsuario],
                    module: this.GetType().Name));
            }

            // Valida que el usuario tenga el registro completado.
            if (validarEstatus)
            {
                ValidarUsuarioRegistroCompletado(usuario: usuario);
                ValidarUsuarioIsActive(usuario: usuario);
            }

            // Carga explícitamente las verificaciones 2FA activas del tipo solicitado.
            await context.Entry(entity: usuario)
                .Collection(propertyExpression: c => c.Verificaciones2Fa)
                .Query()
                .Where(predicate: v => v.Tipo == tipo2FA && v.IsActive)
                .LoadAsync();

            VerificacionResult verificacionResult;
            if (tipo2FA == Tipo2FA.Sms)
            {
                // Verifica el código SMS con Twilio.
                verificacionResult = await twilioService.ConfirmarVerificacionSMS(
                    codigoPais: usuario.CodigoPais, telefono: usuario.Telefono,
                    codigo: codigoVerificacion);
            }
            else
            {
                // Verifica si el correo electrónico está configurado.
                if (string.IsNullOrWhiteSpace(value: usuario.CorreoElectronico))
                {
                    throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                        errorCode: ServiceErrorsBuilder
                            .ClienteCorreoElectronicoNoConfigurado, // Mantiene el código de error existente
                        dynamicContent: [usuario.Id],
                        module: this.GetType().Name));
                }

                // Verifica el código Email con Twilio.
                verificacionResult =
                    await twilioService.ConfirmarVerificacionEmail(correoElectronico: usuario.CorreoElectronico,
                        codigo: codigoVerificacion);
            }

            // Si la verificación externa fue exitosa, marca la verificación interna como completada.
            if (verificacionResult.IsVerified)
            {
                confirmado = usuario.ConfirmarVerificacion2Fa(tipo: tipo2FA, codigo: codigoVerificacion,
                    modificationUser: modificationUser);
            }

            // Guarda los cambios.
            await context.SaveChangesAsync();

            // Si se confirmó exitosamente, retorna true.
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

    /// <inheritdoc />
    public async Task<Usuario> GuardarUsuarioPreRegistroAsync(string codigoPais, string telefono,
        string? testCase = null)
    {
        try
        {
            // Verifica si existe un pre-registro incompleto.
            var usuario = await context.Usuario
                .Include(navigationPropertyPath: x => x.Verificaciones2Fa.Where(x => x.IsActive))
                .FirstOrDefaultAsync(predicate: x => x.CodigoPais == codigoPais && x.Telefono == telefono);

            if (usuario != null)
            {
                // Si ya completó el registro, se regresa un error para que el usuario inicie sesión
                if (usuario.Estatus == EstatusRegistroEnum.RegistroCompletado)
                {
                    throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                        errorCode: ServiceErrorsBuilder.ClienteYaRegistrado,
                        dynamicContent: [usuario.Id],
                        module: this.GetType().Name));
                }

                // Genera código de verificación y envía por Twilio service.
                var verificacion2Fa = await GeneraCodigoVerificacionTwilio2FASMSAsync(
                    codigoPais: codigoPais,
                    telefono: telefono,
                    creationUser: usuario.CreationUser,
                    testCase: testCase);
                // Agrega el código de verificación.
                usuario.AgregarVerificacion2Fa(verificacion: verificacion2Fa,
                    modificationUser: usuario.CreationUser);
            }
            else
            {
                // Crea un nuevo usuario en estado "PreRegistrado".
                usuario = new Usuario(
                    codigoPais: codigoPais,
                    telefono: telefono,
                    correoElectronico: null,
                    contrasena: null,
                    estatus: EstatusRegistroEnum.PreRegistro,
                    creationUser: Guid.NewGuid(),
                    testCase: testCase);
                await context.Usuario.AddAsync(entity: usuario);
                // Genera código de verificación y envía por Twilio service.
                var verificacion2Fa = await GeneraCodigoVerificacionTwilio2FASMSAsync(
                    codigoPais: codigoPais,
                    telefono: telefono,
                    creationUser: usuario.CreationUser,
                    testCase: testCase);
                // Agrega el código de verificación.
                usuario.AgregarVerificacion2Fa(
                    verificacion: verificacion2Fa,
                    modificationUser: usuario.CreationUser);
            }

            // Guardar cambios.
            await context.SaveChangesAsync();
            // Retornar usuario.
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

    /// <inheritdoc />
    public async Task DesactivarUsuarioAsync(int idUsuario, string concurrencyToken, Guid modificationUser)
    {
        try
        {
            // Obtiene el usuario existente.
            var usuario = await ObtenerUsuarioPorIdAsync(idUsuario: idUsuario);

            // Establece el token original para la validación de concurrencia optimista
            context.Entry(entity: usuario).Property(propertyExpression: x => x.ConcurrencyToken).OriginalValue =
                DomCommon.SafeParseConcurrencyToken(token: concurrencyToken, module: this.GetType().Name);

            // Desactiva el usuario (borrado lógico).
            usuario.Deactivate(modificationUser: modificationUser);

            // Guarda los cambios.
            await context.SaveChangesAsync();
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException &&
                                          exception is not DbUpdateConcurrencyException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    #region Metodos privados

    /// <summary>
    /// Valida si existe un usuario duplicado por teléfono.
    /// </summary>
    /// <param name="codigoPais">Código de país.</param>
    /// <param name="telefono">Número de teléfono.</param>
    /// <param name="id">ID del usuario a excluir (opcional).</param>
    /// <exception cref="EMGeneralAggregateException">Si existe duplicidad.</exception>
    private async Task ValidarDuplicidad(string codigoPais, string telefono, int id = 0)
    {
        var usuarioExistente = await context.Usuario.FirstOrDefaultAsync(predicate: x =>
            x.CodigoPais == codigoPais && x.Telefono == telefono && x.Id != id);

        if (usuarioExistente != null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ClienteDuplicado, // Reutiliza código de error
                dynamicContent: [codigoPais, telefono],
                module: this.GetType().Name));
        }
    }

    /// <summary>
    /// Valida si existe un usuario duplicado por correo electrónico.
    /// </summary>
    /// <param name="correoElectronico">Correo electrónico.</param>
    /// <param name="id">ID del usuario a excluir (opcional).</param>
    /// <exception cref="EMGeneralAggregateException">Si existe duplicidad.</exception>
    private async Task ValidarDuplicidad(string correoElectronico, int id = 0)
    {
        var usuarioExistente =
            await context.Usuario.FirstOrDefaultAsync(predicate: x =>
                x.CorreoElectronico == correoElectronico && x.Id != id);

        if (usuarioExistente != null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ClienteDuplicadoPorCorreoElectronico, // Reutiliza código de error
                dynamicContent: [correoElectronico!],
                module: this.GetType().Name));
        }
    }

    /// <summary>
    /// Genera una verificación 2FA por SMS utilizando Twilio.
    /// </summary>
    /// <param name="codigoPais">Código de país.</param>
    /// <param name="telefono">Número de teléfono.</param>
    /// <param name="creationUser">Usuario creador.</param>
    /// <param name="testCase">Caso de prueba.</param>
    /// <returns>Objeto Verificacion2FA.</returns>
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

    /// <summary>
    /// Genera una verificación 2FA por Email utilizando Twilio.
    /// </summary>
    /// <param name="correoElectronico">Correo electrónico.</param>
    /// <param name="nombreCliente">Nombre del cliente.</param>
    /// <param name="nombreEmpresa">Nombre de la empresa.</param>
    /// <param name="creationUser">Usuario creador.</param>
    /// <param name="testCase">Caso de prueba.</param>
    /// <returns>Objeto Verificacion2FA.</returns>
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


    /// <summary>
    /// Valida que el usuario tenga el estatus de registro completado.
    /// </summary>
    /// <param name="usuario">Usuario a validar.</param>
    private void ValidarUsuarioRegistroCompletado(Usuario usuario)
    {
        if (usuario.Estatus != EstatusRegistroEnum.RegistroCompletado)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.InvalidRegistrationState,
                dynamicContent: [usuario.Estatus.ToString(), EstatusRegistroEnum.RegistroCompletado.ToString()],
                module: this.GetType().Name));
        }
    }

    /// <summary>
    /// Valida que el usuario tenga el estatus de activo.
    /// </summary>
    /// <param name="usuario">Usuario a validar.</param>
    private void ValidarUsuarioIsActive(Usuario usuario)
    {
        if (!usuario.IsActive)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.UsuarioInactivo,
                dynamicContent: [],
                module: this.GetType().Name));
        }
    }

    #endregion
}
