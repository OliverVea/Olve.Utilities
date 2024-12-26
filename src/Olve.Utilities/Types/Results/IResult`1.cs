using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Types.Results;

public interface IResult<TResult> : IResult
{
    TResult? Value { get; }
    
    bool TryGetValue([NotNullWhen(true)] out TResult? value, [NotNullWhen(false)] out IReadOnlyCollection<ResultProblem>? problems);
    bool TryGetProblems([NotNullWhen(true)] out IReadOnlyCollection<ResultProblem>? problems, [NotNullWhen(false)] out TResult? value);
}