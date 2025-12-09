using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos;

namespace Wallet.Funcionalidad.Functionality.BrokerFacade
{
    /// <summary>
    /// Implementation of IBrokerFacade.
    /// </summary>
    public class BrokerFacade : IBrokerFacade
    {
        private readonly ServiceDbContext _context;

        /// <summary>
        /// Constructor for BrokerFacade.
        /// </summary>
        /// <param name="context">Database context</param>
        public BrokerFacade(ServiceDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Broker> GuardarBrokerAsync(string nombre, Guid creationUser)
        {
            try
            {
                var broker = new Broker(nombre, creationUser);
                await _context.Broker.AddAsync(broker);
                await _context.SaveChangesAsync();
                return broker;
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
        public async Task<List<Broker>> ObtenerBrokersAsync()
        {
            try
            {
                return await _context.Broker.Where(b => b.IsActive).ToListAsync();
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
        public async Task<Broker> ObtenerBrokerPorIdAsync(int idBroker)
        {
            try
            {
                var broker = await _context.Broker.FirstOrDefaultAsync(b => b.Id == idBroker && b.IsActive);
                if (broker == null)
                {
                    throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                        errorCode: ServiceErrorsBuilder
                            .BrokerNoEncontrado, // Assuming this error code exists or needs to be added
                        dynamicContent: new List<object> { idBroker },
                        module: this.GetType().Name));
                }

                return broker;
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
        public async Task<Broker> ActualizarBrokerAsync(int idBroker, string nombre, Guid modificationUser)
        {
            try
            {
                var broker = await ObtenerBrokerPorIdAsync(idBroker);
                broker.Update(nombre, modificationUser);
                await _context.SaveChangesAsync();
                return broker;
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
        public async Task<Broker> EliminarBrokerAsync(int idBroker, Guid modificationUser)
        {
            try
            {
                var broker = await ObtenerBrokerPorIdAsync(idBroker);
                broker.Deactivate(modificationUser);
                await _context.SaveChangesAsync();
                return broker;
            }
            catch (Exception exception) when (exception is not EMGeneralAggregateException)
            {
                throw GenericExceptionManager.GetAggregateException(
                    serviceName: DomCommon.ServiceName,
                    module: this.GetType().Name,
                    exception: exception);
            }
        }
        public async Task<Broker> ActivarBrokerAsync(int idBroker, Guid modificationUser)
        {
            try
            {
                var broker = await ObtenerBrokerPorIdAsync(idBroker);
                broker.Activate(modificationUser);
                await _context.SaveChangesAsync();
                return broker;
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
        public async Task<List<Proveedor>> ObtenerProveedoresPorBrokerAsync(int idBroker)
        {
            try
            {
                // Verify broker exists first? Or just return empty list?
                // Plan said: querying context.Proveedor.Where(p => p.BrokerId == idBroker)
                // Let's verify broker exists first to throw 404 if not found, consistent with other endpoints.
                await ObtenerBrokerPorIdAsync(idBroker);

                return await _context.Proveedor
                    .Where(p => p.BrokerId == idBroker && p.IsActive)
                    .ToListAsync();
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
}
