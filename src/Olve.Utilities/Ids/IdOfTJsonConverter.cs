using System.Text.Json;
using System.Text.Json.Serialization;

namespace Olve.Utilities.Ids;

/// <summary>
/// AOT-safe JSON converter for <see cref="Id{T}"/> that serializes as a GUID string.
/// </summary>
/// <typeparam name="T">The logical entity type.</typeparam>
public sealed class IdOfTJsonConverter<T> : JsonConverter<Id<T>>
{
    /// <inheritdoc />
    public override Id<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var guid = reader.GetGuid();
        return new Id<T>(new Id(guid));
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Id<T> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.Value);
    }
}
