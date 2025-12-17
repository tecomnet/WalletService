using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;
using Wallet.DOM.Modelos.GestionEmpresa;

namespace Wallet.DOM.Modelos.GestionCliente
{
    /// <summary>
    /// Representa un servicio guardado como favorito por un cliente.
    /// </summary>
    public class ServicioFavorito : ValidatablePersistentObjectLogicalDelete
    {
        /// <summary>
        /// Define las restricciones y validaciones para las propiedades de la entidad <see cref="ServicioFavorito"/>.
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
        /// ID del cliente al que pertenece este servicio favorito.
        /// </summary>
        [Required]
        public int ClienteId { get; private set; }

        /// <summary>
        /// Objeto de navegación que representa el cliente asociado a este servicio favorito.
        /// </summary>
        [ForeignKey(name: "ClienteId")]
        public Cliente Cliente { get; set; }

        /// <summary>
        /// ID del proveedor del servicio favorito.
        /// </summary>
        [Required]
        public int ProveedorId { get; private set; }

        /// <summary>
        /// Objeto de navegación que representa el proveedor asociado.
        /// </summary>
        [ForeignKey(name: "ProveedorId")]
        public Proveedor Proveedor { get; set; }

        /// <summary>
        /// Alias o nombre personalizado asignado por el cliente al servicio favorito.
        /// </summary>
        [Required]
        [MaxLength(length: 50)]
        public string Alias { get; private set; }

        /// <summary>
        /// Número de referencia único asociado al servicio (ej. número de cuenta, contrato, etc.).
        /// </summary>
        [Required]
        [MaxLength(length: 50)]
        public string NumeroReferencia { get; private set; }

        /// <summary>
        /// Constructor privado requerido por Entity Framework para la creación de instancias.
        /// </summary>
        private ServicioFavorito() : base()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServicioFavorito"/> con los IDs de cliente y proveedor.
        /// </summary>
        /// <param name="clienteId">El ID único del cliente.</param>
        /// <param name="proveedorId">El ID único del proveedor.</param>
        /// <param name="alias">El alias o nombre personalizado para el servicio.</param>
        /// <param name="numeroReferencia">El número de referencia asociado al servicio.</param>
        /// <param name="creationUser">El identificador del usuario que crea el registro.</param>
        /// <exception cref="EMGeneralAggregateException">Se lanza si las validaciones de las propiedades fallan.</exception>
        public ServicioFavorito(int clienteId, int proveedorId, string alias, string numeroReferencia,
            Guid creationUser) : base(creationUser: creationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Alias), value: alias, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(NumeroReferencia), value: numeroReferencia,
                exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            ClienteId = clienteId;
            ProveedorId = proveedorId;
            Alias = alias;
            NumeroReferencia = numeroReferencia;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ServicioFavorito"/> utilizando objetos de Cliente y Proveedor.
        /// </summary>
        /// <param name="cliente">El objeto <see cref="Cliente"/> asociado.</param>
        /// <param name="proveedor">El objeto <see cref="Proveedor"/> asociado.</param>
        /// <param name="alias">El alias o nombre personalizado para el servicio.</param>
        /// <param name="numeroReferencia">El número de referencia asociado al servicio.</param>
        /// <param name="creationUser">El identificador del usuario que crea el registro.</param>
        /// <exception cref="EMGeneralAggregateException">Se lanza si las validaciones de las propiedades fallan.</exception>
        public ServicioFavorito(Cliente cliente, Proveedor proveedor, string alias,
            string numeroReferencia, Guid creationUser) : base(creationUser: creationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Alias), value: alias, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(NumeroReferencia), value: numeroReferencia,
                exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            Cliente = cliente;
            ClienteId = cliente.Id;
            Proveedor = proveedor;
            ProveedorId = proveedor.Id;
            Alias = alias;
            NumeroReferencia = numeroReferencia;
        }


        /// <summary>
        /// Actualiza los datos editables del servicio favorito, como el alias y el número de referencia.
        /// </summary>
        /// <param name="alias">El nuevo alias para el servicio.</param>
        /// <param name="numeroReferencia">El nuevo número de referencia del servicio.</param>
        /// <param name="modificationUser">El identificador del usuario que realiza la modificación.</param>
        /// <exception cref="EMGeneralAggregateException">Se lanza si las validaciones de las propiedades fallan durante la actualización.</exception>
        public void Update(string alias, string numeroReferencia, Guid modificationUser)
        {
            var exceptions = new List<EMGeneralException>();

            IsPropertyValid(propertyName: nameof(Alias), value: alias, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(NumeroReferencia), value: numeroReferencia,
                exceptions: ref exceptions);

            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            bool hasChanges = false;
            if (this.Alias != alias)
            {
                Alias = alias;
                hasChanges = true;
            }

            if (this.NumeroReferencia != numeroReferencia)
            {
                NumeroReferencia = numeroReferencia;
                hasChanges = true;
            }

            if (hasChanges)
            {
                base.Update(modificationUser: modificationUser);
            }
        }
    }
}

