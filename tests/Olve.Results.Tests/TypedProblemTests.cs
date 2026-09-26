namespace Olve.Results.Tests;

public class ValidationProblem(string fieldName) : ResultProblem("Validation failed for {0}", fieldName)
{
    public string Field => fieldName;
}

public sealed class RequiredFieldProblem(string fieldName) : ValidationProblem(fieldName);

public sealed class NotFoundProblem(string key) : ResultProblem("Not found: {0}", key)
{
    public string Key => key;
}

public class TypedProblemTests
{
    [Test]
    public async Task Collection_TryPickProblem_ReturnsFirstMatchByReference()
    {
        var first = new ValidationProblem("name");
        var second = new ValidationProblem("email");
        var collection = new ResultProblemCollection(new ResultProblem("plain"), first, second);

        var picked = collection.TryPickProblem<ValidationProblem>(out var problem);

        await Assert.That(picked).IsTrue();
        await Assert.That(problem).IsSameReferenceAs(first);
        await Assert.That(problem!.Field).IsEqualTo("name");
    }

    [Test]
    public async Task Collection_TryPickProblem_ReturnsFalseWhenAbsent()
    {
        var collection = new ResultProblemCollection(new ResultProblem("plain"), new ValidationProblem("name"));

        var picked = collection.TryPickProblem<NotFoundProblem>(out var problem);

        await Assert.That(picked).IsFalse();
        await Assert.That(problem).IsNull();
    }

    [Test]
    public async Task Collection_TryPickProblem_MatchesSubclassesOfRequestedType()
    {
        var required = new RequiredFieldProblem("name");
        var collection = new ResultProblemCollection(new NotFoundProblem("x"), required);

        await Assert.That(collection.TryPickProblem<ValidationProblem>(out var validation)).IsTrue();
        await Assert.That(validation).IsSameReferenceAs(required);
    }

    [Test]
    public async Task Collection_TryPickProblem_BaseTypeMatchesFirstProblem()
    {
        var first = new NotFoundProblem("x");
        var collection = new ResultProblemCollection(first, new ResultProblem("plain"));

        await Assert.That(collection.TryPickProblem<ResultProblem>(out var problem)).IsTrue();
        await Assert.That(problem).IsSameReferenceAs(first);
    }

    [Test]
    public async Task Collection_PickProblems_ReturnsAllMatchesInOrder()
    {
        var a = new ValidationProblem("a");
        var b = new RequiredFieldProblem("b");
        var collection = new ResultProblemCollection(a, new NotFoundProblem("x"), b, new ResultProblem("plain"));

        var picked = collection.PickProblems<ValidationProblem>().ToList();

        await Assert.That(picked.Count).IsEqualTo(2);
        await Assert.That(picked[0]).IsSameReferenceAs(a);
        await Assert.That(picked[1]).IsSameReferenceAs(b);
        await Assert.That(collection.PickProblems<NotFoundProblem>().Count()).IsEqualTo(1);
    }

    [Test]
    public async Task Collection_PickProblems_EmptyWhenAbsent()
    {
        var collection = new ResultProblemCollection(new ResultProblem("plain"));

        await Assert.That(collection.PickProblems<ValidationProblem>().Any()).IsFalse();
    }

    [Test]
    public async Task Collection_SubclassTypeSurvivesPrependAndMerge()
    {
        var validation = new ValidationProblem("name");
        var notFound = new NotFoundProblem("x");

        var prepended = new ResultProblemCollection(validation).Prepend("Context");
        var merged = ResultProblemCollection.Merge(prepended, new ResultProblemCollection(notFound));

        await Assert.That(merged.TryPickProblem<ValidationProblem>(out var pickedValidation)).IsTrue();
        await Assert.That(pickedValidation).IsSameReferenceAs(validation);
        await Assert.That(merged.TryPickProblem<NotFoundProblem>(out var pickedNotFound)).IsTrue();
        await Assert.That(pickedNotFound).IsSameReferenceAs(notFound);
    }

