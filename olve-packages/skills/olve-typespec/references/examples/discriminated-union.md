# Discriminated unions

Generated from `src/typespec-csharp/test/fixtures/widgets.tsp` by the emitter's tests; do not edit.

A `@discriminated(#{ envelope: "none", discriminatorPropertyName })` union becomes an abstract record with `[JsonPolymorphic]` and one `[JsonDerivedType]` per variant; the variants derive from it and lose the discriminator property (the polymorphism writes it). The discriminator may appear anywhere in a request object. A handler that receives the union switches on the variant records (`Circle circle => …, Square square => …`); one that answers it returns a variant, which converts implicitly.

## Contract

```typespec
model Circle {
  kind: "circle";
  radius: float64;
}

model Square {
  kind: "square";
  side: float64;
}

/** A shape, discriminated on `kind`. */
@discriminated(#{ envelope: "none", discriminatorPropertyName: "kind" })
union Shape {
  circle: Circle,
  square: Square,
}
```

```typespec
/** The area of a shape. */
model Area {
  value: float64;
}

@route("/shapes")
interface Shapes {
  @get get(@path id: string): Shape | NotFound;

  /** Takes a shape, answers its area: the handler switches on the variant. */
  @route("area") @post area(@body shape: Shape): Area;
}
```

## Generated

```csharp
/// <summary>A shape, discriminated on `kind`.</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(Circle), "circle")]
[JsonDerivedType(typeof(Square), "square")]
public abstract record Shape;

public sealed record Circle : Shape
{
    [JsonPropertyName("radius")]
    public required double Radius { get; init; }
}

public sealed record Square : Shape
{
    [JsonPropertyName("side")]
    public required double Side { get; init; }
}
```

```csharp
/// <summary>Input of <c>Shapes_area</c> (POST /api/shapes/area).</summary>
public sealed record ShapesAreaRequest(Shape Body);

/// <summary>The responses <c>Shapes_area</c> declares; its handler returns one.</summary>
public abstract record ShapesAreaResponse : IOperationResponse
{
    private ShapesAreaResponse()
    {
    }

    /// <inheritdoc />
    public abstract int Status { get; }

    /// <inheritdoc />
    public abstract IResult ToHttpResult();

    /// <summary>200.</summary>
    public sealed record Ok(Area Body) : ShapesAreaResponse
    {
        /// <inheritdoc />
        public override int Status => 200;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<Area>(Body, 200);
    }

    public static implicit operator ShapesAreaResponse(Area body) => new Ok(body);
}

/// <summary>The responses <c>Shapes_get</c> declares; its handler returns one.</summary>
public abstract record ShapesGetResponse : IOperationResponse
{
    private ShapesGetResponse()
    {
    }

    /// <inheritdoc />
    public abstract int Status { get; }

    /// <inheritdoc />
    public abstract IResult ToHttpResult();

    /// <summary>200.</summary>
    public sealed record Ok(Shape Body) : ShapesGetResponse
    {
        /// <inheritdoc />
        public override int Status => 200;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<Shape>(Body, 200);
    }

    /// <summary>404.</summary>
    public sealed record NotFound(FixtureErrorEnvelope Body) : ShapesGetResponse
    {
        /// <inheritdoc />
        public override int Status => 404;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<FixtureErrorEnvelope>(Body, 404);
    }

    public static implicit operator ShapesGetResponse(Shape body) => new Ok(body);
}
```

## Hand-written handlers

`ShapeArea` receives the union and switches on its variants; `GetShape` answers one, a variant converting implicitly to `Ok`.

```csharp
/// <summary>A union in the request: the handler switches on the variant records.</summary>
[OperationHandler]
internal sealed class ShapeArea : IShapesAreaHandler
{
    public Task<ShapesAreaResponse> HandleAsync(ShapesAreaRequest request, CancellationToken cancellationToken)
    {
        var area = request.Body switch
        {
            Circle circle => Math.PI * circle.Radius * circle.Radius,
            Square square => square.Side * square.Side,
            _ => throw new UnreachableException(),
        };
        return Task.FromResult<ShapesAreaResponse>(new Area { Value = area });
    }
}

/// <summary>A union in the response: the stored data decides the variant; it converts implicitly to <c>Ok</c>.</summary>
[OperationHandler]
internal sealed class GetShape : IShapesGetHandler
{
    private static readonly Dictionary<string, Shape> Shapes = new()
    {
        ["c-1"] = new Circle { Radius = 1.5 },
        ["s-1"] = new Square { Side = 2 },
    };

    public Task<ShapesGetResponse> HandleAsync(ShapesGetRequest request, CancellationToken cancellationToken) =>
        Task.FromResult<ShapesGetResponse>(Shapes.TryGetValue(request.Id, out var shape)
            ? shape
            : new ShapesGetResponse.NotFound(Error("SHAPE_NOT_FOUND", request.Id)));
}
```