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
    public class ProveedorRequest : IEquatable<ProveedorRequest>
    {
        /// <summary>
        /// Gets or Sets Nombre
        /// </summary>
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 1)]
        [DataMember(Name = "nombre")]
        public string Nombre { get; set; }

        /// <summary>
        /// Gets or Sets BrokerId
        /// </summary>
        [Required]
        [DataMember(Name = "brokerId")]
        public int BrokerId { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(value: "class ProveedorRequest {\n");
            sb.Append(value: "  Nombre: ").Append(value: Nombre).Append(value: "\n");
            sb.Append(value: "  BrokerId: ").Append(value: BrokerId).Append(value: "\n");
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
            return obj.GetType() == GetType() && Equals(other: (ProveedorRequest)obj);
        }

        /// <summary>
        /// Returns true if ProveedorRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of ProveedorRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProveedorRequest other)
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
                    BrokerId == other.BrokerId ||
                    BrokerId.Equals(other.BrokerId)
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

                hashCode = hashCode * 59 + BrokerId.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(ProveedorRequest left, ProveedorRequest right)
        {
            return Equals(objA: left, objB: right);
        }

        public static bool operator !=(ProveedorRequest left, ProveedorRequest right)
        {
            return !Equals(objA: left, objB: right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}
