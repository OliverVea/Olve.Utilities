using System.Runtime.CompilerServices;
using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;
using Olve.Utilities.Stores;

namespace Olve.Utilities.Tests.Stores;

public class EntityStoreColumnsTests
{
    private record Train(Id<Train> Id, float StartPosition) : IHasId<Id<Train>>;

    private static Train At(float position) => new(Id.New<Train>(), position);

    private static float PositionOf<T>(EntityStoreColumns<T, Id<T>> columns, EntityStoreColumn<float> column, Id<T> id)
        where T : IHasId<Id<T>>
        => columns.TryGetRow(id, out var row) ? column[row] : float.NaN;

    [Test]
    public async Task Rows_StartFromInitializer_AndLineUpWithIds()
    {
        var a = At(1);
        var b = At(2);
        EntityStore<Train> store = [a, b];
        using var columns = store.CreateColumns();
        var position = columns.AddColumn(t => t.StartPosition);

        await Assert.That(columns.Count).IsEqualTo(2);
        await Assert.That(PositionOf(columns, position, a.Id)).IsEqualTo(1f);
        await Assert.That(PositionOf(columns, position, b.Id)).IsEqualTo(2f);
        for (var row = 0; row < columns.Count; row++)
        {
            store.TryGet(columns.Ids[row], out var train);
            await Assert.That(position.Values[row]).IsEqualTo(train!.StartPosition);
        }
    }

    [Test]
    public async Task DeleteFromMiddle_ValuesFollowTheirIds()
    {
        var trains = Enumerable.Range(0, 5).Select(i => At(i)).ToArray();
        EntityStore<Train> store = [.. trains];
        using var columns = store.CreateColumns();
        var position = columns.AddColumn(t => t.StartPosition);
        foreach (ref var p in position.Values) p += 100; // columns own their values after creation

        _ = store.Delete(trains[1].Id);
        columns.Sync();

        await Assert.That(columns.Count).IsEqualTo(4);
        await Assert.That(columns.TryGetRow(trains[1].Id, out _)).IsFalse();
        foreach (var train in trains.Where(t => t != trains[1]))
            await Assert.That(PositionOf(columns, position, train.Id)).IsEqualTo(train.StartPosition + 100);
    }

    [Test]
    public async Task StoreChanges_ApplyOnlyAtSync()
    {
        var a = At(1);
        EntityStore<Train> store = [a];
        using var columns = store.CreateColumns();
        var position = columns.AddColumn(t => t.StartPosition);

        // Deleting while iterating must not shuffle rows under the loop.
        _ = store.Delete(a.Id);
        store.Set(At(2));
        var countBeforeSync = columns.Count;
        var valueBeforeSync = position.Values[0];

        await Assert.That(countBeforeSync).IsEqualTo(1);
        await Assert.That(valueBeforeSync).IsEqualTo(1f);

        columns.Sync();

        await Assert.That(columns.Count).IsEqualTo(1);
        await Assert.That(position.Values[0]).IsEqualTo(2f);
    }

    [Test]
    public async Task DeleteEventArrivingAfterReAdd_KeepsRow()
    {
        var train = At(1);
        EntityStore<Train> store = [train];
        var reAdded = false;
        store.OnDeleted.Subscribe(_ =>
        {
            if (reAdded) return;
            reAdded = true;
            store.Set(train);
        });
        using var columns = store.CreateColumns();

        _ = store.Delete(train.Id);
        columns.Sync();

        await Assert.That(columns.TryGetRow(train.Id, out _)).IsTrue();
    }

    [Test]
    public async Task AddColumnLate_FillsExistingRows()
    {
        EntityStore<Train> store = [At(3), At(4)];
        using var columns = store.CreateColumns();

        var position = columns.AddColumn(t => t.StartPosition);

        await Assert.That(position.Values.ToArray()).IsEquivalentTo([3f, 4f]);
    }

    [Test]
    public async Task Update_DoesNotResetValues()
    {
        var train = At(1);
        EntityStore<Train> store = [train];
        using var columns = store.CreateColumns();
        var position = columns.AddColumn(t => t.StartPosition);
        position[0] = 42;

        store.Set(train with { StartPosition = 7 });
        columns.Sync();

        await Assert.That(position[0]).IsEqualTo(42f);
    }

    [Test]
    public async Task ConcurrentWriters_ConvergeOnStoreMembership()
    {
        var store = new EntityStore<Train>();
        using var columns = store.CreateColumns();
        _ = columns.AddColumn(t => t.StartPosition);
        const int writerCount = 4;
        using var start = new Barrier(writerCount + 1);
        var done = 0;

        var writers = Enumerable.Range(0, writerCount).Select(w => new Thread(() =>
        {
            start.SignalAndWait();
            for (var i = 0; i < 2000; i++)
            {
                var train = At(i);
                store.Set(train);
                if (i % 3 != 0) _ = store.Delete(train.Id);
            }
            Interlocked.Increment(ref done);
        })).ToList();
        writers.ForEach(t => t.Start());

        start.SignalAndWait();
        while (Volatile.Read(ref done) < writerCount) columns.Sync(); // the owner keeps syncing meanwhile
        writers.ForEach(t => t.Join());
        columns.Sync();

        var inStore = store.Select(t => t.Id).ToHashSet();
        await Assert.That(columns.Ids.ToArray().ToHashSet().SetEquals(inStore)).IsTrue();
        await Assert.That(columns.Count).IsEqualTo(inStore.Count);
    }

    [Test]
    public async Task Dispose_FreezesAndLetsColumnsBeCollected()
    {
        EntityStore<Train> store = [At(1)];
        var columns = store.CreateColumns();
        columns.Dispose();
        store.Set(At(2));
        columns.Sync();
        await Assert.That(columns.Count).IsEqualTo(1);

        var weak = CreateDisposedColumns(store);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        await Assert.That(weak.IsAlive).IsFalse();
        GC.KeepAlive(store);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateDisposedColumns(EntityStore<Train> store)
    {
        var columns = store.CreateColumns();
        columns.Dispose();
        return new WeakReference(columns);
    }
}
