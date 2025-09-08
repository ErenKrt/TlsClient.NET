using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace TlsClient.Core.Converters
{
    public class JsonStringConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString() ?? throw new JsonException($"Cannot convert null to {typeof(T)}.");

            if (typeof(T) == typeof(string))
                return (T)(object)str;

            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), str, ignoreCase: true);

            var ctor = typeof(T).GetConstructor(new[] { typeof(string) });
            if (ctor != null)
                return (T)ctor.Invoke(new object[] { str });

            var parseMethod = typeof(T).GetMethod("Parse", new[] { typeof(string) });
            if (parseMethod != null)
                return (T)parseMethod.Invoke(null, new object[] { str });

            throw new JsonException($"JsonStringConverter cannot convert to {typeof(T)}.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }
    }
}
