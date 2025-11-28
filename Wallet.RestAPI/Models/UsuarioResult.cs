using System;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Estructura para consultar usuarios
    /// </summary>
    [DataContract]
    public partial class UsuarioResult : IEquatable<UsuarioResult>
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [Required]
        [DataMember(Name = "id")]
        public int? Id { get; set; }

        /// <summary>
        /// Gets or Sets CodigoPais
        /// </summary>
        [Required]
        [StringLength(maximumLength: 3, MinimumLength = 3)]
        [DataMember(Name = "codigoPais")]
        public string CodigoPais { get; set; }

        /// <summary>
        /// Gets or Sets Telefono
        /// </summary>
        [Required]
        [StringLength(maximumLength: 10, MinimumLength = 9)]
        [DataMember(Name = "telefono")]
        public string Telefono { get; set; }

        /// <summary>
        /// Gets or Sets CorreoElectronico
        /// </summary>
        [Required]
        [StringLength(maximumLength: 150, MinimumLength = 1)]
        [DataMember(Name = "correoElectronico")]
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Guid of the key-value configuration
        /// </summary>
        /// <value>Guid of the key-value configuration</value>
        [Required]
        [DataMember(Name = "guid")]
        public Guid? Guid { get; set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        /// <value>Creation timestamp</value>
        [Required]
        [DataMember(Name = "creationTimestamp")]
        public DateTime? CreationTimestamp { get; set; }

        /// <summary>
        /// Modification timestamp
        /// </summary>
        /// <value>Modification timestamp</value>
        [Required]
        [DataMember(Name = "modificationTimestamp")]
        public DateTime? ModificationTimestamp { get; set; }

        /// <summary>
        /// Guid of the creation user
        /// </summary>
        /// <value>Guid of the creation user</value>
        [Required]
        [DataMember(Name = "creationUser")]
        public Guid? CreationUser { get; set; }

        /// <summary>
        /// Guid of the modification user
        /// </summary>
        /// <value>Guid of the modification user</value>
        [Required]
        [DataMember(Name = "modificationUser")]
        public Guid? ModificationUser { get; set; }

        /// <summary>
        /// Guid of the modification user
        /// </summary>
        /// <value>Guid of the modification user</value>
        [Required]
        [DataMember(Name = "isActive")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or Sets Estatus
        /// </summary>
        [DataMember(Name = "estatus")]
        public string Estatus { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(value: "class UsuarioResult {\n");
            sb.Append(value: "  Id: ").Append(value: Id).Append(value: "\n");
            sb.Append(value: "  CodigoPais: ").Append(value: CodigoPais).Append(value: "\n");
            sb.Append(value: "  Telefono: ").Append(value: Telefono).Append(value: "\n");
            sb.Append(value: "  CorreoElectronico: ").Append(value: CorreoElectronico).Append(value: "\n");
            sb.Append(value: "  Guid: ").Append(value: Guid).Append(value: "\n");
            sb.Append(value: "  CreationTimestamp: ").Append(value: CreationTimestamp).Append(value: "\n");
            sb.Append(value: "  ModificationTimestamp: ").Append(value: ModificationTimestamp).Append(value: "\n");
            sb.Append(value: "  CreationUser: ").Append(value: CreationUser).Append(value: "\n");
            sb.Append(value: "  ModificationUser: ").Append(value: ModificationUser).Append(value: "\n");
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
            return obj.GetType() == GetType() && Equals(other: (UsuarioResult)obj);
        }

        /// <summary>
        /// Returns true if UsuarioResult instances are equal
        /// </summary>
        /// <param name="other">Instance of UsuarioResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(UsuarioResult other)
        {
            if (ReferenceEquals(objA: null, objB: other)) return false;
            if (ReferenceEquals(objA: this, objB: other)) return true;

            return
                (
                    Id == other.Id ||
                    Id != null &&
                    Id.Equals(other: other.Id)
                ) &&
                (
                    CodigoPais == other.CodigoPais ||
                    CodigoPais != null &&
                    CodigoPais.Equals(value: other.CodigoPais)
                ) &&
                (
                    Telefono == other.Telefono ||
                    Telefono != null &&
                    Telefono.Equals(value: other.Telefono)
                ) &&
                (
                    CorreoElectronico == other.CorreoElectronico ||
                    CorreoElectronico != null &&
                    CorreoElectronico.Equals(value: other.CorreoElectronico)
                ) &&
                (
                    Guid == other.Guid ||
                    Guid != null &&
                    Guid.Equals(other: other.Guid)
                ) &&
                (
                    CreationTimestamp == other.CreationTimestamp ||
                    CreationTimestamp != null &&
                    CreationTimestamp.Equals(other: other.CreationTimestamp)
                ) &&
                (
                    ModificationTimestamp == other.ModificationTimestamp ||
                    ModificationTimestamp != null &&
                    ModificationTimestamp.Equals(other: other.ModificationTimestamp)
                ) &&
                (
                    CreationUser == other.CreationUser ||
                    CreationUser != null &&
                    CreationUser.Equals(other: other.CreationUser)
                ) &&
                (
                    ModificationUser == other.ModificationUser ||
                    ModificationUser != null &&
                    ModificationUser.Equals(other: other.ModificationUser)
                ) &&
                (
                    IsActive == other.IsActive ||
                    IsActive != null &&
                    IsActive.Equals(other: other.IsActive)
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
                if (CodigoPais != null)
                    hashCode = hashCode * 59 + CodigoPais.GetHashCode();
                if (Telefono != null)
                    hashCode = hashCode * 59 + Telefono.GetHashCode();
                if (CorreoElectronico != null)
                    hashCode = hashCode * 59 + CorreoElectronico.GetHashCode();
                if (Guid != null)
                    hashCode = hashCode * 59 + Guid.GetHashCode();
                if (CreationTimestamp != null)
                    hashCode = hashCode * 59 + CreationTimestamp.GetHashCode();
                if (ModificationTimestamp != null)
                    hashCode = hashCode * 59 + ModificationTimestamp.GetHashCode();
                if (CreationUser != null)
                    hashCode = hashCode * 59 + CreationUser.GetHashCode();
                if (ModificationUser != null)
                    hashCode = hashCode * 59 + ModificationUser.GetHashCode();
                if (IsActive != null)
                    hashCode = hashCode * 59 + IsActive.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(UsuarioResult left, UsuarioResult right)
        {
            return Equals(objA: left, objB: right);
        }

        public static bool operator !=(UsuarioResult left, UsuarioResult right)
        {
            return !Equals(objA: left, objB: right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}
