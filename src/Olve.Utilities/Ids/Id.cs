using System.Diagnostics;
using Olve.Results;

namespace Olve.Utilities.Ids;

/// <summary>
/// Represents a strongly-typed, immutable identifier for a given type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type this identifier is associated with, providing type safety.</typeparam>
/// <remarks>
/// This struct is commonly used to distinguish between different identifier types that may share the same underlying
/// primitive type but should not be mixed, such as customer id and order id.
/// </remarks>
[DebuggerDisplay("{ToString()}")]
public readonly record struct Id<T> : IComparable<Id<T>>
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly ThreadSafeUintGenerator ThreadSafeUintGenerator = new();

    /// <summary>
    /// Gets the underlying <see cref="uint"/> value of the identifier.
    /// </summary>
    /// <value>The raw numeric value representing the identifier.</value>
    public uint Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Id{T}"/> struct with a specified value.
    /// </summary>
    /// <param name="value">The numeric value to assign to this identifier.</param>
    public Id(uint value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Id{T}"/> with a unique value.
    /// </summary>
    /// <returns>A new <see cref="Id{T}"/> whose value is guaranteed to be unique for this type.</returns>
    /// <remarks>
    /// This method uses a thread-safe generator to ensure unique values across threads.
    /// </remarks>
    public static Id<T> New() => new(ThreadSafeUintGenerator.Next());

    /// <summary>
    /// Compares the current identifier to another <see cref="Id{T}"/> instance.
    /// </summary>
    /// <param name="other">The other identifier to compare against.</param>
    /// <returns>
    /// A signed integer indicating the relative values:
    /// less than zero if this instance is less than <paramref name="other"/>,
    /// zero if they are equal, and greater than zero if this instance is greater than <paramref name="other"/>.
    /// </returns>
    public int CompareTo(Id<T> other)
    {
        return Value.CompareTo(other.Value);
    }

    /// <summary>
    /// Determines whether the first identifier is less than or equal to the second.
    /// </summary>
    /// <param name="left">The first identifier.</param>
    /// <param name="right">The second identifier.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    public static bool operator <=(Id<T> left, Id<T> right) => left.Value <= right.Value;

    /// <summary>
    /// Determines whether the first identifier is greater than or equal to the second.
    /// </summary>
    /// <param name="left">The first identifier.</param>
    /// <param name="right">The second identifier.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    public static bool operator >=(Id<T> left, Id<T> right) => left.Value >= right.Value;

    /// <summary>
    /// Determines whether the first identifier is greater than the second.
    /// </summary>
    /// <param name="left">The first identifier.</param>
    /// <param name="right">The second identifier.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    public static bool operator >(Id<T> left, Id<T> right) => left.Value > right.Value;

    /// <summary>
    /// Determines whether the first identifier is less than the second.
    /// </summary>
    /// <param name="left">The first identifier.</param>
    /// <param name="right">The second identifier.</param>
    /// <returns><c>true</c> if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    public static bool operator <(Id<T> left, Id<T> right) => left.Value < right.Value;

    /// <summary>
    /// Returns a string that represents the current identifier.
    /// </summary>
    /// <returns>A string in the format <c>Id&lt;TypeName&gt;(Value)</c>.</returns>
    public override string ToString()
    {
        return $"Id<{nameof(T)}>({Value})";
    }

    /// <summary>
    /// Attempts to parse an <see cref="Id{T}"/> from the provided span of chars.
    /// </summary>
    /// <param name="text">The text to parse into an id.</param>
    /// <returns>The parsed Id on a success, a failed result on failure.</returns>
    public static Result<Id<T>> Parse(string text)
    {
        if (uint.TryParse(text, out var parsedId))
        {
            return new Id<T>(parsedId);
        }

        return new ResultProblem("Could not parse '{0}' as id", text);
    }
}
