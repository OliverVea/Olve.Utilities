# Base Validators

## BaseValidator\<TValue, TValidator\>

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.Validators.Base.BaseValidator-2.html

Abstract base class for all validators. Provides rule accumulation, validation execution, and error message overrides. Implements `IValidator<TValue>`. Namespace: `Olve.Validation.Validators.Base`.

Constraints: `TValidator : BaseValidator<TValue, TValidator>` (CRTP for fluent chaining).

Public members:

```csharp
public Result Validate(TValue value);
public TValidator WithProblem(Func<TValue, ResultProblem> problemFactory);
public TValidator WithProblem(ResultProblem problem);
public TValidator WithMessage(string message);
```

Protected members:

```csharp
protected abstract TValidator Validator { get; }
protected TValidator FailIf(Func<TValue, bool> condition, Func<TValue, ResultProblem> problemFactory);
```

- `Validate` runs all rules and aggregates problems. Returns `Result.Success()` if no rules fail.
- `WithProblem` / `WithMessage` override the error of the most recently added rule. Throws `InvalidOperationException` if no rules exist or if the last rule's problem was already overwritten.
- `FailIf` adds a rule that produces a problem when the condition returns true.

---

## BaseStructValidator\<TValue, TValidator\>

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.Validators.Base.BaseStructValidator-2.html

Inherits: `BaseValidator<TValue, TValidator>`. Namespace: `Olve.Validation.Validators.Base`.

Constraints: `TValue : struct`, `TValidator : BaseStructValidator<TValue, TValidator>`.

Protected members:

```csharp
protected TValidator CannotBeDefault();
protected TValidator MustBeDefault();
```

- `CannotBeDefault` -- fails if value equals `default(TValue)`.
- `MustBeDefault` -- fails if value does not equal `default(TValue)`.

---

## BaseObjectValidator\<TValue, TValidator\>

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.Validators.Base.BaseObjectValidator-2.html

Inherits: `BaseValidator<TValue?, TValidator>`. Namespace: `Olve.Validation.Validators.Base`.

Constraints: `TValue : class`, `TValidator : BaseObjectValidator<TValue, TValidator>`.

Note: validates `TValue?` (nullable reference type).

Public members:

```csharp
public TValidator CannotBeNull();
public TValidator MustBeOneOf(params ICollection<TValue?> values);
public TValidator CannotBeOneOf(params ICollection<TValue?> values);
```

- `CannotBeNull` -- fails if value is null.
- `MustBeOneOf` -- fails if value is not in the allowed collection.
- `CannotBeOneOf` -- fails if value is in the disallowed collection.

---

## BaseNumericStructValidator\<TValue, TValidator\>

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.Validators.Base.BaseNumericStructValidator-2.html

Inherits: `BaseStructValidator<TValue, TValidator>`. Namespace: `Olve.Validation.Validators.Base`.

Constraints: `TValue : struct, INumber<TValue>`, `TValidator : BaseNumericStructValidator<TValue, TValidator>`.

Public members:

```csharp
public TValidator MustBeGreaterThan(TValue limit);
public TValidator MustBeLessThan(TValue limit);
public TValidator MustBeGreaterThanOrEqualTo(TValue limit);
public TValidator MustBeLessThanOrEqualTo(TValue limit);
public TValidator MustBeBetween(TValue min, TValue max);
public TValidator MustBePositive();
public TValidator MustBeNegative();
public TValidator MustBeZero();
```

- `MustBeGreaterThan` -- fails when value <= limit.
- `MustBeLessThan` -- fails when value >= limit.
- `MustBeGreaterThanOrEqualTo` -- fails when value < limit.
- `MustBeLessThanOrEqualTo` -- fails when value > limit.
- `MustBeBetween` -- fails when value < min or value > max (inclusive range).
- `MustBePositive` -- fails when value <= 0.
- `MustBeNegative` -- fails when value >= 0.
- `MustBeZero` -- fails when value != 0.

---

## BaseEnumerableValidator\<TValue, TEnumerable, TValidator\>

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.Validators.Base.BaseEnumerableValidator-3.html

Inherits: `BaseObjectValidator<TEnumerable, TValidator>`. Namespace: `Olve.Validation.Validators.Base`.

Constraints: `TEnumerable : class, IEnumerable<TValue>`, `TValidator : BaseEnumerableValidator<TValue, TEnumerable, TValidator>`.

Inherits from BaseObjectValidator, so `CannotBeNull()`, `MustBeOneOf()`, `CannotBeOneOf()` are available.

Public members:

```csharp
public TValidator CannotBeEmpty();
public TValidator CannotContainDuplicates();
public TValidator MustHaveCountGreaterThan(int threshold);
```

Protected virtual members:

```csharp
protected virtual bool HasDuplicates(TEnumerable enumerable);
protected virtual int GetCount(TEnumerable enumerable);
```

- `CannotBeEmpty` -- fails if enumerable is non-null but has no elements.
- `CannotContainDuplicates` -- fails if enumerable contains duplicate elements.
- `MustHaveCountGreaterThan` -- fails if element count is <= threshold.
- `HasDuplicates` -- overridable duplicate detection (default uses `Distinct().Count()`).
- `GetCount` -- overridable count (default uses LINQ `Count()`). `ListValidator<T>` overrides this with `IList<T>.Count`.
