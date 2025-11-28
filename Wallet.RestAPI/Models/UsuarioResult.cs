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

        [DataMember(Name="id")]
        public int? Id { get; set; }

        /// <summary>
        /// Gets or Sets CodigoPais
        /// </summary>
        [Required]

        [StringLength(3, MinimumLength=3)]
        [DataMember(Name="codigoPais")]
        public string CodigoPais { get; set; }

        /// <summary>
        /// Gets or Sets Telefono
        /// </summary>
        [Required]

        [StringLength(10, MinimumLength=9)]
        [DataMember(Name="telefono")]
        public string Telefono { get; set; }

        /// <summary>
        /// Gets or Sets CorreoElectronico
        /// </summary>
        [Required]

        [StringLength(150, MinimumLength=1)]
        [DataMember(Name="correoElectronico")]
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Guid of the key-value configuration
        /// </summary>
        /// <value>Guid of the key-value configuration</value>
        [Required]

        [DataMember(Name="guid")]
        public Guid? Guid { get; set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        /// <value>Creation timestamp</value>
        [Required]

        [DataMember(Name="creationTimestamp")]
        public DateTime? CreationTimestamp { get; set; }

        /// <summary>
        /// Modification timestamp
        /// </summary>
        /// <value>Modification timestamp</value>
        [Required]

        [DataMember(Name="modificationTimestamp")]
        public DateTime? ModificationTimestamp { get; set; }

        /// <summary>
        /// Guid of the creation user
        /// </summary>
        /// <value>Guid of the creation user</value>
        [Required]

        [DataMember(Name="creationUser")]
        public Guid? CreationUser { get; set; }

        /// <summary>
        /// Guid of the modification user
        /// </summary>
        /// <value>Guid of the modification user</value>
        [Required]

        [DataMember(Name="modificationUser")]
        public Guid? ModificationUser { get; set; }

        /// <summary>
        /// Guid of the modification user
        /// </summary>
        /// <value>Guid of the modification user</value>
        [Required]

        [DataMember(Name="isActive")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class UsuarioResult {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  CodigoPais: ").Append(CodigoPais).Append("\n");
            sb.Append("  Telefono: ").Append(Telefono).Append("\n");
            sb.Append("  CorreoElectronico: ").Append(CorreoElectronico).Append("\n");
            sb.Append("  Guid: ").Append(Guid).Append("\n");
            sb.Append("  CreationTimestamp: ").Append(CreationTimestamp).Append("\n");
            sb.Append("  ModificationTimestamp: ").Append(ModificationTimestamp).Append("\n");
            sb.Append("  CreationUser: ").Append(CreationUser).Append("\n");
            sb.Append("  ModificationUser: ").Append(ModificationUser).Append("\n");
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
            return obj.GetType() == GetType() && Equals((UsuarioResult)obj);
        }

        /// <summary>
        /// Returns true if UsuarioResult instances are equal
        /// </summary>
        /// <param name="other">Instance of UsuarioResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(UsuarioResult other)
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
                    CodigoPais == other.CodigoPais ||
                    CodigoPais != null &&
                    CodigoPais.Equals(other.CodigoPais)
                ) && 
                (
                    Telefono == other.Telefono ||
                    Telefono != null &&
                    Telefono.Equals(other.Telefono)
                ) && 
                (
                    CorreoElectronico == other.CorreoElectronico ||
                    CorreoElectronico != null &&
                    CorreoElectronico.Equals(other.CorreoElectronico)
                ) && 
                (
                    Guid == other.Guid ||
                    Guid != null &&
                    Guid.Equals(other.Guid)
                ) && 
                (
                    CreationTimestamp == other.CreationTimestamp ||
                    CreationTimestamp != null &&
                    CreationTimestamp.Equals(other.CreationTimestamp)
                ) && 
                (
                    ModificationTimestamp == other.ModificationTimestamp ||
                    ModificationTimestamp != null &&
                    ModificationTimestamp.Equals(other.ModificationTimestamp)
                ) && 
                (
                    CreationUser == other.CreationUser ||
                    CreationUser != null &&
                    CreationUser.Equals(other.CreationUser)
                ) && 
                (
                    ModificationUser == other.ModificationUser ||
                    ModificationUser != null &&
                    ModificationUser.Equals(other.ModificationUser)
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
            return Equals(left, right);
        }

        public static bool operator !=(UsuarioResult left, UsuarioResult right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
