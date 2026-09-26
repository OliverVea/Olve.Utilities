# ResultProblem

https://olivervea.github.io/Olve.Utilities/api/Olve.Results.ResultProblem.html

Class (not sealed). Represents a problem encountered during an operation. Subclass it for typed problems (see below).

Constructors:
- `ResultProblem(string message, params object[] args)` — message: format string. args: format arguments.
- `ResultProblem(Exception exception, string message, params object[] args)` — exception: causing exception.
- `ResultProblem(string formattedMessage, string[]? tags, int severity, string? source, string? exceptionSummary)` — `[JsonConstructor]` for deserialization; no args, exception or origin.

Static fields:
- `string? DefaultSource` — default source for new problems
- `string[] DefaultTags` — default tags for new problems
- `int DefaultSeverity` — default severity for new problems
- `bool DefaultPrintDebug` — controls ToString() format

Properties:
- `string Message` — raw format string `[JsonIgnore]`
- `string FormattedMessage` — `Message` formatted with `Args`; serialized as `message`
- `object[] Args` — format arguments `[JsonIgnore]`
- `string[] Tags` — categorization tags (init-settable)
- `int Severity` — severity level, higher = more severe (init-settable)
- `bool IsRetryable` — transient failure that may succeed if retried (init-settable). Defaults to `true` for problems created from an exception, `false` otherwise. Serialized; payloads without it infer `ExceptionSummary != null`
- `string? Source` — problem source (init-settable)
- `Exception? Exception` — causing exception `[JsonIgnore]`
- `string? ExceptionSummary` — `"{ExceptionType}: {Message}"`, survives serialization
- `ProblemOriginInformation OriginInformation` — auto-captured file/line `[JsonIgnore]`

Methods:
- `string ToString()` — uses ToDebugString() if DefaultPrintDebug, else ToBriefString()
- `string ToBriefString()` — formatted message, omits code locations. Includes exception type and message if present.
- `string ToDebugString()` — includes code location as clickable link prefix

Typed problems:

```csharp
public sealed class InsufficientFundsProblem(decimal shortfall)
    : ResultProblem("Insufficient funds: short by {0}", shortfall)
{
    public decimal Shortfall => shortfall;
}

if (result.TryPickProblem<InsufficientFundsProblem>(out var funds)) { /* funds.Shortfall */ }
```

Matching is by assignability (`OfType`), so picking a base problem type also returns its subclasses.

ProblemOriginInformation:
- `readonly record struct ProblemOriginInformation(IPath FilePath, int LineNumber)`
- `string LinkString` — clickable link string for the current platform
