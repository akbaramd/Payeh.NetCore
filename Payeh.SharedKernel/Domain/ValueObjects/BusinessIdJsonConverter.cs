using System.Text.Json;
using System.Text.Json.Serialization;

namespace Payeh.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Generic JSON converter for BusinessId to handle serialization and deserialization.
/// </summary>
public class BusinessIdJsonConverter<T, TKey> : JsonConverter<BusinessId<T, TKey>>
    where T : BusinessId<T, TKey>, new()
{
    public override BusinessId<T, TKey> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string token.");

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            throw new JsonException("Business ID cannot be empty.");

        if (typeof(TKey) == typeof(Guid) && Guid.TryParse(value, out var guid))
        {
            return BusinessId<T, TKey>.FromValue((TKey)(object)guid);
        }

        throw new JsonException("Invalid format for Business ID.");
    }

    public override void Write(Utf8JsonWriter writer, BusinessId<T, TKey> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}