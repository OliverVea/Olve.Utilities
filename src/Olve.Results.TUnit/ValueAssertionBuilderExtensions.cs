using TUnit.Assertions.Assertions;
using TUnit.Assertions.Conditions;
using TUnit.Assertions.Core;

namespace Olve.Results.TUnit;

/// <summary>
/// Extension methods that provide fluent TUnit assertions for <see cref="Result{T}"/> and <see cref="Result"/>.
/// </summary>
public static class ValueAssertionBuilderExtensions
{
    /// <summary>
    /// Asserts that the result represents success (<c>Succeeded == true</c>).
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="source">The assertion source for the <see cref="Result{T}"/> to test.</param>
    /// <returns>A property assertion result targeting the <c>Succeeded</c> property.</returns>
    public static PropertyAssertionResult<Result<T>> Succeeded<T>(this IAssertionSource<Result<T>> source) =>
        source.HasProperty(x => x.Succeeded).IsEqualTo(true);

    /// <summary>
    /// Asserts that the result represents success (<c>Succeeded == true</c>).
    /// </summary>
    /// <param name="source">The assertion source for the <see cref="Result"/> to test.</param>
    /// <returns>A property assertion result targeting the <c>Succeeded</c> property.</returns>
    public static PropertyAssertionResult<Result> Succeeded(this IAssertionSource<Result> source) =>
        source.HasProperty(x => x.Succeeded).IsEqualTo(true);

    /// <summary>
    /// Asserts that the result represents failure (<c>Failed == true</c>).
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="source">The assertion source for the <see cref="Result{T}"/> to test.</param>
    /// <returns>A property assertion result targeting the <c>Failed</c> property.</returns>
    public static PropertyAssertionResult<Result<T>> Failed<T>(this IAssertionSource<Result<T>> source) =>
        source.HasProperty(x => x.Failed).IsEqualTo(true);

    /// <summary>
    /// Asserts that the result represents failure (<c>Failed == true</c>).
    /// </summary>
    /// <param name="source">The assertion source for the <see cref="Result"/> to test.</param>
    /// <returns>A property assertion result targeting the <c>Failed</c> property.</returns>
    public static PropertyAssertionResult<Result> Failed(this IAssertionSource<Result> source) =>
        source.HasProperty(x => x.Failed).IsEqualTo(true);

    /// <summary>
    /// Asserts the result is successful and runs an assertion against the unwrapped value.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="source">The assertion source for the <see cref="Result{T}"/> to test.</param>
    /// <param name="assertion">
    /// A function that receives an <see cref="IAssertionSource{T}"/> for the unwrapped value and returns an assertion to apply.
    /// </param>
    /// <returns>
    /// A member assertion result for the original <see cref="Result{T}"/> enabling further chaining.
    /// </returns>
    /// <example>
    /// <code>
    /// await Assert.That(result).SucceededAndValue(v => v.IsEqualTo(expected));
    /// </code>
    /// </example>
    public static MemberAssertionResult<Result<T>> SucceededAndValue<T>(this IAssertionSource<Result<T>> source,
        Func<IAssertionSource<T>, object> assertion)
    {
        return source
            .Satisfies(x => x.Succeeded)
            .And.Member(x => x.Value!, assertion);
    }

    /// <summary>
    /// Asserts the result is a failure and runs an assertion against the problem collection.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="source">The assertion source for the <see cref="Result{T}"/> to test.</param>
    /// <param name="assertion">
    /// A function that receives an <see cref="IAssertionSource{ResultProblemCollection}"/> for the problems and returns an assertion to apply.
    /// </param>
    /// <returns>
    /// A member assertion result for the original <see cref="Result{T}"/> enabling further chaining.
    /// </returns>
    /// <example>
    /// <code>
    /// await Assert.That(result).FailedAndProblemCollection(p => p.IsNotEmpty());
    /// </code>
    /// </example>
    public static MemberAssertionResult<Result<T>> FailedAndProblemCollection<T>(this IAssertionSource<Result<T>> source,
        Func<IAssertionSource<ResultProblemCollection>, object> assertion)
    {
        return source
            .Satisfies(x => x.Failed)
            .And.Member(x => x.Problems!, assertion);
    }

    /// <summary>
    /// Asserts the non-generic result is a failure and runs an assertion against the problem collection.
    /// </summary>
    /// <param name="source">The assertion source for the <see cref="Result"/> to test.</param>
    /// <param name="assertion">
    /// A function that receives an <see cref="IAssertionSource{ResultProblemCollection}"/> for the problems and returns an assertion to apply.
    /// </param>
    /// <returns>
    /// A member assertion result for the original <see cref="Result"/> enabling further chaining.
    /// </returns>
    /// <example>
    /// <code>
    /// await Assert.That(result).FailedAndProblemCollection(p => p.IsNotEmpty());
    /// </code>
    /// </example>
    public static MemberAssertionResult<Result> FailedAndProblemCollection<TAssertion>(this IAssertionSource<Result> source,
        Func<IAssertionSource<ResultProblemCollection>, TAssertion> assertion)
    where TAssertion : Assertion<ResultProblemCollection>
    {
        return source
            .Satisfies(x => x.Failed)
            .And.Member(x => x.Problems!, assertion);
    }
}
