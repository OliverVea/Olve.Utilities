using Olve.Results.TUnit;

namespace Olve.Results.Tests;

public class ResultExtensionsTests
{
    [Test]
    public async Task ToEmptyResult_OnSuccess_ReturnsSuccess()
    {
        var result = Result.Success(42);

        var empty = result.ToEmptyResult();

        await Assert.That(empty).Succeeded();
    }

    [Test]
    public async Task ToEmptyResult_OnFailure_ReturnsFailure()
    {
        Result<int> result = new ResultProblem("error");

        var empty = result.ToEmptyResult();

        await Assert.That(empty).Failed();
    }

    [Test]
    public async Task Map_OnSuccess_MapsValue()
    {
        var result = Result.Success(21);

        var mapped = result.Map(x => x * 2);

        await Assert.That(mapped).SucceededAndValue(v => v.IsEqualTo(42));
    }

    [Test]
    public async Task Map_OnFailure_PropagatesProblems()
    {
        Result<int> result = new ResultProblem("error");

        var mapped = result.Map(x => x * 2);

        await Assert.That(mapped).Failed();
    }

    [Test]
    public async Task Bind_OnSuccess_ReturnsInnerResult()
    {
        var result = Result.Success("42");

        var bound = result.Bind(s =>
            int.TryParse(s, out var n)
                ? Result.Success(n)
                : Result.Failure<int>(new ResultProblem("parse error")));

        await Assert.That(bound).SucceededAndValue(v => v.IsEqualTo(42));
    }

    [Test]
    public async Task Bind_OnSuccess_WhenInnerFails_ReturnsInnerProblems()
    {
        var result = Result.Success("not a number");

        var bound = result.Bind(s =>
            int.TryParse(s, out var n)
                ? Result.Success(n)
                : Result.Failure<int>(new ResultProblem("parse error")));

        await Assert.That(bound).Failed();
    }

    [Test]
    public async Task Bind_OnFailure_PropagatesProblems()
    {
        Result<string> result = new ResultProblem("outer error");

        var bound = result.Bind(s => Result.Success(s.Length));

        await Assert.That(bound).Failed();
    }

    [Test]
    public async Task WithValueOnSuccess_OnSuccess_AttachesValue()
    {
        var result = Result.Success();

        var withValue = result.WithValueOnSuccess(42);

        await Assert.That(withValue).SucceededAndValue(v => v.IsEqualTo(42));
    }

    [Test]
    public async Task WithValueOnSuccess_OnFailure_PropagatesProblems()
    {
        Result result = new ResultProblem("error");

        var withValue = result.WithValueOnSuccess(42);

        await Assert.That(withValue).Failed();
    }
}
