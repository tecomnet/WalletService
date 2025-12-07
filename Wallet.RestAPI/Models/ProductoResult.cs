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
    public class ProductoResult : IEquatable<ProductoResult>
    {
        /// <summary>
        /// Identificador del producto.
        /// </summary>
        [DataMember(Name = "id")]
        public int? Id { get; set; }

        /// <summary>
        /// Identificador del proveedor.
        /// </summary>
        [DataMember(Name = "proveedorId")]
        public int? ProveedorId { get; set; }

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
        /// Precio del producto.
        /// </summary>
        [DataMember(Name = "precio")]
        public decimal? Precio { get; set; }

        /// <summary>
        /// Icono del producto.
        /// </summary>
        [DataMember(Name = "icono")]
        public string Icono { get; set; }

        /// <summary>
        /// Categoría del producto.
        /// </summary>
        [DataMember(Name = "categoria")]
        public string Categoria { get; set; }

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
            sb.Append(value: "class ProductoResult {\n");
            sb.Append(value: "  Id: ").Append(value: Id).Append(value: "\n");
            sb.Append(value: "  ProveedorId: ").Append(value: ProveedorId).Append(value: "\n");
            sb.Append(value: "  Sku: ").Append(value: Sku).Append(value: "\n");
            sb.Append(value: "  Nombre: ").Append(value: Nombre).Append(value: "\n");
            sb.Append(value: "  Precio: ").Append(value: Precio).Append(value: "\n");
            sb.Append(value: "  Icono: ").Append(value: Icono).Append(value: "\n");
            sb.Append(value: "  Categoria: ").Append(value: Categoria).Append(value: "\n");
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
            return obj.GetType() == GetType() && Equals(other: (ProductoResult)obj);
        }

        /// <summary>
        /// Returns true if ProductoResult instances are equal
        /// </summary>
        /// <param name="other">Instance of ProductoResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductoResult other)
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
                    ProveedorId == other.ProveedorId ||
                    ProveedorId != null &&
                    ProveedorId.Equals(other.ProveedorId)
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
                    Precio == other.Precio ||
                    Precio != null &&
                    Precio.Equals(other.Precio)
                ) &&
                (
                    Icono == other.Icono ||
                    Icono != null &&
                    Icono.Equals(other.Icono)
                ) &&
                (
                    Categoria == other.Categoria ||
                    Categoria != null &&
                    Categoria.Equals(other.Categoria)
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
                if (ProveedorId != null)
                    hashCode = hashCode * 59 + ProveedorId.GetHashCode();
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
                if (IsActive != null)
                    hashCode = hashCode * 59 + IsActive.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(ProductoResult left, ProductoResult right)
        {
            return Equals(objA: left, objB: right);
        }

        public static bool operator !=(ProductoResult left, ProductoResult right)
        {
            return !Equals(objA: left, objB: right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}
