# ResultProblem

https://olivervea.github.io/Olve.Utilities/api/Olve.Results.ResultProblem.html

Class. Represents a problem encountered during an operation.

Constructors:
- `ResultProblem(string message, params object[] args)` — message: format string. args: format arguments.
- `ResultProblem(Exception exception, string message, params object[] args)` — exception: causing exception.

Static fields:
- `string? DefaultSource` — default source for new problems
- `string[] DefaultTags` — default tags for new problems
- `int DefaultSeverity` — default severity for new problems
- `bool DefaultPrintDebug` — controls ToString() format

Properties:
- `string Message` — raw format string
- `object[] Args` — format arguments
- `string[] Tags` — categorization tags (init-settable)
- `int Severity` — severity level, higher = more severe (init-settable)
- `string? Source` — problem source (init-settable)
- `Exception? Exception` — causing exception
- `ProblemOriginInformation OriginInformation` — auto-captured file/line

Methods:
- `string ToString()` — uses ToDebugString() if DefaultPrintDebug, else ToBriefString()
- `string ToBriefString()` — formatted message, omits code locations. Includes exception type and message if present.
- `string ToDebugString()` — includes code location as clickable link prefix

ProblemOriginInformation:
- `readonly record struct ProblemOriginInformation(IPath FilePath, int LineNumber)`
- `string LinkString` — clickable link string for the current platform
