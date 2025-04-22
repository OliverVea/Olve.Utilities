using System.Diagnostics.CodeAnalysis;

namespace Olve.Results.Tests;

[SuppressMessage("Usage", "CA2252:This API requires opting into preview features")]
public class Result_Try
{
    private void ThrowsException() => ThrowsException(() => new Exception());

    private void ThrowsException<T>(Func<T> exceptionFactory)
        where T : Exception
    {
        throw exceptionFactory();
    }

    [Test]
    public async Task ResultTry_ExceptionThrown_DidNotSucceed()
    {
        // Arrange

        // Act
        var result = Result.Try<Exception>(ThrowsException);

        // Assert
        await Assert.That(result.Succeeded).IsFalse();
    }

    [Test]
    public async Task ResultTry_ExceptionThrown_ExceptionContainedInResult()
    {
        // Arrange
        NotSupportedException exception = new();

        // Act
        var result = Result.Try<Exception>(() => ThrowsException(() => exception));

        // Assert
        await Assert.That(result.Succeeded).IsFalse();
        var actualException = result.Problems?.Single().Exception;
        await Assert.That(actualException).IsNotNull().And.IsEqualTo(exception);
    }

    [Test]
    public async Task ResultTry_ExceptionThrown_ResultContainsMessage()
    {
        // Arrange
        NotSupportedException exception = new();

        // Act
        var result = Result.Try<Exception>(
            () => ThrowsException(() => exception),
            "Could not call method '{0}'",
            43
        );

        // Assert
        await Assert.That(result.Succeeded).IsFalse();
        var actualProblem = result.Problems?.Single();
        await Assert
            .That(actualProblem)
            .IsNotNull()
            .And.HasMember(x => x!.Message)
            .EqualTo("Could not call method '{0}'");
    }

    [Test]
    public void ResultTry_ExceptionNotInGenericThrown_ExceptionIsNotCaught()
    {
        // Arrange
        NotSupportedException exception = new();

        // Act & Assert
        Assert.Throws<NotSupportedException>(() =>
            Result.Try<ArgumentException>(() => ThrowsException(() => exception))
        );
    }

    [Test]
    public async Task ResultTry_ExceptionNotThrown_ResultContainsValue()
    {
        // Arrange
        const int value = 42;

        // Act
        var result = Result.Try<int, Exception>(() => value);

        // Assert
        await Assert.That(result.Succeeded).IsTrue();
        await Assert.That(result.Value).IsEqualTo(value);
    }
}
