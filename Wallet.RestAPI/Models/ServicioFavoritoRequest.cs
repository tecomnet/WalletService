using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Estructura para crear/actualizar servicio favorito
    /// </summary>
    [DataContract]
    public class ServicioFavoritoRequest : IEquatable<ServicioFavoritoRequest>
    {
        /// <summary>
        /// Gets or Sets ClienteId
        /// </summary>
        [Required]
        [DataMember(Name = "clienteId")]
        public int? ClienteId { get; set; }

        /// <summary>
        /// Gets or Sets ProveedorId
        /// </summary>
        [Required]
        [DataMember(Name = "proveedorId")]
        public int? ProveedorId { get; set; }

        /// <summary>
        /// Gets or Sets Alias
        /// </summary>
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 1)]
        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or Sets NumeroReferencia
        /// </summary>
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 1)]
        [DataMember(Name = "numeroReferencia")]
        public string NumeroReferencia { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(value: "class ServicioFavoritoRequest {\n");
            sb.Append(value: "  ClienteId: ").Append(value: ClienteId).Append(value: "\n");
            sb.Append(value: "  ProveedorId: ").Append(value: ProveedorId).Append(value: "\n");
            sb.Append(value: "  Alias: ").Append(value: Alias).Append(value: "\n");
            sb.Append(value: "  NumeroReferencia: ").Append(value: NumeroReferencia).Append(value: "\n");
            sb.Append(value: "  }\n");
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
            return obj.GetType() == GetType() && Equals(other: (ServicioFavoritoRequest)obj);
        }

        /// <summary>
        /// Returns true if ServicioFavoritoRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of ServicioFavoritoRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ServicioFavoritoRequest other)
        {
            if (ReferenceEquals(objA: null, objB: other)) return false;
            if (ReferenceEquals(objA: this, objB: other)) return true;

            return
                (
                    ClienteId == other.ClienteId ||
                    ClienteId != null &&
                    ClienteId.Equals(other: other.ClienteId)
                ) &&
                (
                    ProveedorId == other.ProveedorId ||
                    ProveedorId != null &&
                    ProveedorId.Equals(other: other.ProveedorId)
                ) &&
                (
                    Alias == other.Alias ||
                    Alias != null &&
                    Alias.Equals(value: other.Alias)
                ) &&
                (
                    NumeroReferencia == other.NumeroReferencia ||
                    NumeroReferencia != null &&
                    NumeroReferencia.Equals(value: other.NumeroReferencia)
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
                if (ClienteId != null)
                    hashCode = hashCode * 59 + ClienteId.GetHashCode();
                if (ProveedorId != null)
                    hashCode = hashCode * 59 + ProveedorId.GetHashCode();
                if (Alias != null)
                    hashCode = hashCode * 59 + Alias.GetHashCode();
                if (NumeroReferencia != null)
                    hashCode = hashCode * 59 + NumeroReferencia.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(ServicioFavoritoRequest left, ServicioFavoritoRequest right)
        {
            return Equals(objA: left, objB: right);
        }

        public static bool operator !=(ServicioFavoritoRequest left, ServicioFavoritoRequest right)
        {
            return !Equals(objA: left, objB: right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}
