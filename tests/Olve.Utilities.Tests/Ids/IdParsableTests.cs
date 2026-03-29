using Olve.Utilities.Ids;
using Assert = TUnit.Assertions.Assert;

namespace Olve.Utilities.Tests.Ids;

public class IdParsableTests
{
    private readonly record struct Pipeline;

    [Test]
    public async Task Id_Parse_ValidString_ReturnsParsedId()
    {
        var original = Id.New();
        var text = original.ToString();

        var parsed = Id.Parse(text, null);

        await Assert.That(parsed).IsEqualTo(original);
    }

    [Test]
    public void Id_Parse_InvalidString_ThrowsFormatException()
    {
        try
        {
            Id.Parse("bad", null);
            Assert.Fail("Should have thrown");
        }
        catch (FormatException)
        {
        }
    }

    [Test]
    public async Task Id_TryParse_NullString_ReturnsFalse()
    {
        var result = Id.TryParse(null, null, out var id);

        await Assert.That(result).IsFalse();
        await Assert.That(id).IsEqualTo(default(Id));
    }

    [Test]
    public async Task IdOfT_Parse_ValidString_ReturnsParsedId()
    {
        var original = Id.New<Pipeline>();
        var text = original.ToString();

        var parsed = Id<Pipeline>.Parse(text, null);

        await Assert.That(parsed).IsEqualTo(original);
    }

    [Test]
    public void IdOfT_Parse_InvalidString_ThrowsFormatException()
    {
        try
        {
            Id<Pipeline>.Parse("bad", null);
            Assert.Fail("Should have thrown");
        }
        catch (FormatException)
        {
        }
    }

    [Test]
    public async Task IdOfT_TryParse_NullString_ReturnsFalse()
    {
        var result = Id<Pipeline>.TryParse(null, null, out var id);

        await Assert.That(result).IsFalse();
        await Assert.That(id).IsEqualTo(default(Id<Pipeline>));
    }
}
