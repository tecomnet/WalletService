using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;
using Wallet.Funcionalidad.Helper;

namespace Wallet.Funcionalidad.Functionality.ProveedorServicioFacade;

public class ProveedorServicioFacade : IProveedorServicioFacade
{
    private readonly ServiceDbContext _context;

    public ProveedorServicioFacade(ServiceDbContext context)
    {
        _context = context;
    }

    public async Task<ProveedorServicio> GuardarProveedorServicioAsync(string nombre,
        Wallet.DOM.Enums.ProductoCategoria categoria, string? urlIcono, Guid creationUser, string? testCase = null)
    {
        try
        {
            var proveedorServicio = new ProveedorServicio(nombre, categoria, urlIcono, creationUser);
            if (testCase != null)
            {
                // Ignoring testCase as per previous decision
            }

            await _context.ProveedorServicio.AddAsync(proveedorServicio);
            await _context.SaveChangesAsync();
            return proveedorServicio;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<ProveedorServicio> ObtenerProveedorServicioPorIdAsync(int idProveedorServicio)
    {
        try
        {
            var proveedorServicio = await _context.ProveedorServicio
                .FirstOrDefaultAsync(x => x.Id == idProveedorServicio);

            if (proveedorServicio == null)
            {
                var serviceError = new ServiceErrors().GetServiceErrorForCode("PROVEEDOR-SERVICIO-NOT-FOUND");
                throw new EMGeneralAggregateException(
                    new EMGeneralException(
                        serviceError.Message,
                        serviceError.ErrorCode,
                        serviceError.Title,
                        serviceError.Description(new object[] { idProveedorServicio }),
                        "ProveedorServicio",
                        null,
                        null,
                        "DOM",
                        new List<object> { idProveedorServicio }
                    )
                );
            }

            return proveedorServicio;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<ProveedorServicio> ActualizarProveedorServicioAsync(int idProveedorServicio, string nombre,
        Wallet.DOM.Enums.ProductoCategoria categoria, string? urlIcono, Guid modificationUser, string? testCase = null)
    {
        try
        {
            var proveedorServicio = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio);

            // Since properties are private set, we might need methods on the domain object to update them.
            // Checking ProveedorServicio.cs again...
            // It has private set for properties.
            // It does NOT have Update methods.
            // This is a problem. I might need to add Update methods to ProveedorServicio in DOM.
            // The user prompt said: "es posible que se deban agregar metodos adicionales o errores en las clases correspondientes"
            // So I should add an Update method to ProveedorServicio.

            // For now I will write the facade assuming the method exists, and then I will update the DOM class.
            proveedorServicio.Update(nombre, categoria, urlIcono, modificationUser);

            await _context.SaveChangesAsync();
            return proveedorServicio;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<ProveedorServicio> EliminarProveedorServicioAsync(int idProveedorServicio, Guid modificationUser)
    {
        try
        {
            var proveedorServicio = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio);
            proveedorServicio.Deactivate(modificationUser);
            await _context.SaveChangesAsync();
            return proveedorServicio;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<ProveedorServicio> ActivarProveedorServicioAsync(int idProveedorServicio, Guid modificationUser)
    {
        try
        {
            var proveedorServicio = await ObtenerProveedorServicioPorIdAsync(idProveedorServicio);
            proveedorServicio.Activate(modificationUser);
            await _context.SaveChangesAsync();
            return proveedorServicio;
        }
        catch (Exception exception) when (exception is not EMGeneralAggregateException)
        {
            throw GenericExceptionManager.GetAggregateException(
                serviceName: DomCommon.ServiceName,
                module: this.GetType().Name,
                exception: exception);
        }
    }

    public async Task<List<ProveedorServicio>> ObtenerProveedoresServicioAsync()
    {
        try
        {
            return await _context.ProveedorServicio.ToListAsync();
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
