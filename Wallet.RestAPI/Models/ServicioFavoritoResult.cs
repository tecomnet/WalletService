using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Estructura para consultar servicios favoritos
    /// </summary>
    [DataContract]
    public class ServicioFavoritoResult : IEquatable<ServicioFavoritoResult>
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [Required]
        [DataMember(Name="id")]
        public int? Id { get; set; }

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
        /// Gets or Sets Guid
        /// </summary>
        [Required]
        [DataMember(Name="guid")]
        public Guid? Guid { get; set; }

        /// <summary>
        /// Gets or Sets CreationTimestamp
        /// </summary>
        [Required]
        [DataMember(Name="creationTimestamp")]
        public DateTime? CreationTimestamp { get; set; }

        /// <summary>
        /// Gets or Sets ModificationTimestamp
        /// </summary>
        [Required]
        [DataMember(Name="modificationTimestamp")]
        public DateTime? ModificationTimestamp { get; set; }

        /// <summary>
        /// Gets or Sets CreationUser
        /// </summary>
        [Required]
        [DataMember(Name="creationUser")]
        public Guid? CreationUser { get; set; }

        /// <summary>
        /// Gets or Sets ModificationUser
        /// </summary>
        [Required]
        [DataMember(Name="modificationUser")]
        public Guid? ModificationUser { get; set; }

        /// <summary>
        /// Gets or Sets IsActive
        /// </summary>
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
            sb.Append("class ServicioFavoritoResult {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  ClienteId: ").Append(ClienteId).Append("\n");
            sb.Append("  ProveedorServicioId: ").Append(ProveedorServicioId).Append("\n");
            sb.Append("  Alias: ").Append(Alias).Append("\n");
            sb.Append("  NumeroReferencia: ").Append(NumeroReferencia).Append("\n");
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
            return obj.GetType() == GetType() && Equals((ServicioFavoritoResult)obj);
        }

        /// <summary>
        /// Returns true if ServicioFavoritoResult instances are equal
        /// </summary>
        /// <param name="other">Instance of ServicioFavoritoResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ServicioFavoritoResult other)
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
                    if (ClienteId != null)
                    hashCode = hashCode * 59 + ClienteId.GetHashCode();
                    if (ProveedorServicioId != null)
                    hashCode = hashCode * 59 + ProveedorServicioId.GetHashCode();
                    if (Alias != null)
                    hashCode = hashCode * 59 + Alias.GetHashCode();
                    if (NumeroReferencia != null)
                    hashCode = hashCode * 59 + NumeroReferencia.GetHashCode();
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

        public static bool operator ==(ServicioFavoritoResult left, ServicioFavoritoResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ServicioFavoritoResult left, ServicioFavoritoResult right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
