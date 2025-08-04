using System.Text.Json;
using System.Text.Json.Serialization;
using Olve.Paths;

namespace Olve.MinimalApi;

/// <summary>
/// Converts <see cref="IPath"/> instances to and from JSON string representations.
/// </summary>
public sealed class PathJsonConverter : JsonConverter<IPath>
{
    /// <summary>
    /// Reads an <see cref="IPath"/> value from JSON.
    /// </summary>
    /// <param name="reader">JSON reader to read from.</param>
    /// <param name="typeToConvert">The target type to convert.</param>
    /// <param name="options">Serializer options.</param>
    /// <returns>The deserialized <see cref="IPath"/> or null if JSON is null.</returns>
    public override IPath? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var pathString = reader.GetString();
        return pathString is null ? null : Paths.Path.Create(pathString);
    }

    /// <summary>
    /// Writes an <see cref="IPath"/> value as a JSON string.
    /// </summary>
    /// <param name="writer">JSON writer to write to.</param>
    /// <param name="value">The <see cref="IPath"/> value to serialize.</param>
    /// <param name="options">Serializer options.</param>
    public override void Write(Utf8JsonWriter writer, IPath value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Path);
    }
}
