using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos
{
    /// <summary>
    /// Representa un producto ofrecido por un proveedor.
    /// </summary>
    public class Producto : ValidatablePersistentObjectLogicalDelete
    {
        /// <summary>
        /// Define las restricciones de las propiedades para la validación del objeto Producto.
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
                propertyName: nameof(Precio),
                isRequired: false,
                allowNegative: false,
                allowZero: true,
                allowPositive: true,
                allowedDecimals: 2),
            PropertyConstraint.StringPropertyConstraint(
                propertyName: nameof(UrlIcono),
                isRequired: false,
                maximumLength: 255,
                minimumLength: 0),
            PropertyConstraint.StringPropertyConstraint(
                propertyName: nameof(Categoria),
                isRequired: false,
                maximumLength: 100,
                minimumLength: 0)
        ];

        /// <summary>
        /// ID del proveedor al que pertenece este producto.
        /// </summary>
        [Required]
        public int ProveedorId { get; internal set; }

        /// <summary>
        /// Objeto de navegación para el proveedor.
        /// </summary>
        [ForeignKey(name: "ProveedorId")]
        public Proveedor Proveedor { get; set; }

        /// <summary>
        /// Colección de empresas que ofrecen este producto.
        /// </summary>
        public ICollection<Empresa> Empresas { get; set; } = new List<Empresa>();

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
        /// Precio del producto.
        /// </summary>
        [Column(TypeName = "decimal(19, 2)")]
        public decimal? Precio { get; private set; }

        /// <summary>
        /// Ícono del producto.
        /// </summary>
        [Required]
        [MaxLength(length: 255)]
        public string UrlIcono { get; private set; }

        /// <summary>
        /// Categoría del producto.
        /// </summary>
        [Required]
        [MaxLength(length: 100)]
        public string Categoria { get; private set; }

        /// <summary>
        /// Constructor privado para uso de Entity Framework.
        /// </summary>
        private Producto() : base()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Producto"/>.
        /// </summary>
        /// <param name="proveedor">El objeto del proveedor al que pertenece el producto.</param>
        /// <param name="sku">El SKU del producto.</param>
        /// <param name="nombre">El nombre del producto.</param>
        /// <param name="precio">El precio del producto.</param>
        /// <param name="urlIcono">El ícono del producto.</param>
        /// <param name="categoria">La categoría del producto.</param>
        /// <param name="creationUser">El usuario que crea el registro.</param>
        internal Producto(Proveedor proveedor, string sku, string nombre, decimal precio, string? urlIcono,
            string? categoria,
            Guid creationUser) : base(creationUser: creationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Sku), value: sku, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Precio), value: precio, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(UrlIcono), value: urlIcono, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Categoria), value: categoria, exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            Proveedor = proveedor;
            ProveedorId = proveedor.Id;
            Sku = sku;
            Nombre = nombre;
            Precio = precio;
            UrlIcono = urlIcono;
            Categoria = categoria;
        }

        /// <summary>
        /// Actualiza los datos del producto.
        /// </summary>
        /// <param name="sku">El nuevo SKU.</param>
        /// <param name="nombre">El nuevo nombre.</param>
        /// <param name="precio">El nuevo precio.</param>
        /// <param name="urlIcono">El nuevo ícono.</param>
        /// <param name="categoria">La nueva categoría.</param>
        /// <param name="modificationUser">El usuario que modifica el registro.</param>
        public void Update(string sku, string nombre, decimal precio, string? urlIcono, string? categoria,
            Guid modificationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Sku), value: sku, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Precio), value: precio, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(UrlIcono), value: urlIcono, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Categoria), value: categoria, exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            Sku = sku;
            Nombre = nombre;
            Precio = precio;
            UrlIcono = urlIcono;
            Categoria = categoria;
            base.Update(modificationUser: modificationUser);
        }
    }
}

