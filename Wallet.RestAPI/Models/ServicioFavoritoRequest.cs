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
        [DataMember(Name="clienteId")]
        public int? ClienteId { get; set; }

        /// <summary>
        /// Gets or Sets ProveedorServicioId
        /// </summary>
        [Required]
        [DataMember(Name="proveedorServicioId")]
        public int? ProveedorServicioId { get; set; }

        /// <summary>
        /// Gets or Sets Alias
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength=1)]
        [DataMember(Name="alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or Sets NumeroReferencia
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength=1)]
        [DataMember(Name="numeroReferencia")]
        public string NumeroReferencia { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ServicioFavoritoRequest {\n");
            sb.Append("  ClienteId: ").Append(ClienteId).Append("\n");
            sb.Append("  ProveedorServicioId: ").Append(ProveedorServicioId).Append("\n");
            sb.Append("  Alias: ").Append(Alias).Append("\n");
            sb.Append("  NumeroReferencia: ").Append(NumeroReferencia).Append("\n");
            sb.Append("  }\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ServicioFavoritoRequest)obj);
        }

        /// <summary>
        /// Returns true if ServicioFavoritoRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of ServicioFavoritoRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ServicioFavoritoRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    ClienteId == other.ClienteId ||
                    ClienteId != null &&
                    ClienteId.Equals(other.ClienteId)
                ) && 
                (
                    ProveedorServicioId == other.ProveedorServicioId ||
                    ProveedorServicioId != null &&
                    ProveedorServicioId.Equals(other.ProveedorServicioId)
                ) && 
                (
                    Alias == other.Alias ||
                    Alias != null &&
                    Alias.Equals(other.Alias)
                ) && 
                (
                    NumeroReferencia == other.NumeroReferencia ||
                    NumeroReferencia != null &&
                    NumeroReferencia.Equals(other.NumeroReferencia)
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
                    if (ProveedorServicioId != null)
                    hashCode = hashCode * 59 + ProveedorServicioId.GetHashCode();
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
            return Equals(left, right);
        }

        public static bool operator !=(ServicioFavoritoRequest left, ServicioFavoritoRequest right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
