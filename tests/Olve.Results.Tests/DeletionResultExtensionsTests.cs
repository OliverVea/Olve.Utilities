using Olve.Results.TUnit;

namespace Olve.Results.Tests;

public class DeletionResultExtensionsTests
{
    [Test]
    public async Task Match_OnSuccess_CallsOnSuccess()
    {
        var result = DeletionResult.Success();

        var value = result.Match(
            onSuccess: () => "success",
            onNotFound: () => "not found",
            onProblems: _ => "problems");

        await Assert.That(value).IsEqualTo("success");
    }

    [Test]
    public async Task Match_OnNotFound_CallsOnNotFound()
    {
        var result = DeletionResult.NotFound();

        var value = result.Match(
            onSuccess: () => "success",
            onNotFound: () => "not found",
            onProblems: _ => "problems");

        await Assert.That(value).IsEqualTo("not found");
    }

    [Test]
    public async Task Match_OnError_CallsOnProblems()
    {
        var result = DeletionResult.Error(new ResultProblem("something went wrong"));

        var value = result.Match(
            onSuccess: () => "success",
            onNotFound: () => "not found",
            onProblems: _ => "problems");

        await Assert.That(value).IsEqualTo("problems");
    }

    [Test]
    public async Task MapToResult_OnSuccess_ReturnsSuccess()
    {
        var result = DeletionResult.Success();

        var mapped = result.MapToResult();

        await Assert.That(mapped).Succeeded();
    }

    [Test]
    public async Task MapToResult_OnNotFound_AllowNotFound_ReturnsSuccess()
    {
        var result = DeletionResult.NotFound();

        var mapped = result.MapToResult(allowNotFound: true);

        await Assert.That(mapped).Succeeded();
    }

    [Test]
    public async Task MapToResult_OnNotFound_DisallowNotFound_ReturnsFailed()
    {
        var result = DeletionResult.NotFound();

        var mapped = result.MapToResult(allowNotFound: false);

        await Assert.That(mapped).Failed();
    }

    [Test]
    public async Task MapToResult_OnError_ReturnsFailed()
    {
        var result = DeletionResult.Error(new ResultProblem("error"));

        var mapped = result.MapToResult();

        await Assert.That(mapped).Failed();
    }

    [Test]
    public async Task ImplicitConversion_FromResultProblem_CreatesErrorResult()
    {
        DeletionResult result = new ResultProblem("something went wrong");

        await Assert.That(result.Failed).IsTrue();
        await Assert.That(result.Problems).IsNotNull();
    }

    [Test]
    public async Task ImplicitConversion_FromResultProblemCollection_CreatesErrorResult()
    {
        DeletionResult result = new ResultProblemCollection(new ResultProblem("error"));

        await Assert.That(result.Failed).IsTrue();
        await Assert.That(result.Problems).IsNotNull();
    }
}
