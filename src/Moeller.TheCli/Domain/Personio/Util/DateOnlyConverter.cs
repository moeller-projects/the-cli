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
        DateOnly.TryParseExact(reader.ToString(), Constants.DATE_FORMAT, out var parsedDateOnly);
        return parsedDateOnly;
    }
}