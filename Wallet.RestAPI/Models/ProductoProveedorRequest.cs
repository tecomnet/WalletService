using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Request para crear o actualizar un producto de proveedor.
    /// </summary>
    [DataContract]
    public class ProductoProveedorRequest : IEquatable<ProductoProveedorRequest>
    {
        /// <summary>
        /// SKU del producto.
        /// </summary>
        [Required]
        [DataMember(Name = "sku")]
        public string Sku { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        [Required]
        [DataMember(Name = "nombre")]
        public string Nombre { get; set; }

        /// <summary>
        /// Monto del producto.
        /// </summary>
        [Required]
        [DataMember(Name = "monto")]
        public decimal? Monto { get; set; }

        /// <summary>
        /// Descripción del producto.
        /// </summary>
        [DataMember(Name = "descripcion")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(value: "class ProductoProveedorRequest {\n");
            sb.Append(value: "  Sku: ").Append(value: Sku).Append(value: "\n");
            sb.Append(value: "  Nombre: ").Append(value: Nombre).Append(value: "\n");
            sb.Append(value: "  Monto: ").Append(value: Monto).Append(value: "\n");
            sb.Append(value: "  Descripcion: ").Append(value: Descripcion).Append(value: "\n");
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
            return obj.GetType() == GetType() && Equals(other: (ProductoProveedorRequest)obj);
        }

        /// <summary>
        /// Returns true if ProductoProveedorRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of ProductoProveedorRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductoProveedorRequest other)
        {
            if (ReferenceEquals(objA: null, objB: other)) return false;
            if (ReferenceEquals(objA: this, objB: other)) return true;

            return
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
                if (Sku != null)
                    hashCode = hashCode * 59 + Sku.GetHashCode();
                if (Nombre != null)
                    hashCode = hashCode * 59 + Nombre.GetHashCode();
                if (Monto != null)
                    hashCode = hashCode * 59 + Monto.GetHashCode();
                if (Descripcion != null)
                    hashCode = hashCode * 59 + Descripcion.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(ProductoProveedorRequest left, ProductoProveedorRequest right)
        {
            return Equals(objA: left, objB: right);
        }

        public static bool operator !=(ProductoProveedorRequest left, ProductoProveedorRequest right)
        {
            return !Equals(objA: left, objB: right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}
