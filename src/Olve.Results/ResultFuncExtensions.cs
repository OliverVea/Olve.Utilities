namespace Olve.Results;

/// <summary>
///     Extensions for <see cref="Func{T, Result}" />.
/// </summary>
public static class ResultFuncExtensions
{
    /// <summary>
    ///     Converts an <see cref="Action{T}" /> to a <see cref="Func{T, Result}" />.
    /// </summary>
    /// <param name="action">The <see cref="Action{T}" /> to convert.</param>
    /// <typeparam name="T">The type of the parameter for the action.</typeparam>
    /// <returns>A <see cref="Func{T, Result}" /> that invokes the action and returns a successful result.</returns>
    public static Func<T, Result> ToResultFunc<T>(this Action<T> action) => t =>
    {
        action(t);
        return Result.Success();
    };
}