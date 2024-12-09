namespace Olve.Utilities.CollectionExtensions;

public static class EnumerableExtensions
{
    
    public static IEnumerable<TOut> ForEach<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> action)
    {
        return enumerable.Select(action);
    }
    
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable) action(item);
    }
    
    public static float Product(this IEnumerable<float> enumerable)
    {
        return enumerable.Aggregate(1f, (current, value) => current * value);
    }
}