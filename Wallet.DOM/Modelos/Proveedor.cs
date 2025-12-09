using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos
{
    /// <summary>
    /// Representa un proveedor en el sistema.
    /// </summary>
    public class Proveedor : ValidatablePersistentObjectLogicalDelete
    {
        /// <summary>
        /// Define las restricciones de las propiedades para la validación del objeto <see cref="Proveedor"/>.
        /// </summary>
        protected override List<PropertyConstraint> PropertyConstraints =>
        [
            PropertyConstraint.StringPropertyConstraint(
                propertyName: nameof(Nombre),
                isRequired: true,
                maximumLength: 100,
                minimumLength: 1)
        ];

        /// <summary>
        /// Obtiene el nombre del proveedor.
        /// </summary>
        [Required]
        [MaxLength(length: 100)]
        public string Nombre { get; private set; }
        
        [Required]
        [MaxLength(length: 255)]
        public string UrlIcono { get; private set; }

        /// <summary>
        /// ID del broker al que pertenece este proveedor.
        /// </summary>
        [Required]
        public int BrokerId { get; internal set; }

        /// <summary>
        /// Objeto de navegación para el broker.
        /// </summary>
        [ForeignKey(name: "BrokerId")]
        public Broker Broker { get; set; }

        /// <summary>
        /// Obtiene la colección de productos ofrecidos por este proveedor.
        /// </summary>
        public ICollection<Producto> Productos { get; set; }

        /// <summary>
        /// Constructor privado para uso exclusivo de Entity Framework.
        /// </summary>
        protected Proveedor() : base()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Proveedor"/>.
        /// </summary>
        /// <param name="nombre">El nombre del proveedor.</param>
        /// <param name="urlIcono">La URL del ícono del proveedor.</param>
        /// <param name="broker">El broker asociado.</param>
        /// <param name="creationUser">El identificador del usuario que crea el registro.</param>
        public Proveedor(string nombre, string urlIcono, Broker broker, Guid creationUser) :
            base(creationUser: creationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(UrlIcono), value: urlIcono, exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            Nombre = nombre;
            Broker = broker;
            BrokerId = broker.Id;
            Productos = new HashSet<Producto>();
        }

        /// <summary>
        /// Actualiza los datos del proveedor.
        /// </summary>
        /// <param name="nombre">El nuevo nombre del proveedor.</param>
        /// <param name="urlIcono">La URL del ícono del proveedor.</param>
        /// <param name="modificationUser">El identificador del usuario que modifica el registro.</param>
        public void Update(string nombre, string urlIcono, Guid modificationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(UrlIcono), value: urlIcono, exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            Nombre = nombre;
            base.Update(modificationUser: modificationUser);
        }

        /// <summary>
        /// Agrega un nuevo producto a la colección de productos ofrecidos por el proveedor.
        /// </summary>
        /// <param name="sku">El SKU (Stock Keeping Unit) único del producto.</param>
        /// <param name="nombre">El nombre del producto.</param>
        /// <param name="precio">El precio del producto.</param>
        /// <param name="icono">El ícono del producto.</param>
        /// <param name="categoria">La categoría del producto.</param>
        /// <param name="creationUser">El identificador del usuario que crea el producto.</param>
        /// <returns>El objeto <see cref="Producto"/> recién creado y agregado a la colección.</returns>
        public Producto AgregarProducto(string sku, string nombre, decimal precio, string icono, string categoria,
            Guid creationUser)
        {
            var producto = new Producto(proveedor: this, sku: sku, nombre: nombre, precio: precio, urlIcono: icono,
                categoria: categoria, creationUser: creationUser);
            this.Productos.Add(item: producto); // Agrega el producto a la colección local.
            return producto;
        }
    }
}

