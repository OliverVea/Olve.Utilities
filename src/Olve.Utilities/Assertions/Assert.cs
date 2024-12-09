using System.Diagnostics;

namespace Olve.Utilities.Assertions;

/// <summary>
/// Provides a set of assertion methods:
/// <li><see cref="That"/> method throws an <see cref="AssertionError"/> if the assertion is false.</li>
/// </summary>
public static class Assert
{
    /// <summary>
    /// Throws an <see cref="AssertionError"/> if the assertion is false.
    /// </summary>
    /// <remarks>Is only executed in debug builds.</remarks>
    /// <param name="assertion">The assertion to check.</param>
    /// <param name="message">The message to include in the exception.</param>
    /// <exception cref="AssertionError">Thrown if the assertion is false.</exception>
    [Conditional("DEBUG")]
    public static void That(Func<bool> assertion, string message)
    {
#if DEBUG
        if (!assertion())
        {
            throw new AssertionError(message);
        }
#endif
    }
}