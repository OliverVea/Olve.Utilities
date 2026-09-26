---
name: olve-utilities
description: Reference for Olve.Utilities — meta-package with typed IDs, specialized collections, observable entity stores with indexes, directed graphs, page/offset pagination, sentinel types, datetime formatting, and extension methods. Use when writing or reading code that uses Olve.Utilities types.
user-invocable: false
---

# Olve.Utilities

Meta-package bundling typed IDs, specialized collections, entity stores, graphs, pagination, sentinel types, and extensions. Source: `Olve.Utilities/src/Olve.Utilities/`.

Reference docs: [README](references/README.md) | [Ids](references/Ids.md) | [Collections](references/Collections.md) | [Stores](references/Stores.md) | [Graphs](references/Graphs.md) | [Pagination](references/Pagination.md) | [Types](references/Types.md) | [Extensions](references/Extensions.md) | [Other](references/Other.md)

## Typed IDs

```csharp
var userId = Id.New<User>();
var aliceId = Id.FromName<User>("alice");   // deterministic UUIDv5
Id.TryParse<User>(userId.Value.ToString(), out var parsed);
```

`Id`/`Id<T>` serialize to JSON as a GUID string and implement `IParsable`. `ShortId<T>(uint)` is a dense 4-byte alternative for store-local, hot ids.

## BidirectionalDictionary

```csharp
var dict = new BidirectionalDictionary<string, int>();
dict.Set("alice", 1);
dict.TryGet("alice", out var id);   // 1
dict.TryGet(1, out var name);       // "alice"
```

## OneToManyLookup

```csharp
var lookup = new OneToManyLookup<string, int>();
lookup.Set("alice", 1, true);
lookup.Set("alice", 2, true);
lookup.TryGet("alice", out var values); // { 1, 2 }
lookup.TryGet(1, out var owner);        // "alice"
```

## ManyToManyLookup

```csharp
var enrollment = new ManyToManyLookup<string, int>();
enrollment.Set("alice", 101, true);
enrollment.Set("bob", 101, true);
enrollment.TryGet("alice", out var courses); // { 101 }
enrollment.TryGet(101, out var students);    // { "alice", "bob" }
```

## FixedSizeQueue

```csharp
var queue = new FixedSizeQueue<string>(maxSize: 3);
queue.Enqueue("a");
queue.Enqueue("b");
queue.Enqueue("c");
queue.Enqueue("d"); // "a" is dropped

var strict = new FixedSizeQueue<string>(maxSize: 3, FullQueueBehavior.Throw);  // or DropNewest
strict.TryEnqueue("x");  // false when full, under any policy
```

## DirectedGraph

```csharp
var graph = new DirectedGraph();
var a = graph.CreateNode();
var b = graph.CreateNode();
graph.CreateEdge(a, b);
graph.TryGetOutgoingEdges(a, out var edges); // 1 edge
```

## EntityStore

Thread-safe, observable in-memory store. Entities implement `IHasId<Id<T>>` (`Olve.Utilities.Lookup`).

```csharp
EntityStore<Train> trains = [];
trains.Set(train);                                                   // OnAdded / OnUpdated
var result = trains.Mutate(id, t => t with { Name = "IC 2" });       // Result; pure delegate, may retry
trains.TryGet(id, out var found);
DeletionResult deleted = trains.Delete(id);                          // OnDeleted
```

**Indexes and views are owned by the caller.** `CreateIndex`, `CreateUniqueIndex` and `CreateOrderedView` subscribe to the store, so the store keeps them alive. Create them once and keep them for the store's lifetime, or `Dispose()` them. One built per request and never disposed leaks (this caused a prod OOM).

```csharp
_byLine = _trains.CreateIndex(t => t.LineId);       // once, e.g. in the constructor
_byLine.GetForKey(lineId);                          // IReadOnlyCollection<Id<Train>>
```

- Never change an indexed key with `Mutate` or a replacing `Set`; indexes ignore updates. Use `Delete` + `Set`.
- Event handlers are isolated; their exceptions go to `EventDispatch.OnHandlerException`. Set it at startup, otherwise they are silently swallowed.

## Pagination

Namespace `Olve.Utilities.Paginations`. `Pagination(Page, PageSize)` -> `Page<T>`; `OffsetPagination(Offset, Limit)` -> `Slice<T>`. No library defaults: bounds are always passed explicitly.

```csharp
var request = new Pagination(Page: 0, PageSize: 0).Clamp(defaultPageSize: 20, maxPageSize: 100);
Page<User> page = db.Users.OrderBy(u => u.Name).Paginate(request);  // IQueryable or IEnumerable
// page.TotalPages, page.HasNextPage, page.Next

Result valid = new OffsetPagination(Offset: 10, Limit: 500).Validate(maxLimit: 100);  // failure

new Pagination(2, 10).ToOffsetPagination();          // Offset 20, Limit 10
new OffsetPagination(20, 10).TryToPagination(out var p);  // true; false if Offset % Limit != 0
```

`Paginate` throws on a negative page/offset or a non-positive size, so `Clamp` or `Validate` request input first.

## Sentinel Types

Zero-size marker types for use with `OneOf<T>` discriminated unions:

```csharp
OneOf<User, NotFound> result = new NotFound();
OneOf<Success, AlreadyExists> upsertResult = new Success();
```

## DictionaryExtensions

```csharp
var cache = new Dictionary<string, List<int>>();
var list = cache.GetOrAdd("scores", () => []);
cache.TryUpdate("scores", old => [..old, 200]);
```
