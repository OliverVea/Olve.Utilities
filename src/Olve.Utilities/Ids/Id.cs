using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Olve.Utilities.Ids;

/// <summary>
/// Represents an opaque, type-agnostic identifier backed by a <see cref="Guid"/>.
/// </summary>
[DebuggerDisplay("{ToDisplayString()}")]
public readonly record struct Id(Guid Value) : IComparable<Id>
{
    /// <summary>
    /// Gets the underlying <see cref="Guid"/> value for this identifier.
    /// </summary>
    public Guid Value { get; } = Value;

    /// <summary>
    /// Creates a new, randomly generated <see cref="Id"/>.
    /// </summary>
    /// <returns>A new <see cref="Id"/> whose underlying <see cref="Guid"/> was generated with <see cref="Guid.NewGuid"/>.</returns>
    public static Id New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a new, randomly generated typed identifier.
    /// </summary>
    /// <typeparam name="T">The logical entity type for the returned identifier.</typeparam>
    /// <returns>A new <see cref="Id{T}"/> whose underlying <see cref="Id"/> is randomly generated.</returns>
    public static Id<T> New<T>() => new(New());

    /// <summary>
    /// Generates a deterministic <see cref="Id"/> from a textual name and an optional namespace.
    /// </summary>
    /// <param name="name">The non-null name used to derive the identifier.</param>
    /// <param name="namespaceId">An optional namespace <see cref="Id"/> that scopes the result; if <c>null</c> a default/empty namespace is used.</param>
    /// <returns>A deterministic <see cref="Id"/> that is stable for the same <paramref name="name"/> and <paramref name="namespaceId"/>.</returns>
    /// <remarks>The method is stateless and thread-safe. Internally it hashes the namespace and name and produces a UUIDv5-compatible GUID.</remarks>
    public static Id FromName(string name, Id? namespaceId = null)
    {
        var nsGuid = (namespaceId ?? default(Id)).Value;
        return new Id(DeterministicGuidFromName(nsGuid, name));
    }

    /// <summary>
    /// Generates a deterministic typed identifier from a textual name and an optional namespace.
    /// </summary>
    /// <typeparam name="T">The logical entity type for the returned identifier.</typeparam>
    /// <param name="name">The non-null name used to derive the identifier.</param>
    /// <param name="namespaceId">An optional namespace <see cref="Id"/> that scopes the result; if <c>null</c> a default/empty namespace is used.</param>
    /// <returns>A deterministic <see cref="Id{T}"/> that is stable for the same <paramref name="name"/> and <paramref name="namespaceId"/>.</returns>
    /// <remarks>The method is stateless and thread-safe. Internally it hashes the namespace and name and produces a UUIDv5-compatible GUID.</remarks>
    public static Id<T> FromName<T>(string name, Id? namespaceId = null)
    {
        return new Id<T>(FromName(name, namespaceId));
    }

    /// <summary>
    /// Attempts to parse a textual representation of an identifier into an <see cref="Id"/>.
    /// </summary>
    /// <param name="text">The string to parse (typically a GUID string).</param>
    /// <param name="id">When the method returns, contains the parsed <see cref="Id"/> if parsing succeeded; otherwise the default value.</param>
    /// <returns><c>true</c> if <paramref name="text"/> was parsed successfully; otherwise <c>false</c>.</returns>
    public static bool TryParse(string text, out Id id)
    {
        id = default;
        if (string.IsNullOrWhiteSpace(text)) return false;
        if (Guid.TryParse(text, out var g))
        {
            id = new Id(g);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempts to parse a textual representation of an identifier into a typed <see cref="Id{T}"/>.
    /// </summary>
    /// <typeparam name="T">The logical entity type for the parsed identifier.</typeparam>
    /// <param name="text">The string to parse (typically a GUID string).</param>
    /// <param name="id">When the method returns, contains the parsed <see cref="Id{T}"/> if parsing succeeded; otherwise the default value.</param>
    /// <returns><c>true</c> if <paramref name="text"/> was parsed successfully; otherwise <c>false</c>.</returns>
    public static bool TryParse<T>(string text, out Id<T> id)
    {
        if (TryParse(text, out var rawId))
        {
            id = new Id<T>(rawId);
            return true;
        }
        id = default;
        return false;
    }

    /// <summary>
    /// Returns the canonical string representation of the underlying <see cref="Guid"/>.
    /// </summary>
    /// <returns>The <see cref="Guid"/> value represented as a string.</returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Returns the canonical string representation of the underlying <see cref="Guid"/>.
    /// </summary>
    /// <returns>The <see cref="Guid"/> value represented as a string.</returns>
    public string ToDisplayString() => $"Id({Value})";

    /// <summary>
    /// Compares this instance with another <see cref="Id"/> for ordering.
    /// </summary>
    /// <param name="other">The other identifier to compare to.</param>
    /// <returns>
    /// A signed integer that indicates the relative order:
    /// less than zero if this instance precedes <paramref name="other"/>,
    /// zero if equal,
    /// greater than zero if it follows.
    /// </returns>
    public int CompareTo(Id other) => Value.CompareTo(other.Value);

    /// <summary>
    /// Returns a value indicating whether the left operand is less than the right operand.
    /// </summary>
    public static bool operator <(Id left, Id right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Returns a value indicating whether the left operand is greater than the right operand.
    /// </summary>
    public static bool operator >(Id left, Id right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Returns a value indicating whether the left operand is less than or equal to the right operand.
    /// </summary>
    public static bool operator <=(Id left, Id right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Returns a value indicating whether the left operand is greater than or equal to the right operand.
    /// </summary>
    public static bool operator >=(Id left, Id right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Computes a deterministic GUID from a namespace GUID and a name string.
    /// </summary>
    /// <param name="namespaceGuid">The namespace GUID providing contextual scoping.</param>
    /// <param name="name">The name used to deterministically derive the identifier.</param>
    /// <returns>A GUID derived from the combination of namespace and name.</returns>
    /// <remarks>
    /// The algorithm uses SHA-256 to hash the namespace GUID and name bytes together,
    /// then uses the first 16 bytes of the hash to form a valid UUIDv5-compliant GUID.
    /// </remarks>
    private static Guid DeterministicGuidFromName(Guid namespaceGuid, string name)
    {
        var nsBytes = namespaceGuid.ToByteArray();
        var nameBytes = Encoding.UTF8.GetBytes(name);

        using (var sha = SHA256.Create())
        {
            sha.TransformBlock(nsBytes, 0, nsBytes.Length, null, 0);
            sha.TransformFinalBlock(nameBytes, 0, nameBytes.Length);
            var hash = sha.Hash!;

            var guidBytes = new byte[16];
            Array.Copy(hash, 0, guidBytes, 0, 16);

            // Set version (UUIDv5)
            guidBytes[6] = (byte)((guidBytes[6] & 0x0F) | (5 << 4));
            // Set variant (RFC 4122)
            guidBytes[8] = (byte)((guidBytes[8] & 0x3F) | 0x80);

            return new Guid(guidBytes);
        }
    }
}

/// <summary>
/// Represents a lightweight, type-safe wrapper around an <see cref="Id"/> to avoid mixing identifiers of different logical types.
/// </summary>
/// <typeparam name="T">The logical entity type represented by this identifier (used only for compile-time safety).</typeparam>
[DebuggerDisplay("{ToDisplayString()}")]
public readonly record struct Id<T> : IComparable<Id<T>>
{
    /// <summary>
    /// Gets the underlying untyped <see cref="Id"/> value held by this typed identifier.
    /// </summary>
    public Id Value { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Id{T}"/> with the specified untyped identifier.
    /// </summary>
    /// <param name="value">The underlying <see cref="Id"/> value.</param>
    public Id(Id value) => Value = value;

    /// <summary>
    /// Returns a human-readable representation of this typed identifier, including the logical type name and value.
    /// </summary>
    /// <returns>A string such as <c>Id&lt;TName&gt;(value)</c>.</returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Returns a human-readable representation of this typed identifier, including the logical type name and value.
    /// </summary>
    /// <returns>A string such as <c>Id&lt;TName&gt;(value)</c>.</returns>
    public string ToDisplayString() => $"Id<{typeof(T).Name}>({Value})";

    /// <summary>
    /// Compares this instance with another <see cref="Id{T}"/> for ordering.
    /// </summary>
    /// <param name="other">The other typed identifier to compare to.</param>
    /// <returns>
    /// A signed integer that indicates the relative order:
    /// less than zero if this instance precedes <paramref name="other"/>,
    /// zero if equal,
    /// greater than zero if it follows.
    /// </returns>
    public int CompareTo(Id<T> other) => Value.CompareTo(other.Value);

    /// <summary>
    /// Returns a value indicating whether the left typed identifier is less than the right typed identifier.
    /// </summary>
    public static bool operator <(Id<T> left, Id<T> right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Returns a value indicating whether the left typed identifier is greater than the right typed identifier.
    /// </summary>
    public static bool operator >(Id<T> left, Id<T> right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Returns a value indicating whether the left typed identifier is less than or equal to the right typed identifier.
    /// </summary>
    public static bool operator <=(Id<T> left, Id<T> right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Returns a value indicating whether the left typed identifier is greater than or equal to the right typed identifier.
    /// </summary>
    public static bool operator >=(Id<T> left, Id<T> right) => left.CompareTo(right) >= 0;
}
