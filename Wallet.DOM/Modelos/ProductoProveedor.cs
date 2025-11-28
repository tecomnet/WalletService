using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos
{
    /// <summary>
    /// Representa un producto ofrecido por un proveedor de servicios.
    /// </summary>
    public class ProductoProveedor : ValidatablePersistentObjectLogicalDelete
    {
        /// <summary>
        /// Define las restricciones de las propiedades para la validación del objeto ProductoProveedor.
        /// </summary>
        protected override List<PropertyConstraint> PropertyConstraints =>
        [
            PropertyConstraint.StringPropertyConstraint(
                propertyName: nameof(Sku),
                isRequired: true,
                maximumLength: 50,
                minimumLength: 1),
            PropertyConstraint.StringPropertyConstraint(
                propertyName: nameof(Nombre),
                isRequired: true,
                maximumLength: 100,
                minimumLength: 1),
            PropertyConstraint.DecimalPropertyConstraint(
                propertyName: nameof(Monto),
                isRequired: true,
                allowNegative: false,
                allowZero: false,
                allowPositive: true,
                allowedDecimals: 2)
        ];

        /// <summary>
        /// ID del proveedor de servicios al que pertenece este producto.
        /// </summary>
        [Required]
        public int ProveedorServicioId { get; internal set; }

        /// <summary>
        /// Objeto de navegación para el proveedor de servicios.
        /// </summary>
        [ForeignKey(name: "ProveedorServicioId")]
        public ProveedorServicio ProveedorServicio { get; set; }

        /// <summary>
        /// SKU (Stock Keeping Unit) del producto.
        /// </summary>
        [Required]
        [MaxLength(length: 50)]
        public string Sku { get; private set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        [Required]
        [MaxLength(length: 100)]
        public string Nombre { get; private set; }

        /// <summary>
        /// Monto o precio del producto.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(19, 2)")]
        public decimal Monto { get; private set; }

        /// <summary>
        /// Descripción opcional del producto.
        /// </summary>
        [MaxLength(length: 255)]
        public string Descripcion { get; private set; }

        /// <summary>
        /// Constructor privado para uso de Entity Framework.
        /// </summary>
        private ProductoProveedor() : base()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProductoProveedor"/>.
        /// </summary>
        /// <param name="proveedorServicio">El objeto del proveedor de servicios al que pertenece el producto.</param>
        /// <param name="sku">El SKU del producto.</param>
        /// <param name="nombre">El nombre del producto.</param>
        /// <param name="monto">El monto del producto.</param>
        /// <param name="descripcion">La descripción del producto.</param>
        /// <param name="creationUser">El usuario que crea el registro.</param>
        internal ProductoProveedor(ProveedorServicio proveedorServicio, string sku, string nombre, decimal monto, string descripcion,
            Guid creationUser) : base(creationUser: creationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Sku), value: sku, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Monto), value: monto, exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            ProveedorServicio = proveedorServicio;
            ProveedorServicioId = proveedorServicio.Id;
            Sku = sku;
            Nombre = nombre;
            Monto = monto;
            // La descripción es opcional y no se valida en el constructor.
            Descripcion = descripcion;
        }

        /// <summary>
        /// Actualiza los datos del producto.
        /// </summary>
        /// <param name="sku">El nuevo SKU.</param>
        /// <param name="nombre">El nuevo nombre.</param>
        /// <param name="monto">El nuevo monto.</param>
        /// <param name="descripcion">La nueva descripción.</param>
        /// <param name="modificationUser">El usuario que modifica el registro.</param>
        public void Update(string sku, string nombre, decimal monto, string descripcion, Guid modificationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Sku), value: sku, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Monto), value: monto, exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            Sku = sku;
            Nombre = nombre;
            Monto = monto;
            Descripcion = descripcion;
            base.Update(modificationUser: modificationUser);
        }
    }
}
