using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Assertions;

/// <summary>
///     Provides a set of assertion methods:
///     <li><see cref="That" /> method throws an <see cref="AssertionError" /> if the assertion is false.</li>
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
}