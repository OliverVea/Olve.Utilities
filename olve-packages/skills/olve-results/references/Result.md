# Result

https://olivervea.github.io/Olve.Utilities/api/Olve.Results.Result.html

Readonly struct. Success or failure without a value. Implements `IResultType`. Marked `[MustBeUsedWhenReturned]` (ORES001).

Properties:
- `bool Succeeded`
- `bool Failed`
- `ResultProblemCollection? Problems`
- `bool IsRetryable` — `true` if failed and every problem is retryable; `false` on success

Static methods:
- `Result Success()`
- `Result<T> Success<T>(T value)`
- `Result Failure(params IEnumerable<ResultProblem> problems)`
- `Result<T> Failure<T>(params IEnumerable<ResultProblem> problems)`
- `Result Try<TException>(Action action, string? message = null, params object[] args)` — catches TException, returns as problems
- `Result<TValue> Try<TValue, TException>(Func<TValue> action, string? message = null, params object[] args)`
- `Try<TException>(Action action, bool retryable, string? message = null, params object[] args)` and `Try<TValue, TException>(Func<TValue> action, bool retryable, ...)` — set `IsRetryable` on the captured problem (default without it: `true`)

Chain (sequential dependent steps, stops on first failure):
- `Result Chain(params IEnumerable<Func<Result>> links)`
- `Result<T2> Chain<T1, T2>(Func<Result<T1>>, Func<T1, Result<T2>>)`
- `Result<T3> Chain<T1, T2, T3>(...)`
- `Result<T4> Chain<T1, T2, T3, T4>(...)`

Concat (independent steps, aggregates all problems):
- `Result Concat(params IEnumerable<Func<Result>> elements)`
- `Result Concat(params IEnumerable<Result> results)`
- `Result<(T1, T2)> Concat<T1, T2>(...)` through `Result<(T1..T6)> Concat<T1..T6>(...)`
- Both direct `Result<T>` and `Func<Result<T>>` overloads for each arity (2 through 6).

Instance methods:
- `bool TryPickProblems(out ResultProblemCollection? problems)` — true if failed
- `bool TryPickProblem<TProblem>(out TProblem? problem) where TProblem : ResultProblem` — true if a problem assignable to `TProblem` exists (first match; subclasses included); false on success
- `IEnumerable<TProblem> PickProblems<TProblem>() where TProblem : ResultProblem` — all matching problems, in order; empty on success
- `Result IfProblem(Action<ResultProblemCollection> action)` — execute on failure, returns self
- `string ToString()` — returns "Success" or "Failure"

Implicit conversions: `ResultProblem` -> `Result`, `ResultProblemCollection` -> `Result`

`IResultType` (implemented by `Result` and `Result<T>`): `Succeeded`, `Failed`, `Problems`, `object? BoxedValue`, `bool HasValue` — for non-reflective handling of results of unknown type.
