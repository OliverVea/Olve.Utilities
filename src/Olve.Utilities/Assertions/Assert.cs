using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Assertions;

/// <summary>
///     Provides a set of assertion methods.
/// </summary>
public static class Assert
{
    /// <summary>
    ///     Throws an <see cref="AssertionError" /> if the assertion is false.
    /// </summary>
    /// <remarks>Is only executed in debug builds.</remarks>
    /// <param name="assertion">The assertion to check.</param>
    /// <param name="message">The message to include in the exception.</param>
    /// <exception cref="AssertionError">Thrown if the assertion is false.</exception>
    [Conditional("DEBUG")]
    public static void That(Func<bool> assertion, string message)
    {
        if (!assertion())
        {
            throw new AssertionError(message);
        }
    }

    /// <summary>
    ///     Throws an <see cref="AssertionError" /> if the value is null.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <param name="message">The message to include in the exception.</param>
    /// <typeparam name="T">Type of the value.</typeparam>
    [Conditional("DEBUG")]
    public static void NotNull<T>([NotNull] T? value, string message = "Value cannot be null.")
        where T : class
    {
        if (value is null)
        {
            throw new AssertionError(message);
        }
    }

    /// <summary>
    ///     Throws an <see cref="AssertionError" /> if the collection is not empty.
    /// </summary>
    /// <typeparam name="T">Type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to check.</param>
    /// <param name="message">The message to include in the exception.</param>
    /// <exception cref="AssertionError">Thrown if the collection is not empty.</exception>
    [Conditional("DEBUG")]
    public static void IsEmpty<T>(
        IEnumerable<T> collection,
        string message = "Collection should be empty."
    )
    {
        if (collection.Any())
        {
            throw new AssertionError(message);
        }
    }

    /// <summary>
    ///     Throws an <see cref="AssertionError" /> if the collection is empty.
    /// </summary>
    /// <typeparam name="T">Type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to check.</param>
    /// <param name="message">The message to include in the exception.</param>
    /// <exception cref="AssertionError">Thrown if the collection is empty.</exception>
    [Conditional("DEBUG")]
    public static void IsNotEmpty<T>(
        IEnumerable<T> collection,
        string message = "Collection should not be empty."
    )
    {
        if (!collection.Any())
        {
            throw new AssertionError(message);
        }
    }
}
