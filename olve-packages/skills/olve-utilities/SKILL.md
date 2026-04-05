---
name: olve-utilities
description: Reference for Olve.Utilities — meta-package with typed IDs, specialized collections, directed graphs, pagination, sentinel types, datetime formatting, and extension methods. Use when writing or reading code that uses Olve.Utilities types.
user-invocable: false
---

# Olve.Utilities

Meta-package bundling typed IDs, specialized collections, graphs, pagination, sentinel types, and extensions. Source: `Olve.Utilities/src/Olve.Utilities/`.

Reference docs: [README](references/README.md) | [Ids](references/Ids.md) | [Collections](references/Collections.md) | [Graphs](references/Graphs.md) | [Pagination](references/Pagination.md) | [Types](references/Types.md) | [Extensions](references/Extensions.md) | [Other](references/Other.md)

## Typed IDs

```csharp
var userId = Id.New<User>();
var aliceId = Id.FromName<User>("alice");   // deterministic UUIDv5
Id.TryParse<User>(userId.Value.ToString(), out var parsed);
```

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
```

## DirectedGraph

```csharp
var graph = new DirectedGraph();
var a = graph.CreateNode();
var b = graph.CreateNode();
graph.CreateEdge(a, b);
graph.TryGetOutgoingEdges(a, out var edges); // 1 edge
```

## Pagination

```csharp
var pagination = new Pagination(Page: 0, PageSize: 10);
// pagination.Offset == 0

var result = new PaginatedResult<string>(items, pagination, totalCount: 100);
// result.TotalPages == 10, result.HasNextPage == true
```

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
