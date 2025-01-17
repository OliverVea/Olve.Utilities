namespace Olve.Utilities.Assertions;

/// <summary>
///     Represents an assertion error from <see cref="Assert" />.
/// </summary>
/// <param name="message">The message of the error.</param>
public class AssertionError(string message) : Exception(message);