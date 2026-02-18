# Olve.Validation

[![NuGet](https://img.shields.io/nuget/v/Olve.Validation?logo=nuget)](https://www.nuget.org/packages/Olve.Validation)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.html)

Fluent validation helpers built on top of [Olve.Results](https://www.nuget.org/packages/Olve.Results). Validators accumulate rules and return a `Result` containing all problems found.

---

## Installation

```bash
dotnet add package Olve.Validation
```

---

## Overview

| Type | Description |
| --- | --- |
| `StringValidator` | String validation: null checks, length constraints, allowed/disallowed values. |
| `IntValidator` | Integer validation: numeric range, even/odd. |
| `DecimalValidator<T>` | Generic numeric validation for any `INumber<T>` type. |
| `EnumerableValidator<T>` | Collection validation: empty, duplicates, count. |
| `ListValidator<T>` | Optimized collection validation for `IList<T>`. |
| `IValidator<T>` | Interface for any validator that returns `Result`. |
| `IValidatable` | Interface for objects that can validate their own state. |

---

## Usage

### String validation

Chain rules to build a validator, then call `Validate()` to get a `Result`.

```cs
// ../../tests/Olve.Validation.Tests/ReadmeDemo.cs#L12-L16

var result = new StringValidator()
    .CannotBeNullOrWhiteSpace()
    .MustHaveMinLength(3)
    .MustHaveMaxLength(50)
    .Validate("Alice");
```

When multiple rules fail, all problems are collected into the result:

```cs
// ../../tests/Olve.Validation.Tests/ReadmeDemo.cs#L24-L34

var result = new StringValidator()
    .CannotBeNullOrWhiteSpace()
    .MustHaveMinLength(3)
    .Validate("");

// Multiple rules can fail at once — all problems are collected
if (result.TryPickProblems(out var problems))
{
    foreach (var p in problems)
        Console.WriteLine(p.Message);
}
```

---

### Numeric validation

`IntValidator` provides integer-specific rules like `MustBeEven()` and `MustBeOdd()` on top of the standard numeric range methods.

```cs
// ../../tests/Olve.Validation.Tests/ReadmeDemo.cs#L43-L46

var result = new IntValidator()
    .MustBePositive()
    .MustBeLessThan(100)
    .Validate(42);
```

`DecimalValidator<T>` works with any `INumber<T>` type (`double`, `decimal`, `float`, etc.):

```cs
// ../../tests/Olve.Validation.Tests/ReadmeDemo.cs#L54-L56

var result = new DecimalValidator<double>()
    .MustBeBetween(0.0, 1.0)
    .Validate(0.5);
```

---

### Collection validation

```cs
// ../../tests/Olve.Validation.Tests/ReadmeDemo.cs#L64-L68

var result = new EnumerableValidator<string>()
    .CannotBeNull()
    .CannotBeEmpty()
    .CannotContainDuplicates()
    .Validate(new[] { "a", "b", "c" });
```

`ListValidator<T>` provides the same API optimized for `IList<T>`.

---

### Custom error messages

Use `WithMessage()` or `WithProblem()` after a rule to override its default error:

```cs
// ../../tests/Olve.Validation.Tests/ReadmeDemo.cs#L76-L87

var result = new StringValidator()
    .CannotBeNullOrWhiteSpace()
    .WithMessage("Username is required")
    .MustHaveMinLength(3)
    .WithMessage("Username must be at least 3 characters")
    .Validate(null);

if (result.TryPickProblems(out var problems))
{
    foreach (var p in problems)
        Console.WriteLine(p.Message);
}
```

---

### Integration with Olve.Results

Validators return `Result`, so they compose naturally with the rest of the Olve.Results API:

```cs
// ../../tests/Olve.Validation.Tests/ReadmeDemo.cs#L95-L104

Result ValidateEmail(string? email)
{
    return new StringValidator()
        .CannotBeNullOrWhiteSpace()
        .WithMessage("Email is required")
        .Validate(email);
}

var valid = ValidateEmail("user@example.com");
var invalid = ValidateEmail(null);
```

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
