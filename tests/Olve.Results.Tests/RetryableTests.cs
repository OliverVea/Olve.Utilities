using System.Text.Json;

namespace Olve.Results.Tests;

// A generated result whose case is named "Retryable": its Is{Case} predicate already provides IsRetryable,
// so the generator must not emit a second IsRetryable member. This compiling is the test.
[GenerateResult]
public readonly partial struct RetryCaseResult
{
    [SuccessCase] public static partial RetryCaseResult Done();
    [GreyCase] public static partial RetryCaseResult Retryable();
    [ErrorCase] public static partial RetryCaseResult Errored(ResultProblemCollection problems);
}

public class RetryableTests
{
    private static ResultProblem Retryable(string message) => new(message) { IsRetryable = true };
    private static ResultProblem Terminal(string message) => new(message);

    private static void Throw() => throw new InvalidOperationException("boom");
    private static int ThrowInt() => throw new InvalidOperationException("boom");

    // ---- ResultProblem defaults ----

    [Test]
    public async Task MessageProblem_IsNotRetryableByDefault()
    {
        var problem = new ResultProblem("oops {0}", 1);

        await Assert.That(problem.IsRetryable).IsFalse();
    }

    [Test]
    public async Task ExceptionProblem_IsRetryableByDefault()
    {
        var problem = new ResultProblem(new TimeoutException(), "timed out");

        await Assert.That(problem.IsRetryable).IsTrue();
    }

    [Test]
    public async Task IsRetryable_CanBeOverriddenWithInit()
    {
        var message = new ResultProblem("oops") { IsRetryable = true };
        var exception = new ResultProblem(new TimeoutException(), "timed out") { IsRetryable = false };

        await Assert.That(message.IsRetryable).IsTrue();
        await Assert.That(exception.IsRetryable).IsFalse();
    }

    // ---- Aggregation ----

    [Test]
    public async Task Collection_AllRetryable_IsRetryable()
    {
        var collection = new ResultProblemCollection(Retryable("a"), Retryable("b"));

        await Assert.That(collection.IsRetryable).IsTrue();
    }

    [Test]
    public async Task Collection_OneTerminal_IsNotRetryable()
    {
        var collection = new ResultProblemCollection(Retryable("a"), Terminal("b"));

        await Assert.That(collection.IsRetryable).IsFalse();
    }

    [Test]
    public async Task Collection_Empty_IsNotRetryable()
    {
        var collection = new ResultProblemCollection();

        await Assert.That(collection.IsRetryable).IsFalse();
    }

    [Test]
    public async Task Result_Success_IsNotRetryable()
    {
        await Assert.That(Result.Success().IsRetryable).IsFalse();
        await Assert.That(Result.Success(1).IsRetryable).IsFalse();
    }

    [Test]
    public async Task Result_Failure_AggregatesProblems()
    {
        await Assert.That(Result.Failure(Retryable("a"), Retryable("b")).IsRetryable).IsTrue();
        await Assert.That(Result.Failure(Retryable("a"), Terminal("b")).IsRetryable).IsFalse();
        await Assert.That(Result.Failure().IsRetryable).IsFalse();
    }

    [Test]
    public async Task ResultOfT_Failure_AggregatesProblems()
    {
        await Assert.That(Result.Failure<int>(Retryable("a"), Retryable("b")).IsRetryable).IsTrue();
        await Assert.That(Result.Failure<int>(Retryable("a"), Terminal("b")).IsRetryable).IsFalse();
        await Assert.That(Result.Failure<int>().IsRetryable).IsFalse();
    }

    [Test]
    public async Task GeneratedResult_AggregatesProblems()
    {
        await Assert.That(LoadResult.Errored(new ResultProblemCollection(Retryable("a"))).IsRetryable).IsTrue();
        await Assert.That(LoadResult.Errored(new ResultProblemCollection(Retryable("a"), Terminal("b"))).IsRetryable).IsFalse();
        await Assert.That(ParseResult.Invalid(Retryable("a")).IsRetryable).IsTrue();
        await Assert.That(LoadResult.Loaded("x").IsRetryable).IsFalse();
        await Assert.That(LoadResult.Missing("p").IsRetryable).IsFalse();
        await Assert.That(DeletionResult.Error(Retryable("a")).IsRetryable).IsTrue();
        await Assert.That(DeletionResult.NotFound().IsRetryable).IsFalse();
    }

