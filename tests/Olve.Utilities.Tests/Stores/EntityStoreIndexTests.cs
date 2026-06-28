using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;
using Olve.Utilities.Stores;

namespace Olve.Utilities.Tests.Stores;

public class EntityStoreIndexTests
{
    private record Item(Id<Item> Id, string Group, string Name = "item") : IHasId<Id<Item>>;

    private static Item ItemIn(string group)
        => new(Id.New<Item>(), group);

    [Test]
    public async Task GetForKey_GroupsByKey()
    {
        var store = new EntityStore<Item>([]);
        var index = store.CreateIndex(s => s.Group);
        const string groupA = "a";
        const string groupB = "b";

        var a1 = ItemIn(groupA);
        var a2 = ItemIn(groupA);
        var b1 = ItemIn(groupB);
        store.Set(a1);
        store.Set(a2);
        store.Set(b1);

        await Assert.That(index.GetForKey(groupA)).Contains(a1.Id);
        await Assert.That(index.GetForKey(groupA)).Contains(a2.Id);
        await Assert.That(index.GetForKey(groupA).Count).IsEqualTo(2);
        await Assert.That(index.GetForKey(groupB).Count).IsEqualTo(1);
    }

    [Test]
    public async Task Delete_RemovesFromIndex_AndDropsEmptyKey()
    {
        var store = new EntityStore<Item>([]);
        var index = store.CreateIndex(s => s.Group);
        const string group = "a";

        var item = ItemIn(group);
        store.Set(item);
        await Assert.That(index.ContainsKey(group)).IsTrue();

        store.Delete(item.Id);

        // Regression: the previous implementation never pruned on delete because the entity is
        // already gone from the store when OnDeleted fires.
        await Assert.That(index.GetForKey(group).Count).IsEqualTo(0);
        await Assert.That(index.ContainsKey(group)).IsFalse();
    }

    [Test]
    public async Task GetForKey_ReturnsStableSnapshot_UnaffectedByLaterAdd()
    {
        // A reference handed out by GetForKey must not reflect a subsequent mutation. Fails on a
        // live-HashSet implementation, passes on the immutable-backed index.
        var store = new EntityStore<Item>([]);
        var index = store.CreateIndex(s => s.Group);
        const string group = "a";

        store.Set(ItemIn(group));
        var snapshot = index.GetForKey(group);
        await Assert.That(snapshot.Count).IsEqualTo(1);

        store.Set(ItemIn(group)); // mutates the index for the same key

        await Assert.That(snapshot.Count).IsEqualTo(1);           // captured reference unchanged
        await Assert.That(index.GetForKey(group).Count).IsEqualTo(2); // fresh read sees it
    }

    [Test]
    public async Task GetForKey_ReturnsStableSnapshot_UnaffectedByLaterDelete()
    {
        var store = new EntityStore<Item>([]);
        var index = store.CreateIndex(s => s.Group);
        const string group = "a";

        var a = ItemIn(group);
        var b = ItemIn(group);
        store.Set(a);
        store.Set(b);

        var snapshot = index.GetForKey(group);
        await Assert.That(snapshot.Count).IsEqualTo(2);

        store.Delete(a.Id);

        await Assert.That(snapshot.Count).IsEqualTo(2);           // captured reference unchanged
        await Assert.That(index.GetForKey(group).Count).IsEqualTo(1); // fresh read sees it
    }

    [Test]
    public async Task ConcurrentEnumerationAndMutation_DoesNotThrow()
    {
        // Stress test: enumerate while concurrently adding/deleting on the same key. A live-HashSet
        // implementation throws InvalidOperationException on concurrent Add during iteration; the
        // immutable index never does.
        var store = new EntityStore<Item>([]);
        var index = store.CreateIndex(s => s.Group);
        const string group = "a";

        var seed = Enumerable.Range(0, 50).Select(_ => ItemIn(group)).ToArray();
        foreach (var s in seed) store.Set(s);

        using var cts = new CancellationTokenSource();

        var reader = Task.Run(() =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var total = 0;
                foreach (var id in index.GetForKey(group)) total += id.GetHashCode();
                _ = total;
            }
        });

        var writer = Task.Run(() =>
        {
            for (var i = 0; i < 2000; i++)
            {
                var s = ItemIn(group);
                store.Set(s);
                store.Delete(s.Id);
            }
        });

        await writer;
        cts.Cancel();
        await reader;

        // No exception means the test passed; assert the index is back to the seeded state.
        await Assert.That(index.GetForKey(group).Count).IsEqualTo(seed.Length);
    }
}
