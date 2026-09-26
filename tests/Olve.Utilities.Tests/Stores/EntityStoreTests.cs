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

        var updates = new List<EntityUpdated<Counter, Id<Counter>>>();
        store.OnUpdated.Subscribe(updates.Add);

        var result = store.Mutate(id, c => c with { Value = c.Value + 1 });

        await Assert.That(result).Succeeded();
        await Assert.That(updates).IsEquivalentTo([new EntityUpdated<Counter, Id<Counter>>(id, new Counter(id, 0), new Counter(id, 1))]);
        store.TryGet(id, out var stored);
        await Assert.That(stored!.Value).IsEqualTo(1);
    }

    [Test]
    public async Task Set_HandlerThrows_WriteSucceeds()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        store.Set(new Counter(id, 0));
        store.OnUpdated.Subscribe(_ => throw new InvalidOperationException("boom"));

        await Assert.That(() => store.Set(new Counter(id, 1))).ThrowsNothing();

        store.TryGet(id, out var stored);
        await Assert.That(stored!.Value).IsEqualTo(1);
    }

    [Test]
    public async Task Mutate_Mutates_HandlerThrows_Succeeds()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        store.Set(new Counter(id, 0));
        store.OnUpdated.Subscribe(_ => throw new InvalidOperationException("boom"));

        var result = store.Mutate(id, c => c with { Value = c.Value + 1 });

        await Assert.That(result).Succeeded();
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

        var added = new List<EntityAdded<Counter, Id<Counter>>>();
        var updated = 0;
        store.OnAdded.Subscribe(added.Add);
        store.OnUpdated.Subscribe(_ => updated++);

        store.Set(new Counter(id, 0));

        await Assert.That(added).IsEquivalentTo([new EntityAdded<Counter, Id<Counter>>(id, new Counter(id, 0))]);
        await Assert.That(updated).IsEqualTo(0);
    }

    [Test]
    public async Task Set_Existing_FiresOnUpdated()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        store.Set(new Counter(id, 0));

        var added = 0;
        var updates = new List<EntityUpdated<Counter, Id<Counter>>>();
        store.OnAdded.Subscribe(_ => added++);
        store.OnUpdated.Subscribe(updates.Add);

        store.Set(new Counter(id, 1));

        await Assert.That(added).IsEqualTo(0);
        await Assert.That(updates).IsEquivalentTo([new EntityUpdated<Counter, Id<Counter>>(id, new Counter(id, 0), new Counter(id, 1))]);
    }

    [Test]
    public async Task Set_EqualValue_DoesNotFire()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        store.Set(new Counter(id, 0));

        var fires = 0;
        store.OnUpdated.Subscribe(_ => fires++);

        store.Set(new Counter(id, 0));

        await Assert.That(fires).IsEqualTo(0);
    }

    [Test]
    public async Task Delete_Existing_Succeeds_FiresOnDeleted()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        store.Set(new Counter(id, 0));

        var deleted = new List<EntityDeleted<Counter, Id<Counter>>>();
        store.OnDeleted.Subscribe(deleted.Add);

        var result = store.Delete(id);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(deleted).IsEquivalentTo([new EntityDeleted<Counter, Id<Counter>>(id, new Counter(id, 0))]);
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

    [Test]
    public async Task Count_TracksSetAndDelete()
    {
        var store = new EntityStore<Counter>([new Counter(Id.New<Counter>(), 0)]);
        var id = Id.New<Counter>();

        store.Set(new Counter(id, 0));
        store.Set(new Counter(id, 1));
        await Assert.That(store.Count).IsEqualTo(2);

        await Assert.That(store.Delete(id).Succeeded).IsTrue();
        await Assert.That(store.Count).IsEqualTo(1);
    }

    [Test]
    public async Task CollectionExpression_Empty_CreatesEmptyStore()
    {
        EntityStore<Counter> store = [];

        await Assert.That(store.Count).IsEqualTo(0);
    }

    [Test]
    public async Task CollectionExpression_WithElements_SeedsStore()
    {
        var a = new Counter(Id.New<Counter>(), 1);
        var b = new Counter(Id.New<Counter>(), 2);

        EntityStore<Counter> store = [a, b];

        await Assert.That(store.Count).IsEqualTo(2);
        await Assert.That(store.Contains(a.Id)).IsTrue();
        await Assert.That(store.Contains(b.Id)).IsTrue();
    }

    [Test]
    public async Task ParameterlessConstructor_CreatesEmptyStore()
    {
        EntityStore<Counter> store = new();

        await Assert.That(store.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Enumeration_YieldsEveryEntity()
    {
        var a = new Counter(Id.New<Counter>(), 1);
        var b = new Counter(Id.New<Counter>(), 2);
        EntityStore<Counter> store = [a, b];

        await Assert.That(store.Select(c => c.Value).Order().ToArray()).IsEquivalentTo(new[] { 1, 2 });
    }

    [Test]
    public async Task Enumeration_ToleratesWritesDuringEnumeration()
    {
        EntityStore<Counter> store = [new Counter(Id.New<Counter>(), 1), new Counter(Id.New<Counter>(), 2)];

        foreach (var counter in store)
        {
            store.Delete(counter.Id);
            store.Set(new Counter(Id.New<Counter>(), counter.Value + 10));
        }

        await Assert.That(store.Count).IsGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task TryAdd_NewId_AddsAndFiresOnAdded()
    {
        var store = new EntityStore<Counter>([]);
        var added = 0;
        store.OnAdded.Subscribe(_ => added++);
        var counter = new Counter(Id.New<Counter>(), 1);

        var result = store.TryAdd(counter);

        await Assert.That(result).IsTrue();
        await Assert.That(added).IsEqualTo(1);
        await Assert.That(store.Contains(counter.Id)).IsTrue();
    }

    [Test]
    public async Task TryAdd_ExistingId_KeepsExistingAndFiresNothing()
    {
        var id = Id.New<Counter>();
        var store = new EntityStore<Counter>([new Counter(id, 1)]);
        var fired = 0;
        store.OnAdded.Subscribe(_ => fired++);
        store.OnUpdated.Subscribe(_ => fired++);

        var result = store.TryAdd(new Counter(id, 2));

        await Assert.That(result).IsFalse();
        await Assert.That(fired).IsEqualTo(0);
        store.TryGet(id, out var stored);
        await Assert.That(stored!.Value).IsEqualTo(1);
    }

    [Test]
    public async Task TryAdd_ConcurrentSameId_ExactlyOneWins()
    {
        var store = new EntityStore<Counter>([]);
        var id = Id.New<Counter>();
        var added = 0;
        store.OnAdded.Subscribe(_ => Interlocked.Increment(ref added));
        const int threadCount = 8;
        using var start = new Barrier(threadCount);
        var wins = 0;

        var threads = Enumerable.Range(0, threadCount).Select(i => new Thread(() =>
        {
            start.SignalAndWait();
            if (store.TryAdd(new Counter(id, i))) Interlocked.Increment(ref wins);
        })).ToList();
        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());

        await Assert.That(wins).IsEqualTo(1);
        await Assert.That(added).IsEqualTo(1);
    }

    [Test]
    public async Task Count_ConcurrentSetsAndDeletes_MatchesContents()
    {
        var store = new EntityStore<Counter>([]);
        // One id keeps the store flipping between empty and one entity, where a lagging counter goes negative.
        var ids = new[] { Id.New<Counter>() };
        const int threadCount = 8;
        using var start = new Barrier(threadCount + 1);
        var writersDone = false;
        Exception? readerFailure = null;

        var writers = Enumerable.Range(0, threadCount).Select(t => new Thread(() =>
        {
            start.SignalAndWait();
            for (var i = 0; i < 5_000; i++)
            {
                var id = ids[(i + t) % ids.Length];
                if (i % 2 == 0) store.Delete(id);
                else store.Set(new Counter(id, i));
            }
        })).ToList();
        // Count and List() are read while writes are in flight, where the counter can momentarily lag.
        var reader = new Thread(() =>
        {
            start.SignalAndWait();
            try
            {
                while (!Volatile.Read(ref writersDone)) _ = store.List();
            }
            catch (Exception ex)
            {
                readerFailure = ex;
            }
        });
        writers.ForEach(t => t.Start());
        reader.Start();
        writers.ForEach(t => t.Join());
        Volatile.Write(ref writersDone, true);
        reader.Join();

        await Assert.That(readerFailure).IsNull();
        await Assert.That(store.Count).IsEqualTo(store.List().Count);
    }
}
