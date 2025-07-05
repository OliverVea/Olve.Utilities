using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Collections;

/// <summary>
/// Represents a fixed-size queue that maintains a maximum number of items.
/// When the maximum size is exceeded, the oldest items are removed.
/// </summary>
/// <typeparam name="T">The type of elements contained in the queue.</typeparam>
public interface IQueue<T> : IEnumerable<T>
{
    /// <summary>
    /// Adds an item to the queue.
    /// If adding the item causes the queue to exceed its maximum size,
    /// the oldest item is removed.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    void Enqueue(T item);

    /// <summary>
    /// Attempts to remove and return the item at the front of the queue.
    /// </summary>
    /// <param name="item">
    /// When this method returns, contains the item removed from the queue,
    /// or <c>null</c> if the queue was empty.
    /// </param>
    /// <returns>
    /// <c>true</c> if an item was successfully removed and returned;
    /// <c>false</c> if the queue was empty.
    /// </returns>
    bool TryDequeue([MaybeNullWhen(false)] out T item);

    /// <summary>
    /// Attempts to return the item at the front of the queue without removing it.
    /// </summary>
    /// <param name="item">
    /// When this method returns, contains the item at the front of the queue,
    /// or <c>null</c> if the queue is empty.
    /// </param>
    /// <returns>
    /// <c>true</c> if an item was returned; <c>false</c> if the queue is empty.
    /// </returns>
    bool TryPeek([MaybeNullWhen(false)] out T item);

    /// <summary>
    /// Gets the number of items currently contained in the queue.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Removes all items from the queue.
    /// </summary>
    void Clear();
}