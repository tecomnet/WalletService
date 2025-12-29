using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    [DataContract]
    public partial class CambioBloqueoRequest
    {
        [Required]
        [DataMember(Name = "bloquear")]
        public bool? Bloquear { get; set; }

        [Required]
        [DataMember(Name = "concurrencyToken")]
        public string ConcurrencyToken { get; set; }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
