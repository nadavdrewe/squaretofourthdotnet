using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RevelFourthPipeline.Infrastructure.Serialization;

public sealed class FlexibleDecimalJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(decimal) || typeToConvert == typeof(decimal?);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(decimal?))
        {
            return new NullableDecimalConverter();
        }

        return new DecimalConverter();
    }

    private sealed class DecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ReadDecimal(ref reader) ?? 0m;
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    private sealed class NullableDecimalConverter : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ReadDecimal(ref reader);
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }

    private static decimal? ReadDecimal(ref Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out parsed))
            {
                return parsed;
            }

            return null;
        }

        throw new JsonException($"Cannot convert token {reader.TokenType} to decimal.");
    }
}
