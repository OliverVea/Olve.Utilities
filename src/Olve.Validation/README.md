# Olve.Results.Validation

Lightweight validation helpers returning `Result` instances.

Use `Validate` to create validators:

```csharp
var result = Validate.That(userName)
    .IsNotNullOrWhiteSpace()
    .MinLength(3);
```
