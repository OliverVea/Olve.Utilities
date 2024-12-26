namespace Olve.Utilities.Types.Results;

public record ResultProblem(string Message, object[]? Args = null, string[]? Tags = null, int Severity = 0);