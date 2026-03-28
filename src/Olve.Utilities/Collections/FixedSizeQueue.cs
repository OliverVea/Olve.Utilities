using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Olve.Utilities.Collections;

/// <summary>
/// Represents a fixed-size queue that maintains a maximum number of items.
/// When the maximum size is exceeded, the oldest items are automatically removed.
/// </summary>
/// <typeparam name="T">The type of items in the queue.</typeparam>
public class FixedSizeQueue<T> : IQueue<T>
{
    private readonly Queue<T> _queue = new();
    private readonly int _maxSize;
    private readonly FullQueueBehavior _fullBehavior;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedSizeQueue{T}"/> class
    /// with the specified maximum size and full-queue behavior.
    /// </summary>
    /// <param name="maxSize">The maximum number of items the queue can hold. Must be greater than zero.</param>
    /// <param name="fullBehavior">The behavior when the queue is full. Defaults to <see cref="FullQueueBehavior.DropOldest"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="maxSize"/> is less than or equal to zero.
    /// </exception>
    public FixedSizeQueue(int maxSize, FullQueueBehavior fullBehavior = FullQueueBehavior.DropOldest)
    {
        if (maxSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSize), "Max size must be greater than 0.");
        }

        _maxSize = maxSize;
        _fullBehavior = fullBehavior;
    }

    /// <inheritdoc />
    public bool Enqueue(T item)
    {
        if (_queue.Count >= _maxSize)
        {
            switch (_fullBehavior)
            {
                case FullQueueBehavior.DropNewest:
                    return false;
                case FullQueueBehavior.Throw:
                    throw new InvalidOperationException(
                        $"Queue is full (capacity: {_maxSize}).");
                case FullQueueBehavior.DropOldest:
                default:
                    while (_queue.Count >= _maxSize)
                    {
                        _queue.Dequeue();
                    }
                    break;
            }
        }

        _queue.Enqueue(item);
        return true;
    }

    /// <inheritdoc />
    public bool TryEnqueue(T item)
    {
        if (_queue.Count >= _maxSize)
        {
            return false;
        }

        _queue.Enqueue(item);
        return true;
    }

    /// <inheritdoc />
    public bool TryDequeue([MaybeNullWhen(false)] out T item)
    {
        return _queue.TryDequeue(out item);
    }

    /// <inheritdoc />
    public bool TryPeek([MaybeNullWhen(false)] out T item)
    {
        return _queue.TryPeek(out item);
    }

    /// <inheritdoc />
    public int Count => _queue.Count;

    /// <inheritdoc />
    public void Clear() => _queue.Clear();

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}