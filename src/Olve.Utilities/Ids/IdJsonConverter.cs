using System.Text.Json;
using System.Text.Json.Serialization;

namespace Olve.Utilities.Ids;

/// <summary>
/// AOT-safe JSON converter for <see cref="Id"/> that serializes as a GUID string.
/// </summary>
public sealed class IdJsonConverter : JsonConverter<Id>
{
    /// <inheritdoc />
    public override Id Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var guid = reader.GetGuid();
        return new Id(guid);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Id value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
