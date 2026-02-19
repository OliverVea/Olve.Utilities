using Olve.Utilities.Lookup;
using Assert = Olve.Utilities.Assertions.Assert;

namespace Olve.Utilities.Tests;

public static class Sandbox
{
    public static void TestMethod()
    {
        var items = new[] { new TestClass(1), new TestClass(2), new TestClass(3), new TestClass(4), new TestClass(5) };

        var lookup = new IdFrozenLookup<TestClass, int>(items);

        var found = lookup.TryGetValue(3, out var item);
        Assert.That(() => found, "Item should be found but was not");
        Assert.That(() => item!.Id == 3, "Item should have id 3 but did not");

        var notFound = lookup.TryGetValue(6, out _);
        Assert.That(() => !notFound, "Item should not be found but was");

        Console.WriteLine(lookup.Count);
    }

    private class TestClass(int id) : IHasId<int>
    {
        public int Id { get; } = id;
    }
}