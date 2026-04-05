# Other Utilities

Source: various subdirectories under `src/Olve.Utilities/`

## IAsyncOnStartup

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.AsyncOnStartup.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.AsyncOnStartup.html)

Priority-based async startup task interface. Register implementations as `IAsyncOnStartup` in DI. Tasks with the same priority run concurrently; groups execute in ascending priority order.

```csharp
public interface IAsyncOnStartup
{
    int Priority => 0;
    Task OnStartupAsync(CancellationToken cancellationToken = default);
}
```

### ServiceProviderExtensions

```csharp
public static class ServiceProviderExtensions
{
    // Runs all IAsyncOnStartup services in priority order
    public static ValueTask RunAsyncOnStartup(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default);
}
```

## IBuilder\<T\>

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Builders.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Builders.html)

Builder pattern interface with optional validation integration.

```csharp
public interface IBuilder<out T>
{
    T Build();
}

public static class BuilderExtensions
{
    // Build and validate in one step
    public static Result<T> ValidateAndBuild<T, TValidator>(
        this IBuilder<T> builder, TValidator validator)
        where TValidator : IValidator<T>;
}
```

## ProjectFileNameHelper

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Projects.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Projects.html)

Generates structured file names from elements with a delimiter and extension.

```csharp
public class ProjectFileNameHelper(string? delimiter = null, string? extension = null)
{
    public string FileNameDelimiter { get; }
    public string FileExtension { get; }
    public string GetFileName(IEnumerable<string> elements);
    public IEnumerable<string> GetElements(string fileName,
        StringComparison comparisonType = StringComparison.Ordinal);
}
```

## ProjectFolderHelper

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Projects.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Projects.html)

Manages project folder paths with OS-aware defaults (XDG_DATA_HOME on Linux, CommonApplicationData on Windows).

```csharp
public class ProjectFolderHelper(
    string projectName,
    string organization = "Olve",
    Environment.SpecialFolder? specialFolder = null)
{
    public string ProjectName { get; }
    public string Organization { get; }
    public Environment.SpecialFolder? SpecialFolder { get; }
    public IPath RootDirectory { get; set; }
    public IPath ProjectRootFolder { get; }  // RootDirectory / Organization / ProjectName
}
```

## DateTimeFormatter

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.StringFormatting.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.StringFormatting.html)

Human-readable relative time formatting.

```csharp
public static class DateTimeFormatter
{
    // Returns e.g. "2 days ago", "1 hour ago", "just now"
    public static string FormatTimeAgo(DateTimeOffset now, DateTimeOffset then);
}
```

## IntegerMath2D

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.IntegerMath2D.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.IntegerMath2D.html)

Integer-based 2D math types.

### Position

```csharp
public readonly record struct Position(int X, int Y)
{
    public static DeltaPosition operator -(Position a, Position b);
    public static Position operator +(Position a, DeltaPosition b);
    public static implicit operator Position((int X, int Y) tuple);
}
```

### Size

```csharp
public readonly record struct Size(int Width, int Height)
{
    public static implicit operator Size((int Width, int Height) tuple);
}
```

### DeltaPosition

```csharp
public readonly record struct DeltaPosition(int X, int Y)
{
    public static DeltaPosition operator +(DeltaPosition a, DeltaPosition b);
    public static DeltaPosition operator -(DeltaPosition a, DeltaPosition b);
    public static DeltaPosition operator *(DeltaPosition a, int scalar);
}
```

## Assert

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Assertions.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Assertions.html)

Debug-only assertion methods. All methods are annotated with `[Conditional("DEBUG")]`.

```csharp
public static class Assert
{
    public static void That(Func<bool> assertion, string message);
    public static void NotNull<T>(T? value, string message = "Value cannot be null.") where T : class;
    public static void IsEmpty<T>(IEnumerable<T> collection, string message = "Collection should be empty.");
    public static void IsNotEmpty<T>(IEnumerable<T> collection, string message = "Collection should not be empty.");
}

public class AssertionError(string message) : Exception(message);
```

## FrozenLookupBase\<TKey, TValue\>

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Lookup.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Lookup.html)

Immutable lookup backed by `FrozenDictionary`.

```csharp
public class FrozenLookupBase<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items)
    : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    public IReadOnlyCollection<TKey> Keys { get; }
    public IReadOnlyCollection<TValue> Values { get; }
    public IReadOnlyCollection<KeyValuePair<TKey, TValue>> Items { get; }
    public int Count { get; }
    public bool ContainsKey(TKey key);
    public bool ContainsValue(TValue value);  // O(n)
    public bool TryGetValue(TKey key, out TValue value);
}
```

## IdFrozenLookup\<T, TId\>

Extends `FrozenLookupBase` to map items by their `Id` property.

```csharp
public class IdFrozenLookup<T, TId>(IEnumerable<T> items)
    : FrozenLookupBase<TId, T>
    where T : IHasId<TId>
    where TId : notnull;
```

### IHasId\<TId\>

```csharp
public interface IHasId<out TId> where TId : notnull
{
    TId Id { get; }
}
```
