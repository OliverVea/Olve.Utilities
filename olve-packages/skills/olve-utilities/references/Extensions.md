# Extensions

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.CollectionExtensions.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.CollectionExtensions.html)

Source: `src/Olve.Utilities/CollectionExtensions/` and `src/Olve.Utilities/Extensions/`

## DictionaryExtensions

High-performance extensions using `CollectionsMarshal` for zero-overhead dictionary operations. Single-threaded only.

```csharp
public static class DictionaryExtensions
{
    // Get existing value or create and add via factory
    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        where TKey : notnull;

    // Get existing value or add the provided value
    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull;

    // Update value if key exists (direct value)
    public static bool TryUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull;

    // Update value if key exists (transform function)
    public static bool TryUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> update)
        where TKey : notnull;
}
```

## EnumerableExtensions

```csharp
public static class EnumerableExtensions
{
    // Lazy projection (alias for Select)
    public static IEnumerable<TOut> ForEach<TIn, TOut>(
        this IEnumerable<TIn> enumerable, Func<TIn, TOut> action);

    // Eager side-effect iteration
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action);

    // Product of float sequence
    public static float Product(this IEnumerable<float> enumerable);

    // Try cast to IReadOnlySet<T>
    public static bool TryAsReadOnlySet<T>(
        this IEnumerable<T> enumerable, out IReadOnlySet<T>? readOnlySet);

    // Try cast to ISet<T>
    public static bool TryAsSet<T>(this IEnumerable<T> enumerable, out ISet<T>? set);
}
```

## ISetExtensions

```csharp
public static class ISetExtensions
{
    // Toggle: add if missing, remove if present. Returns true if added.
    public static bool Toggle<T>(this ISet<T> set, T item);

    // Set: add if value=true, remove if value=false. Returns true if changed.
    public static bool Set<T>(this ISet<T> set, T item, bool value);
}
```

## RandomExtensions

```csharp
public static class RandomExtensions
{
    // Pick a single random element (throws if empty)
    public static T PickRandom<T>(this IEnumerable<T> source);

    // Pick a single random element or default
    public static T? PickRandomOrDefault<T>(this IEnumerable<T> source);

    // Pick N random elements
    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count);
}
```

## OneOfTryGetExtensions

```csharp
public static class OneOfTryGetExtensions
{
    public static T0 GetT0OrDefault<T0, T1>(this OneOf<T0, T1> oneOf, T0 defaultValue);
    public static T1 GetT1OrDefault<T0, T1>(this OneOf<T0, T1> oneOf, T1 defaultValue);
    public static T0 GetT0OrDefault<T0, T1, T2>(this OneOf<T0, T1, T2> oneOf, T0 defaultValue);
}
```

## EnumerableOneOfExtensions

Generated overloads for checking if any element in a sequence of `OneOf` values matches a specific variant. Methods follow the pattern `AnyT0`, `AnyT1`, etc. for `OneOf` types with up to 8 type parameters.

```csharp
public static class EnumerableOneOfExtensions
{
    public static bool AnyT0<T0>(this IEnumerable<OneOf<T0>> source);
    public static bool AnyT0<T0, T1>(this IEnumerable<OneOf<T0, T1>> source);
    public static bool AnyT1<T0, T1>(this IEnumerable<OneOf<T0, T1>> source);
    // ... overloads up to AnyT7 for OneOf with up to 8 type params
}
```
