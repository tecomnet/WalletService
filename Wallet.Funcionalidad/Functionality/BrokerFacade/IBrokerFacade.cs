using Wallet.DOM.Modelos.GestionEmpresa;

namespace Wallet.Funcionalidad.Functionality.BrokerFacade
{
    /// <summary>
    /// Interface for Broker management operations.
    /// </summary>
    public interface IBrokerFacade
    {
        /// <summary>
        /// Guarda un nuevo broker.
        /// </summary>
        /// <param name="nombre">Nombre del broker</param>
        /// <param name="creationUser">Usuario que crea el registro</param>
        /// <returns>El broker creado</returns>
        Task<Broker> GuardarBrokerAsync(string nombre, Guid creationUser);

        /// <summary>
        /// Obtiene todos los brokers.
        /// </summary>
        /// <returns>Lista de brokers</returns>
        Task<List<Broker>> ObtenerBrokersAsync();

        /// <summary>
        /// Obtiene un broker por su ID.
        /// </summary>
        /// <param name="idBroker">ID del broker</param>
        /// <returns>Datos del broker</returns>
        Task<Broker> ObtenerBrokerPorIdAsync(int idBroker);

        /// <summary>
        /// Actualiza un broker existente.
        /// </summary>
        /// <param name="idBroker">ID del broker</param>
        /// <param name="nombre">Nuevo nombre del broker</param>
        /// <param name="concurrencyToken">Token de concurrencia</param>
        /// <param name="modificationUser">Usuario que modifica</param>
        /// <returns>El broker actualizado</returns>
        Task<Broker> ActualizarBrokerAsync(int idBroker, string nombre, string concurrencyToken, Guid modificationUser);

        /// <summary>
        /// Elimina (lógicamente) un broker.
        /// </summary>
        /// <param name="idBroker">ID del broker</param>
        /// <param name="modificationUser">Usuario que elimina</param>
        /// <returns>El broker eliminado</returns>
        Task<Broker> EliminarBrokerAsync(int idBroker, Guid modificationUser);

        /// <summary>
        /// Elimina (lógicamente) un broker.
        /// </summary>
        /// <param name="idBroker">ID del broker</param>
        /// <param name="modificationUser">Usuario que elimina</param>
        /// <returns>El broker eliminado</returns>
        Task<Broker> ActivarBrokerAsync(int idBroker, Guid modificationUser);

        /// <summary>
        /// Obtiene los proveedores asociados a un broker.
        /// </summary>
        /// <param name="idBroker">ID del broker</param>
        /// <returns>Lista de proveedores</returns>
        Task<List<Proveedor>> ObtenerProveedoresPorBrokerAsync(int idBroker);
    }
}
