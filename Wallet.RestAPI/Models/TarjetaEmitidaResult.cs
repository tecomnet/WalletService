using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    [DataContract]
    public partial class TarjetaEmitidaResult : IEquatable<TarjetaEmitidaResult>
    {
        [Required] [DataMember(Name = "id")] public int? Id { get; set; }

        [Required]
        [DataMember(Name = "panEnmascarado")]
        public string PanEnmascarado { get; set; }

        [Required]
        [DataMember(Name = "fechaExpiracion")]
        public DateTime? FechaExpiracion { get; set; }

        [Required]
        [DataMember(Name = "estado")]
        public EstadoTarjetaEnum? Estado { get; set; }

        [Required] [DataMember(Name = "tipo")] public TipoTarjetaEnum? Tipo { get; set; }

        [Required]
        [DataMember(Name = "limiteDiario")]
        public decimal? LimiteDiario { get; set; }

        [Required]
        [DataMember(Name = "comprasEnLineaHabilitadas")]
        public bool? ComprasEnLineaHabilitadas { get; set; }

        [Required]
        [DataMember(Name = "retirosCajeroHabilitados")]
        public bool? RetirosCajeroHabilitados { get; set; }

        [DataMember(Name = "nombreImpreso")] public string NombreImpreso { get; set; }

        [DataMember(Name = "estadoEntrega")] public EstadoEntregaEnum? EstadoEntrega { get; set; }

        [DataMember(Name = "isActive")] public bool? IsActive { get; set; }

        [DataMember(Name = "concurrencyToken")]
        public string ConcurrencyToken { get; set; }

        // Boilerplate IEquatable, ToString, ToJson...
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
            return Equals((TarjetaEmitidaResult)obj);
        }

        public bool Equals(TarjetaEmitidaResult other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id &&
                   string.Equals(PanEnmascarado, other.PanEnmascarado) &&
                   FechaExpiracion == other.FechaExpiracion &&
                   Estado == other.Estado &&
                   Tipo == other.Tipo &&
                   LimiteDiario == other.LimiteDiario &&
                   ComprasEnLineaHabilitadas == other.ComprasEnLineaHabilitadas &&
                   RetirosCajeroHabilitados == other.RetirosCajeroHabilitados &&
                   string.Equals(NombreImpreso, other.NombreImpreso) &&
                   EstadoEntrega == other.EstadoEntrega &&
                   IsActive == other.IsActive &&
                   string.Equals(ConcurrencyToken, other.ConcurrencyToken);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PanEnmascarado != null ? PanEnmascarado.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FechaExpiracion != null ? FechaExpiracion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Estado != null ? Estado.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Tipo != null ? Tipo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LimiteDiario != null ? LimiteDiario.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (ComprasEnLineaHabilitadas != null ? ComprasEnLineaHabilitadas.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (RetirosCajeroHabilitados != null ? RetirosCajeroHabilitados.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NombreImpreso != null ? NombreImpreso.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EstadoEntrega != null ? EstadoEntrega.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (IsActive != null ? IsActive.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ConcurrencyToken != null ? ConcurrencyToken.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
