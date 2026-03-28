namespace Olve.Results;

/// <summary>
///     Common interface implemented by <see cref="Result"/> and <see cref="Result{T}"/>
///     to enable non-reflective type checking and value extraction.
/// </summary>
public interface IResultType
{
    /// <summary>
    ///     Gets a value indicating whether the operation succeeded.
    /// </summary>
    bool Succeeded { get; }

    /// <summary>
    ///     Gets a value indicating whether the operation failed.
    /// </summary>
    bool Failed { get; }

    /// <summary>
    ///     Gets the collection of problems associated with the result, if any.
    /// </summary>
    ResultProblemCollection? Problems { get; }

    /// <summary>
    ///     Gets the result value as an object, or <see langword="null"/> if the result has no value concept
    ///     or the operation failed.
    /// </summary>
    object? BoxedValue { get; }

    /// <summary>
    ///     Gets a value indicating whether this result type carries a value (i.e. is a <see cref="Result{T}"/>).
    /// </summary>
    bool HasValue { get; }
}
