namespace Olve.Paths;

public static class PurePathSwitchExtensions
{
    public static void Switch<T>(this PurePath path, Action<T> action, Action? fallback = null) where T : PurePath
    {
        if (path is T t) action.Invoke(t); 
        else fallback?.Invoke();
    }

    public static void Switch<T1, T2>(this PurePath path, Action<T1> action1, Action<T2> action2, Action? fallback = null)
        where T1 : PurePath
        where T2 : PurePath
    {
        if (path is T1 t1) action1.Invoke(t1); 
        else if (path is T2 t2) action2.Invoke(t2);
        else fallback?.Invoke();
    }

    public static void Switch<T1, T2, T3>(this PurePath path, Action<T1> action1, Action<T2> action2, Action<T3> action3, Action? fallback = null) 
        where T1 : PurePath
        where T2 : PurePath
        where T3 : PurePath
    {
        if (path is T1 t1) action1.Invoke(t1); 
        else if (path is T2 t2) action2.Invoke(t2);
        else if (path is T3 t3) action3.Invoke(t3);
        else fallback?.Invoke();
    }
}