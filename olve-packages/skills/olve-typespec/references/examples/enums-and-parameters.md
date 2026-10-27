# Enums and parameters

Generated from `src/typespec-csharp/test/fixtures/widgets.tsp` by the emitter's tests; do not edit.

String enums serialize by wire value and bind from the query by wire value (`?color=red`), numeric enums by number. Parameters bind from path, query and header; a `@query(#{ explode: false })` string list binds from one comma-separated value.

## Contract

```typespec
enum Color {
  Red: "red",
  /** The default. */
  Green: "green",
}

enum Priority {
  Low: 1,
  High: 2,
}
```

```typespec
@route("/tags")
interface Tags {
  /** Echoes a required `explode: false` list (`?names=a,b`). */
  @get echo(@query(#{ explode: false }) names: string[]): string[];
}
```

## Generated

```csharp
[JsonConverter(typeof(JsonStringEnumConverter<Color>))]
public enum Color
{
    [JsonStringEnumMemberName("red")]
    Red,
    /// <summary>The default.</summary>
    [JsonStringEnumMemberName("green")]
    Green,
}

public enum Priority
{
    Low = 1,
    High = 2,
}
```

```csharp
WidgetsList = app.MapGet("/api/widgets", async (
        [FromServices] IWidgetsListHandler handler,
        [FromQuery(Name = "color")] string? color,
        [FromQuery(Name = "limit")] int? limit,
        [FromHeader(Name = "x-request-id")] string? requestId,
        CancellationToken ct) =>
        (await handler.HandleAsync(new WidgetsListRequest(color is null ? null : FixtureParameters.ParseColor(color, "color"), limit, requestId), ct)).ToHttpResult())
    .WithName("Widgets_list")
    .WithMetadata(FixtureOperations.WidgetsList)
    .Produces<IReadOnlyList<Widget>>(200),

TagsEcho = app.MapGet("/api/tags", async (
        [FromServices] ITagsEchoHandler handler,
        [FromQuery(Name = "names")] string names,
        CancellationToken ct) =>
        (await handler.HandleAsync(new TagsEchoRequest(QueryParameters.List(names)), ct)).ToHttpResult())
    .WithName("Tags_echo")
    .WithMetadata(FixtureOperations.TagsEcho)
    .Produces<IReadOnlyList<string>>(200),
```

```csharp
/// <summary>Parses string-enum parameters by their wire values (minimal APIs only know member names).</summary>
internal static class FixtureParameters
{
    public static Color ParseColor(string value, string parameter) => value switch
    {
        "red" => Color.Red,
        "green" => Color.Green,
        _ => throw new BadHttpRequestException($"Failed to bind parameter \"{parameter}\" from \"{value}\"."),
    };
}
```

## Hand-written handlers

```csharp
private sealed class ListWidgets : IWidgetsListHandler
{
    public Task<WidgetsListResponse> HandleAsync(WidgetsListRequest request, CancellationToken cancellationToken)
    {
        var labels = new Dictionary<string, string>();
        if (request.Color is { } color)
        {
            labels["color"] = color.ToString();
        }

        if (request.Limit is { } limit)
        {
            labels["limit"] = limit.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        if (request.RequestId is { } requestId)
        {
            labels["requestId"] = requestId;
        }

        return Task.FromResult<WidgetsListResponse>(new WidgetsListResponse.Ok([SampleWidget(labels)]));
    }
}
```

```csharp
private sealed class EchoTags : ITagsEchoHandler
{
    public Task<TagsEchoResponse> HandleAsync(TagsEchoRequest request, CancellationToken cancellationToken) =>
        Task.FromResult<TagsEchoResponse>(new TagsEchoResponse.Ok(request.Names));
}
```