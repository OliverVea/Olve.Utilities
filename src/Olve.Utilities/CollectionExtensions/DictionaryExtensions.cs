using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Olve.Utilities.CollectionExtensions;

/// <summary>
/// Provides a set of static methods for querying and modifying objects that implement <see cref="IDictionary{TKey,TValue}"/>.
/// </summary>
/// <remarks>
/// These methods utilize <see cref="CollectionsMarshal.GetValueRefOrAddDefault{TKey,TValue}"/> and related APIs, which provide direct references to dictionary values.
/// They should only be used in single-threaded contexts, as concurrent modifications can result in undefined behavior.
/// </remarks>
public static class DictionaryExtensions
{
    /// <summary>
    /// Gets the value associated with the specified key or adds a new value if the key does not exist.
    /// </summary>
    /// <remarks>
    /// This method provides a direct reference to the dictionary's internal storage and should be used only in single-threaded scenarios.
    /// </remarks>
    /// <param name="dictionary">The dictionary to get or add the value from.</param>
    /// <param name="key">The key of the value to get or add.</param>
    /// <param name="valueFactory">The function used to generate a new value for the key.</param>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <returns>The value associated with the specified key or the newly added value.</returns>
    [SuppressMessage(
        "Design",
        "MA0016:Prefer using collection abstraction instead of implementation"
    )]
    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> valueFactory
    )
        where TKey : notnull
    {
        ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(
            dictionary,
            key,
            out var exists
        );
        if (exists)
        {
            return value!;
        }

        value = valueFactory();
        return value;
    }

    /// <summary>
    /// Gets the value associated with the specified key or adds a new value if the key does not exist.
    /// </summary>
    /// <remarks>
    /// This method should be used in single-threaded contexts due to its direct manipulation of dictionary storage.
    /// </remarks>
    /// <param name="dictionary">The dictionary to get or add the value from.</param>
    /// <param name="key">The key of the value to get or add.</param>
    /// <param name="value">The value to add if the key does not exist.</param>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <returns>The value associated with the specified key or the newly added value.</returns>
    [SuppressMessage(
        "Design",
        "MA0016:Prefer using collection abstraction instead of implementation"
    )]
    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value
    )
        where TKey : notnull
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(
            dictionary,
            key,
            out var exists
        );
        if (exists)
        {
            return val!;
        }

        val = value;
        return value;
    }

    /// <summary>
    /// Updates the value associated with the specified key if the key exists.
    /// </summary>
    /// <remarks>
    /// This method should be used in single-threaded scenarios as it provides a direct reference to the dictionary's internal storage.
    /// </remarks>
    /// <param name="dictionary">The dictionary in which to update the value.</param>
    /// <param name="key">The key of the value to update.</param>
    /// <param name="value">The new value to set.</param>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <returns><see langword="true"/> if the value was updated; otherwise, <see langword="false"/>.</returns>
    [SuppressMessage(
        "Design",
        "MA0016:Prefer using collection abstraction instead of implementation"
    )]
    public static bool TryUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value
    )
        where TKey : notnull
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
        if (Unsafe.IsNullRef(ref val))
        {
            return false;
        }

        val = value;
        return true;
    }

    /// <summary>
    /// Updates the value associated with the specified key using a transformation function if the key exists.
    /// </summary>
    /// <remarks>
    /// This method should be used only in single-threaded contexts, as it modifies dictionary values directly.
    /// </remarks>
    /// <param name="dictionary">The dictionary in which to update the value.</param>
    /// <param name="key">The key of the value to update.</param>
    /// <param name="update">The function used to update the existing value.</param>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <returns><see langword="true"/> if the value was updated; otherwise, <see langword="false"/>.</returns>
    [SuppressMessage(
        "Design",
        "MA0016:Prefer using collection abstraction instead of implementation"
    )]
    public static bool TryUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue, TValue> update
    )
        where TKey : notnull
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
        if (Unsafe.IsNullRef(ref val))
        {
            return false;
        }

        val = update(val);
        return true;
    }
}
