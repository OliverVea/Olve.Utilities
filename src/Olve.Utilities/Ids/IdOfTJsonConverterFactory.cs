using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Olve.Utilities.Ids;

/// <summary>
/// Factory that creates <see cref="IdOfTJsonConverter{T}"/> instances for any <see cref="Id{T}"/>.
/// In AOT contexts, the source generator bypasses this factory entirely when consumers
/// declare <c>[JsonSerializable(typeof(Id&lt;MyEntity&gt;))]</c> on their <see cref="JsonSerializerContext"/>.
/// </summary>
public sealed class IdOfTJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(Id<>);

    /// <inheritdoc />
    [UnconditionalSuppressMessage("AOT", "IL3050",
        Justification = "In AOT contexts, the source generator bypasses this factory. " +
                         "MakeGenericType is only called in JIT scenarios.")]
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var typeArg = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(IdOfTJsonConverter<>).MakeGenericType(typeArg);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
