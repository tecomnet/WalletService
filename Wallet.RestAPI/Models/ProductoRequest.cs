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
    public class ProductoRequest : IEquatable<ProductoRequest>
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
        /// Precio del producto.
        /// </summary>
        [Required]
        [DataMember(Name = "precio")]
        public decimal? Precio { get; set; }

        /// <summary>
        /// Icono del producto.
        /// </summary>
        [Required]
        [DataMember(Name = "icono")]
        public string Icono { get; set; }

        /// <summary>
        /// Categoría del producto.
        /// </summary>
        [Required]
        [DataMember(Name = "categoria")]
        public string Categoria { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(value: "class ProductoRequest {\n");
            sb.Append(value: "  Sku: ").Append(value: Sku).Append(value: "\n");
            sb.Append(value: "  Nombre: ").Append(value: Nombre).Append(value: "\n");
            sb.Append(value: "  Precio: ").Append(value: Precio).Append(value: "\n");
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
            return obj.GetType() == GetType() && Equals(other: (ProductoRequest)obj);
        }

        /// <summary>
        /// Returns true if ProductoRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of ProductoRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductoRequest other)
        {
            if (ReferenceEquals(objA: null, objB: other)) return false;
            if (ReferenceEquals(objA: this, objB: other)) return true;

            return
                (
                    Sku == other.Sku ||
                    Sku != null &&
                    Sku.Equals(value: other.Sku)
                ) &&
                (
                    Nombre == other.Nombre ||
                    Nombre != null &&
                    Nombre.Equals(value: other.Nombre)
                ) &&
                (
                    Precio == other.Precio ||
                    Precio != null &&
                    Precio.Equals(other: other.Precio)
                ) &&
                (
                    Icono == other.Icono ||
                    Icono != null &&
                    Icono.Equals(value: other.Icono)
                ) &&
                (
                    Categoria == other.Categoria ||
                    Categoria != null &&
                    Categoria.Equals(value: other.Categoria)
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
                if (Precio != null)
                    hashCode = hashCode * 59 + Precio.GetHashCode();
                if (Icono != null)
                    hashCode = hashCode * 59 + Icono.GetHashCode();
                if (Categoria != null)
                    hashCode = hashCode * 59 + Categoria.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(ProductoRequest left, ProductoRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProductoRequest left, ProductoRequest right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}
