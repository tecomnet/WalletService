using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    [DataContract]
    public partial class SolicitarTarjetaFisicaRequest
    {
        [Required]
        [DataMember(Name = "idCliente")]
        public int? IdCliente { get; set; }

        [Required]
        [DataMember(Name = "nombreImpreso")]
        public string NombreImpreso { get; set; }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
