using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.Funcionalidad.Functionality.ConsentimientosUsuarioFacade;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;

namespace Wallet.Funcionalidad.Functionality.RegistroFacade;

/// <summary>
/// Facade para orquestar el proceso de registro de un nuevo usuario, manejando los diferentes pasos y estados.
/// </summary>
public class RegistroFacade(
    ServiceDbContext context,
    IUsuarioFacade usuarioFacade,
    IClienteFacade clienteFacade,
    IConsentimientosUsuarioFacade consentimientosUsuarioFacade,
    IEmpresaFacade empresaFacade)
    : IRegistroFacade
{
    /// <summary>
    /// Inicia el proceso de registro creando un usuario en estado de pre-registro.
    /// </summary>
    /// <param name="codigoPais">Código del país del número de teléfono.</param>
    /// <param name="telefono">Número de teléfono del usuario.</param>
    /// <param name="creationUser">ID del usuario que realiza la creación.</param>
    /// <returns>El objeto <see cref="Usuario"/> recién creado en estado de pre-registro.</returns>
    public async Task<Usuario> PreRegistroAsync(string codigoPais, string telefono, Guid creationUser)
    {
        // Llama al facade existente que ya maneja la creación en estado PreRegistro
        return await usuarioFacade.GuardarUsuarioPreRegistroAsync(codigoPais, telefono, creationUser);
    }

    /// <summary>
    /// Confirma el número de teléfono del usuario mediante un código de verificación.
    /// </summary>
    /// <param name="idUsuario">ID del usuario a confirmar.</param>
    /// <param name="codigo">Código de verificación enviado al número de teléfono.</param>
    /// <param name="modificationUser">ID del usuario que realiza la modificación.</param>
    /// <returns>True si el código es válido y el número se confirma, false en caso contrario.</returns>
    public async Task<bool> ConfirmarNumeroAsync(int idUsuario, string codigo, Guid modificationUser)
    {
        // Valida que el usuario esté en el estado esperado (PreRegistro)
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.PreRegistro);

        // Confirma el código de verificación SMS a través del facade de usuario
        var verificado =
            await usuarioFacade.ConfirmarCodigoVerificacion2FAAsync(idUsuario, Tipo2FA.Sms, codigo, modificationUser);
        if (verificado)
        {
            // Si es verificado, actualiza el estado del registro a NumeroConfirmado
            await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.NumeroConfirmado, modificationUser);
        }

        return verificado;
    }

    /// <summary>
    /// Completa los datos personales del cliente asociado al usuario.
    /// </summary>
    /// <param name="idUsuario">ID del usuario.</param>
    /// <param name="nombre">Nombre del cliente.</param>
    /// <param name="apellidoPaterno">Apellido paterno del cliente.</param>
    /// <param name="apellidoMaterno">Apellido materno del cliente.</param>
    /// <param name="nombreEstado">Nombre del estado del cliente.</param>
    /// <param name="fechaNacimiento">Fecha de nacimiento del cliente.</param>
    /// <param name="genero">Genero del cliente.</param>
    /// <param name="modificationUser">ID del usuario que realiza la modificación.</param>
    /// <returns>El objeto <see cref="Usuario"/> con los datos del cliente actualizados.</returns>
    public async Task<Usuario> CompletarDatosClienteAsync(int idUsuario, string nombre, string apellidoPaterno,
        string apellidoMaterno, string nombreEstado, DateOnly fechaNacimiento, Genero genero, Guid modificationUser)
    {
        // Valida que el usuario esté en el estado esperado (NumeroConfirmado)
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.NumeroConfirmado);

        // Busca si ya existe un cliente asociado a este usuario
        var cliente = await context.Cliente.FirstOrDefaultAsync(c => c.UsuarioId == idUsuario);
        if (cliente == null)
        {
            // TODO EMD: UBICARLO EN LA EMPRESA TECOMNET
            var empresa = await empresaFacade.ObtenerPorNombreAsync(nombre: "Tecomnet");
            // Si no existe, lo creamos manualmente ya que IClienteFacade no tiene método de crear expuesto
            cliente = new Cliente(usuario, creationUser: modificationUser, empresa: empresa);
            await context.Cliente.AddAsync(cliente);
            await context.SaveChangesAsync();
        }

        // Actualizamos datos usando el facade de cliente
        await clienteFacade.ActualizarClienteDatosPersonalesAsync(
            idCliente: cliente.Id,
            nombre: nombre,
            primerApellido: apellidoPaterno,
            segundoApellido: apellidoMaterno,
            nombreEstado: nombreEstado,
            fechaNacimiento: fechaNacimiento,
            genero: genero,
            modificationUser: modificationUser);

        // Actualiza el estado del registro a DatosClienteCompletado
        await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.DatosClienteCompletado, modificationUser);
        return usuario;
    }

    /// <summary>
    /// Registra el correo electrónico del usuario.
    /// </summary>
    /// <param name="idUsuario">ID del usuario.</param>
    /// <param name="correo">Correo electrónico a registrar.</param>
    /// <param name="modificationUser">ID del usuario que realiza la modificación.</param>
    /// <returns>El objeto <see cref="Usuario"/> con el correo electrónico registrado.</returns>
    public async Task<Usuario> RegistrarCorreoAsync(int idUsuario, string correo, Guid modificationUser)
    {
        // Valida que el usuario esté en el estado esperado (DatosClienteCompletado)
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.DatosClienteCompletado);

        // Actualiza el correo electrónico del usuario a través del facade de usuario
        await usuarioFacade.ActualizarCorreoElectronicoAsync(idUsuario, correo, modificationUser);

        // Actualiza el estado del registro a CorreoRegistrado
        await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.CorreoRegistrado, modificationUser);
        return usuario;
    }

    /// <summary>
    /// Verifica el correo electrónico del usuario mediante un código de verificación.
    /// </summary>
    /// <param name="idUsuario">ID del usuario a verificar.</param>
    /// <param name="codigo">Código de verificación enviado al correo electrónico.</param>
    /// <param name="modificationUser">ID del usuario que realiza la modificación.</param>
    /// <returns>True si el código es válido y el correo se verifica, false en caso contrario.</returns>
    public async Task<bool> VerificarCorreoAsync(int idUsuario, string codigo, Guid modificationUser)
    {
        // Valida que el usuario esté en el estado esperado (CorreoRegistrado)
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.CorreoRegistrado);

        // Confirma el código de verificación de Email a través del facade de usuario
        var verificado =
            await usuarioFacade.ConfirmarCodigoVerificacion2FAAsync(idUsuario, Tipo2FA.Email, codigo, modificationUser);
        if (verificado)
        {
            // Si es verificado, actualiza el estado del registro a CorreoVerificado
            await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.CorreoVerificado, modificationUser);
        }

        return verificado;
    }

    /// <summary>
    /// Registra los datos biométricos o un dispositivo móvil autorizado para el usuario.
    /// </summary>
    /// <param name="idUsuario">ID del usuario.</param>
    /// <param name="idDispositivo">Identificador único del dispositivo.</param>
    /// <param name="token">Token de autenticación/registro del dispositivo.</param>
    /// <param name="modificationUser">ID del usuario que realiza la modificación.</param>
    /// <returns>El objeto <see cref="Usuario"/> con el dispositivo móvil autorizado registrado.</returns>
    public async Task<Usuario> RegistrarDatosBiometricosAsync(int idUsuario, string idDispositivo, string token,
        string nombre, string caracteristicas, Guid modificationUser)
    {
        // Valida que el usuario esté en el estado esperado (CorreoVerificado)
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.CorreoVerificado);

        // Crea una nueva instancia de DispositivoMovilAutorizado
        var dispositivo = new DispositivoMovilAutorizado(
            token: token,
            idDispositivo: idDispositivo,
            nombre: nombre,
            caracteristicas: caracteristicas,
            creationUser: modificationUser
        );

        // Agrega el dispositivo al usuario
        usuario.AgregarDispositivoMovilAutorizado(dispositivo, modificationUser);

        // Actualiza el estado del registro a DatosBiometricosRegistrado
        await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.DatosBiometricosRegistrado, modificationUser);
        return usuario;
    }

    /// <summary>
    /// Registra la aceptación de los términos y condiciones, política de privacidad y PLD por parte del usuario.
    /// </summary>
    /// <param name="idUsuario">ID del usuario.</param>
    /// <param name="version">Versión de los documentos aceptados.</param>
    /// <param name="aceptoTerminos">Indica si aceptó los términos y condiciones.</param>
    /// <param name="aceptoPrivacidad">Indica si aceptó la política de privacidad.</param>
    /// <param name="aceptoPld">Indica si aceptó la política PLD.</param>
    /// <param name="modificationUser">ID del usuario que realiza la modificación.</param>
    /// <returns>El objeto <see cref="Usuario"/> con los consentimientos registrados.</returns>
    public async Task<Usuario> AceptarTerminosCondicionesAsync(int idUsuario, string version, bool aceptoTerminos,
        bool aceptoPrivacidad, bool aceptoPld, Guid modificationUser)
    {
        // Valida que el usuario esté en el estado esperado (DatosBiometricosRegistrado)
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.DatosBiometricosRegistrado);

        // Valida que se hayan aceptado todos los términos requeridos
        if (!aceptoTerminos || !aceptoPrivacidad || !aceptoPld)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.TerminosNoAceptados,
                dynamicContent: []));
        }

        // Registra los diferentes tipos de consentimientos a través del facade de consentimientos
        await consentimientosUsuarioFacade.GuardarConsentimientoAsync(idUsuario, TipoDocumentoConsentimiento.Terminos,
            version, modificationUser);
        await consentimientosUsuarioFacade.GuardarConsentimientoAsync(idUsuario, TipoDocumentoConsentimiento.Privacidad,
            version, modificationUser);
        await consentimientosUsuarioFacade.GuardarConsentimientoAsync(idUsuario, TipoDocumentoConsentimiento.PLD,
            version, modificationUser);

        // Actualiza el estado del registro a TerminosCondicionesAceptado
        await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.TerminosCondicionesAceptado, modificationUser);
        return usuario;
    }

    /// <summary>
    /// Completa el registro del usuario estableciendo su contraseña.
    /// </summary>
    /// <param name="idUsuario">ID del usuario.</param>
    /// <param name="contrasena">Contraseña a establecer.</param>
    /// <param name="confirmacionContrasena">Confirmación de la contraseña.</param>
    /// <param name="modificationUser">ID del usuario que realiza la modificación.</param>
    /// <returns>El objeto <see cref="Usuario"/> con el registro completado.</returns>
    /// <exception cref="EMGeneralAggregateException">Se lanza si las contraseñas no coinciden.</exception>
    public async Task<Usuario> CompletarRegistroAsync(int idUsuario, string contrasena, string confirmacionContrasena,
        Guid modificationUser)
    {
        // Valida que el usuario esté en el estado esperado (TerminosCondicionesAceptado)
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.TerminosCondicionesAceptado);

        // Verifica que la contraseña y su confirmación coincidan
        if (contrasena != confirmacionContrasena)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenasNoCoinciden,
                dynamicContent: []));
        }

        // Guarda la contraseña del usuario a través del facade de usuario
        await usuarioFacade.GuardarContrasenaAsync(idUsuario, contrasena, modificationUser);

        // Actualiza el estado del registro a RegistroCompletado
        await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.RegistroCompletado, modificationUser);
        return usuario;
    }

    /// <summary>
    /// Valida que el usuario exista y que su estado de registro actual sea el esperado.
    /// </summary>
    /// <param name="idUsuario">ID del usuario a validar.</param>
    /// <param name="estatusEsperado">El estado de registro que se espera del usuario.</param>
    /// <returns>El objeto <see cref="Usuario"/> si la validación es exitosa.</returns>
    /// <exception cref="EMGeneralAggregateException">Se lanza si el usuario no es encontrado o si su estado no coincide con el esperado.</exception>
    private async Task<Usuario> ValidarEstadoAsync(int idUsuario, EstatusRegistroEnum estatusEsperado)
    {
        // Obtiene el usuario por su ID
        var usuario = await usuarioFacade.ObtenerUsuarioPorIdAsync(idUsuario);
        if (usuario == null)
        {
            // Lanza una excepción si el usuario no es encontrado
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.UsuarioNoEncontrado,
                dynamicContent: []));
        }

        // Verifica si el estado actual del usuario coincide con el estado esperado
        if (usuario.Estatus != estatusEsperado)
        {
            // Lanza una excepción si el estado no coincide
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.InvalidRegistrationState,
                dynamicContent: [usuario.Estatus.ToString(), estatusEsperado.ToString()]));
        }

        return usuario;
    }

    /// <summary>
    /// Actualiza el estatus de registro de un usuario y guarda los cambios en la base de datos.
    /// </summary>
    /// <param name="usuario">El objeto <see cref="Usuario"/> cuyo estatus será actualizado.</param>
    /// <param name="nuevoEstatus">El nuevo estatus de registro a establecer.</param>
    /// <param name="modificationUser">ID del usuario que realiza la modificación.</param>
    private async Task ActualizarEstatusAsync(Usuario usuario, EstatusRegistroEnum nuevoEstatus, Guid modificationUser)
    {
        // Actualiza el estatus del usuario
        usuario.ActualizarEstatus(nuevoEstatus, modificationUser);
        // Guarda los cambios en la base de datos
        await context.SaveChangesAsync();
    }
}
