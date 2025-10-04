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
| `ResultProblem`           | A single problem with message, optional exception, metadata, and origin info. |
| `ResultProblemCollection` | Immutable collection supporting merge, prepend, and append.                   |

---

## Usage Examples

### Basic result handling

Wrap operations in `Result.Try()` to convert exceptions into structured problems instead of throwing them.

```csharp
Result<string> result = Result.Try(() => File.ReadAllText("/tmp/example.txt"));

if (result.TryPickProblems(out var problems, out var text))
{
    foreach (var p in problems)
        Console.WriteLine(p.Message);
    return;
}

Console.WriteLine(text);
```

This captures any thrown exceptions and converts them into `ResultProblem`s without disrupting control flow.

---

### Chaining dependent operations

Use `Result.Chain()` to execute dependent steps where the second operation relies on the result of the first.

```csharp
Result<User> LoadUser(string name) => /* ... */;
Result<Profile> LoadProfile(User user) => /* ... */;

var result = Result.Chain(
    () => LoadUser("Alice"),
    user => LoadProfile(user)
);
```

If the first step fails, the chain stops immediately and returns its problems; otherwise, the next step runs with the successful value.

---

### Combining independent operations

Use `Result.Concat()` when multiple operations can run independently, and you want to collect all problems together.

```csharp
Result<(User user, Settings settings)> setup = Result.Concat(
    () => LoadUser("current"),
    () => LoadSettings()
);
```

If any operation fails, all problems are aggregated; otherwise, the combined success values are returned.

---

### Adding context

Attach higher-level context to problems as they propagate upward for clear, hierarchical error traces.

```csharp
var user = LoadUser("unknown");
if (user.TryPickProblems(out var problems))
    return problems.Prepend("User initialization failed");
```

Each layer can prepend its own message, building a human-readable error chain.

---

### Validation pattern

Return `Result` from validation routines to make checks composable and explicit.

```csharp
public Result Validate()
{
    if (string.IsNullOrWhiteSpace(Email))
        return new ResultProblem("Email is required");

    if (!Email.Contains('@'))
        return new ResultProblem("Invalid email: {0}", Email);

    return Result.Success();
}

if (user.Validate().TryPickProblems(out var problems))
    return problems.Prepend("Invalid user input");
```

---

### Working with result collections

`ResultEnumerableExtensions` provide helpers for aggregating problems or values from collections of results.

```csharp
var results = new[]
{
    LoadUser("Alice"),
    LoadUser("Bob"),
    LoadUser("Charlie")
};

if (results.TryPickProblems(out var problems, out var users))
{
    Console.WriteLine("Some users failed to load:");
    foreach (var p in problems) Console.WriteLine(p);
    return;
}

Console.WriteLine($"Loaded {users.Count} users.");
```

Other useful extensions:

* `HasProblems()` — check if any result failed
* `GetProblems()` — flatten all problems
* `GetValues()` — extract only successful values

---

### Performance-oriented `Try*` pattern

For high-performance code paths, use a `Try*` method to avoid allocations entirely.

```csharp
bool TryParseInt(string input, out int value, [MaybeNullWhen(true)] out ResultProblem problem)
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
* `Tags`, `Severity`, `Source`
* `OriginInformation` (automatically populated)

Example:

```csharp
var problem = new ResultProblem("Database query failed: {0}", query)
{
    Tags = ["database", "query"],
    Severity = 2,
    Source = "Repository"
};

// [Data/Repository.cs:42] Database query failed: SELECT * FROM Users
```

Automatic origin capture provides traceable, stacktrace-like context without throwing exceptions.

---

## Design Notes

**Goals**

* Explicit success/failure flow without exceptions
* Immutable, structured problems with optional metadata
* Debuggability through automatic origin capture

**Trade-offs**

1. Slight performance overhead from struct wrapping and collection allocations
2. No typed errors — all failures use the generic `ResultProblem` type

These trade-offs prioritize clarity and composability over micro-optimizations or rigid typing.

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.Results.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Results.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)