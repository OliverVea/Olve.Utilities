using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;
using Olve.Utilities.Stores;

namespace Olve.Utilities.Tests.Stores;

public class EntityStoreUniqueIndexTests
{
    private record Item(Id<Item> Id, string Name) : IHasId<Id<Item>>;

    private static Item Named(string name)
        => new(Id.New<Item>(), name);

    [Test]
    public async Task TryGet_ResolvesKeyToId()
    {
        var store = new EntityStore<Item>([]);
        var index = store.CreateUniqueIndex(s => s.Name);

        var item = Named("build");
        store.Set(item);

        await Assert.That(index.TryGet("build", out var id)).IsTrue();
        await Assert.That(id).IsEqualTo(item.Id);
    }

    [Test]
    public async Task Delete_RemovesFromIndex()
    {
        var store = new EntityStore<Item>([]);
        var index = store.CreateUniqueIndex(s => s.Name);

        var item = Named("build");
        store.Set(item);
        await Assert.That(index.ContainsKey("build")).IsTrue();

        store.Delete(item.Id);

        // Regression: the previous implementation never pruned on delete because the entity is
        // already gone from the store when OnDeleted fires.
        await Assert.That(index.ContainsKey("build")).IsFalse();
        await Assert.That(index.TryGet("build", out _)).IsFalse();
    }

    [Test]
    public async Task Delete_OfReboundId_DoesNotDropTheLiveEntry()
    {
        // A key rebound to a different id (collision), then the original id is deleted: the forward
        // entry must survive because it now points at the newer id.
        var store = new EntityStore<Item>([]);
        var index = store.CreateUniqueIndex(s => s.Name);

        var first = Named("build");
        var second = Named("build"); // same key, different id
        store.Set(first);
        store.Set(second); // rebinds "build" -> second.Id

        store.Delete(first.Id);

        await Assert.That(index.TryGet("build", out var id)).IsTrue();
        await Assert.That(id).IsEqualTo(second.Id);
    }

    [Test]
    public async Task Dispose_StopsTracking()
    {
        var store = new EntityStore<Item>([]);
        var kept = Named("kept");
        store.Set(kept);
        var index = store.CreateUniqueIndex(s => s.Name);

        index.Dispose();
        store.Set(Named("build"));
        store.Delete(kept.Id);

        await Assert.That(index.ContainsKey("build")).IsFalse();
        await Assert.That(index.ContainsKey("kept")).IsTrue();
    }

    [Test]
    public async Task DeleteEventArrivingAfterReAdd_KeepsEntityIndexed()
    {
        // See EntityStoreIndexTests: a late OnDeleted must not drop an entity that is back in the store.
        var store = new EntityStore<Item>([]);
        var item = Named("build");
        store.Set(item);
        var reAdded = false;
        store.OnDeleted.Subscribe(_ =>
        {
            if (reAdded) return;
            reAdded = true;
            store.Set(item);
        });
        using var index = store.CreateUniqueIndex(s => s.Name);

        store.Delete(item.Id);

        await Assert.That(index.TryGet("build", out var id)).IsTrue();
        await Assert.That(id).IsEqualTo(item.Id);
    }

    [Test]
    public async Task Mutate_ChangingKey_MovesId()
    {
        var store = new EntityStore<Item>([]);
        var index = store.CreateUniqueIndex(s => s.Name);
        var item = new Item(Id.New<Item>(), "old");
        store.Set(item);

        store.Mutate(item.Id, s => s with { Name = "new" });

        await Assert.That(index.ContainsKey("old")).IsFalse();
        await Assert.That(index.TryGet("new", out var id)).IsTrue();
        await Assert.That(id).IsEqualTo(item.Id);
    }
}
