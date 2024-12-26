using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Types.Results;

public interface IResult
{
    bool Succeded { get; }

    IReadOnlyCollection<ResultProblem>? Problems { get; }
    bool TryGetProblems([NotNullWhen(true)] out IReadOnlyCollection<ResultProblem>? problems);
}