using Olve.Results;

namespace Olve.Logging;

/// <summary>
/// Extension helpers for converting <see cref="ResultProblem"/> instances into logging artifacts.
/// </summary>
public static class ResultProblemExtensions
{
    /// <summary>
    /// Convert a <see cref="ResultProblem"/> into a <see cref="LogMessage"/> suitable for logging.
    /// The resulting message will be logged at <see cref="LogLevel.Error"/> and will include origin information and tags where available.
    /// </summary>
    /// <param name="resultProblem">The problem to convert.</param>
    /// <returns>A <see cref="LogMessage"/> representing the provided problem.</returns>
    public static LogMessage ToLogMessage(this ResultProblem resultProblem)
    {
        return new LogMessage(LogLevel.Error, 
            resultProblem.ToBriefString(), 
            resultProblem.OriginInformation.FilePath, 
            resultProblem.OriginInformation.LineNumber, 
            DateTime.Now, 
            resultProblem.Tags);
    }
}
