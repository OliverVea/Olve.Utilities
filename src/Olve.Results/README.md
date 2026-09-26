# Olve.Results

[![NuGet](https://img.shields.io/nuget/v/Olve.Results?logo=nuget)](https://www.nuget.org/packages/Olve.Results)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.Results.html)

`Olve.Results` provides a lightweight, functional result type for structured, non-throwing error handling in .NET.
APIs return `Result` or `Result<T>` representing success or a collection of `ResultProblem`s.

---

## Installation

```bash
dotnet add package Olve.Results
```

---

## Overview

This library provides predictable control flow and composable error propagation in codebases where exceptions are undesirable for routine failures.

| Type                      | Description                                                                   |
| ------------------------- | ----------------------------------------------------------------------------- |
| `Result`                  | Represents success or a collection of problems.                               |
| `Result<T>`               | Represents success with a value or a collection of problems.                  |
| `DeletionResult`          | Three-state result: success, not found, or error with problems.               |
| `ResultProblem`           | A single problem with message, optional exception, metadata, and origin info. |
| `ResultProblemCollection` | Immutable collection supporting merge, prepend, and append.                   |

---

## Usage Examples

### Basic result handling

Wrap operations in `Result.Try()` to convert exceptions into structured problems instead of throwing them.

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L11-L18

Result<string> result = Result.Try<string, IOException>(
    () => File.ReadAllText("/tmp/olve-results-readme-test.txt"));

if (result.TryPickProblems(out var problems, out var text))
{
    foreach (var p in problems)
        Console.WriteLine(p.Message);
}
```

This captures any thrown exceptions and converts them into `ResultProblem`s without disrupting control flow.

---

### Chaining dependent operations

Use `Result.Chain()` to execute dependent steps where the second operation relies on the result of the first.

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L26-L32

Result<string> LoadUser(string name) => Result.Success(name);
Result<string> LoadProfile(string user) => Result.Success($"profile:{user}");

var result = Result.Chain(
    () => LoadUser("Alice"),
    user => LoadProfile(user)
);
```

If the first step fails, the chain stops immediately and returns its problems; otherwise, the next step runs with the successful value.

---

### Combining independent operations

Use `Result.Concat()` when multiple operations can run independently, and you want to collect all problems together.

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L40-L46

Result<string> LoadUser(string name) => Result.Success(name);
Result<int> LoadSettings() => Result.Success(42);

Result<(string user, int settings)> setup = Result.Concat(
    () => LoadUser("current"),
    () => LoadSettings()
);
```

If any operation fails, all problems are aggregated; otherwise, the combined success values are returned.

---

### Adding context

Attach higher-level context to problems as they propagate upward for clear, hierarchical error traces.

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L57-L60

var user = LoadUser("unknown");
if (user.TryPickProblems(out var problems))
{
    var contextualized = problems.Prepend("User initialization failed");
```

Each layer can prepend its own message, building a human-readable error chain.

---

### Validation pattern

Return `Result` from validation routines to make checks composable and explicit.

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L72-L81

Result Validate(string? email)
{
    if (string.IsNullOrWhiteSpace(email))
        return new ResultProblem("Email is required");

    if (!email.Contains('@'))
        return new ResultProblem("Invalid email: {0}", email);

    return Result.Success();
}
```

---

### Working with result collections

`ResultEnumerableExtensions` provide helpers for aggregating problems or values from collections of results.

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L95-L102

var results = new[]
{
    LoadUser("Alice"),
    LoadUser("Bob"),
    LoadUser("Charlie"),
};

results.TryPickProblems(out var problems, out var users);
```

Other useful extensions:

* `HasProblems()` — check if any result failed
* `GetProblems()` — flatten all problems
* `GetValues()` — extract only successful values

---

### Performance-oriented `Try*` pattern

For high-performance code paths, use a `Try*` method to avoid allocations entirely.

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L112-L125

bool TryParseInt(
    string input,
    out int value,
    [NotNullWhen(false)] out ResultProblem? problem)
{
    if (!int.TryParse(input, out value))
    {
        problem = new ResultProblem("Failed to parse '{0}'", input);
        return false;
    }

    problem = null;
    return true;
}
```

This approach avoids constructing `Result<T>` entirely, ideal for inner loops.

---

## ResultProblem

`ResultProblem` represents a single issue with optional metadata and automatically captures its origin (file + line number) for debugging.

**Properties**

* `Message`, `Args`, `Exception`
* `Tags`, `Severity`, `Source`, `IsRetryable`
* `OriginInformation` (automatically populated)

Example:

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L139-L144

var problem = new ResultProblem("Database query failed: {0}", query)
{
    Tags = ["database", "query"],
    Severity = 2,
    Source = "Repository",
};
```

Automatic origin capture provides traceable, stacktrace-like context without throwing exceptions.

### Typed problems

Subclass `ResultProblem` to model domain-specific failures, then recover them by type — the result-world equivalent of `catch (SpecificException e)`:

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L244-L248

public sealed class InsufficientFundsProblem(decimal shortfall)
    : ResultProblem("Insufficient funds: short by {0}", shortfall)
{
    public decimal Shortfall => shortfall;
}
```

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L253-L265

Result Withdraw(decimal balance, decimal amount)
    => amount > balance
        ? new InsufficientFundsProblem(amount - balance)
        : Result.Success();

var result = Withdraw(balance: 50, amount: 80);

if (result.TryPickProblem<InsufficientFundsProblem>(out var funds))
{
    Console.WriteLine(funds.Shortfall); // 30
}

var all = result.PickProblems<InsufficientFundsProblem>(); // every match, in order
```

`TryPickProblem<TProblem>()` returns the first problem assignable to `TProblem` (subclasses included), and `false` on success. `PickProblems<TProblem>()` returns every match. Both are available on `Result`, `Result<T>`, `ResultProblemCollection`, and `[GenerateResult]` types such as `DeletionResult`.

### Retryable problems

`IsRetryable` marks transient failures that may succeed if retried. Problems created from an exception (including `Result.Try`) default to `true`; all others default to `false`. A collection, `Result`, `Result<T>` or generated result is retryable only when it failed and every problem is retryable. `Prepend(message)` inherits the flag from the existing problems; pass a leading `bool` to set it explicitly.

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L274-L289

var timeout = new ResultProblem(new TimeoutException(), "Request timed out"); // IsRetryable: true
var invalid = new ResultProblem("Name is required"); // IsRetryable: false
var transient = new ResultProblem("Service unavailable") { IsRetryable = true };

Result result = new ResultProblemCollection(timeout, transient);
var retry = result.IsRetryable; // true: every problem is retryable

var mixed = new ResultProblemCollection(timeout, invalid).IsRetryable; // false: one problem is terminal

var inherited = result.Problems!.Prepend("Sync failed"); // new problem inherits: retryable
var terminal = result.Problems!.Prepend(false, "Sync failed"); // explicit: not retryable

var attempt = Result.Try<IOException>(
    () => File.ReadAllText("/nonexistent/path.txt"),
    retryable: false,
    "Config file is missing");
```

---

## DeletionResult

`DeletionResult` models deletion operations with three distinct outcomes: success, not found, and error.

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L154-L156

var success = DeletionResult.Success();
var notFound = DeletionResult.NotFound();
var error = DeletionResult.Error(new ResultProblem("disk full"));
```

### Exhaustive matching

Use `Match()` to handle all three states explicitly:

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L166-L171

var result = DeletionResult.NotFound();

var message = result.Match(
    onSuccess: () => "Deleted",
    onNotFound: () => "Already gone",
    onError: problems => $"Error: {problems.First().Message}");
```

### Converting to Result

`MapToResult()` converts a `DeletionResult` to a `Result`. By default, not-found is treated as success:

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L179-L182

var result = DeletionResult.NotFound();

var asResult = result.MapToResult(allowNotFound: true); // Success
var strict = result.MapToResult(allowNotFound: false); // Failure
```

---

## Result.Try

`Result.Try<TException>()` catches a specific exception type and converts it into a `ResultProblem`:

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L203-L205

var result = Result.Try<IOException>(
    () => File.ReadAllText("/nonexistent/path.txt"),
    "Could not read config file");
```

---

## Implicit conversions

A `ResultProblem` implicitly converts to `Result`, `Result<T>`, or `DeletionResult`, making failure returns concise:

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L191-L193

Result result = new ResultProblem("not found");
Result<int> typed = new ResultProblem("parse error");
DeletionResult deletion = new ResultProblem("disk full");
```

---

## Functional extensions

### Map and Bind

`Map` transforms the value inside a successful `Result<T>`. `Bind` (flatMap) does the same but the mapping function itself returns a `Result<T>`:

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L213-L220

var result = Result.Success("42");

var mapped = result.Map(s => s.Length); // Result<int> with value 2

var bound = result.Bind(s =>
    int.TryParse(s, out var n)
        ? Result.Success(n)
        : Result.Failure<int>(new ResultProblem("parse error")));
```

Other extensions:

* `ToEmptyResult()` — discard the value, keeping only success/failure status
* `WithValueOnSuccess()` — attach a value to a valueless `Result`

---

## Dictionary extensions

`SetWithResult` and `GetWithResult` wrap dictionary operations in results instead of throwing or returning booleans:

```cs
// ../../tests/Olve.Results.Tests/ReadmeDemo.cs#L229-L236

var dictionary = new Dictionary<string, int>();

var setResult = dictionary.SetWithResult("answer", 42); // Success
var duplicate = dictionary.SetWithResult("answer", 99); // Failure: key exists

IReadOnlyDictionary<string, int> readOnly = dictionary;
var getResult = readOnly.GetWithResult("answer"); // Result<int> with value 42
var missing = readOnly.GetWithResult("unknown"); // Failure: key not found
```

---

## Design Notes

**Goals**

* Explicit success/failure flow without exceptions
* Immutable, structured problems with optional metadata
* Debuggability through automatic origin capture

**Trade-offs**

1. Slight performance overhead from struct wrapping and collection allocations
2. Typed errors are opt-in — problems subclass `ResultProblem` rather than being fully generic error types

These trade-offs prioritize clarity and composability over micro-optimizations or rigid typing.

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.Results.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Results.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
