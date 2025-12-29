using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    [DataContract]
    public partial class ConfiguracionTarjetaRequest
    {
        [Required]
        [DataMember(Name = "limiteDiario")]
        public decimal? LimiteDiario { get; set; }
        
        [Required]
        [DataMember(Name = "comprasEnLinea")]
        public bool? ComprasEnLinea { get; set; }

        [Required]
        [DataMember(Name = "retiros")]
        public bool? Retiros { get; set; }

        [Required]
        [DataMember(Name = "concurrencyToken")]
        public string ConcurrencyToken { get; set; }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
