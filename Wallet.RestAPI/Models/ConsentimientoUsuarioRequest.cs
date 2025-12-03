using System.Runtime.Serialization;
using System.Text;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// ConsentimientoUsuarioRequest
    /// </summary>
    [DataContract]
    public class ConsentimientoUsuarioRequest
    {
        /// <summary>
        /// Gets or Sets TipoDocumento
        /// </summary>
        [DataMember(Name = "tipoDocumento", EmitDefaultValue = false)]
        public TipoDocumentoConsentimientoEnum? TipoDocumento { get; set; }

        /// <summary>
        /// Gets or Sets Version
        /// </summary>
        [DataMember(Name = "version", EmitDefaultValue = false)]
        public string Version { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ConsentimientoUsuarioRequest {\n");
            sb.Append("  TipoDocumento: ").Append(TipoDocumento).Append("\n");
            sb.Append("  Version: ").Append(Version).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
