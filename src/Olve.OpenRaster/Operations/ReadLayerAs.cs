using System.IO.Compression;
using Olve.OpenRaster.Parsing;
using Olve.Operations;
using Olve.Results;

namespace Olve.OpenRaster;

/// <summary>
///     Loads and parses a layer into a specified type.
/// </summary>
/// <typeparam name="T">The type to parse the layer as.</typeparam>
public class ReadLayerAs<T> : IOperation<ReadLayerAs<T>.Request, T>
{
    /// <summary>
    ///     Request to load and parse a layer from an OpenRaster (.ora) file.
    /// </summary>
    /// <param name="FilePath">The absolute or relative path to the OpenRaster (.ora) file.</param>
    /// <param name="LayerSource">The identifier of the layer image to retrieve.</param>
    /// <param name="LayerParser">The parser responsible for converting the image data into the output type.</param>
    public record Request(string FilePath, string LayerSource, ILayerParser<T> LayerParser);

    /// <inheritdoc />
    public Result<T> Execute(Request request)
    {
        if (FilePathValidator
            .ValidateFilePath(request.FilePath)
            .TryPickProblems(out var problems))
        {
            return problems.Prepend(new ResultProblem("File path '{0}' failed validation.", request.FilePath));
        }

        using var zipArchive = ZipFile.OpenRead(request.FilePath);

        if (LayerImageReader
            .ReadImageFile(zipArchive, request.LayerSource, request.LayerParser)
            .TryPickProblems(out problems, out var data))
        {
            return problems.Prepend(new ResultProblem(
                "Could not read layer '{0}' from OpenRaster file '{1}'.",
                request.LayerSource,
                request.FilePath));
        }

        return data;
    }
}