    [Test]
    public async Task Subclass_ToStringUsesFormattedMessage()
    {
        var problem = new ValidationProblem("email");

        await Assert.That(problem.ToBriefString()).IsEqualTo("Validation failed for email");
    }

    [Test]
    public async Task Result_TryPickProblem_ReturnsFirstMatch()
    {
        var validation = new ValidationProblem("name");
        Result result = Result.Failure(new ResultProblem("plain"), validation);

        await Assert.That(result.TryPickProblem<ValidationProblem>(out var problem)).IsTrue();
        await Assert.That(problem).IsSameReferenceAs(validation);
        await Assert.That(result.TryPickProblem<NotFoundProblem>(out _)).IsFalse();
        await Assert.That(result.PickProblems<ValidationProblem>().Single()).IsSameReferenceAs(validation);
    }

    [Test]
    public async Task Result_Success_PicksNothing()
    {
        var result = Result.Success();

        await Assert.That(result.TryPickProblem<ResultProblem>(out var problem)).IsFalse();
        await Assert.That(problem).IsNull();
        await Assert.That(result.PickProblems<ResultProblem>().Any()).IsFalse();
    }

    [Test]
    public async Task Result_Default_PicksNothing()
    {
        Result result = default;

        await Assert.That(result.TryPickProblem<ResultProblem>(out _)).IsFalse();
        await Assert.That(result.PickProblems<ResultProblem>().Any()).IsFalse();
    }

    [Test]
    public async Task ResultOfT_TryPickProblem_ReturnsFirstMatch()
    {
        var notFound = new NotFoundProblem("user:1");
        Result<int> result = notFound;

        await Assert.That(result.TryPickProblem<NotFoundProblem>(out var problem)).IsTrue();
        await Assert.That(problem).IsSameReferenceAs(notFound);
        await Assert.That(problem!.Key).IsEqualTo("user:1");
        await Assert.That(result.TryPickProblem<ValidationProblem>(out _)).IsFalse();
        await Assert.That(result.PickProblems<NotFoundProblem>().Single()).IsSameReferenceAs(notFound);
    }

    [Test]
    public async Task ResultOfT_Success_PicksNothing()
    {
        var result = Result.Success(42);

        await Assert.That(result.TryPickProblem<ResultProblem>(out var problem)).IsFalse();
        await Assert.That(problem).IsNull();
        await Assert.That(result.PickProblems<ResultProblem>().Any()).IsFalse();
    }

    [Test]
    public async Task ResultOfT_Default_PicksNothing()
    {
        Result<int> result = default;

        await Assert.That(result.TryPickProblem<ResultProblem>(out _)).IsFalse();
        await Assert.That(result.PickProblems<ResultProblem>().Any()).IsFalse();
    }

    [Test]
    public async Task DeletionResult_TryPickProblem_PicksFromErrorState()
    {
        var notFound = new NotFoundProblem("row");
        var error = DeletionResult.Error(new ResultProblem("plain"), notFound);

        await Assert.That(error.TryPickProblem<NotFoundProblem>(out var problem)).IsTrue();
        await Assert.That(problem).IsSameReferenceAs(notFound);
        await Assert.That(error.PickProblems<NotFoundProblem>().Single()).IsSameReferenceAs(notFound);

        await Assert.That(DeletionResult.Success().TryPickProblem<ResultProblem>(out _)).IsFalse();
        await Assert.That(DeletionResult.NotFound().TryPickProblem<ResultProblem>(out _)).IsFalse();
        await Assert.That(DeletionResult.NotFound().PickProblems<ResultProblem>().Any()).IsFalse();
    }

    [Test]
    public async Task GeneratedResult_SingleProblemPayload_PicksSubtype()
    {
        var validation = new ValidationProblem("value");
        var invalid = ParseResult.Invalid(validation);

        await Assert.That(invalid.TryPickProblem<ValidationProblem>(out var problem)).IsTrue();
        await Assert.That(problem).IsSameReferenceAs(validation);
        await Assert.That(ParseResult.Parsed(1).TryPickProblem<ValidationProblem>(out _)).IsFalse();
    }
}
