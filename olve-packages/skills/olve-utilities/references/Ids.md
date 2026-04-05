# Ids

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Ids.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Ids.html)

Source: `src/Olve.Utilities/Ids/`

## Id

Opaque, type-agnostic identifier backed by a `Guid`. Implements `IComparable<Id>`.

```csharp
public readonly record struct Id(Guid Value) : IComparable<Id>
{
    public Guid Value { get; }
    public static Id New();
    public static Id<T> New<T>();
    public static Id FromName(string name, Id? namespaceId = null);
    public static Id<T> FromName<T>(string name, Id? namespaceId = null);
    public static bool TryParse(string text, out Id id);
    public static bool TryParse<T>(string text, out Id<T> id);
    public int CompareTo(Id other);
    public override string ToString();
    public string ToDisplayString();

    public static bool operator <(Id left, Id right);
    public static bool operator >(Id left, Id right);
    public static bool operator <=(Id left, Id right);
    public static bool operator >=(Id left, Id right);
}
```

- `New()` / `New<T>()` -- random GUID-based ID.
- `FromName` / `FromName<T>` -- deterministic UUIDv5 from a string name and optional namespace.
- `TryParse` / `TryParse<T>` -- parse a GUID string into an Id.

## Id\<T\>

Lightweight, type-safe wrapper around `Id`. The type parameter `T` is used only for compile-time safety.

```csharp
public readonly record struct Id<T> : IComparable<Id<T>>
{
    public Id Value { get; }
    public Id(Id value);
    public int CompareTo(Id<T> other);
    public override string ToString();
    public string ToDisplayString();

    public static bool operator <(Id<T> left, Id<T> right);
    public static bool operator >(Id<T> left, Id<T> right);
    public static bool operator <=(Id<T> left, Id<T> right);
    public static bool operator >=(Id<T> left, Id<T> right);
}
```

## UnionId\<T1, T2\>

Tagged union of `Id<T1>` and `Id<T2>`. Requires `T1 != T2` (enforced at static constructor time).

```csharp
public readonly record struct UnionId<T1, T2> : IComparable<UnionId<T1, T2>>
{
    public bool IsT1 { get; }
    public bool IsT2 { get; }

    public static UnionId<T1, T2> FromT1(Id<T1> id);
    public static UnionId<T1, T2> FromT2(Id<T2> id);

    public static implicit operator UnionId<T1, T2>(Id<T1> id);
    public static implicit operator UnionId<T1, T2>(Id<T2> id);

    public Id<T1> AsT1();
    public Id<T2> AsT2();

    public bool TryGetT1(out Id<T1> value, out Id<T2> remainder);
    public bool TryGetT2(out Id<T2> value, out Id<T1> remainder);

    public TResult Match<TResult>(Func<Id<T1>, TResult> whenT1, Func<Id<T2>, TResult> whenT2);
    public void Switch(Action<Id<T1>> whenT1, Action<Id<T2>> whenT2);

    public int CompareTo(UnionId<T1, T2> other);
}
```

- Implicit conversions from `Id<T1>` and `Id<T2>`.
- `TryGetT1` / `TryGetT2` return the held variant and provide the other as `remainder`.
- `Match` / `Switch` for exhaustive pattern matching.
