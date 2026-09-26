using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using Olve.Utilities.Ids;
using Olve.Utilities.Stores;

namespace Olve.Utilities.Benchmarks.Stores;

/// <summary>
/// Random access by id (e.g. a train reaching a junction): 1,000 lookups per operation.
/// </summary>
[MemoryDiagnoser]
public class LookupBenchmarks
{
    private const int Lookups = 1_000;

    [Params(1_000, 10_000, 100_000)]
    public int N;

    private Id<Train>[] _probes = null!;
    private ConcurrentDictionary<Id<Train>, float> _concurrentPositions = null!;
    private Dictionary<Id<Train>, float> _positions = null!;
    private EntityStoreColumns<Train, Id<Train>> _columns = null!;
    private EntityStoreColumn<float> _positionColumn = null!;

    [GlobalSetup]
    public void Setup()
    {
        var trains = Enumerable.Range(0, N).Select(i => new Train(Id.New<Train>(), i, 1)).ToArray();
        var random = new Random(42);
        _probes = Enumerable.Range(0, Lookups).Select(_ => trains[random.Next(N)].Id).ToArray();

        _concurrentPositions = new(trains.Select(t => KeyValuePair.Create(t.Id, t.Position)));
        _positions = trains.ToDictionary(t => t.Id, t => t.Position);

        var store = new EntityStore<Train>(trains);
        _columns = store.CreateColumns();
        _positionColumn = _columns.AddColumn(t => t.Position);
    }

    [GlobalCleanup]
    public void Cleanup() => _columns.Dispose();

    [Benchmark(Baseline = true)]
    public float ConcurrentDictionary()
    {
        var sum = 0f;
        foreach (var id in _probes)
        {
            if (_concurrentPositions.TryGetValue(id, out var position)) sum += position;
        }
        return sum;
    }

    [Benchmark]
    public float Dictionary()
    {
        var sum = 0f;
        foreach (var id in _probes)
        {
            if (_positions.TryGetValue(id, out var position)) sum += position;
        }
        return sum;
    }

    [Benchmark]
    public float Columns()
    {
        var sum = 0f;
        foreach (var id in _probes)
        {
            if (_columns.TryGetRow(id, out var row)) sum += _positionColumn[row];
        }
        return sum;
    }
}
