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

## Lifetime: keep indexes and views in a field

`CreateIndex`, `CreateUniqueIndex`, `CreateOrderedView` and `CreateColumns` (extension methods on any `IEntityStore<T, TId>`) subscribe to the store **weakly**: the store does not keep them alive. Keep a reference for as long as you read one, typically in a field next to the store. Once it is unreachable, it is garbage-collected and each of its subscriptions is dropped the next time that event fires. `Dispose()` unsubscribes immediately; afterwards it stays readable but frozen. `Dispose()` is idempotent.

Creating one per call no longer leaks, but each one re-reads the whole store on creation and keeps running on every write until the next GC, so it is still wasteful.

```csharp
// Good: built once, lives as long as the repository
public sealed class TrainRepository
{
    private readonly EntityStore<Train> _trains = [];
    private readonly EntityStoreIndex<Train, Id<Line>> _byLine;

    public TrainRepository() => _byLine = _trains.CreateIndex(t => t.LineId);
}

// Wasteful: rebuilt from the whole store on every call
IReadOnlyCollection<Id<Train>> TrainsOn(Id<Line> line) => trains.CreateIndex(t => t.LineId).GetForKey(line);
```

## Entities are immutable; events carry committed values

Entities are immutable values: records, changed with `with`. Event payloads hand every subscriber the instance the store holds, so mutating one in place would change it for everyone.

| Event | Payload | Fired by |
|---|---|---|
| `OnAdded` | `EntityAdded<T, TId>(Id, Entity)` | `Set` or `TryAdd` of a new id |
| `OnUpdated` | `EntityUpdated<T, TId>(Id, Before, After)` | `Set` of a different value, or a `Mutate` that changed something |
| `OnDeleted` | `EntityDeleted<T, TId>(Id, Entity)` | `Delete` that removed something; `Entity` is the removed value |

- Events fire **after** the write. There is no pre-delete hook: decide whether to delete before calling `Delete`.
- Each payload is exactly what that write committed, but events for the same id can arrive **out of order across threads**. State derived from the store should re-read it (as indexes, views and columns do) rather than apply payloads in arrival order.
- Writing a value equal to the current one (`Set` or `Mutate`) is a no-op and fires nothing.

## Indexes follow key changes

Indexes subscribe to `OnUpdated` and skip updates whose `Before` and `After` share a key, without taking a lock; any other update moves the id. So `Mutate` and a replacing `Set` may change an indexed key. The key selector runs twice per update, so keep it pure and cheap.

## EntityStore\<T, TId\> / EntityStore\<T\>

```csharp
public class EntityStore<T, TId> : IEntityStore<T, TId>, IEnumerable<T>
    where T : IHasId<TId> where TId : notnull
{
    public EntityStore();
    public EntityStore(IEnumerable<T> initialEntities);

    public Event<EntityAdded<T, TId>> OnAdded { get; }
    public Event<EntityUpdated<T, TId>> OnUpdated { get; }
    public Event<EntityDeleted<T, TId>> OnDeleted { get; }

    public void Set(T entity);                        // insert or replace; equal value is a no-op
    public bool TryAdd(T entity);                     // insert only if absent (atomic); OnAdded on success
    public Result Mutate(TId id, Func<T, T> mutate);  // atomic read-modify-write (compare-and-swap)
    public bool TryGet(TId id, [NotNullWhen(true)] out T? entity);
    public bool Contains(TId id);
    public int Count { get; }                         // lock-free counter
    public IReadOnlyList<T> List();                   // lock-free copy; not a moment-in-time snapshot under concurrent writes
    public DeletionResult Delete(TId id);             // Success or NotFound
    public IEnumerator<T> GetEnumerator();            // live view: no copy, no lock, not a snapshot
}

// Id<T>-keyed store: the default for durable, globally-identified entities
public class EntityStore<T>(IEnumerable<T> initialEntities) : EntityStore<T, Id<T>>
    where T : IHasId<Id<T>>
{
    public EntityStore();
}

// Factories, on any IEntityStore<T, TId>
public static class EntityStoreExtensions
{
    CreateIndex(keySelector)        // EntityStoreIndex<T, TKey> for Id<T> stores, else EntityStoreIndex<T, TId, TKey>
    CreateUniqueIndex(keySelector)  // EntityStoreUniqueIndex<T, TKey> / EntityStoreUniqueIndex<T, TId, TKey>
    CreateOrderedView(comparer)     // EntityStoreOrderedView<T, TId>
    CreateColumns()                 // EntityStoreColumns<T, TId>
}
```

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

The entity-at-a-time surface: `OnAdded`/`OnUpdated`/`OnDeleted`, `Set`, `TryAdd`, `Mutate`, `TryGet`, `Count`, `List`, `Delete`, `Contains`. No enumeration. Index/view factories are extension methods on it. `TId` is typically `Id<T>` or `ShortId<T>`.

```csharp
IEntityStore<Slime, ShortId<Slime>> slimes = new EntityStore<Slime, ShortId<Slime>>();
```

## EntityStoreIndex\<T, TId, TKey\> / EntityStoreIndex\<T, TKey\>

Non-unique secondary index: key -> set of ids. `EntityStoreIndex<T, TKey>` is the `Id<T>` shorthand. Reads are lock-free.

```csharp
public class EntityStoreIndex<T, TId, TKey> : IDisposable
{
    public IReadOnlyCollection<TId> GetForKey(TKey key);  // immutable snapshot, safe to enumerate lock-free
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

## EntityStoreUniqueIndex\<T, TId, TKey\> / EntityStoreUniqueIndex\<T, TKey\>

Key -> single id. `EntityStoreUniqueIndex<T, TKey>` is the `Id<T>` shorthand. Reads are lock-free.

```csharp
public class EntityStoreUniqueIndex<T, TId, TKey> : IDisposable
{
    public bool TryGet(TKey key, out TId id);
    public bool ContainsKey(TKey key);
    public void Dispose();
}
```

Does **not** enforce uniqueness: adding a second entity with the same key silently rebinds the key to the newer id (last write wins).

## EntityStoreOrderedView\<T, TId\>

`IReadOnlyList<T>` over the store, sorted by the comparer. The sorted array is cached and rebuilt on the next read after any add, update or delete. Each read returns an immutable snapshot, so `foreach` is consistent, but separate `Count` and indexer calls may see different snapshots.

## EntityStoreColumns\<T, TId\>

Dense per-entity values for hot loops: one row per entity, one contiguous array per column. Rows follow store membership (added on add, swap-removed on delete); values are owned by the columns and ignore `OnUpdated`. Store events only queue ids; call `Sync()` on the owner thread (e.g. once per frame) to apply them.

```csharp
var columns = trains.CreateColumns();
var position = columns.AddColumn(t => t.Position);   // initial value per row
columns.Sync();
var ids = columns.Ids;                               // ReadOnlySpan<TId>, row order
foreach (ref var p in position.Values) p += 1f;     // Span<TValue>, valid until the next Sync
```

## Event\<T\> and EventDispatch

```csharp
public class Event<T>
{
    public void Invoke(T message);             // synchronous, in registration order, never throws
    public void Subscribe(Action<T> handler);
    public void Unsubscribe(Action<T> handler);
}

public class Event                             // same semantics, Action handlers, no message
{
    public void Invoke();
    public void Subscribe(Action handler);
    public void Unsubscribe(Action handler);
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
