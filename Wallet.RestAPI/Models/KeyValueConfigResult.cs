using System;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Estructura de resultado de configuracion key-value
    /// </summary>
    [DataContract]
    public partial class KeyValueConfigResult : IEquatable<KeyValueConfigResult>
    {
        /// <summary>
        /// Gets or Sets Key
        /// </summary>
        [Required]
        [DataMember(Name = "key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or Sets Value
        /// </summary>
        [Required]
        [DataMember(Name = "value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or Sets ConcurrencyToken
        /// </summary>
        [Required]
        [DataMember(Name = "concurrencyToken")]
        public string ConcurrencyToken { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class KeyValueConfigResult {\n");
            sb.Append("  Key: ").Append(Key).Append("\n");
            sb.Append("  Value: ").Append(Value).Append("\n");
            sb.Append("  ConcurrencyToken: ").Append(ConcurrencyToken).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((KeyValueConfigResult)obj);
        }

        /// <summary>
        /// Returns true if KeyValueConfigResult instances are equal
        /// </summary>
        /// <param name="other">Instance of KeyValueConfigResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(KeyValueConfigResult other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return
                (
                    Key == other.Key ||
                    Key != null &&
                    Key.Equals(other.Key)
                ) &&
                (
                    Value == other.Value ||
                    Value != null &&
                    Value.Equals(other.Value)
                ) &&
                (
                    ConcurrencyToken == other.ConcurrencyToken ||
                    ConcurrencyToken != null &&
                    ConcurrencyToken.Equals(other.ConcurrencyToken)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                if (Key != null)
                    hashCode = hashCode * 59 + Key.GetHashCode();
                if (Value != null)
                    hashCode = hashCode * 59 + Value.GetHashCode();
                if (ConcurrencyToken != null)
                    hashCode = hashCode * 59 + ConcurrencyToken.GetHashCode();
                return hashCode;
            }
        }

        #region Operators

#pragma warning disable 1591

        public static bool operator ==(KeyValueConfigResult left, KeyValueConfigResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(KeyValueConfigResult left, KeyValueConfigResult right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591

        #endregion Operators
    }
}
