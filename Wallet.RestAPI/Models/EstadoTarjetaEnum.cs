using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Enumeración para los estados de tarjeta (DTO)
    /// </summary>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum EstadoTarjetaEnum
    {
        [EnumMember(Value = "Activa")]
        Activa = 1,

        [EnumMember(Value = "Inactiva")]
        Inactiva = 2,

        [EnumMember(Value = "BloqueadaTemporalmente")]
        BloqueadaTemporalmente = 3,

        [EnumMember(Value = "CanceladaPorRobo")]
        CanceladaPorRobo = 4,

        [EnumMember(Value = "CanceladaPorExtravio")]
        CanceladaPorExtravio = 5,

        [EnumMember(Value = "CanceladaPorSistema")]
        CanceladaPorSistema = 6,

        [EnumMember(Value = "Expirada")]
        Expirada = 7
    }
}
