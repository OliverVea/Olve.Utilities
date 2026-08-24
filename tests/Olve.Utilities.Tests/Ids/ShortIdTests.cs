using Olve.Utilities.Ids;

namespace Olve.Utilities.Tests.Ids;

public class ShortIdTests
{
    private sealed record Slime;

    [Test]
    public async Task None_IsZero_AndIsNotSome()
    {
        await Assert.That(ShortId<Slime>.None.Value).IsEqualTo(0u);
        await Assert.That(ShortId<Slime>.None.IsSome).IsFalse();
        await Assert.That(new ShortId<Slime>(1).IsSome).IsTrue();
    }

    [Test]
    public async Task Default_IsNone()
    {
        await Assert.That(default(ShortId<Slime>)).IsEqualTo(ShortId<Slime>.None);
    }

    [Test]
    public async Task EqualityAndOrdering_FollowTheUnderlyingValue()
    {
        var a = new ShortId<Slime>(1);
        var b = new ShortId<Slime>(2);

        await Assert.That(a).IsEqualTo(new ShortId<Slime>(1));
        await Assert.That(a).IsNotEqualTo(b);
        await Assert.That(a < b).IsTrue();
        await Assert.That(b > a).IsTrue();
        await Assert.That(a <= new ShortId<Slime>(1)).IsTrue();
        await Assert.That(a.CompareTo(b)).IsLessThan(0);
    }

    [Test]
    public async Task Sorting_IsAscendingByValue()
    {
        ShortId<Slime>[] ids = [new(3), new(1), new(2)];
        Array.Sort(ids);

        await Assert.That(ids.Select(i => i.Value)).IsEquivalentTo([1u, 2u, 3u]);
    }

    [Test]
    public async Task DisplayString_NamesTheLogicalType()
    {
        var id = new ShortId<Slime>(7);

        await Assert.That(id.ToString()).IsEqualTo("7");
        await Assert.That(id.ToDisplayString()).IsEqualTo("ShortId<Slime>(7)");
    }
}
