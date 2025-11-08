using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Olve.Utilities.Ids;

/// <summary>
/// A tagged union of <see cref="Id{T1}"/> and <see cref="Id{T2}"/> that shares a <see cref="uint"/> payload.
/// </summary>
/// <typeparam name="T1">The first identifier type.</typeparam>
/// <typeparam name="T2">The second identifier type.</typeparam>
[DebuggerDisplay("{ToString()}")]
[StructLayout(LayoutKind.Auto)]
public readonly record struct UnionId<T1, T2> : IComparable<UnionId<T1, T2>>
{
    // 0 => T1, 1 => T2
    private readonly byte _tag;
    private readonly Id _value;

    static UnionId()
    {
        if (typeof(T1) == typeof(T2))
        {
            throw new InvalidOperationException("UnionId<T1,T2> requires T1 != T2.");
        }
    }

    private UnionId(Id value, byte tag)
    {
        if (tag > 1) throw new ArgumentOutOfRangeException(nameof(tag));
        _value = value;
        _tag = tag;
    }

    /// <summary>
    /// Gets a value indicating whether the union currently holds an <see cref="Id{T1}"/>.
    /// </summary>
    public bool IsT1 => _tag == 0;

    /// <summary>
    /// Gets a value indicating whether the union currently holds an <see cref="Id{T2}"/>.
    /// </summary>
    public bool IsT2 => _tag == 1;

    /// <summary>
    /// Creates a new <see cref="UnionId{T1, T2}"/> containing the given <see cref="Id{T1}"/>.
    /// </summary>
    /// <param name="id">The identifier of type <typeparamref name="T1"/> to wrap.</param>
    /// <returns>A union containing the specified identifier.</returns>
    public static UnionId<T1, T2> FromT1(Id<T1> id) => new(id.Value, 0);

    /// <summary>
    /// Creates a new <see cref="UnionId{T1, T2}"/> containing the given <see cref="Id{T2}"/>.
    /// </summary>
    /// <param name="id">The identifier of type <typeparamref name="T2"/> to wrap.</param>
    /// <returns>A union containing the specified identifier.</returns>
    public static UnionId<T1, T2> FromT2(Id<T2> id) => new(id.Value, 1);

    /// <summary>
    /// Implicitly converts an <see cref="Id{T1}"/> to a <see cref="UnionId{T1, T2}"/>.
    /// </summary>
    /// <param name="id">The identifier to convert.</param>
    public static implicit operator UnionId<T1, T2>(Id<T1> id) => FromT1(id);

    /// <summary>
    /// Implicitly converts an <see cref="Id{T2}"/> to a <see cref="UnionId{T1, T2}"/>.
    /// </summary>
    /// <param name="id">The identifier to convert.</param>
    public static implicit operator UnionId<T1, T2>(Id<T2> id) => FromT2(id);

    /// <summary>
    /// Returns the contained <see cref="Id{T1}"/> without checking the current variant.
    /// </summary>
    /// <remarks>
    /// This method does not validate whether the union actually holds an <see cref="Id{T1}"/>.
    /// If it does not, the result will be <c>default</c>.
    /// Use <see cref="IsT1"/> or <see cref="TryGetT1"/> for safe access.
    /// </remarks>
    public Id<T1> AsT1() => _tag == 0 ? new Id<T1>(_value) : default;

    /// <summary>
    /// Returns the contained <see cref="Id{T2}"/> without checking the current variant.
    /// </summary>
    /// <remarks>
    /// This method does not validate whether the union actually holds an <see cref="Id{T2}"/>.
    /// If it does not, the result will be <c>default</c>.
    /// Use <see cref="IsT2"/> or <see cref="TryGetT2"/> for safe access.
    /// </remarks>
    public Id<T2> AsT2() => _tag == 1 ? new Id<T2>(_value) : default;

    /// <summary>
    /// Attempts to extract the value as <see cref="Id{T1}"/>.
    /// </summary>
    /// <param name="value">
    /// When this method returns <c>true</c>, contains the contained <see cref="Id{T1}"/>.
    /// When it returns <c>false</c>, this parameter is set to the default value.
    /// </param>
    /// <param name="remainder">
    /// When this method returns <c>false</c>, contains the current <see cref="Id{T2}"/> value.
    /// When it returns <c>true</c>, this parameter is set to the default value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the union currently holds an <see cref="Id{T1}"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool TryGetT1(out Id<T1> value, out Id<T2> remainder)
    {
        if (_tag == 0)
        {
            value = new Id<T1>(_value);
            remainder = default;
            return true;
        }

        value = default;
        remainder = new Id<T2>(_value);
        return false;
    }

    /// <summary>
    /// Attempts to extract the value as <see cref="Id{T2}"/>.
    /// </summary>
    /// <param name="value">
    /// When this method returns <c>true</c>, contains the contained <see cref="Id{T2}"/>.
    /// When it returns <c>false</c>, this parameter is set to the default value.
    /// </param>
    /// <param name="remainder">
    /// When this method returns <c>false</c>, contains the current <see cref="Id{T1}"/> value.
    /// When it returns <c>true</c>, this parameter is set to the default value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the union currently holds an <see cref="Id{T2}"/>; otherwise, <c>false</c>.
    /// </returns>
    public bool TryGetT2(out Id<T2> value, out Id<T1> remainder)
    {
        if (_tag == 1)
        {
            value = new Id<T2>(_value);
            remainder = default;
            return true;
        }

        value = default;
        remainder = new Id<T1>(_value);
        return false;
    }

    /// <inheritdoc/>
    public int CompareTo(UnionId<T1, T2> other)
    {
        var tagCmp = _tag.CompareTo(other._tag);
        return tagCmp != 0 ? tagCmp : _value.CompareTo(other._value);
    }

    /// <summary>Determines whether the first union is less than the second.</summary>
    public static bool operator <(UnionId<T1, T2> a, UnionId<T1, T2> b) => a.CompareTo(b) < 0;

    /// <summary>Determines whether the first union is greater than the second.</summary>
    public static bool operator >(UnionId<T1, T2> a, UnionId<T1, T2> b) => a.CompareTo(b) > 0;

    /// <summary>Determines whether the first union is less than or equal to the second.</summary>
    public static bool operator <=(UnionId<T1, T2> a, UnionId<T1, T2> b) => a.CompareTo(b) <= 0;

    /// <summary>Determines whether the first union is greater than or equal to the second.</summary>
    public static bool operator >=(UnionId<T1, T2> a, UnionId<T1, T2> b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// Executes one of the provided functions depending on which variant the union contains
    /// and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the functions.</typeparam>
    /// <param name="whenT1">The function to execute if the union contains <see cref="Id{T1}"/>.</param>
    /// <param name="whenT2">The function to execute if the union contains <see cref="Id{T2}"/>.</param>
    /// <returns>The result of executing the matching function.</returns>
    public TResult Match<TResult>(Func<Id<T1>, TResult> whenT1, Func<Id<T2>, TResult> whenT2)
        => _tag == 0 ? whenT1(new Id<T1>(_value)) : whenT2(new Id<T2>(_value));

    /// <summary>
    /// Executes one of the provided actions depending on which variant the union contains.
    /// </summary>
    /// <param name="whenT1">The action to execute if the union contains <see cref="Id{T1}"/>.</param>
    /// <param name="whenT2">The action to execute if the union contains <see cref="Id{T2}"/>.</param>
    public void Switch(Action<Id<T1>> whenT1, Action<Id<T2>> whenT2)
    {
        if (_tag == 0) whenT1(new Id<T1>(_value));
        else whenT2(new Id<T2>(_value));
    }

    /// <inheritdoc/>
    public override string ToString()
        => _tag == 0
            ? $"UnionId<{typeof(T1).Name},{typeof(T2).Name}>:T1({_value})"
            : $"UnionId<{typeof(T1).Name},{typeof(T2).Name}>:T2({_value})";
}
