namespace Olve.Utilities.Stores;

/// <summary>
/// Process-wide configuration for <see cref="Event{T}"/> dispatch.
/// </summary>
public static class EventDispatch
{
    /// <summary>
    /// Gets or sets the process-wide sink for exceptions thrown by <see cref="Event{T}"/> subscribers.
    /// <see langword="null"/> (the default) swallows them.
    /// </summary>
    /// <remarks>
    /// Static because <see cref="Event{T}"/> is constructed inline (<c>new()</c>) where dependency injection
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
}
