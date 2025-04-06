namespace Olve.Paths;

public static class PurePathMatchExtensions
{
    public static TOut Match<T, TOut>(this PurePath path, Func<T, TOut> action, Func<TOut> fallback) where T : PurePath
    {
        if (path is T t) return action.Invoke(t); 
        
        return fallback.Invoke();
    }

    public static TOut Match<T1, T2, TOut>(this PurePath path, Func<T1, TOut> action1, Func<T2, TOut> action2, Func<TOut> fallback)
        where T1 : PurePath
        where T2 : PurePath
    {
        if (path is T1 t1) return action1.Invoke(t1); 
        if (path is T2 t2) return action2.Invoke(t2);
        
        return fallback.Invoke();
    }

    public static TOut Match<T1, T2, T3, TOut>(this PurePath path, Func<T1, TOut> action1, Func<T2, TOut> action2, Func<T3, TOut> action3, Func<TOut> fallback) 
        where T1 : PurePath
        where T2 : PurePath
        where T3 : PurePath
    {
        if (path is T1 t1) return action1.Invoke(t1);
        if (path is T2 t2) return action2.Invoke(t2);
        if (path is T3 t3) return action3.Invoke(t3);

        return fallback.Invoke();
    }
    
}