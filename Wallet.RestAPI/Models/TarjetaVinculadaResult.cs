using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    [DataContract]
    public partial class TarjetaVinculadaResult : IEquatable<TarjetaVinculadaResult>
    {
        [Required] [DataMember(Name = "id")] public int? Id { get; set; }

        [Required]
        [DataMember(Name = "panEnmascarado")]
        public string PanEnmascarado { get; set; }

        [Required]
        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        [Required]
        [DataMember(Name = "marca")]
        public MarcaTarjetaEnum? Marca { get; set; }

        [Required]
        [DataMember(Name = "esFavorita")]
        public bool? EsFavorita { get; set; }

        [DataMember(Name = "isActive")] public bool? IsActive { get; set; }

        [DataMember(Name = "concurrencyToken")]
        public string ConcurrencyToken { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TarjetaVinculadaResult)obj);
        }

        public bool Equals(TarjetaVinculadaResult other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id &&
                   string.Equals(PanEnmascarado, other.PanEnmascarado) &&
                   string.Equals(Alias, other.Alias) &&
                   Marca == other.Marca &&
                   EsFavorita == other.EsFavorita &&
                   IsActive == other.IsActive &&
                   string.Equals(ConcurrencyToken, other.ConcurrencyToken);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PanEnmascarado != null ? PanEnmascarado.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Alias != null ? Alias.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Marca != null ? Marca.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EsFavorita != null ? EsFavorita.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (IsActive != null ? IsActive.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ConcurrencyToken != null ? ConcurrencyToken.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
