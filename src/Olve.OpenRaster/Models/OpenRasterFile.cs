namespace Olve.OpenRaster;

/// <summary>
/// Represents the content of an OpenRaster (.ora) file.
/// See <a href="https://www.openraster.org/baseline/layer-stack-spec.html">OpenRaster Stack File</a>.
/// </summary>
public class OpenRasterFile
{
    /// <summary>
    /// The OpenRaster format version.
    /// </summary>
    public required string Version { get; set; }

    /// <summary>
    /// The width of the image in pixels.
    /// </summary>
    public required int Width { get; set; }

    /// <summary>
    /// The height of the image in pixels.
    /// </summary>
    public required int Height { get; set; }

    /// <summary>
    /// The horizontal resolution (DPI) of the image.
    /// Defaults to 72 DPI.
    /// </summary>
    public int XResolution { get; set; } = 72;

    /// <summary>
    /// The vertical resolution (DPI) of the image.
    /// Defaults to 72 DPI.
    /// </summary>
    public int YResolution { get; set; } = 72;

    /// <summary>
    /// The layers in the OpenRaster file.
    /// </summary>
    public IReadOnlyList<Layer> Layers { get; set; } = [];

    /// <summary>
    /// The groups in the OpenRaster file.
    /// </summary>
    public IReadOnlyList<Group> Groups { get; set; } = [];
}