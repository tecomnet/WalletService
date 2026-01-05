using System.Runtime.Serialization;
using Newtonsoft.Json;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Enumeración para las marcas de tarjeta (DTO)
    /// </summary>
    [JsonConverter(converterType: typeof(CustomStringToEnumConverter<MarcaTarjetaEnum>))]
    public enum MarcaTarjetaEnum
    {
        [EnumMember(Value = "Visa")] Visa = 1,

        [EnumMember(Value = "Mastercard")] Mastercard = 2,

        [EnumMember(Value = "Amex")] Amex = 3,

        [EnumMember(Value = "Carnet")] Carnet = 4,

        [EnumMember(Value = "Otra")] Otra = 99
    }
}
