using Olve.Results;

namespace Olve.OpenRaster;

/// <summary>
/// Represents a composite operation.
/// See the <a href="https://www.openraster.org/baseline/layer-stack-spec.html#composite-op-attribute">OpenRaster specification</a>.
/// </summary>
/// <param name="Key">The key of the composite operation in the stack XML.</param>
/// <param name="BlendingFunction">The blending function to use when blending layers.</param>
/// <param name="CompositingOperator">The compositing operator to use when blending layers.</param>
public readonly record struct CompositeOperation(
   string Key,
   BlendingFunction BlendingFunction,
   CompositingOperator CompositingOperator)
{
   /// <summary>
   /// The source-over composite operation.
   /// </summary>
   public static CompositeOperation SrcOver => new("svg:src-over", BlendingFunction.Normal, CompositingOperator.SourceOver);

   /// <summary>
   /// The multiply composite operation.
   /// </summary>
   public static CompositeOperation Multiply => new("svg:multiply", BlendingFunction.Multiply, CompositingOperator.SourceOver);

   /// <summary>
   /// The screen composite operation.
   /// </summary>
   public static CompositeOperation Screen => new("svg:screen", BlendingFunction.Screen, CompositingOperator.SourceOver);

   /// <summary>
   /// The overlay composite operation.
   /// </summary>
   public static CompositeOperation Overlay => new("svg:overlay", BlendingFunction.Overlay, CompositingOperator.SourceOver);

   /// <summary>
   /// The darken composite operation.
   /// </summary>
   public static CompositeOperation Darken => new("svg:darken", BlendingFunction.Darken, CompositingOperator.SourceOver);

   /// <summary>
   /// The lighten composite operation.
   /// </summary>
   public static CompositeOperation Lighten => new("svg:lighten", BlendingFunction.Lighten, CompositingOperator.SourceOver);

   /// <summary>
   /// The color-dodge composite operation.
   /// </summary>
   public static CompositeOperation ColorDodge => new("svg:color-dodge", BlendingFunction.ColorDodge, CompositingOperator.SourceOver);

   /// <summary>
   /// The color-burn composite operation.
   /// </summary>
   public static CompositeOperation ColorBurn => new("svg:color-burn", BlendingFunction.ColorBurn, CompositingOperator.SourceOver);

   /// <summary>
   /// The hard-light composite operation.
   /// </summary>
   public static CompositeOperation HardLight => new("svg:hard-light", BlendingFunction.HardLight, CompositingOperator.SourceOver);

   /// <summary>
   /// The soft-light composite operation.
   /// </summary>
   public static CompositeOperation SoftLight => new("svg:soft-light", BlendingFunction.SoftLight, CompositingOperator.SourceOver);

   /// <summary>
   /// The difference composite operation.
   /// </summary>
   public static CompositeOperation Difference => new("svg:difference", BlendingFunction.Difference, CompositingOperator.SourceOver);

   /// <summary>
   /// The color composite operation.
   /// </summary>
   public static CompositeOperation Color => new("svg:color", BlendingFunction.Color, CompositingOperator.SourceOver);

   /// <summary>
   /// The luminosity composite operation.
   /// </summary>
   public static CompositeOperation Luminosity => new("svg:luminosity", BlendingFunction.Luminosity, CompositingOperator.SourceOver);

   /// <summary>
   /// The hue composite operation.
   /// </summary>
   public static CompositeOperation Hue => new("svg:hue", BlendingFunction.Hue, CompositingOperator.SourceOver);

   /// <summary>
   /// The saturation composite operation.
   /// </summary>
   public static CompositeOperation Saturation => new("svg:saturation", BlendingFunction.Saturation, CompositingOperator.SourceOver);

   /// <summary>
   /// The plus composite operation.
   /// </summary>
   public static CompositeOperation Plus => new("svg:plus", BlendingFunction.Normal, CompositingOperator.Lighter);

   /// <summary>
   /// The destination-in composite operation.
   /// </summary>
   public static CompositeOperation DestinationIn => new("svg:dst-in", BlendingFunction.Normal, CompositingOperator.DestinationIn);

   /// <summary>
   /// The destination-out composite operation.
   /// </summary>
   public static CompositeOperation DestinationOut => new("svg:dst-out", BlendingFunction.Normal, CompositingOperator.DestinationOut);

   /// <summary>
   /// The source-atop composite operation.
   /// </summary>
   public static CompositeOperation SourceAtop => new("svg:src-atop", BlendingFunction.Normal, CompositingOperator.SourceAtop);

   /// <summary>
   /// The destination-atop composite operation.
   /// </summary>
   public static CompositeOperation DestinationAtop => new("svg:dst-atop", BlendingFunction.Normal, CompositingOperator.DestinationAtop);

   /// <summary>
   /// Creates a composite operation from a key.
   /// </summary>
   /// <param name="key">The key of the composite operation.</param>
   /// <returns>The composite operation.</returns>
   public static Result<CompositeOperation> FromKey(string key)
   {
      return key switch
      {
         "svg:src-over" => SrcOver,
         "svg:multiply" => Multiply,
         "svg:screen" => Screen,
         "svg:overlay" => Overlay,
         "svg:darken" => Darken,
         "svg:lighten" => Lighten,
         "svg:color-dodge" => ColorDodge,
         "svg:color-burn" => ColorBurn,
         "svg:hard-light" => HardLight,
         "svg:soft-light" => SoftLight,
         "svg:difference" => Difference,
         "svg:color" => Color,
         "svg:luminosity" => Luminosity,
         "svg:hue" => Hue,
         "svg:saturation" => Saturation,
         "svg:plus" => Plus,
         "svg:dst-in" => DestinationIn,
         "svg:dst-out" => DestinationOut,
         "svg:src-atop" => SourceAtop,
         "svg:dst-atop" => DestinationAtop,
         _ => new ResultProblem("unknown composite operation key: {0}", key)
      };
   }
}