using Olve.Results.TUnit;

namespace Olve.Results.Tests;

public class DictionaryResultExtensionsTests
{
    [Test]
    public async Task SetWithResult_NewKey_Succeeds()
    {
        var dictionary = new Dictionary<string, int>();

        var result = dictionary.SetWithResult("key", 42);

        await Assert.That(result).Succeeded();
        await Assert.That(dictionary["key"]).IsEqualTo(42);
    }

    [Test]
    public async Task SetWithResult_DuplicateKey_Fails()
    {
        var dictionary = new Dictionary<string, int> { ["key"] = 1 };

        var result = dictionary.SetWithResult("key", 42);

        await Assert.That(result).Failed();
    }

    [Test]
    public async Task GetWithResult_ExistingKey_ReturnsValue()
    {
        IReadOnlyDictionary<string, int> dictionary = new Dictionary<string, int> { ["key"] = 42 };

        var result = dictionary.GetWithResult("key");

        await Assert.That(result).SucceededAndValue(v => v.IsEqualTo(42));
    }

    [Test]
    public async Task GetWithResult_MissingKey_Fails()
    {
        IReadOnlyDictionary<string, int> dictionary = new Dictionary<string, int>();

        var result = dictionary.GetWithResult("key");

        await Assert.That(result).Failed();
    }
}
