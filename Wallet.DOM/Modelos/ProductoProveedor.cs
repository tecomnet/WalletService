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
        public int ProveedorServicioId { get; private set; }
        
        /// <summary>
        /// Objeto de navegación para el proveedor de servicios.
        /// </summary>
        [ForeignKey("ProveedorServicioId")]
        public ProveedorServicio ProveedorServicio { get; set; }

        /// <summary>
        /// SKU (Stock Keeping Unit) del producto.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Sku { get; private set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        [Required]
        [MaxLength(100)]
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
        [MaxLength(255)]
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
        /// <param name="proveedorServicioId">El ID del proveedor de servicios.</param>
        /// <param name="sku">El SKU del producto.</param>
        /// <param name="nombre">El nombre del producto.</param>
        /// <param name="monto">El monto del producto.</param>
        /// <param name="descripcion">La descripción del producto.</param>
        /// <param name="creationUser">El usuario que crea el registro.</param>
        public ProductoProveedor(int proveedorServicioId, string sku, string nombre, decimal monto, string descripcion, Guid creationUser) : base(creationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(nameof(Sku), sku, ref exceptions);
            IsPropertyValid(nameof(Nombre), nombre, ref exceptions);
            IsPropertyValid(nameof(Monto), monto, ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions);
            }

            ProveedorServicioId = proveedorServicioId;
            Sku = sku;
            Nombre = nombre;
            Monto = monto;
            // La descripción es opcional, no se valida en el constructor.
            Descripcion = descripcion; 
        }
    }
}
