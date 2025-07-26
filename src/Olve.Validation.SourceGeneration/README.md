# Olve.Validation.SourceGeneration

`ValidatorForGenerator` wires property validators into partial validator classes.

Apply `[ValidatorFor(typeof(T))]` to a partial class and add static methods named
`Get<PropertyName>Validator` returning `IValidator<PropertyType>`. The generator
produces a class deriving from `ValidatorFor<T>` that maps each property to its
validator.

```csharp
[ValidatorFor(typeof(MyDto))]
public partial class MyDtoValidator
{
    private static IValidator<string> GetNameValidator() => new StringValidator();
    private static IValidator<int> GetAgeValidator() => new IntValidator();
}
```
