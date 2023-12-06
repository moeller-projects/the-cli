using Moeller.TheCli.Domain.Personio.Common;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Util
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            var timespanFormatted = $"{value.ToString(Constants.TIME_FORMAT)}";
            writer.WriteValue(timespanFormatted);
        }

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            TimeSpan parsedTimeSpan;
            TimeSpan.TryParseExact((string)reader.Value, Constants.TIME_FORMAT, null, out parsedTimeSpan);
            return parsedTimeSpan;
        }
    }
}
