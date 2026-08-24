using Olve.Results.TUnit;
using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;
using Olve.Utilities.Stores;

namespace Olve.Utilities.Tests.Stores;

/// <summary>
/// The generic <see cref="EntityStore{T,TId}"/> keyed by something other than <see cref="Id{T}"/>,
/// exercised through the <see cref="IEntityStore{T,TId}"/> surface.
/// </summary>
public class ShortIdEntityStoreTests
{
    private record Slime(ShortId<Slime> Id, int Health) : IHasId<ShortId<Slime>>;

    private static IEntityStore<Slime, ShortId<Slime>> NewStore(params Slime[] seed) => new EntityStore<Slime, ShortId<Slime>>(seed);

    [Test]
    public async Task Set_ThenTryGet_RoundTrips()
    {
        var store = NewStore();
        var id = new ShortId<Slime>(1);

        store.Set(new Slime(id, 100));

        await Assert.That(store.Contains(id)).IsTrue();
        store.TryGet(id, out var stored);
        await Assert.That(stored!.Health).IsEqualTo(100);
    }

    [Test]
    public async Task Set_FiresOnAddedThenOnUpdated()
    {
        var store = NewStore();
        var id = new ShortId<Slime>(1);

        var added = 0;
        var updated = 0;
        store.OnAdded.Subscribe(_ => added++);
        store.OnUpdated.Subscribe(_ => updated++);

        store.Set(new Slime(id, 100));
        store.Set(new Slime(id, 90));

        await Assert.That(added).IsEqualTo(1);
        await Assert.That(updated).IsEqualTo(1);
    }

    [Test]
    public async Task Mutate_Mutates_FiresOnUpdatedOnce()
    {
        var id = new ShortId<Slime>(1);
        var store = NewStore(new Slime(id, 100));

        var fires = 0;
        store.OnUpdated.Subscribe(_ => fires++);

        var result = store.Mutate(id, s => s with { Health = s.Health - 10 });

        await Assert.That(result).Succeeded();
        await Assert.That(fires).IsEqualTo(1);
        store.TryGet(id, out var stored);
        await Assert.That(stored!.Health).IsEqualTo(90);
    }

    [Test]
    public async Task Mutate_MissingEntity_Fails()
    {
        var store = NewStore();

        await Assert.That(store.Mutate(new ShortId<Slime>(9), s => s)).Failed();
    }

    [Test]
    public async Task Delete_RemovesAndFiresOnDeleted()
    {
        var id = new ShortId<Slime>(1);
        var store = NewStore(new Slime(id, 100));

        var deleted = 0;
        store.OnDeleted.Subscribe(_ => deleted++);

        store.Delete(id);

        await Assert.That(store.Contains(id)).IsFalse();
        await Assert.That(deleted).IsEqualTo(1);
    }

    [Test]
    public async Task List_ReturnsSeededEntities()
    {
        var store = NewStore(new Slime(new ShortId<Slime>(1), 100), new Slime(new ShortId<Slime>(2), 50));

        await Assert.That(store.List().Select(s => s.Id.Value).Order()).IsEquivalentTo([1u, 2u]);
    }

    [Test]
    public async Task IdKeyedStore_StillSatisfiesTheInterface()
    {
        IEntityStore<Message, Id<Message>> store = new EntityStore<Message>([]);
        var id = Id.New<Message>();

        store.Set(new Message(id, "hello"));

        await Assert.That(store.Contains(id)).IsTrue();
    }

    private record Message(Id<Message> Id, string Text) : IHasId<Id<Message>>;
}
