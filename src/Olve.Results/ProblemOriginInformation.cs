namespace Olve.Results;

/// <summary>
///     Represents the origin of a problem.
/// </summary>
/// <param name="FilePath">The path to the file where the problem originated.</param>
/// <param name="LineNumber">The line number in the file where the problem originated.</param>
public readonly record struct ProblemOriginInformation(string FilePath, int LineNumber);