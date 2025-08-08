using Olve.Utilities.CollectionExtensions;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests.CollectionExtensions;

public class EnumerableExtensionsTests
{
    [Test]
    public async Task TryAsSet_OnHashSet_ReturnsTrue()
    {
        // Arrange
        HashSet<int> set = [1, 2, 3];

        // Act
        var result = set.TryAsSet(out var actual);

        // Assert
        await Assert.That(result).IsTrue().Because("the cast should be successful");
        await Assert.That(actual).IsEquivalentTo(set).Because("the sets should be equivalent");
    }

    [Test]
    public async Task TryAsSet_OnArray_ReturnsFalse()
    {
        // Arrange
        int[] array = [1, 2, 3];

        // Act
        var result = array.TryAsSet(out var actual);

        // Assert
        await Assert.That(result).IsFalse().Because("the cast should not be successful");
        await Assert.That(actual).IsNull().Because("the resulting set should be null");
    }

    [Test]
    public async Task TryAsSet_OnHashSet_AllocatesNoMemory()
    {
        // Arrange
        HashSet<int> set = [1, 2, 3];

        var memoryBefore = GC.GetAllocatedBytesForCurrentThread();

        // Act
        var result = set.TryAsSet(out var actual);

        var memoryAfter = GC.GetAllocatedBytesForCurrentThread();
        var delta = memoryAfter - memoryBefore;

        // Assert
        await Assert.That(result).IsTrue().Because("the cast should be successful");
        await Assert.That(actual).IsEquivalentTo(set).Because("the sets should be equivalent");
        await Assert.That(delta).IsEqualTo(0).Because("no memory should be allocated");
    }

    [Test]
    public async Task TryAsReadOnlySet_OnHashSet_ReturnsTrue()
    {
        // Arrange
        HashSet<int> set = [1, 2, 3];

        // Act
        var result = set.TryAsReadOnlySet(out var actual);

        // Assert
        await Assert.That(result).IsTrue().Because("the cast should be successful");
        await Assert.That(actual).IsEquivalentTo(set).Because("the sets should be equivalent");
    }

    [Test]
    public async Task TryAsReadOnlySet_OnArray_ReturnsFalse()
    {
        // Arrange
        int[] array = [1, 2, 3];

        // Act
        var result = array.TryAsReadOnlySet(out var actual);

        // Assert
        await Assert.That(result).IsFalse().Because("the cast should not be successful");
        await Assert.That(actual).IsNull().Because("the resulting set should be null");
    }

    [Test]
    public async Task TryAsReadOnlySet_OnHashSet_AllocatesNoMemory()
    {
        // Arrange
        HashSet<int> set = [1, 2, 3];

        var memoryBefore = GC.GetAllocatedBytesForCurrentThread();

        // Act
        var result = set.TryAsReadOnlySet(out var actual);

        var memoryAfter = GC.GetAllocatedBytesForCurrentThread();
        var delta = memoryAfter - memoryBefore;

        // Assert
        await Assert.That(result).IsTrue().Because("the cast should be successful");
        await Assert.That(actual).IsEquivalentTo(set).Because("the sets should be equivalent");
        await Assert.That(delta).IsEqualTo(0).Because("no memory should be allocated");
    }
}