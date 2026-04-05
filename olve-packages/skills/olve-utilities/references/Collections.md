# Collections

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Collections.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Collections.html)

Source: `src/Olve.Utilities/Collections/`

## IReadOnlyBidirectionalDictionary\<T1, T2\>

```csharp
public interface IReadOnlyBidirectionalDictionary<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
    where T1 : notnull where T2 : notnull
{
    bool IsSynced { get; }
    int Count { get; }
    IReadOnlyCollection<T1> FirstValues { get; }
    IReadOnlyCollection<T2> SecondValues { get; }
    bool Contains(T1 first);
    bool Contains(T2 second);
    bool TryGet(T1 first, out T2 second);
    bool TryGet(T2 second, out T1 first);
}
```

## IBidirectionalDictionary\<T1, T2\>

Extends `IReadOnlyBidirectionalDictionary<T1, T2>`.

```csharp
public interface IBidirectionalDictionary<T1, T2> : IReadOnlyBidirectionalDictionary<T1, T2>
    where T1 : notnull where T2 : notnull
{
    bool TryAdd(T1 first, T2 second);
    void Set(T1 first, T2 second);
    bool TryRemove(T1 first);
    bool TryRemove(T2 second);
    void Clear();
}
```

## BidirectionalDictionary\<T1, T2\>

Implements `IBidirectionalDictionary<T1, T2>`. Two-way lookup backed by two internal dictionaries.

```csharp
public class BidirectionalDictionary<T1, T2> : IBidirectionalDictionary<T1, T2>
    where T1 : notnull where T2 : notnull
{
    public BidirectionalDictionary(
        IEnumerable<KeyValuePair<T1, T2>>? collection = null,
        IEqualityComparer<T1>? firstComparer = null,
        IEqualityComparer<T2>? secondComparer = null);
}
```

## ReadOnlyBidirectionalDictionary\<T1, T2\>

Implements `IReadOnlyBidirectionalDictionary<T1, T2>`.

```csharp
public class ReadOnlyBidirectionalDictionary<T1, T2> : IReadOnlyBidirectionalDictionary<T1, T2>
    where T1 : notnull where T2 : notnull
{
    public ReadOnlyBidirectionalDictionary(IEnumerable<KeyValuePair<T1, T2>> collection);
}
```

## IOneToManyLookup\<TLeft, TRight\>

One-to-many relationship. Forward lookup returns `IReadOnlySet<TRight>`, reverse lookup returns single `TLeft`.

```csharp
public interface IOneToManyLookup<TLeft, TRight> : IEnumerable<KeyValuePair<TLeft, IReadOnlySet<TRight>>>
    where TLeft : notnull where TRight : notnull
{
    IEnumerable<TLeft> Lefts { get; }
    IEnumerable<TRight> Rights { get; }
    bool Contains(TLeft left, TRight right);
    bool TryGet(TLeft left, out IReadOnlySet<TRight>? rights);
    bool TryGet(TRight right, out TLeft left);
    void Set(TLeft left, ISet<TRight> rights);
    bool Set(TLeft left, TRight right, bool value);
    void Clear();
    bool Remove(TLeft left);
    bool Remove(TRight right);
    bool Remove(TLeft left, TRight right);
}
```

## OneToManyLookup\<TLeft, TRight\>

Implements `IOneToManyLookup<TLeft, TRight>`.

```csharp
public class OneToManyLookup<TLeft, TRight> : IOneToManyLookup<TLeft, TRight>
    where TLeft : notnull where TRight : notnull
{
    public OneToManyLookup(
        IEqualityComparer<TLeft>? leftComparer = null,
        IEqualityComparer<TRight>? rightComparer = null);
}
```

## IManyToManyLookup\<TLeft, TRight\>

Many-to-many relationship. Both directions return `IReadOnlySet`.

```csharp
public interface IManyToManyLookup<TLeft, TRight> : IEnumerable<KeyValuePair<TLeft, TRight>>
    where TLeft : notnull where TRight : notnull
{
    IEnumerable<TLeft> Lefts { get; }
    IEnumerable<TRight> Rights { get; }
    bool Contains(TLeft left, TRight right);
    bool TryGet(TLeft left, out IReadOnlySet<TRight>? rights);
    bool TryGet(TRight right, out IReadOnlySet<TLeft>? lefts);
    void Set(TLeft left, ISet<TRight> rights);
    void Set(TRight right, ISet<TLeft> lefts);
    bool Set(TLeft left, TRight right, bool value);
    bool Set(TRight right, TLeft left, bool value);
    void Clear();
    bool Remove(TLeft left);
    bool Remove(TRight right);
}
```

## ManyToManyLookup\<TLeft, TRight\>

Implements `IManyToManyLookup<TLeft, TRight>`.

```csharp
public class ManyToManyLookup<TLeft, TRight> : IManyToManyLookup<TLeft, TRight>
    where TLeft : notnull where TRight : notnull
{
    public ManyToManyLookup(
        IEnumerable<KeyValuePair<TLeft, TRight>>? initialItems = null,
        IEqualityComparer<TLeft>? leftComparer = null,
        IEqualityComparer<TRight>? rightComparer = null);
}
```

## IQueue\<T\>

```csharp
public interface IQueue<T> : IEnumerable<T>
{
    void Enqueue(T item);
    bool TryDequeue(out T item);
    bool TryPeek(out T item);
    int Count { get; }
    void Clear();
}
```

## FixedSizeQueue\<T\>

Implements `IQueue<T>`. Automatically drops oldest items when `maxSize` is exceeded.

```csharp
public class FixedSizeQueue<T> : IQueue<T>
{
    public FixedSizeQueue(int maxSize);
}
```
