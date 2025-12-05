using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Resultado de una operación con ProductoProveedor.
    /// </summary>
    [DataContract]
    public class ProductoProveedorResult : IEquatable<ProductoProveedorResult>
    {
        /// <summary>
        /// Identificador del producto.
        /// </summary>
        [DataMember(Name = "id")]
        public int? Id { get; set; }

        /// <summary>
        /// Identificador del proveedor de servicio.
        /// </summary>
        [DataMember(Name = "proveedorServicioId")]
        public int? ProveedorServicioId { get; set; }

        /// <summary>
        /// SKU del producto.
        /// </summary>
        [DataMember(Name = "sku")]
        public string Sku { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        [DataMember(Name = "nombre")]
        public string Nombre { get; set; }

        /// <summary>
        /// Monto del producto.
        /// </summary>
        [DataMember(Name = "monto")]
        public decimal? Monto { get; set; }

        /// <summary>
        /// Descripción del producto.
        /// </summary>
        [DataMember(Name = "descripcion")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Indica si el producto está activo.
        /// </summary>
        [DataMember(Name = "isActive")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(value: "class ProductoProveedorResult {\n");
            sb.Append(value: "  Id: ").Append(value: Id).Append(value: "\n");
            sb.Append(value: "  ProveedorServicioId: ").Append(value: ProveedorServicioId).Append(value: "\n");
            sb.Append(value: "  Sku: ").Append(value: Sku).Append(value: "\n");
            sb.Append(value: "  Nombre: ").Append(value: Nombre).Append(value: "\n");
            sb.Append(value: "  Monto: ").Append(value: Monto).Append(value: "\n");
            sb.Append(value: "  Descripcion: ").Append(value: Descripcion).Append(value: "\n");
            sb.Append(value: "  IsActive: ").Append(value: IsActive).Append(value: "\n");
            sb.Append(value: "}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(value: this, formatting: Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(objA: null, objB: obj)) return false;
            if (ReferenceEquals(objA: this, objB: obj)) return true;
            return obj.GetType() == GetType() && Equals(other: (ProductoProveedorResult)obj);
        }

        /// <summary>
        /// Returns true if ProductoProveedorResult instances are equal
        /// </summary>
        /// <param name="other">Instance of ProductoProveedorResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductoProveedorResult other)
        {
            if (ReferenceEquals(objA: null, objB: other)) return false;
            if (ReferenceEquals(objA: this, objB: other)) return true;

            return
                (
                    Id == other.Id ||
                    Id != null &&
                    Id.Equals(other.Id)
                ) &&
                (
                    ProveedorServicioId == other.ProveedorServicioId ||
                    ProveedorServicioId != null &&
                    ProveedorServicioId.Equals(other.ProveedorServicioId)
                ) &&
                (
                    Sku == other.Sku ||
                    Sku != null &&
                    Sku.Equals(other.Sku)
                ) &&
                (
                    Nombre == other.Nombre ||
                    Nombre != null &&
                    Nombre.Equals(other.Nombre)
                ) &&
                (
                    Monto == other.Monto ||
                    Monto != null &&
                    Monto.Equals(other.Monto)
                ) &&
                (
                    Descripcion == other.Descripcion ||
                    Descripcion != null &&
                    Descripcion.Equals(other.Descripcion)
                ) &&
                (
                    IsActive == other.IsActive ||
                    IsActive != null &&
                    IsActive.Equals(other.IsActive)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                if (Id != null)
                    hashCode = hashCode * 59 + Id.GetHashCode();
                if (ProveedorServicioId != null)
                    hashCode = hashCode * 59 + ProveedorServicioId.GetHashCode();
                if (Sku != null)
                    hashCode = hashCode * 59 + Sku.GetHashCode();
                if (Nombre != null)
                    hashCode = hashCode * 59 + Nombre.GetHashCode();
                if (Monto != null)
                    hashCode = hashCode * 59 + Monto.GetHashCode();
                if (Descripcion != null)
                    hashCode = hashCode * 59 + Descripcion.GetHashCode();
                if (IsActive != null)
                    hashCode = hashCode * 59 + IsActive.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(ProductoProveedorResult left, ProductoProveedorResult right)
        {
            return Equals(objA: left, objB: right);
        }

        public static bool operator !=(ProductoProveedorResult left, ProductoProveedorResult right)
        {
            return !Equals(objA: left, objB: right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}
