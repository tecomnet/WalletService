using Microsoft.EntityFrameworkCore;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.DOM;
using Wallet.Funcionalidad.Functionality.ClienteFacade;
using Wallet.Funcionalidad.Functionality.ConsentimientosUsuarioFacade;
using Wallet.Funcionalidad.Functionality.UsuarioFacade;

namespace Wallet.Funcionalidad.Functionality.RegistroFacade;

public class RegistroFacade(
    ServiceDbContext context,
    IUsuarioFacade usuarioFacade,
    IClienteFacade clienteFacade,
    IConsentimientosUsuarioFacade consentimientosUsuarioFacade)
    : IRegistroFacade
{
    public async Task<Usuario> PreRegistroAsync(string codigoPais, string telefono, Guid creationUser)
    {
        // Llama al facade existente que ya maneja la creación en estado PreRegistro
        return await usuarioFacade.GuardarUsuarioPreRegistroAsync(codigoPais, telefono, creationUser);
    }

    public async Task<bool> ConfirmarNumeroAsync(int idUsuario, string codigo, Guid modificationUser)
    {
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.PreRegistro);

        var verificado =
            await usuarioFacade.ConfirmarCodigoVerificacion2FAAsync(idUsuario, Tipo2FA.Sms, codigo, modificationUser);
        if (verificado)
        {
            await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.NumeroConfirmado, modificationUser);
        }

        return verificado;
    }

    public async Task<Usuario> CompletarDatosClienteAsync(int idUsuario, string nombre, string apellidoPaterno,
        string apellidoMaterno, DateOnly fechaNacimiento, Guid modificationUser)
    {
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.NumeroConfirmado);

        var cliente = await context.Cliente.FirstOrDefaultAsync(c => c.UsuarioId == idUsuario);
        if (cliente == null)
        {
            // Si no existe, lo creamos manualmente ya que IClienteFacade no tiene método de crear expuesto
            cliente = new Cliente(usuario, creationUser: modificationUser);
            await context.Cliente.AddAsync(cliente);
            await context.SaveChangesAsync();
        }

        // Actualizamos datos usando el facade
        await clienteFacade.ActualizarClienteDatosPersonalesAsync(
            cliente.Id,
            nombre,
            apellidoPaterno,
            apellidoMaterno,
            "N/A", // Estado por defecto
            fechaNacimiento,
            Genero.Masculino, // Genero por defecto, debería venir en el request
            modificationUser);

        await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.DatosClienteCompletado, modificationUser);
        return usuario;
    }

    public async Task<Usuario> RegistrarCorreoAsync(int idUsuario, string correo, Guid modificationUser)
    {
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.DatosClienteCompletado);

        await usuarioFacade.ActualizarCorreoElectronicoAsync(idUsuario, correo, modificationUser);

        await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.CorreoRegistrado, modificationUser);
        return usuario;
    }

    public async Task<bool> VerificarCorreoAsync(int idUsuario, string codigo, Guid modificationUser)
    {
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.CorreoRegistrado);

        var verificado =
            await usuarioFacade.ConfirmarCodigoVerificacion2FAAsync(idUsuario, Tipo2FA.Email, codigo, modificationUser);
        if (verificado)
        {
            await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.CorreoVerificado, modificationUser);
        }

        return verificado;
    }

    public async Task<Usuario> RegistrarDatosBiometricosAsync(int idUsuario, string idDispositivo, string token,
        Guid modificationUser)
    {
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.CorreoVerificado);

        // Crear DispositivoMovilAutorizado
        // Constructor: token, idDispositivo, nombre, caracteristicas, creationUser
        var dispositivo = new DispositivoMovilAutorizado(
            token: token,
            idDispositivo: idDispositivo,
            nombre: "Dispositivo Móvil", // Nombre por defecto
            caracteristicas: "N/A", // Características por defecto
            creationUser: modificationUser
        );

        // Asignar al usuario (UsuarioId es set privado en Dispositivo, pero Usuario es public set? No, private set)
        // DispositivoMovilAutorizado no tiene propiedad UsuarioId pública settable en constructor, 
        // pero tiene UsuarioId property.
        // Espera, el constructor no recibe Usuario.
        // ¿Cómo se asocia? 
        // Ah, UsuarioId es private set.
        // Necesito ver si puedo agregarlo a la colección del usuario.

        usuario.AgregarDispositivoMovilAutorizado(dispositivo, modificationUser);

        await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.DatosBiometricosRegistrado, modificationUser);
        return usuario;
    }

    public async Task<Usuario> AceptarTerminosCondicionesAsync(int idUsuario, string version, Guid modificationUser)
    {
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.DatosBiometricosRegistrado);

        await consentimientosUsuarioFacade.GuardarConsentimientoAsync(idUsuario, TipoDocumentoConsentimiento.Terminos,
            version, modificationUser);
        await consentimientosUsuarioFacade.GuardarConsentimientoAsync(idUsuario, TipoDocumentoConsentimiento.Privacidad,
            version, modificationUser);
        await consentimientosUsuarioFacade.GuardarConsentimientoAsync(idUsuario, TipoDocumentoConsentimiento.PLD,
            version, modificationUser);

        await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.TerminosCondicionesAceptado, modificationUser);
        return usuario;
    }

    public async Task<Usuario> CompletarRegistroAsync(int idUsuario, string contrasena, string confirmacionContrasena,
        Guid modificationUser)
    {
        var usuario = await ValidarEstadoAsync(idUsuario, EstatusRegistroEnum.TerminosCondicionesAceptado);

        if (contrasena != confirmacionContrasena)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.ContrasenasNoCoinciden,
                dynamicContent: []));
        }

        await usuarioFacade.GuardarContrasenaAsync(idUsuario, contrasena, modificationUser);

        await ActualizarEstatusAsync(usuario, EstatusRegistroEnum.RegistroCompletado, modificationUser);
        return usuario;
    }

    private async Task<Usuario> ValidarEstadoAsync(int idUsuario, EstatusRegistroEnum estatusEsperado)
    {
        var usuario = await usuarioFacade.ObtenerUsuarioPorIdAsync(idUsuario);
        if (usuario == null)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: ServiceErrorsBuilder.UsuarioNoEncontrado,
                dynamicContent: []));
        }

        if (usuario.Estatus != estatusEsperado)
        {
            throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                errorCode: "INVALID_REGISTRATION_STATE",
                dynamicContent: [usuario.Estatus.ToString(), estatusEsperado.ToString()]));
        }

        return usuario;
    }

    private async Task ActualizarEstatusAsync(Usuario usuario, EstatusRegistroEnum nuevoEstatus, Guid modificationUser)
    {
        context.Entry(usuario).Property(u => u.Estatus).CurrentValue = nuevoEstatus;
        await context.SaveChangesAsync();
    }
}
