using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos
{
    /// <summary>
    /// Representa un servicio guardado como favorito por un cliente.
    /// </summary>
    public class ServicioFavorito : ValidatablePersistentObjectLogicalDelete
    {
        /// <summary>
        /// Define las restricciones de las propiedades para la validación del objeto ServicioFavorito.
        /// </summary>
        protected override List<PropertyConstraint> PropertyConstraints =>
        [
            PropertyConstraint.StringPropertyConstraint(
                propertyName: nameof(Alias),
                isRequired: true,
                maximumLength: 50,
                minimumLength: 1),
            PropertyConstraint.StringPropertyConstraint(
                propertyName: nameof(NumeroReferencia),
                isRequired: true,
                maximumLength: 50,
                minimumLength: 1)
        ];

        /// <summary>
        /// ID del cliente que guardó el servicio como favorito.
        /// </summary>
        [Required]
        public int ClienteId { get; private set; }

        /// <summary>
        /// Objeto de navegación para el cliente.
        /// </summary>
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }

        /// <summary>
        /// ID del proveedor del servicio favorito.
        /// </summary>
        [Required]
        public int ProveedorServicioId { get; private set; }

        /// <summary>
        /// Objeto de navegación para el proveedor de servicios.
        /// </summary>
        [ForeignKey("ProveedorServicioId")]
        public ProveedorServicio ProveedorServicio { get; set; }

        /// <summary>
        /// Alias o nombre personalizado para el servicio favorito.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Alias { get; private set; }

        /// <summary>
        /// Número de referencia asociado al servicio (ej. número de cuenta, contrato).
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string NumeroReferencia { get; private set; }

        /// <summary>
        /// Constructor privado para uso de Entity Framework.
        /// </summary>
        private ServicioFavorito() : base()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServicioFavorito"/>.
        /// </summary>
        /// <param name="clienteId">El ID del cliente.</param>
        /// <param name="proveedorServicioId">El ID del proveedor de servicios.</param>
        /// <param name="alias">El alias para el servicio.</param>
        /// <param name="numeroReferencia">El número de referencia del servicio.</param>
        /// <param name="creationUser">El usuario que crea el registro.</param>
        public ServicioFavorito(int clienteId, int proveedorServicioId, string alias, string numeroReferencia,
            Guid creationUser) : base(creationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(nameof(Alias), alias, ref exceptions);
            IsPropertyValid(nameof(NumeroReferencia), numeroReferencia, ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions);
            }

            ClienteId = clienteId;
            ProveedorServicioId = proveedorServicioId;
            Alias = alias;
            NumeroReferencia = numeroReferencia;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServicioFavorito"/> con objetos de Cliente y ProveedorServicio.
        /// </summary>
        /// <param name="cliente">El objeto Cliente.</param>
        /// <param name="proveedorServicio">El objeto ProveedorServicio.</param>
        /// <param name="alias">El alias para el servicio.</param>
        /// <param name="numeroReferencia">El número de referencia del servicio.</param>
        /// <param name="creationUser">El usuario que crea el registro.</param>
        public ServicioFavorito(Cliente cliente, ProveedorServicio proveedorServicio, string alias,
            string numeroReferencia, Guid creationUser) : base(creationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(nameof(Alias), alias, ref exceptions);
            IsPropertyValid(nameof(NumeroReferencia), numeroReferencia, ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions);
            }

            Cliente = cliente;
            ClienteId = cliente.Id;
            ProveedorServicio = proveedorServicio;
            ProveedorServicioId = proveedorServicio.Id;
            Alias = alias;
            NumeroReferencia = numeroReferencia;
        }


        /// <summary>
        /// Actualiza los datos del servicio favorito.
        /// </summary>
        /// <param name="alias">El nuevo alias.</param>
        /// <param name="numeroReferencia">El nuevo número de referencia.</param>
        /// <param name="modificationUser">El usuario que modifica el registro.</param>
        public void Update(string alias, string numeroReferencia, Guid modificationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(nameof(Alias), alias, ref exceptions);
            IsPropertyValid(nameof(NumeroReferencia), numeroReferencia, ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions);
            }

            Alias = alias;
            NumeroReferencia = numeroReferencia;
            base.Update(modificationUser);
        }
    }
}
