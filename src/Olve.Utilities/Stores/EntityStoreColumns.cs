using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Stores;

/// <summary>
/// Dense, column-wise per-entity values kept alongside an <see cref="EntityStore{T,TId}"/>: one row per
/// entity, one array per column. Hot loops walk <see cref="EntityStoreColumn{TValue}.Values"/> as a
/// contiguous span, with no hashing and no allocation.
/// </summary>
/// <remarks>
/// <para>
/// Rows follow the store's membership, not its values: a row is added when an entity is added (each
/// column's initializer supplies the starting value) and removed when it is deleted. After that the
/// columns own their values; <see cref="IEntityStore{T,TId}.OnUpdated"/> is ignored. Deleting swaps the
/// last row into the gap, so rows stay dense and row order changes.
/// </para>
/// <para>
/// Structural changes are deferred. Store events, from any thread, only queue the id (lock-free);
/// <see cref="Sync"/> applies them by reconciling each id against the store. Between two calls to
/// <see cref="Sync"/>, <see cref="Count"/>, <see cref="Ids"/>, row numbers and every column span are
/// stable, so deleting an entity while iterating is safe: its row goes away at the next sync.
/// </para>
/// <para>
/// Threading: the columns belong to one owner thread, which calls <see cref="Sync"/> and reads and
/// writes values. Only the store may be written from other threads. Call <see cref="Sync"/> regularly
/// (e.g. once per frame), or the queue of pending ids grows.
/// </para>
/// <para>
/// Lifetime: the columns subscribe to the store's events, so the store keeps them alive. Dispose them to
/// unsubscribe, or keep them for the store's lifetime.
/// </para>
/// </remarks>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
public sealed class EntityStoreColumns<T, TId> : IDisposable, IRowCount
    where T : IHasId<TId>
    where TId : notnull
{
    private const int InitialCapacity = 16;

    private readonly EntityStore<T, TId> _store;
    private readonly ConcurrentQueue<TId> _pending = new();
    private readonly Dictionary<TId, int> _rowById = new();
    private readonly List<IBinding> _bindings = [];
    private TId[] _ids = new TId[InitialCapacity];
    private int _count;
    private int _disposed;

    internal EntityStoreColumns(EntityStore<T, TId> store)
    {
        _store = store;

        // Subscribe before populating so a write landing in between is queued, not lost; reconciling
        // an id that already has a row is a no-op.
        store.OnAdded.Subscribe(Enqueue);
        store.OnDeleted.Subscribe(Enqueue);

        foreach (var entity in store)
        {
            Reconcile(entity.Id);
        }
    }

    /// <summary>Gets the number of rows, as of the last <see cref="Sync"/>.</summary>
    public int Count => _count;

    /// <summary>Gets the id of each row; <c>Ids[row]</c> is the entity whose values sit at <c>row</c>.</summary>
    public ReadOnlySpan<TId> Ids => _ids.AsSpan(0, _count);

    /// <summary>
    /// Adds a column. <paramref name="initialValue"/> supplies the value for each row when it is added,
    /// including every row that already exists.
    /// </summary>
    public EntityStoreColumn<TValue> AddColumn<TValue>(Func<T, TValue> initialValue)
    {
        var column = new EntityStoreColumn<TValue>(this, _ids.Length);
        var binding = new Binding<TValue>(column, initialValue);

        for (var row = 0; row < _count; row++)
        {
            // A row exists only for an entity the store held at the last sync; one deleted since then
            // falls back to default until the next sync removes its row.
            column.Array[row] = _store.TryGet(_ids[row], out var entity) ? initialValue(entity) : default!;
        }

        _bindings.Add(binding);
        return column;
    }

    /// <summary>
    /// Applies the store changes queued since the last call: adds rows for new entities and swap-removes
    /// rows for deleted ones. Invalidates row numbers and spans obtained before the call.
    /// </summary>
    public void Sync()
    {
        while (_pending.TryDequeue(out var id))
        {
            Reconcile(id);
        }
    }

    /// <summary>Gets the row of <paramref name="id"/>, valid until the next <see cref="Sync"/>.</summary>
    public bool TryGetRow(TId id, out int row) => _rowById.TryGetValue(id, out row);

    /// <summary>
    /// Unsubscribes from the store. The columns stop tracking the store but stay readable and writable,
    /// frozen at their last rows. Safe to call more than once.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) return;

        _store.OnAdded.Unsubscribe(Enqueue);
        _store.OnDeleted.Unsubscribe(Enqueue);
        _pending.Clear();
    }

    private void Enqueue(TId id) => _pending.Enqueue(id);

    // Converge on the store's current state rather than trusting which event fired, as the indexes do:
    // duplicate and reordered events are harmless.
    private void Reconcile(TId id)
    {
        var present = _store.TryGet(id, out var entity);
        var hasRow = _rowById.TryGetValue(id, out var row);

        if (present && !hasRow) AddRow(id, entity!);
        else if (!present && hasRow) RemoveRow(id, row);
    }

    private void AddRow(TId id, T entity)
    {
        if (_count == _ids.Length) Grow(_ids.Length * 2);

        var row = _count++;
        _ids[row] = id;
        _rowById[id] = row;
        foreach (var binding in _bindings) binding.Initialize(row, entity);
    }

    private void RemoveRow(TId id, int row)
    {
        var last = --_count;
        _rowById.Remove(id);

        if (row != last)
        {
            var moved = _ids[last];
            _ids[row] = moved;
            _rowById[moved] = row;
        }

        foreach (var binding in _bindings) binding.MoveLastInto(row, last);
        if (RuntimeHelpers.IsReferenceOrContainsReferences<TId>()) _ids[last] = default!;
    }

    private void Grow(int capacity)
    {
        Array.Resize(ref _ids, capacity);
        foreach (var binding in _bindings) binding.Grow(capacity);
    }

    private interface IBinding
    {
        void Initialize(int row, T entity);
        void MoveLastInto(int row, int last);
        void Grow(int capacity);
    }

    private sealed class Binding<TValue>(EntityStoreColumn<TValue> column, Func<T, TValue> initialValue) : IBinding
    {
        public void Initialize(int row, T entity) => column.Array[row] = initialValue(entity);

        public void MoveLastInto(int row, int last)
        {
            if (row != last) column.Array[row] = column.Array[last];
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>()) column.Array[last] = default!;
        }

        public void Grow(int capacity) => Array.Resize(ref column.Array, capacity);
    }

    int IRowCount.RowCount => _count;
}
