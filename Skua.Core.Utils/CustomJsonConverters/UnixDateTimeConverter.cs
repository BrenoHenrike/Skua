using Newtonsoft.Json;

namespace Skua.Core.Utils.CustomJsonConverters;

public class UnixDateTimeConverter : JsonConverter
{
    private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        long t;
        if (reader.Value!.GetType() != typeof(long))
            t = long.Parse((string)reader.Value);
        else
            t = (long)reader.Value;

        return _epoch.AddMilliseconds(t);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteRawValue(((DateTime)value - _epoch).ToString());
    }
}