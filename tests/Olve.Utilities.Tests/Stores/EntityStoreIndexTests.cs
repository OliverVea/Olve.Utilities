using System.Runtime.CompilerServices;
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

    [Test]
    public async Task UndisposedIndex_IsCollectedWhileStoreLives()
    {
        var store = new EntityStore<Item>([]);

        var index = CreateUndisposedIndex(store, () => { });
        CollectGarbage();

        await Assert.That(index.IsAlive).IsFalse();
        GC.KeepAlive(store);
    }

    [Test]
    public async Task CollectedIndex_StopsRunningOnWrites()
    {
        var store = new EntityStore<Item>([]);
        var selectorCalls = 0;

        CreateUndisposedIndex(store, () => selectorCalls++);
        CollectGarbage();
        store.Set(ItemIn("a"));

        await Assert.That(selectorCalls).IsEqualTo(0);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateUndisposedIndex(EntityStore<Item> store, Action onSelect)
    {
        var index = store.CreateIndex(s =>
        {
            onSelect();
            return s.Group;
        });
        return new WeakReference(index);
    }

    private static void CollectGarbage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    [Test]
    public async Task Dispose_StopsTrackingAddsAndDeletes()
    {
        var store = new EntityStore<Item>([]);
        var index = store.CreateIndex(s => s.Group);
        const string group = "a";

        var kept = ItemIn(group);
        store.Set(kept);
        index.Dispose();

        store.Set(ItemIn(group));
        store.Delete(kept.Id);

        await Assert.That(index.GetForKey(group).Count).IsEqualTo(1);
        await Assert.That(index.GetForKey(group)).Contains(kept.Id);
    }

    [Test]
    public async Task Dispose_Twice_IsSafe_AndLeavesOtherIndexesSubscribed()
    {
        var store = new EntityStore<Item>([]);
        var other = store.CreateIndex(s => s.Group);
        var index = store.CreateIndex(s => s.Group);
        const string group = "a";

        index.Dispose();
        index.Dispose();
        store.Set(ItemIn(group));

        await Assert.That(other.GetForKey(group).Count).IsEqualTo(1);
    }

    [Test]
    public async Task DeleteEventArrivingAfterReAdd_KeepsEntityIndexed()
    {
        // Events fire outside any lock, so a Delete's OnDeleted can reach the index after a concurrent
        // Set has re-added the entity. Simulated deterministically: a handler subscribed before the
        // index re-adds the entity, so the index sees OnDeleted while the entity is back in the store.
        var store = new EntityStore<Item>([]);
        var item = ItemIn("a");
        store.Set(item);
        var reAdded = false;
        store.OnDeleted.Subscribe(_ =>
        {
            if (reAdded) return;
            reAdded = true;
            store.Set(item);
        });
        using var index = store.CreateIndex(s => s.Group);

        store.Delete(item.Id);

        await Assert.That(store.Contains(item.Id)).IsTrue();
        await Assert.That(index.GetForKey("a")).Contains(item.Id);
    }

    [Test]
    public async Task ReplacingWithDifferentKey_MovesId()
    {
        var store = new EntityStore<Item>([]);
        var item = ItemIn("a");
        store.Set(item);
        using var index = store.CreateIndex(s => s.Group);

        store.Delete(item.Id);
        store.Set(item with { Group = "b" });

        await Assert.That(index.ContainsKey("a")).IsFalse();
        await Assert.That(index.GetForKey("b")).Contains(item.Id);
    }

    [Test]
    public async Task Mutate_ChangingKey_MovesId()
    {
        var store = new EntityStore<Item>([]);
        var index = store.CreateIndex(s => s.Group);
        var item = ItemIn("a");
        store.Set(item);

        store.Mutate(item.Id, s => s with { Group = "b" });

        await Assert.That(index.ContainsKey("a")).IsFalse();
        await Assert.That(index.GetForKey("b")).Contains(item.Id);
    }

    [Test]
    public async Task Mutate_KeepingKey_DoesNotReadTheStore()
    {
        var store = new EntityStore<Item>([]);
        var item = ItemIn("a");
        store.Set(item);
        var selectorCalls = 0;
        var index = store.CreateIndex(s =>
        {
            selectorCalls++;
            return s.Group;
        });
        selectorCalls = 0;

        store.Mutate(item.Id, s => s with { Name = "renamed" });

        // Only the before/after filter ran; a reconcile would have called the selector a third time.
        await Assert.That(selectorCalls).IsEqualTo(2);
        await Assert.That(index.GetForKey("a")).Contains(item.Id);
    }

    [Test]
    public async Task CreateIndex_OverIdStore_ReturnsShorthandTypes()
    {
        // Consumers declare index fields with the two-argument shorthand; this must keep compiling.
        var store = new EntityStore<Item>([]);
        EntityStoreIndex<Item, string> byGroup = store.CreateIndex(s => s.Group);
        EntityStoreUniqueIndex<Item, string> byName = store.CreateUniqueIndex(s => s.Name);
        var item = ItemIn("a");

        store.Set(item);

        await Assert.That(byGroup.GetForKey("a")).Contains(item.Id);
        await Assert.That(byName.ContainsKey(item.Name)).IsTrue();
    }

    [Test]
    public async Task CreateIndex_OverNonIdStore_Works()
    {
        var store = new EntityStore<Slime, ShortId<Slime>>([]);
        var index = store.CreateIndex(s => s.Colour);
        var slime = new Slime(new ShortId<Slime>(1), "green");

        store.Set(slime);

        await Assert.That(index.GetForKey("green")).Contains(slime.Id);
    }

    private record Slime(ShortId<Slime> Id, string Colour) : IHasId<ShortId<Slime>>;
}
