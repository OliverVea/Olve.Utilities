using Olve.Results;

namespace Olve.Validation.Validators.Base;

/// <summary>
/// Provides validation for enumerable collections.
/// </summary>
public abstract class BaseEnumerableValidator<TValue, TEnumerable, TValidator> : BaseObjectValidator<TEnumerable, TValidator>
    where TEnumerable : class, IEnumerable<TValue>
    where TValidator : BaseEnumerableValidator<TValue, TEnumerable, TValidator>
{
    /// <summary>
    /// Fails if the enumerable is empty.
    /// </summary>
    public TValidator CannotBeEmpty() => FailIf(
        enumerable => enumerable != null && !enumerable.Any(), 
        _ => new ResultProblem("Value cannot be empty."));

    /// <summary>
    /// Fails if the enumerable contains duplicate elements.
    /// </summary>
    public TValidator CannotContainDuplicates() => FailIf(
        enumerable => enumerable != null && HasDuplicates(enumerable),
        _ => new ResultProblem("Value cannot contain duplicates."));
    
    /// <summary>
    /// Fails if the enumerable has count less than or equal to the specified threshold.
    /// </summary>
    /// <param name="threshold">Minimum count required.</param>
    public TValidator MustHaveCountGreaterThan(int threshold) => FailIf(
        enumerable => GetCount(enumerable!) <= threshold,
        enumerable => new ResultProblem(
            "Enumerable must have more than {0} items but found {1}",
            threshold,
            GetCount(enumerable!)));

    /// <summary>
    /// Determines whether the enumerable contains duplicate elements.
    /// </summary>
    /// <param name="enumerable">Enumerable to check.</param>
    /// <returns>True if duplicates exist; otherwise, false.</returns>
    protected virtual bool HasDuplicates(TEnumerable enumerable)
    {
        var values = enumerable.ToList();
        var valuesCount = values.Count;
        var distinctCount = values.Distinct().Count();
        return valuesCount != distinctCount;
    }

    /// <summary>
    /// Gets the count of elements in the enumerable.
    /// </summary>
    /// <param name="enumerable">Enumerable to count.</param>
    /// <returns>The element count.</returns>
    protected virtual int GetCount(TEnumerable enumerable) => enumerable.Count();
}
