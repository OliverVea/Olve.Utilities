using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using Olve.Utilities.Ids;
using Olve.Utilities.Stores;

namespace Olve.Utilities.Benchmarks.Stores;

/// <summary>
/// A frame with churn: 1% of trains are deleted and as many added through the store, then every
/// position advances. Prices keeping the per-frame state in sync with the store.
/// </summary>
[MemoryDiagnoser]
public class ChurnBenchmarks
{
    private const float Dt = 1f / 60;

    [Params(1_000, 10_000, 100_000)]
    public int N;

    private int _churn;
    private int _next;

    // Olve.Trains today: store events maintain per-field ConcurrentDictionaries.
    private EntityStore<Train> _concurrentStore = null!;
    private Queue<Id<Train>> _concurrentLive = null!;
    private ConcurrentDictionary<Id<Train>, float> _concurrentPositions = null!;
    private ConcurrentDictionary<Id<Train>, float> _concurrentSpeeds = null!;

    private EntityStore<Train> _columnStore = null!;
    private Queue<Id<Train>> _columnLive = null!;
    private EntityStoreColumns<Train, Id<Train>> _columns = null!;
    private EntityStoreColumn<float> _positionColumn = null!;
    private EntityStoreColumn<float> _speedColumn = null!;

    [GlobalSetup]
    public void Setup()
    {
        _churn = Math.Max(1, N / 100);

        _concurrentStore = new EntityStore<Train>();
        _concurrentPositions = new();
        _concurrentSpeeds = new();
        _concurrentStore.OnAdded.Subscribe(id =>
        {
            if (!_concurrentStore.TryGet(id, out var train)) return;
            _concurrentPositions[id] = train.Position;
            _concurrentSpeeds[id] = train.Speed;
        });
        _concurrentStore.OnDeleted.Subscribe(id =>
        {
            _concurrentPositions.TryRemove(id, out _);
            _concurrentSpeeds.TryRemove(id, out _);
        });
        _concurrentLive = new Queue<Id<Train>>();

        _columnStore = new EntityStore<Train>();
        _columns = _columnStore.CreateColumns();
        _positionColumn = _columns.AddColumn(t => t.Position);
        _speedColumn = _columns.AddColumn(t => t.Speed);
        _columnLive = new Queue<Id<Train>>();

        for (var i = 0; i < N; i++)
        {
            _concurrentLive.Enqueue(AddTrain(_concurrentStore));
            _columnLive.Enqueue(AddTrain(_columnStore));
        }
        _columns.Sync();
    }

    [GlobalCleanup]
    public void Cleanup() => _columns.Dispose();

    [Benchmark(Baseline = true)]
    public void ConcurrentDictionaries()
    {
        Churn(_concurrentStore, _concurrentLive);

        foreach (var (id, position) in _concurrentPositions)
        {
            _concurrentPositions[id] = position + _concurrentSpeeds[id] * Dt;
        }
    }

    [Benchmark]
    public void Columns()
    {
        Churn(_columnStore, _columnLive);
        _columns.Sync();

        var positions = _positionColumn.Values;
        var speeds = _speedColumn.Values;
        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] += speeds[i] * Dt;
        }
    }

    // Delete the oldest trains and add as many new ones, keeping the size at N.
    private void Churn(EntityStore<Train> store, Queue<Id<Train>> live)
    {
        for (var i = 0; i < _churn; i++)
        {
            _ = store.Delete(live.Dequeue());
            live.Enqueue(AddTrain(store));
        }
    }

    private Id<Train> AddTrain(EntityStore<Train> store)
    {
        var train = new Train(Id.New<Train>(), _next, 1 + _next % 7);
        _next++;
        store.Set(train);
        return train.Id;
    }
}
