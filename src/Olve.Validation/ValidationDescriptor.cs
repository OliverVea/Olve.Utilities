using Olve.Results;

namespace Olve.Validation;

/// <summary>
/// Collects property validators for a type and executes them.
/// </summary>
/// <typeparam name="T">Object type being validated.</typeparam>
public class ValidationDescriptor<T>
{
    private readonly List<Func<T, Result>> _rules = [];

    /// <summary>
    /// Registers a property validator.
    /// </summary>
    /// <typeparam name="TProp">Property type.</typeparam>
    /// <param name="getter">Accessor for the property.</param>
    /// <param name="validator">Validator for the property value.</param>
    /// <param name="propertyName">Name used for problem source.</param>
    public void For<TProp>(Func<T, TProp> getter, IValidator<TProp> validator, string propertyName)
    {
        _rules.Add(target =>
        {
            var prev = ResultProblem.DefaultSource;
            ResultProblem.DefaultSource = propertyName;
            try
            {
                return validator.Validate(getter(target));
            }
            finally
            {
                ResultProblem.DefaultSource = prev;
            }
        });
    }

    /// <summary>
    /// Validates the specified instance.
    /// </summary>
    /// <param name="value">Instance to validate.</param>
    /// <returns>A <see cref="Result"/> with aggregated problems.</returns>
    public Result Validate(T value)
    {
        var results = _rules.Select(r => r(value));
        return results.TryPickProblems(out var problems) ? problems! : Result.Success();
    }
}
