using Olve.Results;

namespace Olve.OpenRaster;

/// <summary>
/// Defines a parser for extracting and converting a layer from a stream.
/// </summary>
/// <typeparam name="T">The output type of the parsed layer.</typeparam>
public interface ILayerParser<T>
{
    /// <summary>
    /// Parses a layer from a provided stream.
    /// </summary>
    /// <param name="stream">A stream positioned at the beginning of the layer data.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the parsed layer if successful,
    /// or a <see cref="ResultProblem"/> if an error occurs.
    /// </returns>
    Result<T> ParseLayer(Stream stream);
}