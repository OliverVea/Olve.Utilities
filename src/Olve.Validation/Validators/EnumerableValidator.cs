using Olve.Validation.Validators.Base;

namespace Olve.Validation.Validators;

/// <summary>
/// Provides validation for generic enumerable sequences.
/// </summary>
/// <typeparam name="T">Type of elements in the enumerable.</typeparam>
public class EnumerableValidator<T> : BaseEnumerableValidator<T, IEnumerable<T>, EnumerableValidator<T>>
{
    /// <inheritdoc />
    protected override EnumerableValidator<T> Validator => this;
}