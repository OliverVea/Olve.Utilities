namespace Olve.Utilities.CollectionExtensions;

public static class EnumerableOneOfExtensions
{
    public static IEnumerable<T1> WhereT0<T1>(this IEnumerable<OneOf<T1>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT0)
            {
                yield return item.AsT0;
            }
        }
    }
}