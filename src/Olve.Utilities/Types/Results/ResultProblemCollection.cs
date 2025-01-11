using System.Collections;

namespace Olve.Utilities.Types.Results;

/// <summary>
/// Represents a collection of problems encountered during an operation.
/// </summary>
/// <param name="resultProblems">The problems encountered during the operation.</param>
public class ResultProblemCollection(params IEnumerable<ResultProblem> resultProblems) : IReadOnlyList<ResultProblem>
{
    private readonly ResultProblem[] _problems = resultProblems.ToArray();
    
    /// <summary>
    /// Appends the specified problems to the collection.
    /// </summary>
    /// <param name="resultProblems">The problems to append.</param>
    /// <returns>A new collection with the specified problems appended.</returns>
    public ResultProblemCollection Append(params IEnumerable<ResultProblem> resultProblems)
    {
        return new(_problems.Concat(resultProblems));
    }
    
    /// <summary>
    /// Prepends the specified problems to the collection.
    /// </summary>
    /// <param name="resultProblems">The problems to prepend.</param>
    /// <returns>A new collection with the specified problems prepended.</returns>
    public ResultProblemCollection Prepend(params IEnumerable<ResultProblem> resultProblems)
    {
        return new(resultProblems.Concat(_problems));
    }

    /// <inheritdoc />
    public IEnumerator<ResultProblem> GetEnumerator()
    {
        return _problems.AsEnumerable().GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public int Count => _problems.Length;
    
    /// <inheritdoc />
    public ResultProblem this[int index] => _problems[index];
}