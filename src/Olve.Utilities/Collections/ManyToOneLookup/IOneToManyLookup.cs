namespace Olve.Utilities.Collections;

/// <summary>
///     Represents a one-to-many lookup between two sets of items.
/// </summary>
/// <typeparam name="TLeft">The type of the left-hand elements.</typeparam>
/// <typeparam name="TRight">The type of the right-hand elements.</typeparam>
public interface IOneToManyLookup<TLeft, TRight> : IEnumerable<KeyValuePair<TLeft, IReadOnlySet<TRight>>>
    where TLeft : notnull
    where TRight : notnull
{
    /// <summary>
    ///     Gets all left-hand elements in the lookup.
    /// </summary>
    IEnumerable<TLeft> Lefts { get; }

    /// <summary>
    ///     Gets all right-hand elements in the lookup.
    /// </summary>
    IEnumerable<TRight> Rights { get; }

    /// <summary>
    ///     Determines whether the specified pair exists in the lookup.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <param name="right">The right-hand element.</param>
    /// <returns><see langword="true" /> if the pair exists; otherwise, <see langword="false" />.</returns>
    bool Contains(TLeft left, TRight right);

    /// <summary>
    ///     Gets the right-hand elements associated with the specified left-hand element.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <returns>A set of right-hand elements if found; otherwise, <see cref="NotFound" />.</returns>
    OneOf<IReadOnlySet<TRight>, NotFound> Get(TLeft left);

    /// <summary>
    ///     Gets the left-hand element associated with the specified right-hand element.
    /// </summary>
    /// <param name="right">The right-hand element.</param>
    /// <returns>The left-hand element if found; otherwise, <see cref="NotFound" />.</returns>
    OneOf<TLeft, NotFound> Get(TRight right);

    /// <summary>
    ///     Sets the mapping for the specified left-hand element to the provided right-hand elements.
    ///     Overwrites any existing mappings.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <param name="rights">The set of right-hand elements to associate.</param>
    void Set(TLeft left, ISet<TRight> rights);

    /// <summary>
    ///     Adds or removes a pair to/from the lookup.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <param name="right">The right-hand element.</param>
    /// <param name="value"><see langword="true" /> to add the pair; <see langword="false" /> to remove it.</param>
    /// <returns>
    ///     <see langword="true" /> if the operation made changes to the lookup; otherwise, <see langword="false" />.
    /// </returns>
    bool Set(TLeft left, TRight right, bool value);

    /// <summary>
    ///     Clears all mappings in the lookup.
    /// </summary>
    void Clear();

    /// <summary>
    ///     Removes all mappings for the specified left-hand element.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <returns><see langword="true" /> if mappings were removed; otherwise, <see langword="false" />.</returns>
    bool Remove(TLeft left);

    /// <summary>
    ///     Removes the mapping for the specified right-hand element.
    /// </summary>
    /// <param name="right">The right-hand element.</param>
    /// <returns><see langword="true" /> if the mapping was removed; otherwise, <see langword="false" />.</returns>
    bool Remove(TRight right);

    /// <summary>
    ///     Removes the specified pair from the lookup.
    /// </summary>
    /// <param name="left">The left-hand element.</param>
    /// <param name="right">The right-hand element.</param>
    /// <returns><see langword="true" /> if the pair was removed; otherwise, <see langword="false" />.</returns>
    bool Remove(TLeft left, TRight right);
}
