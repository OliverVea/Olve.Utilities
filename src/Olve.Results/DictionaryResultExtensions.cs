namespace Olve.Results;

/// <summary>
///     Extension methods for dictionary operations that return <see cref="Result"/> instead of throwing.
/// </summary>
public static class DictionaryResultExtensions
{
    /// <summary>
    ///     Attempts to add a key-value pair to the dictionary, returning a <see cref="Result"/> indicating success or failure.
    /// </summary>
    /// <param name="dictionary">The dictionary to add to.</param>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add.</param>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <returns>A successful result if the key was added; a failure if the key already exists.</returns>
    public static Result SetWithResult<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
        where TKey : notnull
    {
        if (!dictionary.TryAdd(key, value))
        {
            return new ResultProblem("Key '{0}' already exists", key);
        }

        return Result.Success();
    }

    /// <summary>
    ///     Attempts to retrieve the value associated with the specified key, returning a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="dictionary">The dictionary to look up.</param>
    /// <param name="key">The key to look up.</param>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <returns>A successful result with the value if the key was found; a failure if the key was not found.</returns>
    public static Result<TValue> GetWithResult<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary,
        TKey key)
        where TKey : notnull
    {
        return dictionary.TryGetValue(key, out var value)
            ? Result.Success(value)
            : new ResultProblem("Could not find value for key '{0}'", key);
    }
}
