using TUnit.Assertions.AssertConditions;
using TUnit.Assertions.AssertConditions.Operators;
using TUnit.Assertions.AssertionBuilders;

namespace Olve.Results.TUnit;

/// <summary>
/// Provides fluent assertion extensions for verifying <see cref="Result{T}"/> values in TUnit tests.
/// </summary>
/// <remarks>
/// These extensions allow asserting whether a <see cref="Result{T}"/> represents success or failure,
/// and provide convenient access to the unwrapped value or problems for further assertions.
/// </remarks>
public static class ValueAssertionBuilderExtensions
{
    /// <summary>
    /// Asserts that the result was successful (<c>Succeeded == true</c>).
    /// </summary>
    /// <typeparam name="T">The result's value type.</typeparam>
    /// <param name="builder">The assertion builder for the result.</param>
    /// <returns>A fluent assertion chain for further conditions.</returns>
    /// <example>
    /// <code>
    /// await Assert.That(myResult).Succeeded();
    /// </code>
    /// </example>
    public static InvokableValueAssertionBuilder<Result<T>> Succeeded<T>(this ValueAssertionBuilder<Result<T>> builder) =>
        builder.HasMember(x => x.Succeeded).EqualTo(true);
    
    /// <summary>
    /// Asserts that the result was successful (<c>Succeeded == true</c>).
    /// </summary>
    /// <param name="builder">The assertion builder for the result.</param>
    /// <returns>A fluent assertion chain for further conditions.</returns>
    /// <example>
    /// <code>
    /// await Assert.That(myResult).Succeeded();
    /// </code>
    /// </example>
    public static InvokableValueAssertionBuilder<Result> Succeeded(this ValueAssertionBuilder<Result> builder) =>
        builder.HasMember(x => x.Succeeded).EqualTo(true);

    /// <summary>
    /// Asserts that the result failed (<c>Succeeded == false</c>).
    /// </summary>
    /// <typeparam name="T">The result's value type.</typeparam>
    /// <param name="builder">The assertion builder for the result.</param>
    /// <returns>A fluent assertion chain for further conditions.</returns>
    /// <example>
    /// <code>
    /// await Assert.That(myResult).Failed();
    /// </code>
    /// </example>
    public static InvokableValueAssertionBuilder<Result<T>> Failed<T>(this ValueAssertionBuilder<Result<T>> builder) =>
        builder.HasMember(x => x.Failed).EqualTo(true);

    /// <summary>
    /// Asserts that the result failed (<c>Succeeded == false</c>).
    /// </summary>
    /// <param name="builder">The assertion builder for the result.</param>
    /// <returns>A fluent assertion chain for further conditions.</returns>
    /// <example>
    /// <code>
    /// await Assert.That(myResult).Failed();
    /// </code>
    /// </example>
    public static InvokableValueAssertionBuilder<Result> Failed(this ValueAssertionBuilder<Result> builder) =>
        builder.HasMember(x => x.Failed).EqualTo(true);

    /// <summary>
    /// Asserts that the result is successful, and unwraps its value for further assertions.
    /// </summary>
    /// <typeparam name="T">The result's value type.</typeparam>
    /// <param name="builder">The assertion builder for the result.</param>
    /// <returns>A value assertion builder for the unwrapped value.</returns>
    /// <example>
    /// <code>
    /// await Assert.That(myResult).SucceededAndValue().IsEqualTo(expectedValue);
    /// </code>
    /// </example>
    public static ValueAnd<T> SucceededAndValue<T>(this ValueAssertionBuilder<Result<T>> builder)
    {
        return builder.RegisterConversionAssertion(new ResultSucceededCondition<T>(), []).And;
    }

    /// <summary>
    /// Asserts that the result failed and unwraps its <see cref="ResultProblemCollection"/> for further assertions.
    /// </summary>
    /// <typeparam name="T">The result's value type.</typeparam>
    /// <param name="builder">The assertion builder for the result.</param>
    /// <returns>A value assertion builder for the problem collection.</returns>
    /// <example>
    /// <code>
    /// await Assert.That(myResult).FailedAndProblemCollection().IsNotEmpty();
    /// </code>
    /// </example>
    public static ValueAnd<ResultProblemCollection> FailedAndProblemCollection<T>(this ValueAssertionBuilder<Result<T>> builder)
    {
        return builder.RegisterConversionAssertion(new ResultFailedCondition<T>(), []).And;
    }

    /// <summary>
    /// Asserts that the result failed and unwraps its <see cref="ResultProblemCollection"/> for further assertions.
    /// </summary>
    /// <param name="builder">The assertion builder for the result.</param>
    /// <returns>A value assertion builder for the problem collection.</returns>
    /// <example>
    /// <code>
    /// await Assert.That(myResult).FailedAndProblemCollection().IsNotEmpty();
    /// </code>
    /// </example>
    public static ValueAnd<ResultProblemCollection> FailedAndProblemCollection(this ValueAssertionBuilder<Result> builder)
    {
        return builder.RegisterConversionAssertion(new ResultFailedCondition(), []).And;
    }

    /// <summary>
    /// Converts a successful result into its value for assertion purposes.
    /// </summary>
    /// <typeparam name="T">The result's value type.</typeparam>
    private class ResultSucceededCondition<T> : ConvertToAssertCondition<Result<T>, T>
    {
        /// <inheritdoc />
        protected override string GetExpectation() => "to be a success";

        /// <inheritdoc />
        public override ValueTask<(AssertionResult, T?)> ConvertValue(Result<T> result)
        {
            return ValueTask.FromResult(ConvertValueInternal(result));
        }

        private static (AssertionResult, T?) ConvertValueInternal(Result<T> result)
        {
            var failed = result.TryPickProblems(out var problems, out var value);
            var problemString = string.Join(", ", problems?.Select(x => $"'{x}'") ?? []);
            var errorString = $"got problem(s): {problemString}";

            return (AssertionResult.FailIf(failed, errorString), value);
        }
    }

    /// <summary>
    /// Converts a failed result into its problem collection for assertion purposes.
    /// </summary>
    private class ResultFailedCondition : ConvertToAssertCondition<Result, ResultProblemCollection>
    {
        /// <inheritdoc />
        protected override string GetExpectation() => "to be a failure";

        /// <inheritdoc />
        public override ValueTask<(AssertionResult, ResultProblemCollection?)> ConvertValue(Result result)
        {
            return ValueTask.FromResult(ConvertValueInternal(result));
        }

        private static (AssertionResult, ResultProblemCollection?) ConvertValueInternal(Result result)
        {
            var failed = result.TryPickProblems(out var problems);
            var succeeded = !failed;

            return (AssertionResult.FailIf(succeeded, "got a success"), problems);
        }
    }

    /// <summary>
    /// Converts a failed result into its problem collection for assertion purposes.
    /// </summary>
    /// <typeparam name="T">The result's value type.</typeparam>
    private class ResultFailedCondition<T> : ConvertToAssertCondition<Result<T>, ResultProblemCollection>
    {
        /// <inheritdoc />
        protected override string GetExpectation() => "to be a failure";

        /// <inheritdoc />
        public override ValueTask<(AssertionResult, ResultProblemCollection?)> ConvertValue(Result<T> result)
        {
            return ValueTask.FromResult(ConvertValueInternal(result));
        }

        private static (AssertionResult, ResultProblemCollection?) ConvertValueInternal(Result<T> result)
        {
            var failed = result.TryPickProblems(out var problems);
            var succeeded = !failed;

            return (AssertionResult.FailIf(succeeded, "got a success"), problems);
        }
    }
}
