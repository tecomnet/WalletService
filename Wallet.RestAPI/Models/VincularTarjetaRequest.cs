using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    [DataContract]
    public partial class VincularTarjetaRequest
    {
        [Required]
        [DataMember(Name = "idCliente")]
        public int? IdCliente { get; set; }

        [Required]
        [DataMember(Name = "tokenPasarela")]
        public string TokenPasarela { get; set; }
        
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
        [DataMember(Name = "fechaExpiracion")]
        public DateTime? FechaExpiracion { get; set; }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
