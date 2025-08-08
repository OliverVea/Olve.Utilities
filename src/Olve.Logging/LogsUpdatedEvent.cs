namespace Olve.Logging;

/// <summary>
/// Simple event aggregator used to notify subscribers when the log collection is updated.
/// </summary>
public class LogsUpdatedEvent
{
    private readonly HashSet<Action> _subscribers = [];

    /// <summary>
    /// Subscribe to notifications. Duplicate subscriptions are ignored.
    /// </summary>
    /// <param name="subscriber">Callback to invoke when the event is raised.</param>
    public void Subscribe(Action subscriber)
    {
        _subscribers.Add(subscriber);
    }

    /// <summary>
    /// Unsubscribe a previously registered callback.
    /// </summary>
    /// <param name="subscriber">The callback to remove.</param>
    public void Unsubscribe(Action subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    /// <summary>
    /// Invoke all subscribed callbacks.
    /// </summary>
    public void Invoke()
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber();
        }
    }
}
