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
}
