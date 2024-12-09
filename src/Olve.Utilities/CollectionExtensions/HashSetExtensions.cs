namespace Olve.Utilities.CollectionExtensions;

public static class HashSetExtensions
{
    public static bool Toggle<T>(this ISet<T> set, T item)
    {
        if (set.Add(item)) return true;
        
        set.Remove(item);
        return false;
    }
    
    public static bool Set<T>(this ISet<T> set, T item, bool value)
    {
        if (value) return set.Add(item);
        return set.Remove(item);
    }
}