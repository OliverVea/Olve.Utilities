using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.CollectionExtensions;

/// <summary>
///     Provides extension methods for enumerables.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Performs an action on each item in the enumerable.
    /// </summary>
    /// <param name="enumerable">The enumerable to perform the action on.</param>
    /// <param name="action">The action to perform on each item.</param>
    /// <typeparam name="TIn">The type of the items in the enumerable.</typeparam>
    /// <typeparam name="TOut">The type of the items in the resulting enumerable.</typeparam>
    /// <returns>An enumerable of the results of the action.</returns>
    /// <remarks>
    ///     Please note that this method is lazy and will not execute the action on each item until the enumerable is
    ///     enumerated.
    /// </remarks>
    public static IEnumerable<TOut> ForEach<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> action) =>
        enumerable.Select(action);

    /// <summary>
    ///     Performs an action on each item in the enumerable.
    /// </summary>
    /// <param name="enumerable">The enumerable to perform the action on.</param>
    /// <param name="action">The action to perform on each item.</param>
    /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
    /// <remarks>Please note that this method is not lazy and will execute the action on each item immediately.</remarks>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
        }
    }

    /// <summary>
    ///     Returns the product of all the items in the enumerable.
    /// </summary>
    /// <param name="enumerable">The enumerable to calculate the product of.</param>
    /// <returns>The product of all the items in the enumerable.</returns>
    public static float Product(this IEnumerable<float> enumerable)
    {
        return enumerable.Aggregate(1f, (current, value) => current * value);
    }

    /// <summary>
    /// Attempts to cast an <see cref="IEnumerable{T}"/> to an <see cref="IReadOnlySet{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="enumerable">The enumerable to attempt to cast.</param>
    /// <param name="readOnlySet">
    /// When this method returns, contains the resulting <see cref="IReadOnlySet{T}"/>
    /// if the cast was successful, or <c>null</c> otherwise.
    /// </param>
    /// <returns><c>true</c> if the cast was successful; otherwise, <c>false</c>.</returns>
    public static bool TryAsReadOnlySet<T>(this IEnumerable<T> enumerable, [NotNullWhen(true)] out IReadOnlySet<T>? readOnlySet)
    {
        switch (enumerable)
        {
            case IReadOnlySet<T> asReadOnlySet:
                readOnlySet = asReadOnlySet;
                return true;
            case ISet<T> asSet:
                readOnlySet = (IReadOnlySet<T>)asSet;
                return true;
            default:
                readOnlySet = null;
                return false;
        }
    }

    /// <summary>
    /// Attempts to cast an <see cref="IEnumerable{T}"/> to an <see cref="ISet{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="enumerable">The enumerable to attempt to cast.</param>
    /// <param name="set">
    /// When this method returns, contains the resulting <see cref="ISet{T}"/>
    /// if the cast was successful, or <c>null</c> otherwise.
    /// </param>
    /// <returns><c>true</c> if the cast was successful; otherwise, <c>false</c>.</returns>
    public static bool TryAsSet<T>(this IEnumerable<T> enumerable, [NotNullWhen(true)] out ISet<T>? set)
    {
        switch (enumerable)
        {
            case ISet<T> asSet:
                set = asSet;
                return true;
            default:
                set = null;
                return false;
        }
    }
}