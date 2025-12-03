using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Tipo de documento de consentimiento
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TipoDocumentoConsentimientoEnum
    {
        /// <summary>
        /// Enum Terminos for Terminos
        /// </summary>
        [EnumMember(Value = "Terminos")] Terminos = 0,

        /// <summary>
        /// Enum Privacidad for Privacidad
        /// </summary>
        [EnumMember(Value = "Privacidad")] Privacidad = 1,

        /// <summary>
        /// Enum PLD for PLD
        /// </summary>
        [EnumMember(Value = "PLD")] PLD = 2
    }
}
