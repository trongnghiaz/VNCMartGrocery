using System.Text.Json;
using System.Text.Json.Serialization;
using VNC.Domain.Enumerations;

namespace VNC.Api.Factories
{
    public class EnumerationJsonConverterFactory : JsonConverterFactory
    {
        // Kiểm tra xem kiểu dữ liệu có phải là class kế thừa từ Enumeration không
        public override bool CanConvert(Type typeToConvert)
        {
            return IsSubclassOfRawGeneric(typeof(Enumeration), typeToConvert);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(EnumerationJsonConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                if (toCheck.BaseType != null && toCheck.BaseType == generic) return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }

    public class EnumerationJsonConverter<T> : JsonConverter<T> where T : Enumeration
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                var value = reader.GetUInt16();
                // Sử dụng Reflection để gọi hàm FromValue có sẵn trong class của bạn
                var method = typeof(T).GetMethod("FromValue", new[] { typeof(short) });
                return (T)method.Invoke(null, new object[] { value });
            }
            throw new JsonException($"Binding error for {typeof(T).Name}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}
