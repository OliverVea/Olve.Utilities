namespace Olve.Utilities.Collections;

/// <summary>
/// Specifies the behavior of a <see cref="FixedSizeQueue{T}"/> when an item
/// is enqueued and the queue is already at maximum capacity.
/// </summary>
public enum FullQueueBehavior
{
    /// <summary>
    /// Remove the oldest item to make room for the new item. This is the default behavior.
    /// </summary>
    DropOldest,

    /// <summary>
    /// Reject the incoming item, leaving the queue unchanged.
    /// </summary>
    DropNewest,

    /// <summary>
    /// Throw an <see cref="InvalidOperationException"/>.
    /// </summary>
    Throw,
}
