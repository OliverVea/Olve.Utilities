namespace Olve.Paths;

public static class PurePathSwitchExtensions
{
    public static void Switch<T>(this IPurePath path, Action<T> action, Action? fallback = null) where T : IPurePath
    {
        if (path is T t) action.Invoke(t); 
        else fallback?.Invoke();
    }

    public static void Switch<T1, T2>(this IPurePath path, Action<T1> action1, Action<T2> action2, Action? fallback = null)
        where T1 : IPurePath
        where T2 : IPurePath
    {
        if (path is T1 t1) action1.Invoke(t1); 
        else if (path is T2 t2) action2.Invoke(t2);
        else fallback?.Invoke();
    }

    public static void Switch<T1, T2, T3>(this IPurePath path, Action<T1> action1, Action<T2> action2, Action<T3> action3, Action? fallback = null) 
        where T1 : IPurePath
        where T2 : IPurePath
        where T3 : IPurePath
    {
        if (path is T1 t1) action1.Invoke(t1); 
        else if (path is T2 t2) action2.Invoke(t2);
        else if (path is T3 t3) action3.Invoke(t3);
        else fallback?.Invoke();
    }
}