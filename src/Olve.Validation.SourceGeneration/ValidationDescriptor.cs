using Olve.Results;

namespace Olve.Validation.SourceGeneration;

/// <summary>
/// Describes validation rules for a type.
/// </summary>
/// <typeparam name="T">Type being validated.</typeparam>
public sealed class ValidationDescriptor<T>
{
    private readonly List<Func<T, Result>> _validators = [];

    /// <summary>
    /// Adds a validator for the specified property.
    /// </summary>
    /// <typeparam name="TProperty">Property type.</typeparam>
    /// <param name="getter">Accessor for the property.</param>
    /// <param name="validator">Validator to apply.</param>
    /// <param name="name">Property name.</param>
    public void For<TProperty>(Func<T, TProperty> getter, IValidator<TProperty> validator, string name)
    {
        _validators.Add(instance =>
        {
            var result = validator.Validate(getter(instance));
            if (result.TryPickProblems(out var problems))
            {
                return problems.Prepend(name);
            }
            return Result.Success();
        });
    }

    /// <summary>
    /// Executes all validators against the instance.
    /// </summary>
    /// <param name="instance">Instance to validate.</param>
    /// <returns>Validation result.</returns>
    public Result Validate(T instance)
    {
        var results = _validators.Select(v => v(instance));
        return results.TryPickProblems(out var problems) ? problems : Result.Success();
    }
}
