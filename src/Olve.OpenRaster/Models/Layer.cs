namespace Olve.OpenRaster;

/// <summary>
/// Represents a layer in an Open Raster file.
/// See <a href="https://www.openraster.org/baseline/layer-stack-spec.html#layer-element">Open Raster Layer Element</a>.
/// </summary>
public class Layer
{
    /// <summary>
    /// The name of the layer.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The source path or identifier of the layer's image data within the OpenRaster file.
    /// </summary>
    public required string Source { get; set; }

    /// <summary>
    /// The compositing operation that determines how the layer blends with layers beneath it.
    /// </summary>
    public CompositeOperation CompositeOperation { get; set; } = CompositeOperation.SrcOver;

    /// <summary>
    /// The opacity of the layer (0.0 = fully transparent, 1.0 = fully opaque).
    /// </summary>
    public float Opacity { get; set; } = 1.0f;

    /// <summary>
    /// Whether the layer is visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// The X position of the layer relative to the canvas.
    /// </summary>
    public int X { get; set; } = 0;

    /// <summary>
    /// The Y position of the layer relative to the canvas.
    /// </summary>
    public int Y { get; set; } = 0;

    /// <summary>
    /// The groups (stacks) that the layer belongs to.
    /// </summary>
    public IList<Group> Groups { get; set; } = [];
}