    [Test]
    public async Task GeneratedResult_CaseNamedRetryable_KeepsCasePredicate()
    {
        await Assert.That(RetryCaseResult.Retryable().IsRetryable).IsTrue();
        await Assert.That(RetryCaseResult.Done().IsRetryable).IsFalse();
        await Assert.That(RetryCaseResult.Errored(new ResultProblemCollection(Retryable("a"))).IsRetryable).IsFalse();
    }

    // ---- Prepend (message) ----

    [Test]
    public async Task Prepend_NoFlag_AllRetryable_Inherits()
    {
        var collection = new ResultProblemCollection(Retryable("a"), Retryable("b"));

        var prepended = collection.Prepend("context");

        await Assert.That(prepended.First().IsRetryable).IsTrue();
        await Assert.That(prepended.IsRetryable).IsTrue();
    }

    [Test]
    public async Task Prepend_NoFlag_OneTerminal_IsNotRetryable()
    {
        var collection = new ResultProblemCollection(Retryable("a"), Terminal("b"));

        var prepended = collection.Prepend("context");

        await Assert.That(prepended.First().IsRetryable).IsFalse();
    }

    [Test]
    public async Task Prepend_NoFlag_Empty_IsNotRetryable()
    {
        var prepended = new ResultProblemCollection().Prepend("context");

        await Assert.That(prepended.First().IsRetryable).IsFalse();
    }

    [Test]
    public async Task Prepend_ExplicitTrue_OnTerminalList_IsRetryable()
    {
        var collection = new ResultProblemCollection(Terminal("a"));

        var prepended = collection.Prepend(true, "context {0}", 1);

        await Assert.That(prepended.First().IsRetryable).IsTrue();
        await Assert.That(prepended.First().FormattedMessage).IsEqualTo("context 1");
    }

    [Test]
    public async Task Prepend_ExplicitFalse_OnRetryableList_IsNotRetryable()
    {
        var collection = new ResultProblemCollection(Retryable("a"));

        var prepended = collection.Prepend(false, "context");

        await Assert.That(prepended.First().IsRetryable).IsFalse();
        await Assert.That(prepended.IsRetryable).IsFalse();
    }

    [Test]
    public async Task Prepend_BoolFormatArg_BindsToLegacyOverload()
    {
        // `true` must be a format argument, not the retryable flag. On an empty list inheritance gives false,
        // so a true IsRetryable here would mean the flag overload was chosen.
        var prepended = new ResultProblemCollection().Prepend("Enabled: {0}", true);

        var problem = prepended.Single();
        await Assert.That(problem.IsRetryable).IsFalse();
        await Assert.That(problem.FormattedMessage).IsEqualTo("Enabled: True");
    }

    // ---- Prepend (exception) ----

    [Test]
    public async Task PrependException_NoFlag_AllRetryable_Inherits()
    {
        var collection = new ResultProblemCollection(Retryable("a"));

        var prepended = collection.Prepend(new TimeoutException(), "context");

        await Assert.That(prepended.First().IsRetryable).IsTrue();
        await Assert.That(prepended.First().Exception).IsTypeOf<TimeoutException>();
    }

    [Test]
    public async Task PrependException_NoFlag_OneTerminal_IsNotRetryable()
    {
        var collection = new ResultProblemCollection(Retryable("a"), Terminal("b"));

        var prepended = collection.Prepend(new TimeoutException(), "context");

        await Assert.That(prepended.First().IsRetryable).IsFalse();
    }

    [Test]
    public async Task PrependException_NoFlag_Empty_IsNotRetryable()
    {
        var prepended = new ResultProblemCollection().Prepend(new TimeoutException(), "context");

        await Assert.That(prepended.First().IsRetryable).IsFalse();
    }

    [Test]
    public async Task PrependException_ExplicitFlag_UsesFlag()
    {
        var retryable = new ResultProblemCollection(Terminal("a")).Prepend(true, new TimeoutException(), "context");
        var terminal = new ResultProblemCollection(Retryable("a")).Prepend(false, new TimeoutException(), "context {0}", 2);

        await Assert.That(retryable.First().IsRetryable).IsTrue();
        await Assert.That(terminal.First().IsRetryable).IsFalse();
        await Assert.That(terminal.First().FormattedMessage).IsEqualTo("context 2");
    }

    // ---- Prepend / Append of existing problems ----

    [Test]
    public async Task PrependAndAppend_ExistingProblems_KeepTheirFlags()
    {
        var collection = new ResultProblemCollection(Retryable("a"));

        var prepended = collection.Prepend(Terminal("p"));
        var appended = collection.Append(Terminal("q"), Retryable("r"));

        await Assert.That(prepended.Select(p => p.IsRetryable)).IsEquivalentTo([false, true]);
        await Assert.That(appended.Select(p => p.IsRetryable)).IsEquivalentTo([true, false, true]);
    }

