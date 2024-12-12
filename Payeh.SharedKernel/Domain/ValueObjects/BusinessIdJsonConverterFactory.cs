using System.Text.Json;
using System.Text.Json.Serialization;

namespace Payeh.SharedKernel.Domain.ValueObjects;

/// <summary>
/// JSON converter factory for BusinessId to handle various generic instantiations.
/// </summary>
public class BusinessIdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableTo(typeof(BusinessId<,>));
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var genericArgs = typeToConvert.GenericTypeArguments;
        var converterType = typeof(BusinessIdJsonConverter<,>).MakeGenericType(genericArgs);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}