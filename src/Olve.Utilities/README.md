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
| **Pagination** | `Pagination`, `PaginatedResult<T>` | Page/offset calculation and paginated result wrapper |
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
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L18-L26

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
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L35-L42

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
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L51-L62

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
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L71-L81

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

`FixedSizeQueue<T>` automatically drops the oldest items when the maximum size is exceeded.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L90-L95

var queue = new FixedSizeQueue<string>(maxSize: 3);

queue.Enqueue("a");
queue.Enqueue("b");
queue.Enqueue("c");
queue.Enqueue("d"); // "a" is dropped, queue is now { "b", "c", "d" }
```

---

### DateTime formatting

`DateTimeFormatter.FormatTimeAgo()` produces human-readable relative time strings like "2 days ago" or "just now".

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L104-L107

var now = new DateTimeOffset(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);
var then = new DateTimeOffset(2025, 6, 13, 12, 0, 0, TimeSpan.Zero);

var text = DateTimeFormatter.FormatTimeAgo(now, then); // "2 days ago"
```

---

### Pagination

`Pagination` computes offsets from page number and size. `PaginatedResult<T>` wraps a page of items with total count and navigation metadata.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L115-L124

var items = new[] { "alice", "bob", "charlie" };
var pagination = new Pagination(Page: 0, PageSize: 2);

var result = new PaginatedResult<string>(
    items: items[..2],
    pagination: pagination,
    totalCount: items.Length);

// result.HasNextPage == true
// result.TotalPages == 2
```

---

### DirectedGraph

`DirectedGraph` provides an ID-based directed graph with node and edge management.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L134-L146

var graph = new DirectedGraph();

// Create nodes
var nodeA = graph.CreateNode();
var nodeB = graph.CreateNode();
var nodeC = graph.CreateNode();

// Create edges: A -> B, A -> C
graph.CreateEdge(nodeA, nodeB);
graph.CreateEdge(nodeA, nodeC);

// Query outgoing edges
graph.TryGetOutgoingEdges(nodeA, out var edges); // 2 edges
```

---

### DictionaryExtensions

High-performance `GetOrAdd` and `TryUpdate` extensions using `CollectionsMarshal` for zero-overhead dictionary operations.

```cs
// ../../tests/Olve.Utilities.Tests/ReadmeDemo.cs#L155-L165

var cache = new Dictionary<string, List<int>>();

// GetOrAdd: get existing value or create it
var list = cache.GetOrAdd("scores", () => []);
list.Add(100);

var same = cache.GetOrAdd("scores", () => []); // same reference as list

// TryUpdate: update only if the key exists
var updated = cache.TryUpdate("scores", old => [..old, 200]); // true
var missed = cache.TryUpdate("missing", _ => []); // false
```

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
