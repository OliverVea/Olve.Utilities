using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;
using Olve.Utilities.Stores;

namespace Olve.Utilities.Tests.Stores;

public class EntityStoreOrderedViewTests
{
    private record Item(Id<Item> Id, int Rank) : IHasId<Id<Item>>;

    private static Item Ranked(int rank) => new(Id.New<Item>(), rank);

    private static readonly IComparer<Item> ByRank = Comparer<Item>.Create((a, b) => a.Rank.CompareTo(b.Rank));

    private static int[] Ranks(IEnumerable<Item> items) => items.Select(i => i.Rank).ToArray();

    [Test]
    public async Task OrdersByComparer()
    {
        EntityStore<Item> store = [Ranked(3), Ranked(1), Ranked(2)];
        using var view = store.CreateOrderedView(ByRank);

        await Assert.That(Ranks(view)).IsEquivalentTo([1, 2, 3], TUnit.Assertions.Enums.CollectionOrdering.Matching);
        await Assert.That(view.Count).IsEqualTo(3);
        await Assert.That(view[0].Rank).IsEqualTo(1);
    }

    [Test]
    public async Task Add_IsReflectedOnNextRead()
    {
        EntityStore<Item> store = [Ranked(2)];
        using var view = store.CreateOrderedView(ByRank);
        _ = view.Count; // populate the cache

        store.Set(Ranked(1));

        await Assert.That(Ranks(view)).IsEquivalentTo([1, 2], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    public async Task Update_IsReflectedOnNextRead()
    {
        var item = Ranked(1);
        EntityStore<Item> store = [item, Ranked(2)];
        using var view = store.CreateOrderedView(ByRank);
        _ = view.Count;

        store.Set(item with { Rank = 3 });

        await Assert.That(Ranks(view)).IsEquivalentTo([2, 3], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    public async Task Delete_IsReflectedOnNextRead()
    {
        var item = Ranked(1);
        EntityStore<Item> store = [item, Ranked(2)];
        using var view = store.CreateOrderedView(ByRank);
        _ = view.Count;

        store.Delete(item.Id);

        await Assert.That(Ranks(view)).IsEquivalentTo([2], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    public async Task ChangeDuringRebuild_IsNotServedStale()
    {
        // A change landing while the view sorts must invalidate the array being built. The comparer
        // adds an entity on its first call, i.e. after the view has listed the store.
        EntityStore<Item> store = [Ranked(2), Ranked(3)];
        var added = false;
        var comparer = Comparer<Item>.Create((a, b) =>
        {
            if (!added)
            {
                added = true;
                store.Set(Ranked(1));
            }
            return ByRank.Compare(a, b);
        });
        using var view = store.CreateOrderedView(comparer);

        _ = view.Count; // this build misses the entity added mid-sort

        await Assert.That(Ranks(view)).IsEquivalentTo([1, 2, 3], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }

    [Test]
    public async Task Dispose_UnsubscribesAndFreezesView()
    {
        EntityStore<Item> store = [Ranked(1)];
        var added = store.OnAdded.SubscriberCount;
        var updated = store.OnUpdated.SubscriberCount;
        var deleted = store.OnDeleted.SubscriberCount;
        var view = store.CreateOrderedView(ByRank);
        _ = view.Count;

        view.Dispose();
        view.Dispose();
        store.Set(Ranked(2));

        await Assert.That(store.OnAdded.SubscriberCount).IsEqualTo(added);
        await Assert.That(store.OnUpdated.SubscriberCount).IsEqualTo(updated);
        await Assert.That(store.OnDeleted.SubscriberCount).IsEqualTo(deleted);
        await Assert.That(Ranks(view)).IsEquivalentTo([1], TUnit.Assertions.Enums.CollectionOrdering.Matching);
    }
}
