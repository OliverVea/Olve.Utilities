using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Olve.Utilities.Ids;
using Olve.Utilities.Stores;

namespace Olve.Utilities.Benchmarks.Stores;

/// <summary>
/// One simulation frame: advance every train's position by its speed. Compares where the per-frame
/// state lives.
/// </summary>
[MemoryDiagnoser]
public class FrameUpdateBenchmarks
{
    private const float Dt = 1f / 60;

    [Params(1_000, 10_000, 100_000)]
    public int N;

    private EntityStore<Train> _store = null!;
    private ConcurrentDictionary<Id<Train>, float> _concurrentPositions = null!;
    private ConcurrentDictionary<Id<Train>, float> _concurrentSpeeds = null!;
    private Id<Train>[] _ids = null!;
    private Dictionary<Id<Train>, float> _positions = null!;
    private Dictionary<Id<Train>, float> _speeds = null!;
    private EntityStoreColumns<Train, Id<Train>> _columns = null!;
    private EntityStoreColumn<float> _positionColumn = null!;
    private EntityStoreColumn<float> _speedColumn = null!;

    [GlobalSetup]
    public void Setup()
    {
        var trains = Enumerable.Range(0, N).Select(i => new Train(Id.New<Train>(), i, 1 + i % 7)).ToArray();

        _store = new EntityStore<Train>(trains);
        _ids = trains.Select(t => t.Id).ToArray();
        _concurrentPositions = new(trains.Select(t => KeyValuePair.Create(t.Id, t.Position)));
        _concurrentSpeeds = new(trains.Select(t => KeyValuePair.Create(t.Id, t.Speed)));
        _positions = trains.ToDictionary(t => t.Id, t => t.Position);
        _speeds = trains.ToDictionary(t => t.Id, t => t.Speed);

        _columns = _store.CreateColumns();
        _positionColumn = _columns.AddColumn(t => t.Position);
        _speedColumn = _columns.AddColumn(t => t.Speed);
    }

    [GlobalCleanup]
    public void Cleanup() => _columns.Dispose();

    /// <summary>State on the entity, rewritten with <c>Mutate</c> each frame.</summary>
    [Benchmark]
    public void EntityMutate()
    {
        foreach (var train in _store)
        {
            _ = _store.Mutate(train.Id, t => t with { Position = t.Position + t.Speed * Dt });
        }
    }

    /// <summary>Olve.Trains today: one <c>ConcurrentDictionary</c> per field, keyed by id.</summary>
    [Benchmark(Baseline = true)]
    public void ConcurrentDictionaries()
    {
        foreach (var (id, position) in _concurrentPositions)
        {
            _concurrentPositions[id] = position + _concurrentSpeeds[id] * Dt;
        }
    }

    /// <summary>Plain dictionaries at their best: iterate an id array, update in place by ref.</summary>
    [Benchmark]
    public void DictionariesByRef()
    {
        foreach (var id in _ids)
        {
            ref var position = ref CollectionsMarshal.GetValueRefOrNullRef(_positions, id);
            position += _speeds[id] * Dt;
        }
    }

    [Benchmark]
    public void Columns()
    {
        var positions = _positionColumn.Values;
        var speeds = _speedColumn.Values;
        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] += speeds[i] * Dt;
        }
    }
}
