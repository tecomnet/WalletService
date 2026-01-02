using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    [DataContract]
    public partial class VincularTarjetaRequest : IEquatable<VincularTarjetaRequest>
    {
        [Required]
        [DataMember(Name = "numeroTarjeta")]
        public string NumeroTarjeta { get; set; }

        [Required]
        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        [Required]
        [DataMember(Name = "marca")]
        public MarcaTarjetaEnum? Marca { get; set; }

        [Required]
        [DataMember(Name = "fechaExpiracion")]
        public DateTime? FechaExpiracion { get; set; }

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
            return Equals((VincularTarjetaRequest)obj);
        }

        public bool Equals(VincularTarjetaRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(NumeroTarjeta, other.NumeroTarjeta) &&
                   string.Equals(Alias, other.Alias) &&
                   Marca == other.Marca &&
                   FechaExpiracion == other.FechaExpiracion;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (NumeroTarjeta != null ? NumeroTarjeta.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Alias != null ? Alias.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Marca != null ? Marca.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FechaExpiracion != null ? FechaExpiracion.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
