using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Types.Results;

public readonly struct Result : IResult
{
    private Result(IReadOnlyCollection<ResultProblem>? problems)
    {
        Succeded = problems is null;
        Problems = problems;
    }

    public bool Succeded { get; private init; }
    public IReadOnlyCollection<ResultProblem>? Problems { get; private init; }

    public static Result Success => new(null);
    public static Result Failure(IReadOnlyCollection<ResultProblem> problems) => new(problems);

    public bool TryGetProblems([NotNullWhen(true)] out IReadOnlyCollection<ResultProblem>? problems)
    {
        problems = Problems;
        return problems is not null;
    }
}
