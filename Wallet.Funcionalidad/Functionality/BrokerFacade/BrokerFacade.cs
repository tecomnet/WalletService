using Microsoft.EntityFrameworkCore;
using Wallet.DOM;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionEmpresa;

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
                var broker = new Broker(nombre: nombre, creationUser: creationUser);
                ValidarDuplicidad(nombre: nombre);
                await _context.Broker.AddAsync(entity: broker);
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
                return await _context.Broker.ToListAsync();
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
                var broker = await _context.Broker.FirstOrDefaultAsync(predicate: b => b.Id == idBroker);
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
        public async Task<Broker> ActualizarBrokerAsync(int idBroker, string nombre, string concurrencyToken,
            Guid modificationUser)
        {
            try
            {
                var broker = await ObtenerBrokerPorIdAsync(idBroker: idBroker);
                // Establece el token original para la validación de concurrencia optimista
                _context.Entry(entity: broker).Property(propertyExpression: x => x.ConcurrencyToken).OriginalValue =
                    DomCommon.SafeParseConcurrencyToken(token: concurrencyToken, module: this.GetType().Name);

                ValidarIsActive(broker: broker);
                ValidarDuplicidad(nombre: nombre, id: broker.Id);
                broker.Update(nombre: nombre, modificationUser: modificationUser);
                await _context.SaveChangesAsync();
                return broker;
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
        public async Task<Broker> EliminarBrokerAsync(int idBroker, Guid modificationUser)
        {
            try
            {
                var broker = await ObtenerBrokerPorIdAsync(idBroker: idBroker);
                broker.Deactivate(modificationUser: modificationUser);
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
                var broker = await ObtenerBrokerPorIdAsync(idBroker: idBroker);
                broker.Activate(modificationUser: modificationUser);
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
                await ObtenerBrokerPorIdAsync(idBroker: idBroker);

                return await _context.Proveedor
                    .Where(predicate: p => p.BrokerId == idBroker && p.IsActive)
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


        #region Metodos privados

        /// <summary>
        /// Valida si ya existe un broker con el mismo nombre.
        /// </summary>
        /// <param name="nombre">Nombre de la broker a validar.</param>
        /// <param name="id">ID de la broker (opcional, para excluir en actualizaciones).</param>
        /// <exception cref="EMGeneralAggregateException">Si ya existe un broker con ese nombre.</exception>
        private void ValidarDuplicidad(string nombre, int id = 0)
        {
            // Obtiene estado existente
            var brokerExiste = _context.Broker.FirstOrDefault(predicate: x => x.Nombre == nombre && x.Id != id);
            // Duplicado por nombre
            if (brokerExiste != null)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.BrokerExistente,
                    dynamicContent: [nombre],
                    module: this.GetType().Name));
            }
        }

        /// <summary>
        /// Valida si la broker se encuentra activa.
        /// </summary>
        /// <param name="broker">La broker a validar.</param>
        /// <exception cref="EMGeneralAggregateException">Si la broker está inactiva.</exception>
        private void ValidarIsActive(Broker broker)
        {
            if (!broker.IsActive)
            {
                throw new EMGeneralAggregateException(exception: DomCommon.BuildEmGeneralException(
                    errorCode: ServiceErrorsBuilder.BrokerInactivo,
                    dynamicContent: [broker.Nombre]));
            }
        }

        #endregion
    }
}
