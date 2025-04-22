using System.Diagnostics;

namespace Olve.Results.Tests;

public class ResultTests
{
    private const string Message = "some problem occurred";

    private static string GetPlatformString(string path) =>
        OperatingSystem.IsWindows() ? path.Replace("/", "\\") : path;

    private void SyntaxTesting()
    {
        const string message = "Hi!";
        const int severity = 1;
        var arg0 = new { x = 1 };
        var arg1 = new { x = 2 };
        string[] tags = ["tag1", "tag2"];

        _ = new ResultProblem(message);
        _ = new ResultProblem(message) { Severity = severity };
        _ = new ResultProblem(message) { Tags = tags };
        _ = new ResultProblem(message) { Severity = severity, Tags = tags };

        // With arguments
        _ = new ResultProblem(message + "{0}", arg0);
        _ = new ResultProblem(message + "{0}, {1}", arg0, arg1);
        _ = new ResultProblem(message + "{0}", arg0) { Severity = severity };
        _ = new ResultProblem(message + "{0}", arg0) { Tags = tags };
        _ = new ResultProblem(message + "{0}", arg0) { Severity = severity, Tags = tags };
    }

    [Test]
    public async Task Test()
    {
        // Arrange

        // Act
        var problem = new ResultProblem(Message);

        // Assert
        await Assert.That(problem.OriginInformation.FilePath.Path).EndsWith("ResultTests.cs");
        await Assert.That(problem.OriginInformation.LineNumber).IsEqualTo(40);
    }

    [Test]
    public async Task Test2()
    {
        // Arrange

        // Act
        var problem = GetResultProblem();

        // Assert
        await Assert.That(problem.OriginInformation.FilePath.Path).EndsWith("ResultTests.cs");
        await Assert.That(problem.OriginInformation.LineNumber).IsEqualTo(62);
    }

    private static ResultProblem GetResultProblem()
    {
        return new ResultProblem(Message);
    }

    [Test]
    public async Task Test3()
    {
        // Arrange
        var problem = GetResultProblem();

        // Act
        var debugString = problem.ToDebugString();

        Debug.WriteLine(debugString);
        Console.WriteLine(debugString);

        // Assert
        var expected = GetPlatformString(
            "Olve.Utilities/tests/Olve.Results.Tests/ResultTests.cs:62] some problem occurred"
        );
        await Assert.That(debugString).EndsWith(expected);
    }

    [Test]
    [Arguments(false, false, false, false)]
    [Arguments(true, false, true, false)]
    [Arguments(false, true, false, false)]
    [Arguments(true, true, true, true)]
    public async Task Test4(bool aSucceeds, bool bSucceeds, bool bInvokedExpected, bool expected)
    {
        // Arrange
        bool aInvoked = false,
            bInvoked = false;

        Func<Result> a = aSucceeds
            ? () => Success(() => aInvoked = true)
            : () => Fail(() => aInvoked = true);
        Func<Result> b = bSucceeds
            ? () => Success(() => bInvoked = true)
            : () => Fail(() => bInvoked = true);

        // Act
        var actual = Result.Chain(a, b);

        await Assert.That(actual.Succeeded).IsEqualTo(expected);
        await Assert.That(aInvoked).IsTrue();
        await Assert.That(bInvoked).IsEqualTo(bInvokedExpected);
    }

    private Result Fail(Action? action)
    {
        action?.Invoke();

        return new ResultProblem("fail!");
    }

    private Result Success(Action? action = null)
    {
        action?.Invoke();

        return Result.Success();
    }

    [Test]
    public async Task Test5()
    {
        // Arrange
        Func<Result<int>> a = () => Result.Success(1),
            b = () => Result.Success(2),
            c = () => Result.Success(3);

        // Act
        var result = Result.Concat(a, b, c);

        // Assert
        await Assert.That(result.Succeeded).IsTrue();

        var (aValue, bValue, cValue) = result.Value;

        await Assert.That(aValue).IsEqualTo(1);
        await Assert.That(bValue).IsEqualTo(2);
        await Assert.That(cValue).IsEqualTo(3);
    }

    [Test]
    [Arguments(false, false, false, false)]
    [Arguments(true, false, true, false)]
    [Arguments(false, true, false, false)]
    [Arguments(true, true, true, true)]
    public async Task Result_Then(
        bool aSucceeds,
        bool bSucceeds,
        bool bInvokedExpected,
        bool expected
    )
    {
        // Arrange
        bool aInvoked = false,
            bInvoked = false;

        Func<Result<int>> a = aSucceeds
            ? () => SuccessInt(() => aInvoked = true)
            : () => FailInt(() => aInvoked = true);
        Func<int, Result<int>> b = bSucceeds
            ? v => SuccessInt(() => bInvoked = true, v)
            : v => FailInt(() => bInvoked = true, v);

        // Act
        var actual = Result.Chain(a, b);

        // Assert
        await Assert.That(actual.Succeeded).IsEqualTo(expected);
        await Assert.That(aInvoked).IsTrue();
        await Assert.That(bInvoked).IsEqualTo(bInvokedExpected);
    }

    private Result<int> FailInt(Action? action, int value = 0)
    {
        action?.Invoke();

        return new ResultProblem("fail!");
    }

    private Result<int> SuccessInt(Action? action = null, int value = 0)
    {
        action?.Invoke();

        return value + 42;
    }

    private Result<string> GetResultString(string value)
    {
        return value;
    }

    private Result<int> GetParsedInt(string value)
    {
        if (int.TryParse(value, out var result))
        {
            return result;
        }

        return new ResultProblem("Could not parse value");
    }

    [Test]
    public async Task Result_Then_Chaining_Succeeds()
    {
        // Arrange
        var value = "42";

        // Act
        var result = Result.Chain(() => GetResultString(value), GetParsedInt);

        // Assert
        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(result.Value).IsEqualTo(42);
    }

    [Test]
    public async Task Result_Then_Chaining_Fails()
    {
        // Arrange
        var value = "not a number";

        // Act
        var result = Result.Chain(() => GetResultString(value), GetParsedInt);

        // Assert
        await Assert.That(result.Succeeded).IsFalse();
        var problem = result.Problems!.Single();
        await Assert.That(problem.Message).IsEqualTo("Could not parse value");
    }
}
