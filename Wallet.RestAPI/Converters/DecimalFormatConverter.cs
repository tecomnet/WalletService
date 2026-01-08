using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Wallet.RestAPI.Converters
{
    public class DecimalFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal) || objectType == typeof(decimal?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            return Convert.ToDecimal(reader.Value, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            decimal d = (decimal)value;
            // WriteRawValue writes precise string content as a JSON token.
            // "0.00" ensures always 2 decimal places.
            writer.WriteRawValue(d.ToString("0.00", CultureInfo.InvariantCulture));
        }
    }
}
