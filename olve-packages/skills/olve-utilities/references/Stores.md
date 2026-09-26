# Stores

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Stores.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Stores.html)

Source: `src/Olve.Utilities/Stores/`

Mutable, observable, thread-safe in-memory entity store (the mutable sibling of `IdFrozenLookup`). Every change fires a synchronous event, which is how secondary indexes and ordered views stay in sync.

```csharp
using Olve.Utilities.Ids;      // Id<T>, ShortId<T>
using Olve.Utilities.Lookup;   // IHasId<TId>
using Olve.Utilities.Stores;

public record Train(Id<Train> Id, Id<Line> LineId, string Name) : IHasId<Id<Train>>;
```

## Ownership rule: dispose indexes and views

`CreateIndex`, `CreateUniqueIndex` and `CreateOrderedView` subscribe to the store's events, so **the store keeps them alive**. The caller owns each one and must either:

- keep it for the store's lifetime (create once, e.g. alongside a singleton store), or
- `Dispose()` it when done (`using var view = ...`).

Creating one per request/call and never disposing it leaks: every stale index stays reachable from the store, and all of them still run on every add/delete. This caused a production OOM. After `Dispose()` the index/view stays readable but frozen at its last state. `Dispose()` is idempotent.

```csharp
// Good: built once, lives as long as the store
public sealed class TrainRepository
{
    private readonly EntityStore<Train> _trains = [];
    private readonly EntityStoreIndex<Train, Id<Line>> _byLine;

    public TrainRepository() => _byLine = _trains.CreateIndex(t => t.LineId);
}

// Good: short-lived, disposed
using var ordered = trains.CreateOrderedView(Comparer<Train>.Create((a, b) => string.CompareOrdinal(a.Name, b.Name)));

// BAD: new index per call, never disposed -> leak
IReadOnlyCollection<Id<Train>> TrainsOn(Id<Line> line) => trains.CreateIndex(t => t.LineId).GetForKey(line);
```

## Never change an indexed key in place

Indexes subscribe only to `OnAdded`/`OnDeleted`, not `OnUpdated`, by design. So neither `Mutate` nor a `Set` that replaces an existing entity may change a value an index keys on; the index would keep the stale key. To change an indexed key, `Delete` then `Set`. (Ordered views do re-sort on update.)

## EntityStore\<T, TId\> / EntityStore\<T\>

```csharp
public class EntityStore<T, TId> : IEntityStore<T, TId>, IEnumerable<T>
    where T : IHasId<TId> where TId : notnull
{
    public EntityStore();
    public EntityStore(IEnumerable<T> initialEntities);

    public Event<TId> OnAdded { get; }     // Set of a new id
    public Event<TId> OnUpdated { get; }   // Set of an existing id, or a Mutate that changed something
    public Event<TId> OnDeleted { get; }   // Delete that removed something

    public void Set(T entity);                        // insert or replace
    public Result Mutate(TId id, Func<T, T> mutate);  // atomic read-modify-write (compare-and-swap)
    public bool TryGet(TId id, [NotNullWhen(true)] out T? entity);
    public bool Contains(TId id);
    public int Count { get; }
    public IReadOnlyList<T> List();                   // snapshot copy
    public DeletionResult Delete(TId id);             // Success or NotFound
    public IEnumerator<T> GetEnumerator();            // live view: no copy, no lock, not a snapshot

    public EntityStoreOrderedView<T, TId> CreateOrderedView(IComparer<T> comparer);
}

// Id<T>-keyed store: the default for durable, globally-identified entities
public class EntityStore<T>(IEnumerable<T> initialEntities) : EntityStore<T, Id<T>>
    where T : IHasId<Id<T>>
{
    public EntityStore();
    public EntityStoreIndex<T, TKey> CreateIndex<TKey>(Func<T, TKey> keySelector) where TKey : notnull;
    public EntityStoreUniqueIndex<T, TKey> CreateUniqueIndex<TKey>(Func<T, TKey> keySelector) where TKey : notnull;
}
```

