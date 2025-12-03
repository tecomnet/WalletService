using System;
using System.Runtime.Serialization;
using System.Text;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// ConsentimientoUsuarioResult
    /// </summary>
    [DataContract]
    public class ConsentimientoUsuarioResult
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? Id { get; set; }

        /// <summary>
        /// Gets or Sets IdUsuario
        /// </summary>
        [DataMember(Name = "idUsuario", EmitDefaultValue = false)]
        public int? IdUsuario { get; set; }

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
        /// Gets or Sets FechaAceptacion
        /// </summary>
        [DataMember(Name = "fechaAceptacion", EmitDefaultValue = false)]
        public DateTime? FechaAceptacion { get; set; }

        /// <summary>
        /// Gets or Sets IsActive
        /// </summary>
        [DataMember(Name = "isActive", EmitDefaultValue = false)]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ConsentimientoUsuarioResult {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  IdUsuario: ").Append(IdUsuario).Append("\n");
            sb.Append("  TipoDocumento: ").Append(TipoDocumento).Append("\n");
            sb.Append("  Version: ").Append(Version).Append("\n");
            sb.Append("  FechaAceptacion: ").Append(FechaAceptacion).Append("\n");
            sb.Append("  IsActive: ").Append(IsActive).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
