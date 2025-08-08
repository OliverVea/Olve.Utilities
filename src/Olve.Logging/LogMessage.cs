using System.Text;
using System.Text.Json.Serialization;
using Olve.MinimalApi;
using Olve.Paths;

namespace Olve.Logging;

/// <summary>
/// Represents a single log message with metadata such as source path, line and tags.
/// </summary>
/// <param name="Level">Severity of the log.</param>
/// <param name="Message">The textual message.</param>
/// <param name="SourcePath">Optional source file path associated with the log.</param>
/// <param name="SourceLine">Optional source line number associated with the log.</param>
/// <param name="Time">Timestamp when the log was created.</param>
/// <param name="Tags">Optional tags associated with the log.</param>
public sealed record LogMessage(
    LogLevel Level,
    string Message,
    [property: JsonConverter(typeof(PathJsonConverter))]
    IPath? SourcePath,
    int? SourceLine,
    DateTime Time,
    string[]? Tags
)
{
    internal string SearchableText { get; } = GetSearchableText(Message, SourcePath, Tags);

    private static string GetSearchableText(string text, IPath? sourcePath, IEnumerable<string>? tags)
    {
        StringBuilder sb = new(text);
        sb.Append(' ');

        if (sourcePath is not null)
        {
            sb.Append(sourcePath.Path);
            sb.Append(' ');
        }
        
        if (tags is not null)
        {
            foreach (var tag in tags)
            {
                sb.Append(tag);
                sb.Append(' ');
            }
        }
        
        return sb.ToString();
    }

    /// <summary>
    /// Returns the log message text.
    /// </summary>
    public override string ToString()
    {
        return Message;
    }
}
