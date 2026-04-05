---
name: olve-results-tunit
description: TUnit assertion extensions for Olve.Results Result and Result<T> types. Use when writing tests that verify Result success/failure states and their values or problems.
user-invocable: false
---

# Olve.Results.TUnit

TUnit assertion extensions for `Result` and `Result<T>`. Source: `Olve.Utilities/src/Olve.Results.TUnit/`.

All methods are extension methods on `IAssertionSource<Result>` or `IAssertionSource<Result<T>>`, used inside `await Assert.That(...)`.

## API Reference

### Succeeded

Asserts the result represents success.

```csharp
// Result<T>
PropertyAssertionResult<Result<T>> Succeeded<T>(this IAssertionSource<Result<T>> source)

// Result (valueless)
PropertyAssertionResult<Result> Succeeded(this IAssertionSource<Result> source)
```

```csharp
Result<int> result = 42;
await Assert.That(result).Succeeded();

Result empty = Result.Success();
await Assert.That(empty).Succeeded();
```

### Failed

Asserts the result represents failure.

```csharp
// Result<T>
PropertyAssertionResult<Result<T>> Failed<T>(this IAssertionSource<Result<T>> source)

// Result (valueless)
PropertyAssertionResult<Result> Failed(this IAssertionSource<Result> source)
```

```csharp
Result<int> result = new ResultProblem("Not found");
await Assert.That(result).Failed();

Result empty = new ResultProblem("Something went wrong");
await Assert.That(empty).Failed();
```

### SucceededAndValue

Asserts the result succeeded and runs a nested assertion against the unwrapped value. Only available for `Result<T>`.

```csharp
MemberAssertionResult<Result<T>> SucceededAndValue<T>(this IAssertionSource<Result<T>> source,
    Func<IAssertionSource<T>, object> assertion)
```

```csharp
Result<int> result = 42;
await Assert.That(result).SucceededAndValue(v => v.IsEqualTo(42));

Result<string> nameResult = "Alice";
await Assert.That(nameResult).SucceededAndValue(v => v.IsEqualTo("Alice"));
```

### FailedAndProblemCollection

Asserts the result failed and runs a nested assertion against the `ResultProblemCollection`.

```csharp
// Result<T>
MemberAssertionResult<Result<T>> FailedAndProblemCollection<T>(this IAssertionSource<Result<T>> source,
    Func<IAssertionSource<ResultProblemCollection>, object> assertion)

// Result (valueless)
MemberAssertionResult<Result> FailedAndProblemCollection<TAssertion>(this IAssertionSource<Result> source,
    Func<IAssertionSource<ResultProblemCollection>, TAssertion> assertion)
    where TAssertion : Assertion<ResultProblemCollection>
```

```csharp
Result<int> result = new ResultProblem("Not found");
await Assert.That(result).FailedAndProblemCollection(p => p.IsNotEmpty());

Result empty = new ResultProblem("Bad request");
await Assert.That(empty).FailedAndProblemCollection(p => p.IsNotEmpty());
```
