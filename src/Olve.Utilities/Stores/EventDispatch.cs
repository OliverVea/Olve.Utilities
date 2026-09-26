namespace Olve.Utilities.Stores;

/// <summary>
/// Process-wide configuration for <see cref="Event{T}"/> and <see cref="Event"/> dispatch.
/// </summary>
public static class EventDispatch
{
    /// <summary>
    /// Gets or sets the process-wide sink for exceptions thrown by <see cref="Event{T}"/> and
    /// <see cref="Event"/> subscribers. <see langword="null"/> (the default) swallows them.
    /// </summary>
    /// <remarks>
    /// Static because events are constructed inline (<c>new()</c>) where dependency injection
    /// can't reach; set it once at startup, for example from an injected logger. The sink runs on the
    /// thread that called <see cref="Event{T}.Invoke"/>. An exception thrown by the sink itself is swallowed,
    /// so it can neither stop the remaining subscribers nor reach the caller.
    /// </remarks>
    public static Action<Exception>? OnHandlerException { get; set; }

    internal static void ReportHandlerException(Exception exception)
    {
        try
        {
            OnHandlerException?.Invoke(exception);
        }
        catch
        {
            // A failing sink must not break the "Invoke never throws" guarantee.
        }
    }

    internal static void Combine<TDelegate>(ref TDelegate? handlers, TDelegate handler)
        where TDelegate : Delegate
        => Update(ref handlers, handler, static (current, h) => (TDelegate?)Delegate.Combine(current, h));

    internal static void Remove<TDelegate>(ref TDelegate? handlers, TDelegate handler)
        where TDelegate : Delegate
        => Update(ref handlers, handler, static (current, h) => (TDelegate?)Delegate.Remove(current, h));

    private static void Update<TDelegate>(
        ref TDelegate? handlers, TDelegate handler, Func<TDelegate?, TDelegate, TDelegate?> change)
        where TDelegate : Delegate
    {
        var current = Volatile.Read(ref handlers);
        while (true)
        {
            var observed = Interlocked.CompareExchange(ref handlers, change(current, handler), current);
            if (ReferenceEquals(observed, current)) return;
            current = observed;
        }
    }
}
