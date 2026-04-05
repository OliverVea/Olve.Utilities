# Concrete Validators

## StringValidator

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.Validators.StringValidator.html

Inherits: `BaseObjectValidator<string, StringValidator>`. Namespace: `Olve.Validation.Validators`.

Validates `string?` values. Inherits `CannotBeNull()`, `MustBeOneOf()`, `CannotBeOneOf()` from BaseObjectValidator.

Public members:

```csharp
public StringValidator CannotBeNullOrEmpty();
public StringValidator CannotBeNullOrWhiteSpace();
public StringValidator MustHaveMinLength(int minLength);
public StringValidator MustHaveMaxLength(int maxLength);
```

- `CannotBeNullOrEmpty` -- fails when string is null or empty.
- `CannotBeNullOrWhiteSpace` -- fails when string is null or whitespace.
- `MustHaveMinLength` -- fails when string length < minLength. Throws `ArgumentException` if minLength < 0.
- `MustHaveMaxLength` -- fails when string length > maxLength. Throws `ArgumentException` if maxLength < 0.

Also inherits from BaseValidator: `Validate()`, `WithProblem()`, `WithMessage()`.

---

## IntValidator

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.Validators.IntValidator.html

Inherits: `BaseNumericStructValidator<int, IntValidator>`. Namespace: `Olve.Validation.Validators`.

Validates `int` values. Inherits all numeric rules from BaseNumericStructValidator (`MustBeGreaterThan`, `MustBeLessThan`, `MustBeBetween`, `MustBePositive`, `MustBeNegative`, `MustBeZero`, etc.) and struct rules from BaseStructValidator (`CannotBeDefault`, `MustBeDefault`).

Public members:

```csharp
public IntValidator MustBeEven();
public IntValidator MustBeOdd();
```

- `MustBeEven` -- fails when value is not even.
- `MustBeOdd` -- fails when value is not odd.

---

## DecimalValidator\<T\>

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.Validators.DecimalValidator-1.html

Inherits: `BaseNumericStructValidator<TValue, DecimalValidator<TValue>>`. Namespace: `Olve.Validation.Validators`.

Constraints: `TValue : struct, INumber<TValue>`.

Generic numeric validator for any `INumber<T>` type (`double`, `decimal`, `float`, etc.). Has no additional members beyond those inherited from BaseNumericStructValidator and BaseStructValidator.

Usage:

```csharp
var result = new DecimalValidator<double>()
    .MustBeBetween(0.0, 1.0)
    .Validate(0.5);
```

---

## EnumerableValidator\<T\>

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.Validators.EnumerableValidator-1.html

Inherits: `BaseEnumerableValidator<T, IEnumerable<T>, EnumerableValidator<T>>`. Namespace: `Olve.Validation.Validators`.

Validates `IEnumerable<T>?` values. Has no additional members beyond those inherited from BaseEnumerableValidator and BaseObjectValidator.

Inherited public rules: `CannotBeNull()`, `CannotBeEmpty()`, `CannotContainDuplicates()`, `MustHaveCountGreaterThan()`, `MustBeOneOf()`, `CannotBeOneOf()`, `WithProblem()`, `WithMessage()`, `Validate()`.

---

## ListValidator\<T\>

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.Validators.ListValidator-1.html

Inherits: `BaseEnumerableValidator<T, IList<T>, ListValidator<T>>`. Namespace: `Olve.Validation.Validators`.

Validates `IList<T>?` values. Optimized version of EnumerableValidator that overrides `GetCount` to use `IList<T>.Count` instead of LINQ `Count()`.

Overridden members:

```csharp
protected override int GetCount(IList<T> enumerable);  // uses .Count property
```

Inherited public rules: same as EnumerableValidator.
