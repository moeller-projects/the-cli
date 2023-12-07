using Moeller.TheCli.Domain.Personio.Common;
using Newtonsoft.Json;

namespace Moeller.TheCli.Domain.Personio.Util;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
    {
        var timespanFormatted = $"{value.ToString(Constants.DATE_FORMAT)}";
        writer.WriteValue(timespanFormatted);
    }

    public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        DateOnly parsedDateOnly;
        DateOnly.TryParseExact((string)reader.Value, Constants.DATE_FORMAT, out parsedDateOnly);
        return parsedDateOnly;
    }
}