using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// ConsentimientoUsuarioResult
    /// </summary>
    [DataContract]
    public class ConsentimientoUsuarioResult : IEquatable<ConsentimientoUsuarioResult>
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? Id { get; set; }

        /// <summary>
        /// Gets or Sets IdUsuario
        /// </summary>
        [DataMember(Name = "idUsuario", EmitDefaultValue = false)]
        public int? IdUsuario { get; set; }

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
        /// Gets or Sets FechaAceptacion
        /// </summary>
        [DataMember(Name = "fechaAceptacion", EmitDefaultValue = false)]
        public DateTime? FechaAceptacion { get; set; }

        /// <summary>
        /// Gets or Sets IsActive
        /// </summary>
        [DataMember(Name = "isActive", EmitDefaultValue = false)]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ConsentimientoUsuarioResult {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  IdUsuario: ").Append(IdUsuario).Append("\n");
            sb.Append("  TipoDocumento: ").Append(TipoDocumento).Append("\n");
            sb.Append("  Version: ").Append(Version).Append("\n");
            sb.Append("  FechaAceptacion: ").Append(FechaAceptacion).Append("\n");
            sb.Append("  IsActive: ").Append(IsActive).Append("\n");
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
            return obj.GetType() == GetType() && Equals((ConsentimientoUsuarioResult)obj);
        }

        /// <summary>
        /// Returns true if ConsentimientoUsuarioResult instances are equal
        /// </summary>
        /// <param name="other">Instance of ConsentimientoUsuarioResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ConsentimientoUsuarioResult other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    Id == other.Id ||
                    Id != null &&
                    Id.Equals(other.Id)
                ) &&
                (
                    IdUsuario == other.IdUsuario ||
                    IdUsuario != null &&
                    IdUsuario.Equals(other.IdUsuario)
                ) &&
                (
                    TipoDocumento == other.TipoDocumento ||
                    TipoDocumento != null &&
                    TipoDocumento.Equals(other.TipoDocumento)
                ) &&
                (
                    Version == other.Version ||
                    Version != null &&
                    Version.Equals(other.Version)
                ) &&
                (
                    FechaAceptacion == other.FechaAceptacion ||
                    FechaAceptacion != null &&
                    FechaAceptacion.Equals(other.FechaAceptacion)
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
                if (Id != null)
                    hashCode = hashCode * 59 + Id.GetHashCode();
                if (IdUsuario != null)
                    hashCode = hashCode * 59 + IdUsuario.GetHashCode();
                if (TipoDocumento != null)
                    hashCode = hashCode * 59 + TipoDocumento.GetHashCode();
                if (Version != null)
                    hashCode = hashCode * 59 + Version.GetHashCode();
                if (FechaAceptacion != null)
                    hashCode = hashCode * 59 + FechaAceptacion.GetHashCode();
                if (IsActive != null)
                    hashCode = hashCode * 59 + IsActive.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(ConsentimientoUsuarioResult left, ConsentimientoUsuarioResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ConsentimientoUsuarioResult left, ConsentimientoUsuarioResult right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}
