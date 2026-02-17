namespace Olve.OpenRaster;

/// <summary>
/// The blending function to use when blending layers.
/// See the <a href="https://www.openraster.org/baseline/layer-stack-spec.html#composite-op-attribute">OpenRaster specification</a>.
/// </summary>
public enum BlendingFunction
{
    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendingnormal">W3C Compositing and Blending</a>.
    /// </summary>
    Normal,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendingmultiply">W3C Compositing and Blending</a>.
    /// </summary>
    Multiply,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendingscreen">W3C Compositing and Blending</a>.
    /// </summary>
    Screen,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendingoverlay">W3C Compositing and Blending</a>.
    /// </summary>
    Overlay,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendingdarken">W3C Compositing and Blending</a>.
    /// </summary>
    Darken,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendinglighten">W3C Compositing and Blending</a>.
    /// </summary>
    Lighten,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendingcolor-dodge">W3C Compositing and Blending</a>.
    /// </summary>
    ColorDodge,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendingcolor-burn">W3C Compositing and Blending</a>.
    /// </summary>
    ColorBurn,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendinghard-light">W3C Compositing and Blending</a>.
    /// </summary>
    HardLight,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendingsoft-light">W3C Compositing and Blending</a>.
    /// </summary>
    SoftLight,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendingdifference">W3C Compositing and Blending</a>.
    /// </summary>
    Difference,

    /// <summary>
    ///    See <a href="http://www.w3.org/TR/compositing-1/#blendingcolor">W3C Compositing and Blending</a>.
    /// </summary>
    Color,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendingluminosity">W3C Compositing and Blending</a>.
    /// </summary>
    Luminosity,

    /// <summary>
    ///     See <a href="http://www.w3.org/TR/compositing-1/#blendinghue">W3C Compositing and Blending</a>.
    /// </summary>
    Hue,

    /// <summary>
    ///     See <a href="https://www.w3.org/TR/compositing-1/#blendingsaturation">W3C Compositing and Blending</a>.
    /// </summary>
    Saturation
}