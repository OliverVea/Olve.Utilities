---
name: olve-validation
description: Reference for Olve.Validation — fluent validators that accumulate rules and return Result with all problems found. Covers StringValidator, IntValidator, DecimalValidator, EnumerableValidator, ListValidator, IValidator<T>, IValidatable, and base validator classes. Use when writing or reading code that uses Olve.Validation.
user-invocable: false
---

# Olve.Validation

Fluent validation helpers built on top of Olve.Results. Validators accumulate rules and return a `Result` containing all problems found. Source: `Olve.Utilities/src/Olve.Validation/`.

Reference docs: [README](references/README.md) | [Interfaces](references/Interfaces.md) | [BaseValidators](references/BaseValidators.md) | [ConcreteValidators](references/ConcreteValidators.md)

## Quick Examples

### String validation

```csharp
var result = new StringValidator()
    .CannotBeNullOrWhiteSpace()
    .MustHaveMinLength(3)
    .MustHaveMaxLength(50)
    .Validate("Alice");
```

### Numeric validation

```csharp
var result = new IntValidator()
    .MustBePositive()
    .MustBeLessThan(100)
    .Validate(42);

var decResult = new DecimalValidator<double>()
    .MustBeBetween(0.0, 1.0)
    .Validate(0.5);
```

### Collection validation

```csharp
var result = new EnumerableValidator<string>()
    .CannotBeNull()
    .CannotBeEmpty()
    .CannotContainDuplicates()
    .Validate(new[] { "a", "b", "c" });
```

### Custom error messages

Use `WithMessage()` or `WithProblem()` after a rule to override its default error:

```csharp
var result = new StringValidator()
    .CannotBeNullOrWhiteSpace()
    .WithMessage("Username is required")
    .MustHaveMinLength(3)
    .WithMessage("Username must be at least 3 characters")
    .Validate(null);
```

### Integration with Olve.Results

Validators return `Result`, so they compose naturally with the rest of the Olve.Results API:

```csharp
Result ValidateEmail(string? email)
{
    return new StringValidator()
        .CannotBeNullOrWhiteSpace()
        .WithMessage("Email is required")
        .Validate(email);
}
```

## Key Patterns

- Chain rules fluently, then call `Validate()` to get a `Result`.
- When multiple rules fail, all problems are collected into the result.
- Use `WithMessage()` / `WithProblem()` immediately after a rule to customize its error.
- Validators implement `IValidator<T>`, so they can be passed around as interfaces.
- Objects implementing `IValidatable` can validate their own state via a parameterless `Validate()`.