    // ---- Origin capture ----

    [Test]
    public async Task ContextOverloads_CaptureCallerOrigin()
    {
        var collection = new ResultProblemCollection(Retryable("a"));

        var problems = new[]
        {
            collection.Prepend("x").First(),
            collection.Prepend(true, "x").First(),
            collection.Prepend(new TimeoutException(), "x").First(),
            collection.Prepend(false, new TimeoutException(), "x").First(),
            Result.Try<Exception>(Throw, false).Problems!.Single(),
            Result.Try<int, Exception>(ThrowInt, false).Problems!.Single(),
        };

        foreach (var problem in problems)
        {
            await Assert.That(problem.OriginInformation.FilePath.Name).IsEqualTo("RetryableTests.cs");
        }
    }

    // ---- Result.Try ----

    [Test]
    public async Task Try_Default_IsRetryable()
    {
        await Assert.That(Result.Try<Exception>(Throw).IsRetryable).IsTrue();
        await Assert.That(Result.Try<int, Exception>(ThrowInt).IsRetryable).IsTrue();
    }

    [Test]
    public async Task Try_LegacyShapeWithBoolFormatArg_BindsFormatArg()
    {
        var result = Result.Try<Exception>(Throw, "m {0}", false);
        var typed = Result.Try<int, Exception>(ThrowInt, "m {0}", false);

        foreach (var problem in new[] { result.Problems!.Single(), typed.Problems!.Single() })
        {
            await Assert.That(problem.IsRetryable).IsTrue();
            await Assert.That(problem.FormattedMessage).IsEqualTo("m False");
        }
    }

    [Test]
    public async Task Try_ExplicitFlag_UsesFlag()
    {
        var result = Result.Try<Exception>(Throw, false, "m {0}", true);
        var typed = Result.Try<int, Exception>(ThrowInt, false, "m {0}", true);
        var retryable = Result.Try<Exception>(Throw, true);

        foreach (var problem in new[] { result.Problems!.Single(), typed.Problems!.Single() })
        {
            await Assert.That(problem.IsRetryable).IsFalse();
            await Assert.That(problem.FormattedMessage).IsEqualTo("m True");
        }

        await Assert.That(retryable.IsRetryable).IsTrue();
    }

    [Test]
    public async Task Try_ExplicitFlag_Success_IsNotRetryable()
    {
        var result = Result.Try<Exception>(() => { }, true);
        var typed = Result.Try<int, Exception>(() => 5, true);

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(result.IsRetryable).IsFalse();
        await Assert.That(typed.Value).IsEqualTo(5);
        await Assert.That(typed.IsRetryable).IsFalse();
    }

    // ---- JSON ----

    private static ResultProblem RoundTrip(ResultProblem problem) =>
        JsonSerializer.Deserialize<ResultProblem>(JsonSerializer.Serialize(problem))!;

    [Test]
    public async Task Json_RoundTripsFlag_WhenItDiffersFromInferredDefault()
    {
        var exceptionTerminal = RoundTrip(new ResultProblem(new TimeoutException(), "t") { IsRetryable = false });
        var messageRetryable = RoundTrip(new ResultProblem("m") { IsRetryable = true });

        await Assert.That(exceptionTerminal.IsRetryable).IsFalse();
        await Assert.That(exceptionTerminal.ExceptionSummary).IsNotNull();
        await Assert.That(messageRetryable.IsRetryable).IsTrue();
    }

    [Test]
    public async Task Json_WritesIsRetryable()
    {
        var json = JsonSerializer.Serialize(new ResultProblem("m") { IsRetryable = true });

        await Assert.That(json).Contains("\"IsRetryable\":true");
    }

    [Test]
    public async Task Json_LegacyPayloadWithoutFlag_InfersFromExceptionSummary()
    {
        const string withException = """{"message":"t","Tags":[],"Severity":0,"Source":null,"ExceptionSummary":"TimeoutException: late"}""";
        const string withoutException = """{"message":"m","Tags":[],"Severity":0,"Source":null,"ExceptionSummary":null}""";

        var fromException = JsonSerializer.Deserialize<ResultProblem>(withException)!;
        var fromMessage = JsonSerializer.Deserialize<ResultProblem>(withoutException)!;

        await Assert.That(fromException.IsRetryable).IsTrue();
        await Assert.That(fromMessage.IsRetryable).IsFalse();
    }
}
