using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Enums;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos
{
    /// <summary>
    /// Representa a un proveedor de servicios en el sistema.
    /// </summary>
    public class ProveedorServicio : ValidatablePersistentObjectLogicalDelete
    {
        /// <summary>
        /// Define las restricciones de las propiedades para la validación del objeto ProveedorServicio.
        /// </summary>
        protected override List<PropertyConstraint> PropertyConstraints =>
        [
            PropertyConstraint.StringPropertyConstraint(
                propertyName: nameof(Nombre),
                isRequired: true,
                maximumLength: 100,
                minimumLength: 1),
            PropertyConstraint.ObjectPropertyConstraint(
                propertyName: nameof(Categoria),
                isRequired: true)
        ];

        /// <summary>
        /// Nombre del proveedor de servicios.
        /// </summary>
        [Required]
        [MaxLength(length: 100)]
        public string Nombre { get; private set; }

        /// <summary>
        /// Categoría a la que pertenece el proveedor de servicios.
        /// </summary>
        [Required]
        public ProductoCategoria Categoria { get; private set; }

        /// <summary>
        /// URL del ícono representativo del proveedor.
        /// </summary>
        [MaxLength(length: 255)]
        public string? UrlIcono { get; private set; }

        /// <summary>
        /// Colección de productos ofrecidos por este proveedor.
        /// </summary>
        public ICollection<ProductoProveedor> Productos { get; set; }

        /// <summary>
        /// Colección de servicios favoritos asociados a este proveedor.
        /// </summary>
        public ICollection<ServicioFavorito> ServiciosFavoritos { get; set; }


        /// <summary>
        /// Constructor privado para uso de Entity Framework.
        /// </summary>
        protected ProveedorServicio() : base()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProveedorServicio"/>.
        /// </summary>
        /// <param name="nombre">El nombre del proveedor.</param>
        /// <param name="categoria">La categoría del proveedor.</param>
        /// <param name="urlIcono">La URL del ícono del proveedor (opcional).</param>
        /// <param name="creationUser">El usuario que crea el registro.</param>
        public ProveedorServicio(string nombre, ProductoCategoria categoria, string? urlIcono, Guid creationUser) :
            base(creationUser: creationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Categoria), value: categoria, exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            Nombre = nombre;
            Categoria = categoria;
            UrlIcono = urlIcono;
            Productos = new HashSet<ProductoProveedor>();
            ServiciosFavoritos = new HashSet<ServicioFavorito>();
        }

        /// <summary>
        /// Actualiza los datos del proveedor de servicio.
        /// </summary>
        /// <param name="nombre">El nuevo nombre.</param>
        /// <param name="categoria">La nueva categoría.</param>
        /// <param name="urlIcono">La nueva URL del ícono.</param>
        /// <param name="modificationUser">El usuario que modifica el registro.</param>
        public void Update(string nombre, ProductoCategoria categoria, string? urlIcono, Guid modificationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
            IsPropertyValid(propertyName: nameof(Categoria), value: categoria, exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            Nombre = nombre;
            Categoria = categoria;
            UrlIcono = urlIcono;
            base.Update(modificationUser: modificationUser);
        }

        public ProductoProveedor AgregarProducto(string sku, string nombre, decimal monto, string descripcion, Guid creationUser)
        {
            var producto = new ProductoProveedor(proveedorServicio: this, sku: sku, nombre: nombre, monto: monto, descripcion: descripcion, creationUser: creationUser);
            this.Productos.Add(item: producto);
            return producto;
        }
    }
}
