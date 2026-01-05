using System.Runtime.Serialization;
using Newtonsoft.Json;
using Wallet.RestAPI.Helpers;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Enumeración para los tipos de tarjeta (DTO)
    /// </summary>
    [JsonConverter(converterType: typeof(CustomStringToEnumConverter<TipoTarjetaEnum>))]
    public enum TipoTarjetaEnum
    {
        /// <summary>
        /// Tarjeta física (plástico).
        /// </summary>
        [EnumMember(Value = "Fisica")] Fisica = 1,

        /// <summary>
        /// Tarjeta virtual (digital).
        /// </summary>
        [EnumMember(Value = "Virtual")] Virtual = 2
    }
}
