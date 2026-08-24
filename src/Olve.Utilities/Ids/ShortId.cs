using System.Diagnostics.CodeAnalysis;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Ids;

/// <summary>
/// A dense 32-bit identifier, type-safe in the same way <see cref="Id{T}"/> is but four bytes
/// instead of sixteen.
/// <para>
/// Use it where identity is <em>local and hot</em>: values allocated by one store, iterated every
/// frame, and written to a wire format that has a <c>uint32</c> id field. Use <see cref="Id{T}"/>
/// where identity is <em>durable and global</em> — anything that must survive a restart, be
/// referenced across services, or appear in a URL.
/// </para>
/// <para>
/// <b>Scope:</b> a <see cref="ShortId{T}"/> is unique only within the store that issued it, and only
/// for the lifetime of that store's id sequence. It carries no meaning anywhere else.
/// </para>
/// <para>
/// <b>Allocation:</b> the issuing store owns the sequence and must allocate monotonically. Ids must
/// never be recycled — reusing the value of a departed entity aliases a stale client reference onto
/// a live one. A sequence that outlives the process must be persisted, or recomputed as
/// <c>max + 1</c> on load.
/// </para>
/// </summary>
/// <typeparam name="T">The logical entity type, used only for compile-time safety.</typeparam>
public readonly record struct ShortId<T>(uint Value) : IComparable<ShortId<T>>
{
    /// <summary>The unallocated value. No entity is ever issued this id.</summary>
    public static ShortId<T> None => new(0);

    /// <summary>Whether this identifier refers to an entity at all.</summary>
    public bool IsSome => Value != 0;

    /// <inheritdoc />
    public int CompareTo(ShortId<T> other) => Value.CompareTo(other.Value);

    /// <summary>Returns whether the left identifier sorts before the right.</summary>
    public static bool operator <(ShortId<T> left, ShortId<T> right) => left.Value < right.Value;

    /// <summary>Returns whether the left identifier sorts after the right.</summary>
    public static bool operator >(ShortId<T> left, ShortId<T> right) => left.Value > right.Value;

    /// <summary>Returns whether the left identifier sorts before or equal to the right.</summary>
    public static bool operator <=(ShortId<T> left, ShortId<T> right) => left.Value <= right.Value;

    /// <summary>Returns whether the left identifier sorts after or equal to the right.</summary>
    public static bool operator >=(ShortId<T> left, ShortId<T> right) => left.Value >= right.Value;

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <summary>Returns a representation including the logical type name, such as <c>ShortId&lt;Slime&gt;(7)</c>.</summary>
    public string ToDisplayString() => $"ShortId<{typeof(T).Name}>({Value})";
}
