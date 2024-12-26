namespace Olve.Utilities.Types.Results;

/// <summary>
/// Represents a problem encountered during an operation.
/// </summary>
/// <param name="Message">The message describing the problem.</param>
/// <param name="Args">Optional arguments providing additional details about the problem.</param>
/// <param name="Tags">Optional tags categorizing the problem.</param>
/// <param name="Severity">The severity level of the problem, where higher values indicate more severe problems.</param>
public record ResultProblem(string Message, object[]? Args = null, string[]? Tags = null, int Severity = 0);