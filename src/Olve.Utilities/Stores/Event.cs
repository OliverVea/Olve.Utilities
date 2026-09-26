namespace Olve.Utilities.Stores;

/// <summary>
/// A minimal synchronous multicast notification. <see cref="Invoke"/> runs every subscriber inline,
/// on the calling thread, in registration order, and the caller blocks until they finish.
/// </summary>
/// <remarks>
/// Synchronous dispatch is intentional: <see cref="EntityStoreIndex{T,TKey}"/> subscribes to an
/// <see cref="EntityStore{T}"/>'s events to keep its index consistent with the store, and that
/// update must happen synchronously with the mutation. A subscriber that needs async decoupling
/// should bridge to heavier machinery (a channel, a queue) at the subscription site, not here.
/// <see cref="Subscribe"/> and <see cref="Unsubscribe"/> are atomic, so subscribers may come and go
/// concurrently with each other and with <see cref="Invoke"/>.
/// </remarks>
/// <typeparam name="T">The message type passed to subscribers.</typeparam>
public class Event<T>
{
    private Action<T>? _handlers;

    /// <summary>Gets the number of currently registered subscribers.</summary>
    public int SubscriberCount => Volatile.Read(ref _handlers)?.GetInvocationList().Length ?? 0;

    /// <summary>Invokes every subscriber synchronously, in registration order.</summary>
    public void Invoke(T message) => Volatile.Read(ref _handlers)?.Invoke(message);

    /// <summary>Registers <paramref name="handler"/> to be called on every <see cref="Invoke"/>.</summary>
    public void Subscribe(Action<T> handler) => Update(current => current + handler);

    /// <summary>Removes a previously registered <paramref name="handler"/>.</summary>
    public void Unsubscribe(Action<T> handler) => Update(current => current - handler);

    private void Update(Func<Action<T>?, Action<T>?> change)
    {
        var current = Volatile.Read(ref _handlers);
        while (true)
        {
            var observed = Interlocked.CompareExchange(ref _handlers, change(current), current);
            if (ReferenceEquals(observed, current)) return;
            current = observed;
        }
    }
}
