using System;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Models
{
    /// <summary>
    /// Request model for changing the status of an entity (Activate/Deactivate).
    /// </summary>
    [DataContract]
    public class StatusChangeRequest : IEquatable<StatusChangeRequest>
    {
        /// <summary>
        /// Gets or Sets ConcurrencyToken
        /// </summary>
        [DataMember(Name = "concurrencyToken", EmitDefaultValue = false)]
        public string ConcurrencyToken { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(value: "class StatusChangeRequest {\n");
            sb.Append(value: "  ConcurrencyToken: ").Append(value: ConcurrencyToken).Append(value: "\n");
            sb.Append(value: "}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(value: this, formatting: Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(objA: null, objB: obj)) return false;
            if (ReferenceEquals(objA: this, objB: obj)) return true;
            return obj.GetType() == GetType() && Equals(other: (StatusChangeRequest)obj);
        }

        /// <summary>
        /// Returns true if StatusChangeRequest instances are equal
        /// </summary>
        /// <param name="other">Instance of StatusChangeRequest to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(StatusChangeRequest other)
        {
            if (ReferenceEquals(objA: null, objB: other)) return false;
            if (ReferenceEquals(objA: this, objB: other)) return true;
            return 
                (
                    ConcurrencyToken == other.ConcurrencyToken ||
                    ConcurrencyToken != null &&
                    ConcurrencyToken.Equals(value: other.ConcurrencyToken)
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
                    if (ConcurrencyToken != null)
                    hashCode = hashCode * 59 + ConcurrencyToken.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(StatusChangeRequest left, StatusChangeRequest right)
        {
            return Equals(objA: left, objB: right);
        }

        public static bool operator !=(StatusChangeRequest left, StatusChangeRequest right)
        {
            return !Equals(objA: left, objB: right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
