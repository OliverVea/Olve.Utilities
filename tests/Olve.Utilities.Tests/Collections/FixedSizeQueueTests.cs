using Olve.Utilities.Collections;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace Olve.Utilities.Tests.Collections;

public class FixedSizeQueueTests
{
    private static FixedSizeQueue<T> GetNewQueue<T>(
        int maxSize,
        FullQueueBehavior fullBehavior = FullQueueBehavior.DropOldest) => new(maxSize, fullBehavior);

    [Test]
    public async Task Constructor_WithValidMaxSize_InitializesEmptyQueue()
    {
        // Act
        var queue = GetNewQueue<int>(3);

        // Assert
        await Assert
            .That(queue.Count)
            .IsEqualTo(0);
    }

    [Test]
    public Task Constructor_WithZeroOrNegativeMaxSize_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => GetNewQueue<int>(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => GetNewQueue<int>(-1));
        return Task.CompletedTask;
    }

    [Test]
    public async Task Enqueue_WithinMaxSize_AddsAllItems()
    {
        // Arrange
        var queue = GetNewQueue<string>(3);

        // Act
        queue.Enqueue("A");
        queue.Enqueue("B");

        // Assert
        await Assert
            .That(queue.Count)
            .IsEqualTo(2);
        await Assert
            .That(queue)
            .Contains("A");
        await Assert
            .That(queue)
            .Contains("B");
    }

    [Test]
    public async Task Enqueue_ExceedingMaxSize_RemovesOldestItems()
    {
        // Arrange
        var queue = GetNewQueue<int>(2);

        // Act
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);

        // Assert
        await Assert
            .That(queue.Count)
            .IsEqualTo(2);
        var items = queue.ToList();
        await Assert.That(items[0]).IsEqualTo(2);
        await Assert.That(items[1]).IsEqualTo(3);
    }

    [Test]
    public async Task Clear_RemovesAllItems()
    {
        // Arrange
        var queue = GetNewQueue<double>(3);
        queue.Enqueue(1.1);
        queue.Enqueue(2.2);

        // Act
        queue.Clear();

        // Assert
        await Assert.That(queue.Count).IsEqualTo(0);
        await Assert.That(queue).IsEmpty();
    }

    [Test]
    public async Task Enumerator_YieldsItemsInOrder()
    {
        // Arrange
        var queue = GetNewQueue<char>(3);
        queue.Enqueue('X');
        queue.Enqueue('Y');
        queue.Enqueue('Z');

        // Act
        var items = queue.ToList();

        // Assert
        await Assert.That(items[0]).IsEqualTo('X');
        await Assert.That(items[1]).IsEqualTo('Y');
        await Assert.That(items[2]).IsEqualTo('Z');
    }
    
    [Test]
    public async Task TryPeek_OnNonEmptyQueue_ReturnsFirstItemWithoutRemoving()
    {
        // Arrange
        var queue = GetNewQueue<string>(3);
        queue.Enqueue("first");
        queue.Enqueue("second");

        // Act
        var result = queue.TryPeek(out var peekedItem);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(peekedItem).IsEqualTo("first");
        await Assert.That(queue.Count).IsEqualTo(2);
    }

    [Test]
    public async Task TryPeek_OnEmptyQueue_ReturnsFalse()
    {
        // Arrange
        var queue = GetNewQueue<int>(3);

        // Act
        var result = queue.TryPeek(out var peekedItem);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(peekedItem).IsEqualTo(0);
    }

    [Test]
    public async Task TryDequeue_OnNonEmptyQueue_RemovesAndReturnsFirstItem()
    {
        // Arrange
        var queue = GetNewQueue<int>(3);
        queue.Enqueue(10);
        queue.Enqueue(20);

        // Act
        var result = queue.TryDequeue(out var dequeuedItem);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(dequeuedItem).IsEqualTo(10);
        await Assert.That(queue.Count).IsEqualTo(1);
        await Assert.That(queue).Contains(20);
    }

    [Test]
    public async Task TryDequeue_OnEmptyQueue_ReturnsFalse()
    {
        // Arrange
        var queue = GetNewQueue<int>(3);

        // Act
        var result = queue.TryDequeue(out var dequeuedItem);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(dequeuedItem).IsEqualTo(0);
    }

    [Test]
    public async Task Enqueue_DropNewest_WhenFull_RejectsNewItem()
    {
        // Arrange
        var queue = GetNewQueue<int>(2, FullQueueBehavior.DropNewest);
        queue.Enqueue(1);
        queue.Enqueue(2);

        // Act
        var result = queue.Enqueue(3);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(queue.Count).IsEqualTo(2);
        var items = queue.ToList();
        await Assert.That(items[0]).IsEqualTo(1);
        await Assert.That(items[1]).IsEqualTo(2);
    }

    [Test]
    public async Task Enqueue_DropNewest_WhenNotFull_AddsItem()
    {
        // Arrange
        var queue = GetNewQueue<int>(3, FullQueueBehavior.DropNewest);

        // Act
        var result = queue.Enqueue(1);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(queue.Count).IsEqualTo(1);
    }

    [Test]
    public Task Enqueue_Throw_WhenFull_ThrowsInvalidOperationException()
    {
        // Arrange
        var queue = GetNewQueue<int>(2, FullQueueBehavior.Throw);
        queue.Enqueue(1);
        queue.Enqueue(2);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => queue.Enqueue(3));
        return Task.CompletedTask;
    }

    [Test]
    public async Task Enqueue_Throw_WhenNotFull_AddsItem()
    {
        // Arrange
        var queue = GetNewQueue<int>(3, FullQueueBehavior.Throw);

        // Act
        var result = queue.Enqueue(1);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(queue.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Enqueue_DropOldest_ReturnsTrue()
    {
        // Arrange
        var queue = GetNewQueue<int>(2);
        queue.Enqueue(1);
        queue.Enqueue(2);

        // Act
        var result = queue.Enqueue(3);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(queue.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Constructor_DefaultBehavior_IsDropOldest()
    {
        // Arrange
        var queue = new FixedSizeQueue<int>(2);
        queue.Enqueue(1);
        queue.Enqueue(2);

        // Act
        queue.Enqueue(3);

        // Assert - oldest item (1) should be gone
        var items = queue.ToList();
        await Assert.That(items[0]).IsEqualTo(2);
        await Assert.That(items[1]).IsEqualTo(3);
    }

    [Test]
    public async Task TryEnqueue_WhenNotFull_AddsItem()
    {
        // Arrange
        var queue = GetNewQueue<int>(2);

        // Act
        var result = queue.TryEnqueue(1);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(queue.Count).IsEqualTo(1);
    }

    [Test]
    public async Task TryEnqueue_WhenFull_ReturnsFalse()
    {
        // Arrange
        var queue = GetNewQueue<int>(2);
        queue.Enqueue(1);
        queue.Enqueue(2);

        // Act
        var result = queue.TryEnqueue(3);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(queue.Count).IsEqualTo(2);
    }

    [Test]
    public async Task TryEnqueue_WhenFull_IgnoresFullQueueBehavior()
    {
        // Arrange — even with DropOldest, TryEnqueue should just return false
        var queue = GetNewQueue<int>(2, FullQueueBehavior.DropOldest);
        queue.Enqueue(1);
        queue.Enqueue(2);

        // Act
        var result = queue.TryEnqueue(3);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(queue.Count).IsEqualTo(2);
        var items = queue.ToList();
        await Assert.That(items[0]).IsEqualTo(1);
        await Assert.That(items[1]).IsEqualTo(2);
    }
}
