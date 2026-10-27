# Discriminated unions

Generated from `src/typespec-csharp/test/fixtures/widgets.tsp` by the emitter's tests; do not edit.

A `@discriminated(#{ envelope: "none", discriminatorPropertyName })` union becomes an abstract record with `[JsonPolymorphic]` and one `[JsonDerivedType]` per variant; the variants derive from it and lose the discriminator property (the polymorphism writes it). The discriminator may appear anywhere in a request object.

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
@route("/shapes")
interface Shapes {
  @get get(@path id: string): Shape | NotFound;
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
        public override IResult ToHttpResult() => TypedResults.Json(Body, statusCode: 200);
    }

    /// <summary>404.</summary>
    public sealed record NotFound(FixtureErrorEnvelope Body) : ShapesGetResponse
    {
        /// <inheritdoc />
        public override int Status => 404;

        /// <inheritdoc />
        public override IResult ToHttpResult() => TypedResults.Json(Body, statusCode: 404);
    }

    public static implicit operator ShapesGetResponse(Shape body) => new Ok(body);
}
```

## Hand-written handler

A union variant converts implicitly to the `Ok` of a response union that carries the union.

```csharp
private sealed class GetShape : IShapesGetHandler
{
    public Task<ShapesGetResponse> HandleAsync(ShapesGetRequest request, CancellationToken cancellationToken) =>
        Task.FromResult<ShapesGetResponse>(request.Id switch
        {
            "circle" => new Circle { Radius = 1.5 },
            "square" => new Square { Side = 2 },
            _ => new ShapesGetResponse.NotFound(Error("SHAPE_NOT_FOUND", request.Id)),
        });
}
```