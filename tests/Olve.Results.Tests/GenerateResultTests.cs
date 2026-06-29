namespace Olve.Results.Tests;

// A DeletionResult-shaped sample (success / grey / error) exercising the generator.
[GenerateResult]
public readonly partial struct SampleResult
{
    [SuccessCase] public static partial SampleResult Created();
    [GreyCase] public static partial SampleResult Missing();
    [ErrorCase] public static partial SampleResult Broke();
}

// A sample carrying typed payloads on every case, including a problem-collection error.
[GenerateResult]
public readonly partial struct LoadResult
{
    [SuccessCase] public static partial LoadResult Loaded(string content);
    [GreyCase] public static partial LoadResult Missing(string path);
    [ErrorCase] public static partial LoadResult Errored(ResultProblemCollection problems);
}

// A sample whose error case carries a single ResultProblem subtype (wrapped into a collection).
[GenerateResult]
public readonly partial struct ParseResult
{
    [SuccessCase] public static partial ParseResult Parsed(int value);
    [ErrorCase] public static partial ParseResult Invalid(ResultProblem problem);
}

// A DeletionResult-shaped sample (single collection error case) for the implicit conversions.
[GenerateResult]
public readonly partial struct SaveResult
{
    [SuccessCase] public static partial SaveResult Saved();
    [GreyCase] public static partial SaveResult Skipped();
    [ErrorCase] public static partial SaveResult Errored(ResultProblemCollection problems);
}

// A two-error-case sample: implicit conversions are suppressed (ambiguous target).
[GenerateResult]
public readonly partial struct DualErrorResult
{
    [SuccessCase] public static partial DualErrorResult Ok();
    [ErrorCase] public static partial DualErrorResult Left(ResultProblemCollection problems);
    [ErrorCase] public static partial DualErrorResult Right(ResultProblemCollection problems);
}

public class GenerateResultTests
{
    [Test]
    public async Task SuccessCase_IsSucceeded_AndNotFailed()
    {
        var result = SampleResult.Created();

        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(result.Failed).IsFalse();
        await Assert.That(result.IsCreated).IsTrue();
    }

    [Test]
    public async Task GreyCase_IsNeitherSucceededNorFailed()
    {
        var result = SampleResult.Missing();

        await Assert.That(result.Succeeded).IsFalse();
        await Assert.That(result.Failed).IsFalse();
        await Assert.That(result.IsMissing).IsTrue();
    }

    [Test]
    public async Task ErrorCase_IsFailed_AndNotSucceeded()
    {
        var result = SampleResult.Broke();

        await Assert.That(result.Succeeded).IsFalse();
        await Assert.That(result.Failed).IsTrue();
        await Assert.That(result.IsBroke).IsTrue();
    }

    [Test]
    public async Task Match_DispatchesToActiveCase()
    {
        var result = SampleResult.Missing();

        var label = result.Match(
            onCreated: () => "created",
            onMissing: () => "missing",
            onBroke: () => "broke");

        await Assert.That(label).IsEqualTo("missing");
    }

    [Test]
    public async Task TypedMatch_HandsBackPayloadForEachCase()
    {
        var loaded = LoadResult.Loaded("hello");
        var missing = LoadResult.Missing("/tmp/x");

        var loadedValue = loaded.Match(
            onLoaded: content => content,
            onMissing: path => path,
            onErrored: problems => problems.Count().ToString());

        var missingValue = missing.Match(
            onLoaded: content => content,
            onMissing: path => path,
            onErrored: problems => problems.Count().ToString());

        await Assert.That(loadedValue).IsEqualTo("hello");
        await Assert.That(missingValue).IsEqualTo("/tmp/x");
    }

    [Test]
    public async Task Problems_NonNullOnlyForErrorState_AndRoundTrips()
    {
        var collection = new ResultProblemCollection(new ResultProblem("boom"));
        var failed = LoadResult.Errored(collection);

        await Assert.That(LoadResult.Loaded("x").Problems).IsNull();
        await Assert.That(LoadResult.Missing("p").Problems).IsNull();
        await Assert.That(failed.Problems).IsSameReferenceAs(collection);
    }

    [Test]
    public async Task TryPickProblems_TrueForErrorState_FalseOtherwise()
    {
        var failed = LoadResult.Errored(new ResultProblemCollection(new ResultProblem("boom")));

        await Assert.That(failed.TryPickProblems(out var problems)).IsTrue();
        await Assert.That(problems).IsNotNull();

        await Assert.That(LoadResult.Loaded("x").TryPickProblems(out _)).IsFalse();
    }

    [Test]
    public async Task TypedPayloads_PreserveSucceededFailedGreySemantics()
    {
        await Assert.That(LoadResult.Loaded("x").Succeeded).IsTrue();
        await Assert.That(LoadResult.Loaded("x").Failed).IsFalse();

        await Assert.That(LoadResult.Missing("p").Succeeded).IsFalse();
        await Assert.That(LoadResult.Missing("p").Failed).IsFalse();

        await Assert.That(LoadResult.Errored(new ResultProblemCollection()).Succeeded).IsFalse();
        await Assert.That(LoadResult.Errored(new ResultProblemCollection()).Failed).IsTrue();
    }

