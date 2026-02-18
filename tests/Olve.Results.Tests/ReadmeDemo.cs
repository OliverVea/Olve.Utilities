using System.Diagnostics.CodeAnalysis;
using Olve.Results.TUnit;

namespace Olve.Results.Tests;

public class ReadmeDemo
{
    [Test]
    public async Task BasicResultHandling()
    {
        Result<string> result = Result.Try<string, IOException>(
            () => File.ReadAllText("/tmp/olve-results-readme-test.txt"));

        if (result.TryPickProblems(out var problems, out var text))
        {
            foreach (var p in problems)
                Console.WriteLine(p.Message);
        }

        await Assert.That(result).Failed();
    }

    [Test]
    public async Task ChainingDependentOperations()
    {
        Result<string> LoadUser(string name) => Result.Success(name);
        Result<string> LoadProfile(string user) => Result.Success($"profile:{user}");

        var result = Result.Chain(
            () => LoadUser("Alice"),
            user => LoadProfile(user)
        );

        await Assert.That(result).Succeeded();
    }

    [Test]
    public async Task CombiningIndependentOperations()
    {
        Result<string> LoadUser(string name) => Result.Success(name);
        Result<int> LoadSettings() => Result.Success(42);

        Result<(string user, int settings)> setup = Result.Concat(
            () => LoadUser("current"),
            () => LoadSettings()
        );

        await Assert.That(setup).Succeeded();
    }

    [Test]
    public async Task AddingContext()
    {
        Result<string> LoadUser(string name)
            => Result.Failure<string>(new ResultProblem("not found: {0}", name));

        var user = LoadUser("unknown");
        if (user.TryPickProblems(out var problems))
        {
            var contextualized = problems.Prepend("User initialization failed");

            await Assert.That(contextualized).IsNotNull();
            return;
        }

        Assert.Fail("Expected problems");
    }

    [Test]
    public async Task ValidationPattern()
    {
        Result Validate(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new ResultProblem("Email is required");

            if (!email.Contains('@'))
                return new ResultProblem("Invalid email: {0}", email);

            return Result.Success();
        }

        var invalid = Validate("not-an-email");
        var valid = Validate("user@example.com");

        await Assert.That(invalid).Failed();
        await Assert.That(valid).Succeeded();
    }

    [Test]
    public async Task WorkingWithResultCollections()
    {
        Result<string> LoadUser(string name) => Result.Success(name);

        var results = new[]
        {
            LoadUser("Alice"),
            LoadUser("Bob"),
            LoadUser("Charlie"),
        };

        results.TryPickProblems(out var problems, out var users);

        // Assertions
        await Assert.That(problems).IsNull();
        await Assert.That(users!.Count).IsEqualTo(3);
    }

    [Test]
    public async Task TryStarPattern()
    {
        bool TryParseInt(
            string input,
            out int value,
            [NotNullWhen(false)] out ResultProblem? problem)
        {
            if (!int.TryParse(input, out value))
            {
                problem = new ResultProblem("Failed to parse '{0}'", input);
                return false;
            }

            problem = null;
            return true;
        }

        var success = TryParseInt("42", out var value, out var problem);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(42);
        await Assert.That(problem).IsNull();
    }

    [Test]
    public async Task ResultProblemMetadata()
    {
        var query = "SELECT * FROM Users";

        var problem = new ResultProblem("Database query failed: {0}", query)
        {
            Tags = ["database", "query"],
            Severity = 2,
            Source = "Repository",
        };

        await Assert.That(problem.Tags).Contains("database");
        await Assert.That(problem.Severity).IsEqualTo(2);
        await Assert.That(problem.Source).IsEqualTo("Repository");
    }

    [Test]
    public async Task DeletionResult_ThreeStates()
    {
        var success = DeletionResult.Success();
        var notFound = DeletionResult.NotFound();
        var error = DeletionResult.Error(new ResultProblem("disk full"));

        await Assert.That(success.Succeeded).IsTrue();
        await Assert.That(notFound.WasNotFound).IsTrue();
        await Assert.That(error.Failed).IsTrue();
    }

    [Test]
    public async Task DeletionResult_Match()
    {
        var result = DeletionResult.NotFound();

        var message = result.Match(
            onSuccess: () => "Deleted",
            onNotFound: () => "Already gone",
            onProblems: problems => $"Error: {problems.First().Message}");

        await Assert.That(message).IsEqualTo("Already gone");
    }

    [Test]
    public async Task DeletionResult_MapToResult()
    {
        var result = DeletionResult.NotFound();

        var asResult = result.MapToResult(allowNotFound: true); // Success
        var strict = result.MapToResult(allowNotFound: false); // Failure

        await Assert.That(asResult).Succeeded();
        await Assert.That(strict).Failed();
    }

    [Test]
    public async Task ImplicitConversions()
    {
        Result result = new ResultProblem("not found");
        Result<int> typed = new ResultProblem("parse error");
        DeletionResult deletion = new ResultProblem("disk full");

        await Assert.That(result).Failed();
        await Assert.That(typed).Failed();
        await Assert.That(deletion.Failed).IsTrue();
    }

    [Test]
    public async Task ResultTry()
    {
        var result = Result.Try<IOException>(
            () => File.ReadAllText("/nonexistent/path.txt"),
            "Could not read config file");

        await Assert.That(result).Failed();
    }

    [Test]
    public async Task MapAndBind()
    {
        var result = Result.Success("42");

        var mapped = result.Map(s => s.Length); // Result<int> with value 2

        var bound = result.Bind(s =>
            int.TryParse(s, out var n)
                ? Result.Success(n)
                : Result.Failure<int>(new ResultProblem("parse error")));

        await Assert.That(mapped).SucceededAndValue(v => v.IsEqualTo(2));
        await Assert.That(bound).SucceededAndValue(v => v.IsEqualTo(42));
    }

    [Test]
    public async Task DictionaryExtensions()
    {
        var dictionary = new Dictionary<string, int>();

        var setResult = dictionary.SetWithResult("answer", 42); // Success
        var duplicate = dictionary.SetWithResult("answer", 99); // Failure: key exists

        IReadOnlyDictionary<string, int> readOnly = dictionary;
        var getResult = readOnly.GetWithResult("answer"); // Result<int> with value 42
        var missing = readOnly.GetWithResult("unknown"); // Failure: key not found

        await Assert.That(setResult).Succeeded();
        await Assert.That(duplicate).Failed();
        await Assert.That(getResult).SucceededAndValue(v => v.IsEqualTo(42));
        await Assert.That(missing).Failed();
    }
}
