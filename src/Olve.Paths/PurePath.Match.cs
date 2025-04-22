namespace Olve.Paths;

/// <summary>
/// Provides functional matching methods for <see cref="IPurePath"/> implementations.
/// </summary>
public static class PurePathMatchExtensions
{
    /// <summary>
    /// Matches the given <see cref="IPurePath"/> instance against a single type and returns a result.
    /// </summary>
    public static TOut Match<T, TOut>(
        this IPurePath path,
        Func<T, TOut> action,
        Func<TOut> fallback
    )
        where T : IPurePath
    {
        if (path is T t)
            return action.Invoke(t);

        return fallback.Invoke();
    }

    /// <summary>
    /// Matches the given <see cref="IPurePath"/> instance against two possible types and returns a result.
    /// </summary>
    public static TOut Match<T1, T2, TOut>(
        this IPurePath path,
        Func<T1, TOut> action1,
        Func<T2, TOut> action2,
        Func<TOut> fallback
    )
        where T1 : IPurePath
        where T2 : IPurePath
    {
        if (path is T1 t1)
            return action1.Invoke(t1);
        if (path is T2 t2)
            return action2.Invoke(t2);

        return fallback.Invoke();
    }

    /// <summary>
    /// Matches the given <see cref="IPurePath"/> instance against three possible types and returns a result.
    /// </summary>
    public static TOut Match<T1, T2, T3, TOut>(
        this IPurePath path,
        Func<T1, TOut> action1,
        Func<T2, TOut> action2,
        Func<T3, TOut> action3,
        Func<TOut> fallback
    )
        where T1 : IPurePath
        where T2 : IPurePath
        where T3 : IPurePath
    {
        if (path is T1 t1)
            return action1.Invoke(t1);
        if (path is T2 t2)
            return action2.Invoke(t2);
        if (path is T3 t3)
            return action3.Invoke(t3);

        return fallback.Invoke();
    }
}
