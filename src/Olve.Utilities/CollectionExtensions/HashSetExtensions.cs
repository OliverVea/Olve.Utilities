namespace Olve.Utilities.CollectionExtensions;

public static class HashSetExtensions
{
    public static bool Toggle<T>(this HashSet<T> set, T item)
    {
        if (set.Add(item)) return true;
        
        set.Remove(item);
        return false;
    }
    
    public static void Toggle<T>(this HashSet<T> set, T item, bool value)
    {
        if (value) set.Add(item);
        else set.Remove(item);
    }
}