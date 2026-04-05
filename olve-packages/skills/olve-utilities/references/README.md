# Olve.Utilities Reference

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.html)

`Olve.Utilities` is a meta-package that also brings in `Olve.Results`, `Olve.Paths`, and `Olve.Validation`.

## Reference Files

| File | Contents |
| --- | --- |
| [Ids](Ids.md) | `Id`, `Id<T>`, `UnionId<T1,T2>` -- GUID-backed typed identifiers |
| [Collections](Collections.md) | `BidirectionalDictionary`, `OneToManyLookup`, `ManyToManyLookup`, `FixedSizeQueue` |
| [Graphs](Graphs.md) | `Node`, `DirectedEdge`, `DirectedGraph` |
| [Pagination](Pagination.md) | `Pagination`, `PaginatedResult<T>` |
| [Types](Types.md) | Sentinel types: `NotFound`, `Success`, `AlreadyExists`, `Skipped`, `Waiting`, `Any`, `Yes` |
| [Extensions](Extensions.md) | `DictionaryExtensions`, `EnumerableExtensions`, `ISetExtensions`, `RandomExtensions`, `OneOfTryGetExtensions`, `EnumerableOneOfExtensions` |
| [Other](Other.md) | `IAsyncOnStartup`, `IBuilder<T>`, `ProjectFileNameHelper`, `ProjectFolderHelper`, `DateTimeFormatter`, `Position`, `Size`, `DeltaPosition`, `Assert`, `FrozenLookupBase`, `IdFrozenLookup` |

## Installation

```bash
dotnet add package Olve.Utilities
```

## Source Location

`src/Olve.Utilities/`
