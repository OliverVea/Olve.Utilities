# DeletionResult

https://olivervea.github.io/Olve.Utilities/api/Olve.Results.DeletionResult.html

Readonly struct, source-generated via `[GenerateResult]`. Three states: success, not found (grey), or error.

States:
- `static DeletionResult Success()` — `[SuccessCase]`
- `static DeletionResult NotFound()` — `[GreyCase]`: neither succeeded nor failed
- `static DeletionResult Error(ResultProblemCollection problems)` — `[ErrorCase]`
- `static DeletionResult Error(params IEnumerable<ResultProblem> problems)` — convenience overload

Properties:
- `bool IsSuccess`, `bool IsNotFound`, `bool IsError` — one per state
- `bool WasNotFound` — same as `IsNotFound`
- `bool Succeeded` — success state only
- `bool Failed` — error state only (**false for not-found**)
- `ResultProblemCollection? Problems` — null unless error

Methods:
- `bool TryPickProblems(out ResultProblemCollection? problems)` — true in the error state only
- `bool TryPickProblem<TProblem>(out TProblem? problem) where TProblem : ResultProblem` — first problem assignable to `TProblem`
- `IEnumerable<TProblem> PickProblems<TProblem>() where TProblem : ResultProblem` — all matches
- `TResult Match<TResult>(Func<TResult> onSuccess, Func<TResult> onNotFound, Func<ResultProblemCollection, TResult> onError)` — exhaustive
- `Result MapToResult(bool allowNotFound = true)` — not-found maps to success by default; `false` maps it to a problem
- `string ToString()` — `"Success"`, `"NotFound"`, or `"Error(...)"`
- Value equality: `Equals`, `==`, `!=`

Implicit conversions: `ResultProblem` -> `DeletionResult` (error), `ResultProblemCollection` -> `DeletionResult` (error)

Marked `[MustBeUsedWhenReturned]` (ORES001). Discard with `_ = ...` or `.DiscardResult()`.

```csharp
var message = store.Delete(id).Match(
    onSuccess: () => "Deleted",
    onNotFound: () => "Already gone",
    onError: problems => problems.First().ToBriefString());
```

## [GenerateResult]: custom multi-state results

`DeletionResult` is built with a source generator that consumers can use for their own result types. Declare a `readonly partial struct` with `static partial` factories, each tagged with a case attribute and taking at most one payload parameter:

```csharp
[GenerateResult]
public readonly partial struct ReservationResult
{
    [SuccessCase] public static partial ReservationResult Reserved(int seatNumber);
    [GreyCase]    public static partial ReservationResult SoldOut();
    [ErrorCase]   public static partial ReservationResult Error(ResultProblemCollection problems);
}
```

Generated: the factory bodies, `Is{Case}` predicates, `Succeeded` (any success case), `Failed` (any error case; grey cases are neither), `Match(on{Case}: ...)` with typed payloads, `Problems`/`TryPickProblems`/`TryPickProblem<T>`/`PickProblems<T>`, `MapToResult(bool allow{GreyCase} = true, ...)`, `ToString`, and value equality. When there is exactly one error case, implicit conversions from `ResultProblem`/`ResultProblemCollection` and `[MustBeUsedWhenReturned]` are also emitted.

Diagnostics: ORES002 (error) factory has more than one parameter; ORES003 (warning) no case factories; ORES004 (error) factory is not `static partial`.
