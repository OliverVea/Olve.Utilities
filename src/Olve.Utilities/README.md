# Olve.Utilities

[![NuGet](https://img.shields.io/nuget/v/Olve.Utilities?logo=nuget)](https://www.nuget.org/packages/Olve.Utilities)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.html)

`Olve.Utilities` is a meta-package bundling typed IDs, specialized collections, datetime formatting, pagination, graph utilities, and more for .NET projects.

---

## Installation

```bash
dotnet add package Olve.Utilities
```

---

## Included Sub-packages

Installing `Olve.Utilities` also brings in:

| Package | Description |
| --- | --- |
| [`Olve.Results`](https://www.nuget.org/packages/Olve.Results) | Lightweight functional result types for non-throwing error handling |
| [`Olve.Paths`](https://www.nuget.org/packages/Olve.Paths) | Cross-platform path manipulation (Unix and Windows) |
| [`Olve.Validation`](https://www.nuget.org/packages/Olve.Validation) | Fluent validation helpers |

---

## Overview

| Category | Key Types | Description |
| --- | --- | --- |
| **IDs** | `Id`, `Id<T>`, `UnionId<T1, T2>` | GUID-backed typed identifiers with deterministic generation (UUIDv5) |
| **Collections** | `BidirectionalDictionary<T1, T2>`, `FixedSizeQueue<T>`, `OneToManyLookup<TLeft, TRight>`, `ManyToManyLookup<TLeft, TRight>` | Specialized collection types with `TryGet` pattern lookups |
| **DateTime** | `DateTimeFormatter` | Human-readable relative time formatting |
| **Pagination** | `Pagination`, `Page<T>`, `OffsetPagination`, `Slice<T>` | Page-number and offset/limit pagination with result wrappers |
| **Stores** | `EntityStore<T>`, `EntityStoreIndex<T, TKey>`, `EntityStoreUniqueIndex<T, TKey>`, `EntityStoreOrderedView<T, TId>`, `Event<T>` | Concurrent, observable in-memory entity store with secondary indexes and ordered views |
| **Graphs** | `DirectedGraph`, `Node`, `DirectedEdge` | ID-based directed graph with node/edge management |
| **Builders** | `IBuilder<T>`, `BuilderExtensions` | Builder pattern interface with validation integration |
| **Sentinel types** | `NotFound`, `Success`, `AlreadyExists`, `Waiting`, `Skipped`, `Yes`, `Any` | Zero-size marker types for use with `OneOf<T>` discriminated unions |
| **Startup** | `IAsyncOnStartup` | Priority-based async startup task interface with DI integration |
| **Extensions** | `DictionaryExtensions`, `EnumerableExtensions`, `OneOfTryGetExtensions` | Collection, enumerable, and `OneOf` helper extensions |

---

## Usage Examples

### Typed IDs

`Id<T>` provides compile-time safety so you can't accidentally pass a user ID where an order ID is expected. `Id.FromName()` generates deterministic UUIDv5 identifiers from strings.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L20-L28

// Create a random typed ID
var userId = Id.New<User>();

// Deterministic ID from a name (UUIDv5)
var aliceId = Id.FromName<User>("alice");
var aliceId2 = Id.FromName<User>("alice"); // same as aliceId

// Parse from string
Id.TryParse<User>(userId.Value.ToString(), out var parsed); // parsed == userId
```

---

### BidirectionalDictionary

`BidirectionalDictionary<T1, T2>` maintains two-way lookups. Both directions use the `TryGet` pattern.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L37-L44

var dict = new BidirectionalDictionary<string, int>();

dict.Set("alice", 1);
dict.Set("bob", 2);

// Look up in both directions
dict.TryGet("alice", out var id);    // 1
dict.TryGet(2, out var name);        // "bob"
```

---

### OneToManyLookup

`OneToManyLookup<TLeft, TRight>` maps one key to many values. Reverse lookup returns the single owner of a value.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L53-L64

var lookup = new OneToManyLookup<string, int>();

// A parent maps to many children
lookup.Set("alice", 1, true);
lookup.Set("alice", 2, true);
lookup.Set("bob", 3, true);

// Get all values for a key
lookup.TryGet("alice", out var aliceValues); // { 1, 2 }

// Reverse lookup: which key owns this value?
lookup.TryGet(1, out var owner); // "alice"
```

---

### ManyToManyLookup

`ManyToManyLookup<TLeft, TRight>` maintains a bidirectional many-to-many relationship. Both directions use the `TryGet` pattern.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L73-L83

var enrollment = new ManyToManyLookup<string, int>();

enrollment.Set("alice", 101, true);
enrollment.Set("alice", 102, true);
enrollment.Set("bob", 101, true);

// Get all course IDs for a student
enrollment.TryGet("alice", out var aliceCourses); // { 101, 102 }

// Get all students in a course
enrollment.TryGet(101, out var mathStudents); // { "alice", "bob" }
```

---

### FixedSizeQueue

`FixedSizeQueue<T>` automatically manages items when the maximum size is exceeded. Configure the behavior with `FullQueueBehavior`: `DropOldest` (default), `DropNewest`, or `Throw`.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L92-L113

var queue = new FixedSizeQueue<string>(maxSize: 3);

queue.Enqueue("a");
queue.Enqueue("b");
queue.Enqueue("c");
queue.Enqueue("d"); // "a" is dropped, queue is now { "b", "c", "d" }

queue.TryDequeue(out var first); // "b"

// configure back-pressure behavior
var strict = new FixedSizeQueue<string>(maxSize: 2, FullQueueBehavior.Throw);
strict.Enqueue("x");
strict.Enqueue("y");
// strict.Enqueue("z"); // throws InvalidOperationException

var dropping = new FixedSizeQueue<string>(maxSize: 2, FullQueueBehavior.DropNewest);
dropping.Enqueue("x");
dropping.Enqueue("y");
var accepted = dropping.Enqueue("z"); // false — "z" is rejected

// TryEnqueue fails when full, regardless of policy
var tried = strict.TryEnqueue("z"); // false — queue is at capacity
```

---

### DateTime formatting

`DateTimeFormatter.FormatTimeAgo()` produces human-readable relative time strings like "2 days ago" or "just now".

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L124-L127

var now = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);
var then = new DateTimeOffset(2025, 6, 13, 12, 0, 0, TimeSpan.Zero);

var text = DateTimeFormatter.FormatTimeAgo(now, then); // "2 days ago"
```

---

### Pagination

`Pagination` computes offsets from page number and size. `Page<T>` wraps a page of items with total count and navigation metadata.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L134-L143


var items = new[] { "alice", "bob", "charlie" };

var page = new Page<string>(
    Items: items[..2],
    PageNumber: 0,
    PageSize: 2,
    TotalCount: items.Length);

// page.HasNextPage == true
```

#### Offset/limit pagination

`OffsetPagination` and `Slice<T>` mirror `Pagination` and `Page<T>` for offset/limit APIs, where the offset need not be a multiple of the limit. Both request types have `Validate(max)` (returns a `Result`) and `Clamp(default, max)` (fixes up out-of-range values), with the bounds always passed explicitly, and `.Paginate(...)` works on `IEnumerable<T>` and `IQueryable<T>` for either kind.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L196-L210

var users = new[] { "alice", "bob", "charlie", "dave", "eve" };

// e.g. bound from a request body like { "offset": 1, "limit": 2 }
var request = new OffsetPagination(Offset: 1, Limit: 2).Clamp(defaultLimit: 20, maxLimit: 100);

var slice = users.Paginate(request);
// slice.Items == ["bob", "charlie"], slice.TotalCount == 5
// slice.HasMore == true, slice.Next == OffsetPagination { Offset = 3, Limit = 2 }

// Page-based pagination always converts to offset/limit...
var fromPage = new Pagination(Page: 2, PageSize: 2).ToOffsetPagination(); // Offset = 4, Limit = 2

// ...but offset/limit only converts back when the offset is a multiple of the limit
request.TryToPagination(out _); // false
fromPage.TryToPagination(out var pagination); // true, pagination == Pagination { Page = 2, PageSize = 2 }
```

---

### EntityStore

`EntityStore<T>` is a concurrent, observable in-memory store keyed by `Id<T>`. Every change fires `OnAdded`, `OnUpdated` or `OnDeleted`, which keeps secondary indexes (`CreateIndex`, `CreateUniqueIndex`) and ordered views (`CreateOrderedView`) in sync with the store.

Indexes and views subscribe to the store's events, so the store keeps them alive. Dispose them when you're done, or keep them for the store's lifetime. One built per request and never disposed is a memory leak.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L225-L240

// record Train(Id<Train> Id, string Line, int Order) : IHasId<Id<Train>>;
EntityStore<Train> trains = [];

// Indexes and views subscribe to the store: dispose them, or keep them for the store's lifetime
using var byLine = trains.CreateIndex(t => t.Line);
using var byOrder = trains.CreateOrderedView(Comparer<Train>.Create((a, b) => a.Order.CompareTo(b.Order)));

var express = new Train(Id.New<Train>(), "red", 2);
trains.Set(express);
trains.Set(new Train(Id.New<Train>(), "red", 1));

// Mutate is an atomic read-modify-write; it must not change a key an index uses (Line here)
var mutated = trains.Mutate(express.Id, t => t with { Order = 3 });

var redTrains = byLine.GetForKey("red"); // 2 ids
var first = byOrder[0]; // the Order = 1 train
```

Event handlers run synchronously and in isolation: a handler that throws doesn't stop the others and never fails the write that raised the event. The exception goes to `EventDispatch.OnHandlerException` instead, which is `null` (swallow) by default, so route it to your logger at startup:

```cs
EventDispatch.OnHandlerException = ex =>
    logger.LogError(ex, "Unhandled exception in an event handler; other handlers still ran.");
```

---

### DirectedGraph

`DirectedGraph` provides an ID-based directed graph with node and edge management.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L153-L165


var graph = new DirectedGraph();

// Create nodes
var nodeA = graph.CreateNode();
var nodeB = graph.CreateNode();
var nodeC = graph.CreateNode();

// Create edges: A -> B, A -> C
graph.CreateEdge(nodeA, nodeB);
graph.CreateEdge(nodeA, nodeC);

// Query outgoing edges
```

---

### DictionaryExtensions

High-performance `GetOrAdd` and `TryUpdate` extensions using `CollectionsMarshal` for zero-overhead dictionary operations.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L174-L184


var cache = new Dictionary<string, List<int>>();

// GetOrAdd: get existing value or create it
var list = cache.GetOrAdd("scores", () => []);
list.Add(100);

var same = cache.GetOrAdd("scores", () => []); // same reference as list

// TryUpdate: update only if the key exists
var updated = cache.TryUpdate("scores", old => [..old, 200]); // true
```

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
