using Olve.Utilities.Ids;
using Assert = TUnit.Assertions.Assert;

namespace Olve.Utilities.Tests.Ids;

public class IdGenericTests
{
    private readonly record struct Person(string Name, string PhoneNumber);

    [Test]
    public async Task New_ReturnsDistinctIds_ForMultipleCalls()
    {
        var idA = Id.New<Person>();
        var idB = Id.New<Person>();

        await Assert.That(idA.Equals(idB)).IsFalse();
    }

    [Test]
    public async Task FromName_IsDeterministic_ForSameInputs()
    {
        var id1 = Id.FromName("Alice");
        var id2 = Id.FromName("Alice");

        await Assert.That(id1.Equals(id2)).IsTrue();

        var typed1 = Id.FromName<Person>("Alice");
        var typed2 = Id.FromName<Person>("Alice");

        await Assert.That(typed1.Equals(typed2)).IsTrue();
    }

    [Test]
    public async Task TryParse_ParsesStringRepresentation_ForUntypedAndTyped()
    {
        var original = Id.New();
        var text = original.ToString();

        var parsedOk = Id.TryParse(text, out var parsed);
        await Assert.That(parsedOk).IsTrue();
        await Assert.That(parsed.Equals(original)).IsTrue();

        var typedOriginal = Id.New<int>();
        var typedText = typedOriginal.ToString();

        var typedParsedOk = Id.TryParse<int>(typedText, out var parsedTyped);
        await Assert.That(typedParsedOk).IsTrue();
        await Assert.That(parsedTyped.Equals(typedOriginal)).IsTrue();
    }

    [Test]
    public async Task ToString_Value_And_ComparisonOperators_Work_ForHappyPath()
    {
        var id = Id.New();
        var typed = new Id<int>(id);

        await Assert.That(typed.Value.Equals(id)).IsTrue();
        await Assert.That(typed.ToDisplayString()).Contains($"Id<{typeof(int).Name}>(");

        await Assert.That(typed.CompareTo(typed)).IsEqualTo(0);
        await Assert.That(id <= typed.Value).IsTrue();
        await Assert.That(id >= typed.Value).IsTrue();
        await Assert.That(id < typed.Value).IsFalse();
        await Assert.That(id > typed.Value).IsFalse();
    }

    [Test]
    public async Task ToString_And_ToDisplayString_Format()
    {
        var id = Id.New();
        var typed = new Id<int>(id);


        await Assert.That(id.ToString()).IsEqualTo(id.Value.ToString());
        await Assert.That(typed.ToDisplayString()).Contains($"Id<{typeof(int).Name}>(");
    }


// High priority tests


    [Test]
    public async Task DeterministicGuid_HasUuidV5_VersionAndVariantBitsSet()
    {
        var id = Id.FromName("alice");
        var bytes = id.Value.ToByteArray();


        var version = (bytes[6] >> 4) & 0x0F;
        await Assert.That(version).IsEqualTo(5);

        await Assert.That((bytes[8] & 0xC0) == 0x80).IsTrue();
    }


    [Test]
    public async Task FromName_RespectsEmptyAndNullNamespaceBehavior()
    {
        var a = Id.FromName("name");
        var b = Id.FromName("name", null);
        var c = Id.FromName("name", new Id(Guid.Empty));


        await Assert.That(a.Equals(b)).IsTrue();
        await Assert.That(a.Equals(c)).IsTrue();
    }


    [Test]
    public async Task FromName_NonAscii_And_LongNames_AreDeterministic_And_DoNotThrow()
    {
        var unicode = "日本語-テスト-😊";
        var longName = new string('x', 10_000);


        var u1 = Id.FromName(unicode);
        var u2 = Id.FromName(unicode);
        await Assert.That(u1.Equals(u2)).IsTrue();


        var l1 = Id.FromName(longName);
        var l2 = Id.FromName(longName);
        await Assert.That(l1.Equals(l2)).IsTrue();
    }


    [Test]
    public void Id_FromName_NullName_ThrowsArgumentNullException_ForTypedAndUntyped()
    {
        try
        {
            Id.FromName(null!);
            Assert.Fail("Should not reach");
        }
        catch (ArgumentNullException)
        {
        }


// Typed
        try
        {
            Id.FromName<Person>(null!);
            Assert.Fail("Should not reach");
        }
        catch (ArgumentNullException)
        {
        }
    }


    [Test]
    public async Task TryParse_TypedOverload_ReturnsFalseForInvalid()
    {
        var ok = Id.TryParse<int>("bad", out var id);
        await Assert.That(ok).IsFalse();
        await Assert.That(id.Equals(default)).IsTrue();
    }
}
