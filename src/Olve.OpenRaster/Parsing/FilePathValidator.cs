using Olve.Results;

namespace Olve.OpenRaster.Parsing;

internal static class FilePathValidator
{
    public static Result ValidateFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return new ResultProblem("The file path cannot be null or empty.");
        }

        var fullPath = Path.GetFullPath(filePath);
        if (!File.Exists(fullPath))
        {
            return new ResultProblem("No file was found at path '{0}'.", fullPath);
        }

        return Result.Success();
    }
}