namespace Olve.Utilities.Ids;

/// <summary>
/// Generates unique <see cref="uint"/> values in a thread-safe manner.
/// </summary>
/// <remarks>
/// This class ensures that each call to <see cref="Next"/> returns a unique unsigned integer,
/// even when accessed concurrently across multiple threads.
/// </remarks>
public class ThreadSafeUintGenerator
{
    private uint _nextId;

    /// <summary>
    /// Gets the next unique unsigned integer.
    /// </summary>
    /// <returns>
    /// A unique <see cref="uint"/> value that is guaranteed to be greater than all previously returned values
    /// from this instance.
    /// </returns>
    /// <remarks>
    /// Internally uses <see cref="Interlocked.Increment(ref int)"/> to ensure atomicity.
    /// </remarks>
    public uint Next()
    {
        return Interlocked.Increment(ref _nextId);
    }

    /// <summary>
    /// Gets a shared singleton instance of the <see cref="ThreadSafeUintGenerator"/> class.
    /// </summary>
    /// <remarks>
    /// Use this instance when a single shared generator is sufficient, for example,
    /// to generate IDs for a specific type or system-wide context.
    /// </remarks>
    public static readonly ThreadSafeUintGenerator Shared = new();
}