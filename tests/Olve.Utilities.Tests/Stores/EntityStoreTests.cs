using Olve.Results.TUnit;
using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;
using Olve.Utilities.Stores;

namespace Olve.Utilities.Tests.Stores;

public class EntityStoreTests
{
    private record Counter(Id<Counter> Id, int Value) : IHasId<Id<Counter>>;

    [Test]
    public async Task Mutate_Mutates_FiresOnUpdatedOnce()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        store.Set(new Counter(id, 0));

        var fires = 0;
        store.OnUpdated.Subscribe(_ => fires++);

        var result = store.Mutate(id, c => c with { Value = c.Value + 1 });

        await Assert.That(result).Succeeded();
        await Assert.That(fires).IsEqualTo(1);
        store.TryGet(id, out var stored);
        await Assert.That(stored!.Value).IsEqualTo(1);
    }

    [Test]
    public async Task Mutate_NoOp_DoesNotFire()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        store.Set(new Counter(id, 5));

        var fires = 0;
        store.OnUpdated.Subscribe(_ => fires++);

        // Returns an equal record — present but unchanged.
        var result = store.Mutate(id, c => c with { Value = 5 });

        await Assert.That(result).Succeeded();
        await Assert.That(fires).IsEqualTo(0);
    }

    [Test]
    public async Task Mutate_Missing_Fails_NoFire()
    {
        var store = new EntityStore<Counter>([]);

        var fires = 0;
        store.OnUpdated.Subscribe(_ => fires++);

        var result = store.Mutate(Id.New<Counter>(), c => c with { Value = c.Value + 1 });

        await Assert.That(result).Failed();
        await Assert.That(fires).IsEqualTo(0);
    }

    [Test]
    public async Task Set_New_FiresOnAdded()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();

        var added = 0;
        var updated = 0;
        store.OnAdded.Subscribe(_ => added++);
        store.OnUpdated.Subscribe(_ => updated++);

        store.Set(new Counter(id, 0));

        await Assert.That(added).IsEqualTo(1);
        await Assert.That(updated).IsEqualTo(0);
    }

    [Test]
    public async Task Set_Existing_FiresOnUpdated()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        store.Set(new Counter(id, 0));

        var added = 0;
        var updated = 0;
        store.OnAdded.Subscribe(_ => added++);
        store.OnUpdated.Subscribe(_ => updated++);

        store.Set(new Counter(id, 1));

        await Assert.That(added).IsEqualTo(0);
        await Assert.That(updated).IsEqualTo(1);
    }

    [Test]
    public async Task Delete_Existing_Succeeds_FiresOnDeleted()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        store.Set(new Counter(id, 0));

        var deleted = 0;
        store.OnDeleted.Subscribe(_ => deleted++);

        var result = store.Delete(id);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(deleted).IsEqualTo(1);
        await Assert.That(store.Contains(id)).IsFalse();
    }

    [Test]
    public async Task Delete_Missing_NotFound_NoFire()
    {
        var store = new EntityStore<Counter>([]);

        var deleted = 0;
        store.OnDeleted.Subscribe(_ => deleted++);

        var result = store.Delete(Id.New<Counter>());

        await Assert.That(result.WasNotFound).IsTrue();
        await Assert.That(deleted).IsEqualTo(0);
    }

    [Test]
    public async Task Mutate_ConcurrentIncrements_NoLostUpdates()
    {
        const int threads = 16;
        const int incrementsPerThread = 200;

        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        store.Set(new Counter(id, 0));

        using var start = new Barrier(threads);
        var tasks = Enumerable.Range(0, threads).Select(_ => Task.Run(() =>
        {
            start.SignalAndWait();
            for (var i = 0; i < incrementsPerThread; i++)
            {
                // Under heavy same-key contention a single Mutate may exhaust its bounded CAS
                // attempts; the contract is that the caller retries. Looping here proves no update
                // is ever lost (the regression target), independent of the attempt cap.
                while (store.Mutate(id, c => c with { Value = c.Value + 1 }).Failed)
                {
                }
            }
        }));

        await Task.WhenAll(tasks);

        store.TryGet(id, out var result);
        await Assert.That(result!.Value).IsEqualTo(threads * incrementsPerThread);
    }
}
