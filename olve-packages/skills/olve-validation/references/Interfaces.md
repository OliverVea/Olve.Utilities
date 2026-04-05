# Interfaces

## IValidator\<T\>

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.IValidator-1.html

Interface for any validator that returns `Result`. Namespace: `Olve.Validation`.

```csharp
public interface IValidator<in TValue>
{
    Result Validate(TValue value);
}
```

All concrete validators implement this interface. It allows validators to be passed as dependencies and used polymorphically.

## IValidatable

https://olivervea.github.io/Olve.Utilities/api/Olve.Validation.IValidatable.html

Interface for objects that can validate their own state. Namespace: `Olve.Validation`.

```csharp
public interface IValidatable
{
    Result Validate();
}
```

Implement this on domain objects or DTOs that need self-validation. The parameterless `Validate()` inspects the object's own fields and returns a `Result` indicating success or containing all problems found.
