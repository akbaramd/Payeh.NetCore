using System.Text.Json;
using System.Text.Json.Serialization;

namespace Payeh.SharedKernel.Domain.Enumerations;

/// <summary>
///     JSON Converter for Enumeration types.
/// </summary>
public class EnumerationJsonConverter : JsonConverter<Enumeration>
{
    public override Enumeration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string value.");

        var value = reader.GetString();
        if (value == null)
            throw new JsonException("Expected non-null string value.");

        var method = typeof(Enumeration).GetMethod(nameof(Enumeration.FromName))?.MakeGenericMethod(typeToConvert);

        if (method == null)
            throw new InvalidOperationException($"Could not find {nameof(Enumeration.FromName)} method for type {typeToConvert}.");

        var result = method.Invoke(null, new object[] { value });

        return result as Enumeration ?? throw new JsonException($"Invalid value '{value}' for enumeration '{typeToConvert.Name}'");
    }

    public override void Write(Utf8JsonWriter writer, Enumeration value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}