using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AccountsApi.Infrastructure.Helpers {
    public class SecondEpochConverter : DateTimeConverterBase {
        private static readonly DateTime _epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteRawValue (((DateTime) value - _epoch).TotalSeconds + "");
        }

        public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (reader.Value == null) { return null; }
            return _epoch.AddSeconds ((long) reader.Value );
        }

        public static DateTime ConvertFrom (double microseconds) {
            return _epoch.AddSeconds (microseconds);
        }

        public static double ConvertTo (DateTime date) {
            return ((date - _epoch).TotalSeconds);
        }
    }
}