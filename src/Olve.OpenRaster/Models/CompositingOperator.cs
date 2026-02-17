namespace Olve.OpenRaster;

/// <summary>
/// The compositing operator to use when compositing layers.
/// See the <a href="https://www.openraster.org/baseline/layer-stack-spec.html#composite-op-attribute">OpenRaster specification</a>.
/// </summary>
public enum CompositingOperator
{
    /// <summary>
    /// The source-over compositing operator.
    /// See <a href="http://www.w3.org/TR/compositing-1/#porterduffcompositingoperators_srcover">W3C Compositing and Blending</a>.
    /// </summary>
    SourceOver,

    /// <summary>
    /// The source-in compositing operator.
    /// See <a href="https://www.w3.org/TR/compositing-1/#porterduffcompositingoperators_plus">W3C Compositing and Blending</a>.
    /// </summary>
    Lighter,

    /// <summary>
    /// The source-out compositing operator.
    /// See <a href="http://www.w3.org/TR/compositing-1/#porterduffcompositingoperators_dstin">W3C Compositing and Blending</a>.
    /// </summary>
    DestinationIn,

    /// <summary>
    /// The destination-over compositing operator.
    /// See <a href="http://www.w3.org/TR/compositing-1/#porterduffcompositingoperators_dstout">W3C Compositing and Blending</a>.
    /// </summary>
    DestinationOut,

    /// <summary>
    /// The destination-in compositing operator.
    /// See <a href="http://www.w3.org/TR/compositing-1/#porterduffcompositingoperators_srcatop">W3C Compositing and Blending</a>.
    /// </summary>
    SourceAtop,


    /// <summary>
    /// The destination-atop compositing operator.
    /// See <a href="http://www.w3.org/TR/compositing-1/#porterduffcompositingoperators_dstatop">W3C Compositing and Blending</a>.
    /// </summary>
    DestinationAtop
}