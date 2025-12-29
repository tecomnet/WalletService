using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Enumeración para el estado de entrega logística (DTO)
    /// </summary>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum EstadoEntregaEnum
    {
        [EnumMember(Value = "Solicitada")]
        Solicitada = 1,

        [EnumMember(Value = "EnProduccion")]
        EnProduccion = 2,

        [EnumMember(Value = "Enviada")]
        Enviada = 3,

        [EnumMember(Value = "Entregada")]
        Entregada = 4,

        [EnumMember(Value = "Devuelta")]
        Devuelta = 5
    }
}
