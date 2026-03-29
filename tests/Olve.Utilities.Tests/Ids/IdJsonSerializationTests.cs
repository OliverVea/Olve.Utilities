using System.Text.Json;
using System.Text.Json.Serialization;
using Olve.Utilities.Ids;
using Assert = TUnit.Assertions.Assert;

namespace Olve.Utilities.Tests.Ids;

public partial class IdJsonSerializationTests
{
    private readonly record struct Pipeline;

    [JsonSerializable(typeof(Id))]
    [JsonSerializable(typeof(Id<Pipeline>))]
    private partial class TestJsonContext : JsonSerializerContext;

    [Test]
    public async Task Id_RoundTrips_ThroughJson()
    {
        var original = Id.New();
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<Id>(json);

        await Assert.That(deserialized).IsEqualTo(original);
    }

    [Test]
    public async Task IdOfT_RoundTrips_ThroughJson()
    {
        var original = Id.New<Pipeline>();
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<Id<Pipeline>>(json);

        await Assert.That(deserialized).IsEqualTo(original);
    }

    [Test]
    public async Task Id_SerializesAsBareGuidString()
    {
        var id = Id.New();
        var json = JsonSerializer.Serialize(id);

        await Assert.That(json).IsEqualTo($"\"{id.Value}\"");
    }

    [Test]
    public async Task IdOfT_SerializesAsBareGuidString()
    {
        var id = Id.New<Pipeline>();
        var json = JsonSerializer.Serialize(id);

        await Assert.That(json).IsEqualTo($"\"{id.Value.Value}\"");
    }

    [Test]
    public async Task Id_RoundTrips_ThroughAotContext()
    {
        var original = Id.New();
        var json = JsonSerializer.Serialize(original, TestJsonContext.Default.Id);
        var deserialized = JsonSerializer.Deserialize(json, TestJsonContext.Default.Id);

        await Assert.That(deserialized).IsEqualTo(original);
    }

    [Test]
    public async Task IdOfT_RoundTrips_ThroughAotContext()
    {
        var original = Id.New<Pipeline>();
        var json = JsonSerializer.Serialize(original, TestJsonContext.Default.IdPipeline);
        var deserialized = JsonSerializer.Deserialize(json, TestJsonContext.Default.IdPipeline);

        await Assert.That(deserialized).IsEqualTo(original);
    }

    [Test]
    public void Id_DeserializeInvalidString_ThrowsJsonException()
    {
        var json = "\"not-a-guid\"";

        try
        {
            JsonSerializer.Deserialize<Id>(json);
            Assert.Fail("Should have thrown");
        }
        catch (JsonException)
        {
        }
    }
}
