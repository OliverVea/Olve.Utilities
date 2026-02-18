# Guidelines for Olve.Validation

This library provides fluent validator helpers built on top of **Olve.Results**. Validators accumulate rules via method chaining and return `Result` when `Validate()` is called.

## Class Hierarchy

Abstract base classes provide shared validation logic:

- **`BaseValidator<TValue, TValidator>`** — core fluent infrastructure. Provides `FailIf()`, `Validate()`, `WithProblem()`, `WithMessage()`.
- **`BaseObjectValidator<TValue, TValidator>`** — for reference types. Adds `CannotBeNull()`, `MustBeOneOf()`, `CannotBeOneOf()`.
- **`BaseStructValidator<TValue, TValidator>`** — for value types. Adds `CannotBeDefault()`, `MustBeDefault()`.
- **`BaseNumericStructValidator<TValue, TValidator>`** — for `INumber<T>` types. Adds range methods (`MustBeGreaterThan`, `MustBeLessThan`, `MustBeBetween`, `MustBePositive`, `MustBeNegative`, `MustBeZero`).
- **`BaseEnumerableValidator<TValue, TEnumerable, TValidator>`** — for collections. Adds `CannotBeEmpty()`, `CannotContainDuplicates()`, `MustHaveCountGreaterThan()`.

## Concrete Validators

- **`StringValidator`** — extends `BaseObjectValidator<string>`. Adds `CannotBeNullOrEmpty()`, `CannotBeNullOrWhiteSpace()`, `MustHaveMinLength(int)`, `MustHaveMaxLength(int)`.
- **`IntValidator`** — extends `BaseNumericStructValidator<int>`. Adds `MustBeEven()`, `MustBeOdd()`.
- **`DecimalValidator<T>`** — extends `BaseNumericStructValidator<T>` for any `INumber<T>`. No additional methods.
- **`EnumerableValidator<T>`** — extends `BaseEnumerableValidator` for `IEnumerable<T>`.
- **`ListValidator<T>`** — extends `BaseEnumerableValidator` for `IList<T>` with optimized `Count` access.

## Interfaces

- **`IValidator<TValue>`** — single method: `Result Validate(TValue value)`.
- **`IValidatable`** — single method: `Result Validate()`. For objects that validate their own state.

## Fluent API Pattern

All validation methods return the concrete validator type (`TValidator`) for chaining. `WithProblem()` / `WithMessage()` override the last rule's error. `Validate()` is the terminal method that executes all rules and returns `Result`.

## Maintenance

- When adding new validators or changing behaviour, update this document and the README.
- `workflow-triggers.txt` lists paths that trigger NuGet publishing when changed.
