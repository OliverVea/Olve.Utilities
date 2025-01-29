#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Olve.Utilities.CollectionExtensions;

public static class EnumerableOneOfExtensions
{

    #region AnyT0 - AnyT7

    public static bool AnyT0<T0>(this IEnumerable<OneOf<T0>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT0)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT0<T0, T1>(this IEnumerable<OneOf<T0, T1>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT0)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT0<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT0)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT0<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT0)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT0<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT0)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT0<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT0)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT0<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT0)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT0<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT0)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT1<T0, T1>(this IEnumerable<OneOf<T0, T1>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT1)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT1<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT1)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT1<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT1)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT1<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT1)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT1<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT1)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT1<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT1)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT1<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT1)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT2<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT2)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT2<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT2)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT2<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT2)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT2<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT2)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT2<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT2)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT2<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT2)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT3<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT3)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT3<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT3)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT3<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT3)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT3<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT3)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT3<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT3)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT4<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT4)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT4<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT4)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT4<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT4)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT4<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT4)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT5<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT5)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT5<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT5)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT5<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT5)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT6<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT6)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT6<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT6)
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyT7<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.IsT7)
            {
                return true;
            }
        }

        return false;
    }

    # endregion

    # region AllT0 - AllT7

    public static bool AllT0<T0>(this IEnumerable<OneOf<T0>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT0<T0, T1>(this IEnumerable<OneOf<T0, T1>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT0<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT0<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT0<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT0<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT0<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT0<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT0)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT1<T0, T1>(this IEnumerable<OneOf<T0, T1>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT1)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT1<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT1)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT1<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT1)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT1<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT1)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT1<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT1)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT1<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT1)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT1<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT1)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT2<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT2)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT2<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT2)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT2<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT2)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT2<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT2)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT2<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT2)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT2<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT2)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT3<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT3)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT3<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT3)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT3<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT3)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT3<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT3)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT3<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT3)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT4<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT4)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT4<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT4)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT4<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT4)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT4<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT4)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT5<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT5)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT5<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT5)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT5<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT5)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT6<T0, T1, T2, T3, T4, T5, T6>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT6)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT6<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT6)
            {
                return false;
            }
        }

        return true;
    }

    public static bool AllT7<T0, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (!item.IsT7)
            {
                return false;
            }
        }

        return true;
    }

    # endregion

    # region OfT0 - OfT7

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

    public static IEnumerable<T0> OfT0<T0, T1>(this IEnumerable<OneOf<T0, T1>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T0> OfT0<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T0> OfT0<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T0> OfT0<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T0> OfT0<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T0> OfT0<T0, T1, T2, T3, T4, T5, T6>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T0> OfT0<T0, T1, T2, T3, T4, T5, T6, T7>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT0(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T1> OfT1<T0, T1>(this IEnumerable<OneOf<T0, T1>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T1> OfT1<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T1> OfT1<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T1> OfT1<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T1> OfT1<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T1> OfT1<T0, T1, T2, T3, T4, T5, T6>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T1> OfT1<T0, T1, T2, T3, T4, T5, T6, T7>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT1(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T2> OfT2<T0, T1, T2>(this IEnumerable<OneOf<T0, T1, T2>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T2> OfT2<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T2> OfT2<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T2> OfT2<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T2> OfT2<T0, T1, T2, T3, T4, T5, T6>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T2> OfT2<T0, T1, T2, T3, T4, T5, T6, T7>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT2(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T3> OfT3<T0, T1, T2, T3>(this IEnumerable<OneOf<T0, T1, T2, T3>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT3(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T3> OfT3<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT3(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T3> OfT3<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT3(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T3> OfT3<T0, T1, T2, T3, T4, T5, T6>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT3(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T3> OfT3<T0, T1, T2, T3, T4, T5, T6, T7>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT3(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T4> OfT4<T0, T1, T2, T3, T4>(this IEnumerable<OneOf<T0, T1, T2, T3, T4>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT4(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T4> OfT4<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT4(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T4> OfT4<T0, T1, T2, T3, T4, T5, T6>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT4(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T4> OfT4<T0, T1, T2, T3, T4, T5, T6, T7>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT4(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T5> OfT5<T0, T1, T2, T3, T4, T5>(this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT5(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T5> OfT5<T0, T1, T2, T3, T4, T5, T6>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT5(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T5> OfT5<T0, T1, T2, T3, T4, T5, T6, T7>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT5(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T6> OfT6<T0, T1, T2, T3, T4, T5, T6>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT6(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T6> OfT6<T0, T1, T2, T3, T4, T5, T6, T7>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT6(out var value, out _))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T7> OfT7<T0, T1, T2, T3, T4, T5, T6, T7>(
        this IEnumerable<OneOf<T0, T1, T2, T3, T4, T5, T6, T7>> source)
    {
        foreach (var item in source)
        {
            if (item.TryPickT7(out var value, out _))
            {
                yield return value;
            }
        }
    }

    # endregion

}