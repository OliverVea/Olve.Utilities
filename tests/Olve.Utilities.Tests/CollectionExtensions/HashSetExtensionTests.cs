using Olve.Utilities.CollectionExtensions;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests.CollectionExtensions;

public class HashSetExtensionTests
{
    [Test]
    public async Task Set_WithTrueWhenNotInSet_NowInSetAndReturnsTrue()
    {
        // Arrange
        HashSet<int> set = [1, 2, 3];

        // Act
        var returnValue = set.Set(4, true);

        // Assert
        await Assert
            .That(set)
            .Contains(4);
        await Assert
            .That(returnValue)
            .IsTrue();
    }

    [Test]
    public async Task Set_WithFalseWhenInSet_NowNotInSetAndReturnsTrue()
    {
        // Arrange
        HashSet<int> set = [1, 2, 3];

        // Act
        var returnValue = set.Set(2, false);

        // Assert
        await Assert
            .That(set)
            .DoesNotContain(2);
        await Assert
            .That(returnValue)
            .IsTrue();
    }

    [Test]
    public async Task Set_WithTrueWhenAlreadyInSet_NoChangeAndReturnsFalse()
    {
        // Arrange
        HashSet<int> set = [1, 2, 3];

        // Act
        var returnValue = set.Set(2, true);

        // Assert
        await Assert
            .That(set)
            .Contains(2);
        await Assert
            .That(returnValue)
            .IsFalse();
    }

    [Test]
    public async Task Set_WithFalseWhenNotInSet_NoChangeAndReturnsFalse()
    {
        // Arrange
        HashSet<int> set = [1, 2, 3];

        // Act
        var returnValue = set.Set(4, false);

        // Assert
        await Assert
            .That(set)
            .DoesNotContain(4);
        await Assert
            .That(returnValue)
            .IsFalse();
    }

    [Test]
    public async Task Toggle_WhenNotInSet_NowInSetAndReturnsTrue()
    {
        // Arrange
        HashSet<int> set = [1, 2, 3];

        // Act
        var returnValue = set.Toggle(4);

        // Assert
        await Assert
            .That(set)
            .Contains(4);
        await Assert
            .That(returnValue)
            .IsTrue();
    }

    [Test]
    public async Task Toggle_WhenInSet_NowNotInSetAndReturnsFalse()
    {
        // Arrange
        HashSet<int> set = [1, 2, 3];

        // Act
        var returnValue = set.Toggle(2);

        // Assert
        await Assert
            .That(set)
            .DoesNotContain(2);
        await Assert
            .That(returnValue)
            .IsFalse();
    }
}