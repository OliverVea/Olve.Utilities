namespace Olve.Results;

/// <summary>
///     Extension methods for <see cref="Result"/> and <see cref="Result{T}"/>.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    ///     Discards the value of a <see cref="Result{T}"/>, keeping only the success/failure status.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <typeparam name="T">The type of the discarded value.</typeparam>
    /// <returns>A <see cref="Result"/> with the same success/failure status.</returns>
    public static Result ToEmptyResult<T>(this Result<T> result) =>
        result.TryPickProblems(out var problems)
            ? problems
            : Result.Success();

    /// <summary>
    ///     Maps the value of a successful <see cref="Result{T}"/> using the specified mapping function.
    /// </summary>
    /// <param name="result">The result to map.</param>
    /// <param name="map">The function to apply to the value.</param>
    /// <typeparam name="TSource">The source value type.</typeparam>
    /// <typeparam name="TDestination">The destination value type.</typeparam>
    /// <returns>A new result with the mapped value, or the original problems on failure.</returns>
    public static Result<TDestination> Map<TSource, TDestination>(
        this Result<TSource> result,
        Func<TSource, TDestination> map)
    {
        if (result.TryPickProblems(out var problems, out var value))
        {
            return problems;
        }

        return map(value);
    }

    /// <summary>
    ///     Maps the value of a successful <see cref="Result{T}"/> using a function that itself returns a result.
    /// </summary>
    /// <param name="result">The result to bind.</param>
    /// <param name="bind">The function to apply to the value, returning a new result.</param>
    /// <typeparam name="TSource">The source value type.</typeparam>
    /// <typeparam name="TDestination">The destination value type.</typeparam>
    /// <returns>The result of the bind function, or the original problems on failure.</returns>
    public static Result<TDestination> Bind<TSource, TDestination>(
        this Result<TSource> result,
        Func<TSource, Result<TDestination>> bind)
    {
        if (result.TryPickProblems(out var problems, out var value))
        {
            return problems;
        }

        return bind(value);
    }

    /// <summary>
    ///     Attaches a value to a successful <see cref="Result"/>, producing a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="result">The result to attach a value to.</param>
    /// <param name="value">The value to attach on success.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>A <see cref="Result{T}"/> with the value on success, or the original problems on failure.</returns>
    public static Result<T> WithValueOnSuccess<T>(this Result result, T value) =>
        result.TryPickProblems(out var problems)
            ? problems
            : value;
}
