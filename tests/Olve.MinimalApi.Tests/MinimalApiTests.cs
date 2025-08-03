using System.Text.Json;
using Olve.Paths;

namespace Olve.MinimalApi.Tests;

public class MinimalApiTests
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new();

    [Before(Class)]
    public static void Before()
    {
        JsonSerializerOptions.Converters.Add(new PathJsonConverter());
    }
    
    [Test]
    public async Task PathJsonConverter_SerializesAndDeserializes()
    {
        // Arrange
        var original = Paths.Path.Create("folder/sub/file.txt");

        // Act
        var json = Serialize(original);
        var deserialized = Deserialize(json);

        // Assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Path).IsEqualTo(original.Path);
    }

    [Test]
    public async Task PathJsonConverter_ReadsNullAsNull()
    {
        // Act
        var result = Deserialize("null");
        
        // Assert
        await Assert.That(result).IsNull();
    }
    
    private static IPath? Deserialize(string path) => JsonSerializer.Deserialize<IPath>(path, JsonSerializerOptions);
    private static string Serialize(IPath path) => JsonSerializer.Serialize(path, JsonSerializerOptions);
}
