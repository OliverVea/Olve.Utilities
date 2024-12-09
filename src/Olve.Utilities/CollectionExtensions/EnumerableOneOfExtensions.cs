#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Olve.Utilities.CollectionExtensions;

public static class EnumerableOneOfExtensions
{
    public static IEnumerable<T0> OfT0<T0>(this IEnumerable<OneOf<T0>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT0)
            {
                yield return item.AsT0;
            }
        }
    }
    
    public static IEnumerable<T0> OfT0<T0, T1>(this IEnumerable<OneOf<T0, T1>> source, Action<T1>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T0> OfT0<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source, Action<OneOf<T1, T2>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T0> OfT0<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source, Action<OneOf<T1, T2, T3>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T0> OfT0<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source, Action<OneOf<T1, T2, T3, T4>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T0> OfT0<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source, Action<OneOf<T1, T2, T3, T4, T5>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T0> OfT0<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source, Action<OneOf<T1, T2, T3, T4, T5, T6>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T0> OfT0<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source, Action<OneOf<T1, T2, T3, T4, T5, T6, T7>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T1> OfT1<T0, T1>(this IEnumerable<OneOf<T0, T1>> source, Action<T0>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T1> OfT1<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source, Action<OneOf<T0, T2>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T1> OfT1<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source, Action<OneOf<T0, T2, T3>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T1> OfT1<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source, Action<OneOf<T0, T2, T3, T4>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T1> OfT1<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source, Action<OneOf<T0, T2, T3, T4, T5>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T1> OfT1<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source, Action<OneOf<T0, T2, T3, T4, T5, T6>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T1> OfT1<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source, Action<OneOf<T0, T2, T3, T4, T5, T6, T7>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T2> OfT2<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source, Action<OneOf<T0, T1>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T2> OfT2<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source, Action<OneOf<T0, T1, T3>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T2> OfT2<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source, Action<OneOf<T0, T1, T3, T4>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T2> OfT2<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source, Action<OneOf<T0, T1, T3, T4, T5>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T2> OfT2<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source, Action<OneOf<T0, T1, T3, T4, T5, T6>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T2> OfT2<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source, Action<OneOf<T0, T1, T3, T4, T5, T6, T7>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T3> OfT3<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source, Action<OneOf<T0, T1, T2>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT3(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T3> OfT3<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source, Action<OneOf<T0, T1, T2, T4>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT3(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T3> OfT3<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source, Action<OneOf<T0, T1, T2, T4, T5>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT3(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T3> OfT3<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source, Action<OneOf<T0, T1, T2, T4, T5, T6>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT3(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T3> OfT3<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source, Action<OneOf<T0, T1, T2, T4, T5, T6, T7>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT3(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T4> OfT4<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source, Action<OneOf<T0, T1, T2, T3>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT4(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T4> OfT4<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source, Action<OneOf<T0, T1, T2, T3, T5>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT4(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T4> OfT4<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source, Action<OneOf<T0, T1, T2, T3, T5, T6>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT4(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T4> OfT4<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source, Action<OneOf<T0, T1, T2, T3, T5, T6, T7>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT4(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T5> OfT5<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source, Action<OneOf<T0, T1, T2, T3, T4>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT5(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T5> OfT5<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source, Action<OneOf<T0, T1, T2, T3, T4, T6>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT5(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T5> OfT5<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source, Action<OneOf<T0, T1, T2, T3, T4, T6, T7>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT5(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T6> OfT6<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source, Action<OneOf<T0, T1, T2, T3, T4, T5>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT6(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T6> OfT6<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source, Action<OneOf<T0, T1, T2, T3, T4, T5, T7>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT6(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
    
    public static IEnumerable<T7> OfT7<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source, Action<OneOf<T0, T1, T2, T3, T4, T5, T6>>? fallbackAction = null)
    {
        foreach (var item in source)
        {
            if (item.TryPickT7(out var value, out var remainder))
            {
                yield return value;
            }
            else
            {
                fallbackAction?.Invoke(remainder);
            }
        }
    }
}