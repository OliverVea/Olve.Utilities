# Guidelines for Olve.Results

This file documents the design and usage of the **Olve.Results** project. Keep it up to date when the API changes.

## Purpose

`Olve.Results` provides a lightweight result type used throughout this repository for error handling. It enables functions to return either a success indicator or a collection of problems instead of throwing exceptions.

## Core Types

- **Result** – value-less result representing success or failure. Use `Result.Success()` or `Result.Failure(IEnumerable<ResultProblem>)` to create instances.
- **Result<T>** – result carrying a value. Create with `Result<T>.Success(value)` or `Result<T>.Failure(...)`.
- **DeletionResult** – specialized result for delete operations. It distinguishes between success, not found, and error states.
- **ResultProblem** – describes a single problem. Automatically captures file path and line number via stack traces. Supports optional tags, severity, source and args.
- **ResultProblemCollection** – enumerable wrapper over multiple problems. Provides `Append`, `Prepend` and `Merge` helpers.
- **ProblemOriginInformation** – stores origin file path and line number. `LinkString` returns a clickable link string via `Olve.Paths`.

## Helpers

- **Result.Chain** – chain multiple result-returning functions, stopping on first failure.
- **Result.Concat** – invoke multiple functions and combine their values if all succeed.
- **Result.Try** – execute actions/functions and turn caught exceptions into problems.
- **IfProblem** – run an action when a result has problems.
- **ResultEnumerableExtensions** – utilities for enumerations of results: `HasProblems`, `TryPickProblems`, `GetValues` and `GetProblems`.
- **ResultFuncExtensions** – converts `Action<T>` to `Func<T, Result>`.

## Testing Utilities

The `Olve.Results.TUnit` project adds TUnit assertion extensions used in the tests (`tests/Olve.Results.Tests`). They provide fluent checks such as `Succeeded()` or `FailedAndProblemCollection()`.

## Usage Notes

1. Prefer returning `Result`/`Result<T>` from operations to make error conditions explicit.
2. For errors, construct `ResultProblem` with a message and optional `Exception`. Additional context such as `Severity`, `Tags` and `Source` can be set. Defaults are defined on the static properties of `ResultProblem`.
3. When collecting multiple results, use `TryPickProblems` to gather all problems without throwing.
4. `DeletionResult.NotFound()` is intended for delete attempts where the target did not exist.
5. All problems capture the caller file and line number automatically. Avoid manual creation unless necessary.

## Maintenance

- When adding new result-related types or changing behaviour, update this document.
- `workflow-triggers.txt` lists paths that trigger NuGet publishing when changed.

