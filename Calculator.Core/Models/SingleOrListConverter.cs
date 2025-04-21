using System.Text.Json.Serialization;
using System.Text.Json;

namespace Calculator.Core.Models;

    public class SingleOrListConverter<T> : JsonConverter<List<T>>
    {
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new List<T>();

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Deserialize a single object and add to list
                var item = JsonSerializer.Deserialize<T>(ref reader, options);
                if (item != null) result.Add(item);
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Deserialize as list
                result = JsonSerializer.Deserialize<List<T>>(ref reader, options);
            }

            return result ?? new List<T>();
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }