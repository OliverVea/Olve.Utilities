# Olve.Validation

Source: https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.html

Fluent validation helpers built on top of Olve.Results. Validators accumulate rules via a fluent API and return a `Result` containing all problems found.

## Key Types

- **IValidator\<T\>** -- Interface for any validator that returns `Result`.
- **IValidatable** -- Interface for objects that can validate their own state.
- **BaseValidator\<TValue, TValidator\>** -- Abstract base with rule accumulation, `Validate()`, `WithProblem()`, `WithMessage()`.
- **BaseStructValidator** -- Adds `CannotBeDefault()` / `MustBeDefault()` for struct types.
- **BaseObjectValidator** -- Adds `CannotBeNull()`, `MustBeOneOf()`, `CannotBeOneOf()` for reference types.
- **BaseNumericStructValidator** -- Numeric range rules (`MustBeGreaterThan`, `MustBeBetween`, `MustBePositive`, etc.).
- **BaseEnumerableValidator** -- Collection rules (`CannotBeEmpty`, `CannotContainDuplicates`, `MustHaveCountGreaterThan`).
- **StringValidator** -- String validation: null checks, length constraints.
- **IntValidator** -- Integer validation: numeric range, even/odd.
- **DecimalValidator\<T\>** -- Generic numeric validation for any `INumber<T>` type.
- **EnumerableValidator\<T\>** -- Collection validation for `IEnumerable<T>`.
- **ListValidator\<T\>** -- Optimized collection validation for `IList<T>`.

## Primary Usage Pattern

1. Instantiate a concrete validator (e.g., `new StringValidator()`).
2. Chain rules fluently (e.g., `.CannotBeNullOrWhiteSpace().MustHaveMinLength(3)`).
3. Optionally override error messages with `.WithMessage()` or `.WithProblem()` after each rule.
4. Call `.Validate(value)` to get a `Result`.
5. Check the result with `TryPickProblems` as you would with any `Result`.

## Design Philosophy

Validators are composable, stateless rule builders. Each rule is a predicate paired with a problem factory. Rules are evaluated independently -- all failing rules contribute problems to the result rather than short-circuiting. This makes validation results comprehensive.

The inheritance hierarchy (BaseValidator -> BaseStructValidator/BaseObjectValidator -> concrete) provides reusable rule sets at each level while keeping the fluent API strongly typed via CRTP (curiously recurring template pattern).

Validators return `Result` from Olve.Results, so they integrate directly with the rest of the error-handling ecosystem (Chain, Concat, Prepend, etc.).
