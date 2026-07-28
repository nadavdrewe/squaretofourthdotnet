using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;

namespace RevelFourthPipeline.Infrastructure.Serialization;

public sealed class FlexibleStringJsonConverter : JsonConverter<string?>
{
    public override string? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.Number => ReadRawTokenText(ref reader),
            JsonTokenType.True => "true",
            JsonTokenType.False => "false",
            _ => throw new JsonException($"Cannot convert JSON token {reader.TokenType} to string.")
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        string? value,
        JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value);
    }

    private static string ReadRawTokenText(ref Utf8JsonReader reader)
    {
        return reader.HasValueSequence
            ? Encoding.UTF8.GetString(reader.ValueSequence.ToArray())
            : Encoding.UTF8.GetString(reader.ValueSpan);
    }
}
