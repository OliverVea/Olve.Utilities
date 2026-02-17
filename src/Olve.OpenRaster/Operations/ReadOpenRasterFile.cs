using System.IO.Compression;
using Olve.OpenRaster.Parsing;
using Olve.Operations;
using Olve.Results;

namespace Olve.OpenRaster;

/// <summary>
///     Reads and validates an OpenRaster (.ora) file, extracting necessary metadata.
/// </summary>
public class ReadOpenRasterFile : IOperation<ReadOpenRasterFile.Request, OpenRasterFile>
{
    /// <summary>
    ///     Represents a request to read an OpenRaster file.
    /// </summary>
    /// <param name="FilePath">The absolute or relative path to the OpenRaster (.ora) file.</param>
    public record Request(string FilePath);

    /// <inheritdoc />
    public Result<OpenRasterFile> Execute(Request request)
    {
        if (FilePathValidator.ValidateFilePath(request.FilePath).TryPickProblems(out var problems))
        {
            return problems.Prepend(new ResultProblem("File path failed validation."));
        }

        using var zipArchive = ZipFile.OpenRead(request.FilePath);

        if (StackFileReader.ReadStackXml(zipArchive).TryPickProblems(out problems, out var openRasterFile))
        {
            return problems.Prepend(new ResultProblem("Failed to read stack.xml from OpenRaster file."));
        }

        return openRasterFile;
    }
}