- Indexes exist only on `EntityStore<T>` (keyed by `Id<T>`). `CreateOrderedView` works on any `EntityStore<T, TId>`.
- Both support collection expressions: `EntityStore<Train> trains = [];` or `[train1, train2]`.
- `Mutate`'s delegate may run more than once under contention, so it must be pure (use a `with` expression, no side effects). It returns a failure if the id is missing or the compare-and-swap loses 10 times in a row. A no-op mutation (result equals the current value) fires nothing.
- `Mutate` returns `Result` and `Delete` returns `DeletionResult`; both are `[MustBeUsedWhenReturned]` (ORES001), so check them or discard explicitly.

```csharp
EntityStore<Train> trains = [];
var id = Id.New<Train>();

trains.Set(new Train(id, lineId, "IC 1"));   // OnAdded
trains.Set(new Train(id, lineId, "IC 2"));   // OnUpdated

if (trains.Mutate(id, t => t with { Name = "IC 3" }).TryPickProblems(out var problems))
{
    return problems;
}

trains.TryGet(id, out var train);            // train.Name == "IC 3"
var snapshot = trains.List();
foreach (var t in trains) { /* live enumeration */ }

if (trains.Delete(id).WasNotFound) { /* already gone */ }
```

## IEntityStore\<T, TId\>

The entity-at-a-time surface: `OnAdded`/`OnUpdated`/`OnDeleted`, `Set`, `Mutate`, `TryGet`, `Count`, `List`, `Delete`, `Contains`. No enumeration and no index/view factories. `TId` is typically `Id<T>` or `ShortId<T>`.

```csharp
IEntityStore<Slime, ShortId<Slime>> slimes = new EntityStore<Slime, ShortId<Slime>>();
```

## EntityStoreIndex\<T, TKey\>

Non-unique secondary index: key -> set of ids.

```csharp
public sealed class EntityStoreIndex<T, TKey> : IDisposable
{
    public IReadOnlyCollection<Id<T>> GetForKey(TKey key);  // immutable snapshot, safe to enumerate lock-free
    public bool ContainsKey(TKey key);
    public void Dispose();
}
```

Returns ids, not entities: resolve with `store.TryGet`.

```csharp
foreach (var trainId in _byLine.GetForKey(lineId))
{
    if (_trains.TryGet(trainId, out var train)) { /* ... */ }
}
```

## EntityStoreUniqueIndex\<T, TKey\>

Key -> single id.

```csharp
public sealed class EntityStoreUniqueIndex<T, TKey> : IDisposable
{
    public bool TryGet(TKey key, out Id<T> id);
    public bool ContainsKey(TKey key);
    public void Dispose();
}
```

Does **not** enforce uniqueness: adding a second entity with the same key silently rebinds the key to the newer id (last write wins).

## EntityStoreOrderedView\<T, TId\>

`IReadOnlyList<T>` over the store, sorted by the comparer. The sorted array is cached and rebuilt on the next read after any add, update or delete. Each read returns an immutable snapshot, so `foreach` is consistent, but separate `Count` and indexer calls may see different snapshots.

## Event\<T\> and EventDispatch

```csharp
public class Event<T>
{
    public void Invoke(T message);             // synchronous, in registration order, never throws
    public void Subscribe(Action<T> handler);
    public void Unsubscribe(Action<T> handler);
}

public static class EventDispatch
{
    public static Action<Exception>? OnHandlerException { get; set; }  // null (default) = swallow
}
```

- Handlers run inline on the writer's thread; the store call blocks until they finish. Bridge to a channel/queue in the handler if you need async work.
- Each handler is isolated in its own try/catch: a throwing handler does not stop later handlers and never fails the store call that raised the event.
- The exception goes to `EventDispatch.OnHandlerException`. **Apps should set it once at startup**; otherwise handler exceptions are silently swallowed. That includes a throwing index key selector, which leaves the index silently missing entries.

```csharp
var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
EventDispatch.OnHandlerException = ex => logger.LogError(ex, "Store event handler failed");
```
