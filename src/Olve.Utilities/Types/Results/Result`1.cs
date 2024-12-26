namespace Olve.Utilities.Types.Results;

public readonly struct Result<T> : IResult<T>
{
    private Result(T? result, IReadOnlyCollection<ResultProblem>? problems)
    {
        Succeded = problems is null;
        Value = result;
        Problems = problems;
    }

    public bool Succeded { get; }
    public IReadOnlyCollection<ResultProblem>? Problems { get; }
    public T? Value { get; }
    
    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(IReadOnlyCollection<ResultProblem> problems) => new(default, problems);

    public bool TryGetProblems([NotNullWhen(true)] out IReadOnlyCollection<ResultProblem>? problems)
    {
        problems = Problems;
        return problems is not null;
    }

    public bool TryGetValue([NotNullWhen(true)] out T? value, [NotNullWhen(false)] out IReadOnlyCollection<ResultProblem>? problems)
    {
        value = Value;
        problems = Problems;
        return value is not null;
    }

    public bool TryGetProblems([NotNullWhen(true)] out IReadOnlyCollection<ResultProblem>? problems, [NotNullWhen(false)] out T? value)
    {
        problems = Problems;
        value = Value;
        return problems is not null;
    }
}