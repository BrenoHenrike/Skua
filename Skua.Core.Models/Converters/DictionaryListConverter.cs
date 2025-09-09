using Newtonsoft.Json;

namespace Skua.Core.Models.Converters;

public class DictionaryListConverter<TKey, TVal> : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<TVal>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return serializer.Deserialize<Dictionary<TKey, TVal>>(reader).Values.ToList();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}