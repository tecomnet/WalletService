using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Estructura para crear/actualizar proveedor de servicio
    /// </summary>
    [DataContract]
    public class ProveedorServicioRequest : IEquatable<ProveedorServicioRequest>
    {
        /// <summary>
        /// Gets or Sets Nombre
        /// </summary>
        [Required]
        [StringLength(maximumLength: 100, MinimumLength=1)]
        [DataMember(Name="nombre")]
        public string Nombre { get; set; }

        /// <summary>
        /// Categoria del producto (Servicios, Recargas, Movilidad)
        /// </summary>
        /// <value>Categoria del producto (Servicios, Recargas, Movilidad)</value>
        [Required]
        [DataMember(Name="categoria")]
        public string Categoria { get; set; }

        /// <summary>
        /// Gets or Sets UrlIcono
        /// </summary>
        [MaxLength(length: 500)]
        [DataMember(Name="urlIcono")]
        public string UrlIcono { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(value: "class ProveedorServicioRequest {\n");
            sb.Append(value: "  Nombre: ").Append(value: Nombre).Append(value: "\n");
            sb.Append(value: "  Categoria: ").Append(value: Categoria).Append(value: "\n");
            sb.Append(value: "  UrlIcono: ").Append(value: UrlIcono).Append(value: "\n");
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
            return obj.GetType() == GetType() && Equals(other: (ProveedorServicioRequest)obj);
        }

        /// <summary>
        /// Returns true if ProveedorServicioRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of ProveedorServicioRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProveedorServicioRequest other)
        {
            if (ReferenceEquals(objA: null, objB: other)) return false;
            if (ReferenceEquals(objA: this, objB: other)) return true;

            return 
                (
                    Nombre == other.Nombre ||
                    Nombre != null &&
                    Nombre.Equals(value: other.Nombre)
                ) && 
                (
                    Categoria == other.Categoria ||
                    Categoria != null &&
                    Categoria.Equals(value: other.Categoria)
                ) && 
                (
                    UrlIcono == other.UrlIcono ||
                    UrlIcono != null &&
                    UrlIcono.Equals(value: other.UrlIcono)
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
                    if (Nombre != null)
                    hashCode = hashCode * 59 + Nombre.GetHashCode();
                    if (Categoria != null)
                    hashCode = hashCode * 59 + Categoria.GetHashCode();
                    if (UrlIcono != null)
                    hashCode = hashCode * 59 + UrlIcono.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(ProveedorServicioRequest left, ProveedorServicioRequest right)
        {
            return Equals(objA: left, objB: right);
        }

        public static bool operator !=(ProveedorServicioRequest left, ProveedorServicioRequest right)
        {
            return !Equals(objA: left, objB: right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
