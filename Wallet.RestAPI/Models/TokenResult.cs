using System.Runtime.Serialization;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Token Result
    /// </summary>
    [DataContract]
    public class TokenResult
    {
        /// <summary>
        /// Gets or Sets Token
        /// </summary>
        [DataMember(Name="token")]
        public string Token { get; set; }
    }
}
