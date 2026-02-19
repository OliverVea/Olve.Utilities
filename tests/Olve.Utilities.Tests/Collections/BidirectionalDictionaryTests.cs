using Olve.Utilities.Collections;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests.Collections;

public class BidirectionalDictionaryTests
{
    private static IBidirectionalDictionary<T1, T2> GetNewBidirectionalDictionary<T1, T2>()
        where T1 : notnull where T2 : notnull => new BidirectionalDictionary<T1, T2>();

    [Test]
    public async Task Set_WithNewKeyValuePair_PairIsAdded()
    {
        // Arrange
        var dictionary = GetNewBidirectionalDictionary<int, string>();

        // Act
        dictionary.Set(1, "one");

        // Assert
        await Assert
            .That(dictionary.FirstValues)
            .Contains(1);
        await Assert
            .That(dictionary.SecondValues)
            .Contains("one");
    }

    [Test]
    public async Task Set_WithDuplicateKey_PreviousValueIsReplaced()
    {
        const int first = 1;
        const string validSecond = "uno";
        const string invalidSecond = "one";

        // Arrange
        var dictionary = GetNewBidirectionalDictionary<int, string>();
        dictionary.Set(first, invalidSecond);

        // Act
        dictionary.Set(first, validSecond);

        // Assert
        await Assert
            .That(dictionary.FirstValues)
            .Contains(1);
        await Assert
            .That(dictionary.SecondValues)
            .Contains("uno");
        await Assert
            .That(dictionary.SecondValues)
            .DoesNotContain("one");

        var foundValid = dictionary.TryGet(validSecond, out var firstWithValidSecond);
        var foundInvalid = dictionary.TryGet(invalidSecond, out _);
        var foundFirst = dictionary.TryGet(first, out var secondFromFirst);

        await Assert
            .That(foundValid)
            .IsTrue();
        await Assert
            .That(firstWithValidSecond)
            .IsEqualTo(first);

        await Assert
            .That(foundInvalid)
            .IsFalse();

        await Assert
            .That(foundFirst)
            .IsTrue();
        await Assert
            .That(secondFromFirst)
            .IsEqualTo(validSecond);
    }

    [Test]
    public async Task Remove_ByKey_PairIsRemoved()
    {
        // Arrange
        var dictionary = GetNewBidirectionalDictionary<int, string>();
        dictionary.Set(1, "one");

        // Act
        var removed = dictionary.TryRemove(1);

        // Assert
        await Assert
            .That(removed)
            .IsTrue();
        await Assert
            .That(dictionary.FirstValues)
            .DoesNotContain(1);
        await Assert
            .That(dictionary.SecondValues)
            .DoesNotContain("one");
    }

    [Test]
    public async Task Remove_ByValue_PairIsRemoved()
    {
        // Arrange
        var dictionary = GetNewBidirectionalDictionary<int, string>();
        dictionary.Set(2, "two");

        // Act
        var removed = dictionary.TryRemove("two");

        // Assert
        await Assert
            .That(removed)
            .IsTrue();
        await Assert
            .That(dictionary.FirstValues)
            .DoesNotContain(2);
        await Assert
            .That(dictionary.SecondValues)
            .DoesNotContain("two");
    }

    [Test]
    public async Task ContainsKey_WithExistingKey_ReturnsTrue()
    {
        // Arrange
        var dictionary = GetNewBidirectionalDictionary<int, string>();
        dictionary.Set(3, "three");

        // Act
        var containsKey = dictionary.Contains(3);

        // Assert
        await Assert
            .That(containsKey)
            .IsTrue();
    }

    [Test]
    public async Task ContainsValue_WithExistingValue_ReturnsTrue()
    {
        // Arrange
        var dictionary = GetNewBidirectionalDictionary<int, string>();
        dictionary.Set(4, "four");

        // Act
        var containsValue = dictionary.Contains("four");

        // Assert
        await Assert
            .That(containsValue)
            .IsTrue();
    }

    [Test]
    public async Task Set_WithNullKey_ThrowsArgumentNullException()
    {
        // Arrange
        var dictionary = GetNewBidirectionalDictionary<string, string>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() => dictionary.Set(null!, "value")));
    }

    [Test]
    public async Task Set_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var dictionary = GetNewBidirectionalDictionary<string, string>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Run(() => dictionary.Set("key", null!)));
    }

    [Test]
    public async Task TryGet_WithNonExistentKey_ReturnsFalse()
    {
        // Arrange
        var dictionary = GetNewBidirectionalDictionary<int, string>();

        // Act
        var found = dictionary.TryGet(5, out _);

        // Assert
        await Assert
            .That(found)
            .IsFalse();
    }

    [Test]
    public async Task TryGet_WithNonExistentValue_ReturnsFalse()
    {
        // Arrange
        var dictionary = GetNewBidirectionalDictionary<int, string>();

        // Act
        var found = dictionary.TryGet("non-existent", out _);

        // Assert
        await Assert
            .That(found)
            .IsFalse();
    }

    [Test]
    public async Task Clear_RemovesAllPairs()
    {
        // Arrange
        var dictionary = GetNewBidirectionalDictionary<int, string>();
        dictionary.Set(1, "one");
        dictionary.Set(2, "two");

        // Act
        dictionary.Clear();

        // Assert
        await Assert
            .That(dictionary.FirstValues)
            .IsEmpty();
        await Assert
            .That(dictionary.SecondValues)
            .IsEmpty();
    }
}