using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// ConsentimientoUsuarioRequest
    /// </summary>
    [DataContract]
    public class ConsentimientoUsuarioRequest : IEquatable<ConsentimientoUsuarioRequest>
    {
        /// <summary>
        /// Gets or Sets TipoDocumento
        /// </summary>
        [DataMember(Name = "tipoDocumento", EmitDefaultValue = false)]
        public TipoDocumentoConsentimientoEnum? TipoDocumento { get; set; }

        /// <summary>
        /// Gets or Sets Version
        /// </summary>
        [DataMember(Name = "version", EmitDefaultValue = false)]
        public string Version { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ConsentimientoUsuarioRequest {\n");
            sb.Append("  TipoDocumento: ").Append(TipoDocumento).Append("\n");
            sb.Append("  Version: ").Append(Version).Append("\n");
            sb.Append("}\n");
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
            return obj.GetType() == GetType() && Equals((ConsentimientoUsuarioRequest)obj);
        }

        /// <summary>
        /// Returns true if ConsentimientoUsuarioRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of ConsentimientoUsuarioRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ConsentimientoUsuarioRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    TipoDocumento == other.TipoDocumento ||
                    TipoDocumento != null &&
                    TipoDocumento.Equals(other.TipoDocumento)
                ) &&
                (
                    Version == other.Version ||
                    Version != null &&
                    Version.Equals(other.Version)
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
                if (TipoDocumento != null)
                    hashCode = hashCode * 59 + TipoDocumento.GetHashCode();
                if (Version != null)
                    hashCode = hashCode * 59 + Version.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(ConsentimientoUsuarioRequest left, ConsentimientoUsuarioRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ConsentimientoUsuarioRequest left, ConsentimientoUsuarioRequest right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}