    [Test]
    public async Task SingleProblemErrorPayload_IsWrappedIntoCollection()
    {
        var problem = new ResultProblem("nope");
        var invalid = ParseResult.Invalid(problem);

        await Assert.That(invalid.Failed).IsTrue();
        await Assert.That(invalid.TryPickProblems(out var problems)).IsTrue();
        await Assert.That(problems!.Single()).IsSameReferenceAs(problem);

        await Assert.That(ParseResult.Parsed(42).Problems).IsNull();
        await Assert.That(ParseResult.Parsed(42).Match(
            onParsed: value => value,
            onInvalid: _ => -1)).IsEqualTo(42);
    }

    [Test]
    public async Task ImplicitConversion_FromCollection_ProducesErrorState_AndRoundTrips()
    {
        var collection = new ResultProblemCollection(new ResultProblem("boom"));

        SaveResult result = collection;

        await Assert.That(result.IsErrored).IsTrue();
        await Assert.That(result.Failed).IsTrue();
        await Assert.That(result.Problems).IsSameReferenceAs(collection);
    }

    [Test]
    public async Task ImplicitConversion_FromSingleProblem_WrapsIntoCollection()
    {
        var problem = new ResultProblem("boom");

        SaveResult result = problem;

        await Assert.That(result.IsErrored).IsTrue();
        await Assert.That(result.Failed).IsTrue();
        await Assert.That(result.Problems!.Single()).IsSameReferenceAs(problem);
    }

    [Test]
    public async Task TwoErrorCases_SuppressImplicitConversions_ButTypeStillWorks()
    {
        // No implicit operator is generated when more than one error case exists (ambiguous target),
        // so a collection cannot convert implicitly — the explicit factories still work.
        var collection = new ResultProblemCollection(new ResultProblem("boom"));

        var left = DualErrorResult.Left(collection);
        var right = DualErrorResult.Right(collection);

        await Assert.That(left.IsLeft).IsTrue();
        await Assert.That(left.Failed).IsTrue();
        await Assert.That(right.IsRight).IsTrue();
        await Assert.That(DualErrorResult.Ok().Succeeded).IsTrue();
    }

    [Test]
    public async Task ToString_NoPayloadCase_IsTheCaseName()
    {
        await Assert.That(SampleResult.Created().ToString()).IsEqualTo("Created");
        await Assert.That(SampleResult.Missing().ToString()).IsEqualTo("Missing");
    }

    [Test]
    public async Task ToString_PayloadCase_AppendsThePayload()
    {
        await Assert.That(LoadResult.Loaded("hello").ToString()).IsEqualTo("Loaded(hello)");
        await Assert.That(LoadResult.Missing("/tmp/x").ToString()).IsEqualTo("Missing(/tmp/x)");
        await Assert.That(ParseResult.Parsed(42).ToString()).IsEqualTo("Parsed(42)");
    }

    [Test]
    public async Task Equality_SameCase_NoPayload_AreEqual()
    {
        await Assert.That(SampleResult.Created()).IsEqualTo(SampleResult.Created());
        await Assert.That(SampleResult.Created() == SampleResult.Created()).IsTrue();
        await Assert.That(SampleResult.Created().GetHashCode()).IsEqualTo(SampleResult.Created().GetHashCode());
    }

    [Test]
    public async Task Equality_DifferentCase_AreUnequal()
    {
        await Assert.That(SampleResult.Created()).IsNotEqualTo(SampleResult.Missing());
        await Assert.That(SampleResult.Created() != SampleResult.Broke()).IsTrue();
    }

    [Test]
    public async Task Equality_SameCase_SamePayload_AreEqual()
    {
        await Assert.That(LoadResult.Loaded("hello")).IsEqualTo(LoadResult.Loaded("hello"));
        await Assert.That(LoadResult.Loaded("hello") == LoadResult.Loaded("hello")).IsTrue();
        await Assert.That(LoadResult.Loaded("hello").GetHashCode()).IsEqualTo(LoadResult.Loaded("hello").GetHashCode());
    }

    [Test]
    public async Task Equality_SameCase_DifferentPayload_AreUnequal()
    {
        await Assert.That(LoadResult.Loaded("hello")).IsNotEqualTo(LoadResult.Loaded("world"));
        await Assert.That(LoadResult.Loaded("hello") != LoadResult.Loaded("world")).IsTrue();
    }

    [Test]
    public async Task Equality_AgainstBoxedObject_AndOtherType()
    {
        object boxed = SampleResult.Created();

        await Assert.That(SampleResult.Created().Equals(boxed)).IsTrue();
        await Assert.That(SampleResult.Created().Equals("not a result")).IsFalse();
    }
}
