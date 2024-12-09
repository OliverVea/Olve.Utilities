namespace Olve.Utilities.CollectionExtensions;

/// <summary>
/// Extension methods for <see cref="ISet{T}"/>.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class ISetExtensions
{
    /// <summary>
    /// Toggles the item in the set. If the item is not in the set, it is added. If the item is in the set, it is removed.
    /// </summary>
    /// <param name="set">The set to toggle the item in.</param>
    /// <param name="item">The item to toggle.</param>
    /// <typeparam name="T">The type of the items in the set.</typeparam>
    /// <returns>True if the item was added, false if it was removed.</returns>
    public static bool Toggle<T>(this ISet<T> set, T item)
    {
        if (set.Add(item)) return true;
        
        set.Remove(item);
        return false;
    }
    
    /// <summary>
    /// Sets the item in the set. If the value is true, the item is added. If the value is false, the item is removed.
    /// </summary>
    /// <param name="set">The set to set the item in.</param>
    /// <param name="item">The item to set.</param>
    /// <param name="value">The value to set the item to.</param>
    /// <typeparam name="T">The type of the items in the set.</typeparam>
    /// <returns>True if the item was added or removed from the set, false if nothing changed.</returns>
    public static bool Set<T>(this ISet<T> set, T item, bool value)
    {
        if (value) return set.Add(item);
        return set.Remove(item);
    }
}