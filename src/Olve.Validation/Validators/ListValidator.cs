using Olve.Validation.Validators.Base;

namespace Olve.Validation.Validators;

/// <summary>
/// Provides validation for list collections.
/// </summary>
/// <typeparam name="T">Type of elements in the list.</typeparam>
public class ListValidator<T> : BaseEnumerableValidator<T, IList<T>, ListValidator<T>>
{
    /// <inheritdoc />
    protected override ListValidator<T> Validator => this;

    /// <inheritdoc />
    protected override int GetCount(IList<T> enumerable) => enumerable.Count;
}