namespace Olve.Utilities.Extensions;

/// <summary>
///     Provides extension methods for <see cref="OneOf{T0, T1}" /> and <see cref="OneOf{T0, T1, T2}" />.
/// </summary>
public static class OneOfTryGetExtensions
{
    /// <summary>
    ///     Gets the value of the <see cref="OneOf{T0, T1}" /> if it is of type <typeparamref name="T0" />, otherwise returns
    ///     the default value.
    /// </summary>
    /// <param name="oneOf">The <see cref="OneOf{T0, T1}" /> to get the value from.</param>
    /// <param name="defaultValue">
    ///     The default value to return if the <see cref="OneOf{T0, T1}" /> is not of type
    ///     <typeparamref name="T0" />.
    /// </param>
    /// <typeparam name="T0">The type of the first value in the <see cref="OneOf{T0, T1}" />.</typeparam>
    /// <typeparam name="T1">The type of the second value in the <see cref="OneOf{T0, T1}" />.</typeparam>
    /// <returns></returns>
    public static T0 GetT0OrDefault<T0, T1>(this OneOf<T0, T1> oneOf, T0 defaultValue)
    {
        if (oneOf.IsT0)
        {
            return oneOf.AsT0;
        }

        return defaultValue;
    }

    /// <summary>
    ///     Gets the value of the <see cref="OneOf{T0, T1}" /> if it is of type <typeparamref name="T1" />, otherwise returns
    ///     the default value.
    /// </summary>
    /// <param name="oneOf">The <see cref="OneOf{T0, T1}" /> to get the value from.</param>
    /// <param name="defaultValue">
    ///     The default value to return if the <see cref="OneOf{T0, T1}" /> is not of type
    ///     <typeparamref name="T1" />.
    /// </param>
    /// <typeparam name="T0">The type of the first value in the <see cref="OneOf{T0, T1}" />.</typeparam>
    /// <typeparam name="T1">The type of the second value in the <see cref="OneOf{T0, T1}" />.</typeparam>
    /// <returns></returns>
    public static T1 GetT1OrDefault<T0, T1>(this OneOf<T0, T1> oneOf, T1 defaultValue)
    {
        return oneOf.Match(
            _ => defaultValue,
            t1 => t1
        );
    }

    /// <summary>
    ///     Gets the value of the <see cref="OneOf{T0, T1, T2}" /> if it is of type <typeparamref name="T0" />, otherwise
    ///     returns the default value.
    /// </summary>
    /// <param name="oneOf">The <see cref="OneOf{T0, T1, T2}" /> to get the value from.</param>
    /// <param name="defaultValue">
    ///     The default value to return if the <see cref="OneOf{T0, T1, T2}" /> is not of type
    ///     <typeparamref name="T0" />.
    /// </param>
    /// <typeparam name="T0">The type of the first value in the <see cref="OneOf{T0, T1, T2}" />.</typeparam>
    /// <typeparam name="T1">The type of the second value in the <see cref="OneOf{T0, T1, T2}" />.</typeparam>
    /// <typeparam name="T2">The type of the third value in the <see cref="OneOf{T0, T1, T2}" />.</typeparam>
    /// <returns></returns>
    public static T0 GetT0OrDefault<T0, T1, T2>(this OneOf<T0, T1, T2> oneOf, T0 defaultValue)
    {
        return oneOf.Match(
            t0 => t0,
            _ => defaultValue,
            _ => defaultValue
        );
    }
}