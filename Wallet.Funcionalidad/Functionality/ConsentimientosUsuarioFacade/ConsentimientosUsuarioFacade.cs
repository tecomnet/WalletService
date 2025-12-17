using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionUsuario;

namespace Wallet.Funcionalidad.Functionality.ConsentimientosUsuarioFacade;

public class ConsentimientosUsuarioFacade(ServiceDbContext context) : IConsentimientosUsuarioFacade
{
    public async Task<ConsentimientosUsuario> GuardarConsentimientoAsync(int idUsuario,
        TipoDocumentoConsentimiento tipoDocumento, string version, Guid creationUser)
    {
        try
        {
            // Verificar si el usuario existe
            var usuario = await context.Usuario.FindAsync(idUsuario);
            if (usuario == null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.UsuarioNoEncontrado,
                    dynamicContent: [idUsuario],
                    module: this.GetType().Name));
            }

            // Crear el nuevo consentimiento
            var consentimiento = new ConsentimientosUsuario(
                idUsuario: idUsuario,
                tipoDocumento: tipoDocumento,
                version: version,
                fechaAceptacion: DateTime.Now,
                creationUser: creationUser
            );

            await context.ConsentimientosUsuario.AddAsync(consentimiento);
            await context.SaveChangesAsync();

            return consentimiento;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<List<ConsentimientosUsuario>> ObtenerUltimosConsentimientosAsync(int idUsuario)
    {
        try
        {
            var consentimientos = await context.ConsentimientosUsuario
                .Where(c => c.IdUsuario == idUsuario && c.IsActive)
                .ToListAsync();

            var ultimosConsentimientos = consentimientos
                .GroupBy(c => c.TipoDocumento)
                .Select(g => g.OrderByDescending(c => c.FechaAceptacion).First())
                .ToList();

            return ultimosConsentimientos;
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
