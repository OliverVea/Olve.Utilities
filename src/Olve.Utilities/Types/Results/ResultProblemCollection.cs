using System.Collections;

namespace Olve.Utilities.Types.Results;

/// <summary>
/// Represents a collection of problems encountered during an operation.
/// </summary>
/// <param name="problems">The problems encountered during the operation.</param>
public class ResultProblemCollection(params IEnumerable<ResultProblem> problems) : IEnumerable<ResultProblem>
{
    /// <summary>
    /// Appends the specified problems to the collection.
    /// </summary>
    /// <param name="resultProblems">The problems to append.</param>
    /// <returns>A new collection with the specified problems appended.</returns>
    public ResultProblemCollection Append(params IEnumerable<ResultProblem> resultProblems)
    {
        return new(problems.Concat(resultProblems));
    }
    
    /// <summary>
    /// Prepends the specified problems to the collection.
    /// </summary>
    /// <param name="resultProblems">The problems to prepend.</param>
    /// <returns>A new collection with the specified problems prepended.</returns>
    public ResultProblemCollection Prepend(params IEnumerable<ResultProblem> resultProblems)
    {
        return new(resultProblems.Concat(problems));
    }

    /// <summary>
    /// Merges multiple problem collections together.
    /// </summary>
    /// <param name="problemCollections"></param>
    /// <returns></returns>
    public static ResultProblemCollection Merge(params IEnumerable<ResultProblemCollection> problemCollections)
    {
        var allProblems = problemCollections.SelectMany(x => x);
        return new ResultProblemCollection(allProblems);
    }

    /// <inheritdoc />
    public IEnumerator<ResultProblem> GetEnumerator()
    {
        return problems.AsEnumerable().GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}