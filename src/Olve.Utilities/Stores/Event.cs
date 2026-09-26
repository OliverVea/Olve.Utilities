namespace Olve.Utilities.Stores;

/// <summary>
/// A minimal synchronous multicast notification. <see cref="Invoke"/> runs every subscriber inline,
/// on the calling thread, in registration order, and the caller blocks until they finish.
/// </summary>
/// <remarks>
/// Synchronous dispatch is intentional: <see cref="EntityStoreIndex{T,TId,TKey}"/> subscribes to an
/// <see cref="IEntityStore{T,TId}"/>'s events to keep its index consistent with the store, and that
/// update must happen synchronously with the mutation. A subscriber that needs async decoupling
/// should bridge to heavier machinery (a channel, a queue) at the subscription site, not here.
/// <see cref="Subscribe"/> and <see cref="Unsubscribe"/> are atomic, so subscribers may come and go
/// concurrently with each other and with <see cref="Invoke"/>.
/// <para>
/// Subscribers are isolated from each other: <see cref="Invoke"/> runs each one in its own try/catch,
/// so a subscriber that throws does not prevent later subscribers from running, and the exception is
/// routed to <see cref="EventDispatch.OnHandlerException"/> instead of the caller. <see cref="Invoke"/>
/// never throws, so a store mutation that raises an event is never reported as failed after it committed.
/// </para>
/// </remarks>
/// <typeparam name="T">The message type passed to subscribers.</typeparam>
public class Event<T>
{
    private Action<T>? _handlers;

    /// <summary>
    /// Invokes every subscriber synchronously, in registration order. A subscriber that throws is reported
    /// to <see cref="EventDispatch.OnHandlerException"/> and does not stop the remaining subscribers.
    /// Never throws.
    /// </summary>
    /// <remarks>
    /// Dispatches to the subscribers registered when the call starts; subscribers added or removed during
    /// dispatch (including by a subscriber) take effect from the next <see cref="Invoke"/>.
    /// </remarks>
    public void Invoke(T message)
    {
        var handlers = Volatile.Read(ref _handlers);
        if (handlers is null) return;

        foreach (var handler in Delegate.EnumerateInvocationList(handlers))
        {
            try
            {
                handler(message);
            }
            catch (Exception ex)
            {
                EventDispatch.ReportHandlerException(ex);
            }
        }
    }

    /// <summary>Registers <paramref name="handler"/> to be called on every <see cref="Invoke"/>.</summary>
    public void Subscribe(Action<T> handler) => EventDispatch.Combine(ref _handlers, handler);

    /// <summary>Removes a previously registered <paramref name="handler"/>.</summary>
    public void Unsubscribe(Action<T> handler) => EventDispatch.Remove(ref _handlers, handler);

    /// <summary>
    /// Registers <paramref name="handler"/> without keeping <paramref name="owner"/> alive. Once nothing
    /// else references the owner and it is collected, the subscription removes itself on the next
    /// <see cref="Invoke"/>. Dispose the returned subscription to unsubscribe immediately.
    /// </summary>
    /// <remarks>
    /// <paramref name="handler"/> receives the owner as an argument and must not capture it (use a
    /// <see langword="static"/> lambda); a captured owner is held strongly and never collected.
    /// </remarks>
    internal IDisposable SubscribeWeak<TOwner>(TOwner owner, Action<TOwner, T> handler)
        where TOwner : class
    {
        var subscription = new WeakSubscription<TOwner>(this, owner, handler);
        Subscribe(subscription.Handler);
        return subscription;
    }

    private sealed class WeakSubscription<TOwner> : IDisposable
        where TOwner : class
    {
        private readonly Event<T> _source;
        private readonly WeakReference<TOwner> _owner;
        private readonly Action<TOwner, T> _handler;

        public WeakSubscription(Event<T> source, TOwner owner, Action<TOwner, T> handler)
        {
            _source = source;
            _owner = new WeakReference<TOwner>(owner);
            _handler = handler;
            Handler = Invoke;
        }

        // One delegate instance, so Unsubscribe removes exactly what Subscribe added.
        public Action<T> Handler { get; }

        public void Dispose() => _source.Unsubscribe(Handler);

        private void Invoke(T message)
        {
            if (_owner.TryGetTarget(out var owner)) _handler(owner, message);
            else Dispose();
        }
    }
}

/// <summary>
/// A synchronous multicast notification without a message: the non-generic counterpart of
/// <see cref="Event{T}"/>, with the same dispatch, isolation and thread-safety guarantees.
/// </summary>
public class Event
{
    private Action? _handlers;

    /// <summary>
    /// Invokes every subscriber synchronously, in registration order. A subscriber that throws is reported
    /// to <see cref="EventDispatch.OnHandlerException"/> and does not stop the remaining subscribers.
    /// Never throws.
    /// </summary>
    /// <remarks>
    /// Dispatches to the subscribers registered when the call starts; subscribers added or removed during
    /// dispatch (including by a subscriber) take effect from the next <see cref="Invoke"/>.
    /// </remarks>
    public void Invoke()
    {
        var handlers = Volatile.Read(ref _handlers);
        if (handlers is null) return;

        foreach (var handler in Delegate.EnumerateInvocationList(handlers))
        {
            try
            {
                handler();
            }
            catch (Exception ex)
            {
                EventDispatch.ReportHandlerException(ex);
            }
        }
    }

    /// <summary>Registers <paramref name="handler"/> to be called on every <see cref="Invoke"/>.</summary>
    public void Subscribe(Action handler) => EventDispatch.Combine(ref _handlers, handler);

    /// <summary>Removes a previously registered <paramref name="handler"/>.</summary>
    public void Unsubscribe(Action handler) => EventDispatch.Remove(ref _handlers, handler);
}
