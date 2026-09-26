# Olve.Results

Source: https://olivervea.github.io/Olve.Utilities/src/Olve.Results/README.html

A lightweight, functional result type for structured, non-throwing error handling in .NET. APIs return `Result` or `Result<T>` representing either success or a collection of `ResultProblem`s.

## Key Types

- **Result** — Represents success or problems.
- **Result\<T\>** — Success with a value or problems.
- **DeletionResult** — Three-state result (success, not found, or error).
- **ResultProblem** — Individual problem with message, exception, metadata, and origin info.
- **ResultProblemCollection** — Immutable collection supporting merge, prepend, append, and pick-by-type.

## Primary Usage Patterns

**Exception conversion:** `Result.Try()` wraps operations to convert exceptions into `ResultProblem`s instead of throwing.

**Chaining:** `Result.Chain()` executes dependent steps where the second depends on the first's success.

**Combining:** `Result.Concat()` runs independent operations and aggregates all problems.

**Context building:** Problems can have higher-level context prepended as they propagate upward.

**Validation:** Return `Result` from validation routines to make checks composable.

## Notable Features

The library automatically captures origin information (file and line number) for debugging without throwing exceptions. Functional extensions like `Map` and `Bind` enable value transformations. Dictionary extensions (`SetWithResult`, `GetWithResult`) wrap dictionary operations as results.

Typed problems: subclass `ResultProblem` and recover it with `TryPickProblem<TProblem>` / `PickProblems<TProblem>` (the result-world `catch (SpecificException)`), available on `Result`, `Result<T>`, `ResultProblemCollection`, and `[GenerateResult]` types.

`[GenerateResult]` source-generates custom multi-state result structs like `DeletionResult`.

The bundled analyzer reports ORES001 (warning; an error under `TreatWarningsAsErrors`) when a returned `Result`, `Result<T>`, `DeletionResult`, or other `[MustBeUsedWhenReturned]` value is ignored. Discard deliberately with `_ = ...` or `.DiscardResult()`.

Performance-oriented code can use `Try*` methods following a pattern that avoids allocations entirely.

## Design Philosophy

Explicit success/failure flow without exceptions. Immutable, structured problems with optional metadata. Trade-offs include slight performance overhead from struct wrapping, and typed errors are opt-in: problems subclass `ResultProblem` rather than being fully generic error types.
