using System.Diagnostics;

namespace Olve.TypeSpec.Conformance;

/// <summary>
/// Trivial handlers for the fixture's operations. They echo what they received (so tests can
/// check parameter binding without shared state) and answer magic ids with a declared error, so
/// tests can drive every response variant:
/// <list type="bullet">
///   <item><c>missing</c>: 404.</item>
///   <item><c>conflict</c>: 409.</item>
///   <item><c>invalid</c>: 400 from the handler (not the validator).</item>
///   <item><c>queued</c> (a create's serial): 202 instead of 201.</item>
/// </list>
/// </summary>
public static class FixtureHandlers
{
    public const string Missing = "missing";
    public const string Conflict = "conflict";
    public const string Invalid = "invalid";
    public const string Queued = "queued";

    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IWidgetsListHandler, ListWidgets>();
        services.AddSingleton<IWidgetsCreateHandler, CreateWidget>();
        services.AddSingleton<IWidgetsUpdateHandler, UpdateWidget>();
        services.AddSingleton<IWidgetsDeleteHandler, DeleteWidget>();
        services.AddSingleton<IShapesGetHandler, GetShape>();
        services.AddSingleton<IShapesAreaHandler, ShapeArea>();
        services.AddSingleton<IWidgetEventsStreamHandler, StreamWidgetEvents>();
        services.AddSingleton<ITagsEchoHandler, EchoTags>();
        services.AddSingleton<IFilesGetHandler, GetFile>();
        services.AddSingleton<IFilesHeadHandler, HeadFile>();
        services.AddSingleton<INoticesGetHandler, GetNotice>();
        services.AddSingleton<IDeliveriesReceiveHandler, ReceiveDelivery>();
    }

    /// <summary>A file's content: its path, so tests can see what the route bound.</summary>
    public static byte[] FileContent(string path) => System.Text.Encoding.UTF8.GetBytes($"content of {path}");

    public const string FileContentType = "text/x-fixture";
    public const int FileRevision = 3;
    public const int MaxDeliveryBytes = 1024;

    public static FixtureError Error(string code, string id) => FixtureError.Create(code, $"'{id}' failed with {code}.");

    public static Widget SampleWidget(IReadOnlyDictionary<string, string>? labels = null) => new()
    {
        Id = Guid.NewGuid(),
        Serial = "SN-1",
        Name = "sample",
        Priority = Priority.High,
        Description = null,
        CreatedAt = DateTimeOffset.UnixEpoch,
        Shape = new Circle { Radius = 1 },
        Labels = labels,
    };

    #region list
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

    #endregion

    #region create
    private sealed class CreateWidget : IWidgetsCreateHandler
    {
        public Task<WidgetsCreateResponse> HandleAsync(WidgetsCreateRequest request, CancellationToken cancellationToken)
        {
            var body = request.Body;
            if (body.Serial == Invalid)
            {
                return Task.FromResult<WidgetsCreateResponse>(new WidgetsCreateResponse.BadRequest(Error("WIDGET_INVALID", body.Serial)));
            }

            var widget = new Widget
            {
                Id = Guid.NewGuid(),
                Serial = body.Serial,
                Name = body.Name,
                Color = body.Color,
                Priority = body.Priority,
                Description = body.Description,
                Parent = body.Parent,
                Tags = body.Tags,
                Labels = body.Labels,
                Weight = body.Weight,
                CreatedAt = body.CreatedAt,
                Shape = body.Shape,
            };
            return Task.FromResult<WidgetsCreateResponse>(body.Serial == Queued
                ? new WidgetsCreateResponse.Accepted(widget)
                : new WidgetsCreateResponse.Created(widget));
        }
    }

    #endregion

    #region update
    private sealed class UpdateWidget : IWidgetsUpdateHandler
    {
        public Task<WidgetsUpdateResponse> HandleAsync(WidgetsUpdateRequest request, CancellationToken cancellationToken)
        {
            WidgetsUpdateResponse? failure = request.Id switch
            {
                Missing => new WidgetsUpdateResponse.NotFound(Error("WIDGET_NOT_FOUND", request.Id)),
                Conflict => new WidgetsUpdateResponse.Conflict(Error("WIDGET_LOCKED", request.Id)),
                Invalid => new WidgetsUpdateResponse.BadRequest(Error("WIDGET_INVALID", request.Id)),
                _ => null,
            };
            if (failure is not null)
            {
                return Task.FromResult(failure);
            }

            var body = request.Body;
            var labels = new Dictionary<string, string> { ["id"] = request.Id };
            if (request.DryRun is { } dryRun)
            {
                labels["dryRun"] = dryRun ? "true" : "false";
            }

            if (body.Note is { } note)
            {
                labels["note"] = note;
            }

            // A success body converts implicitly to its variant (Ok).
            return Task.FromResult<WidgetsUpdateResponse>(SampleWidget(labels) with
            {
                Serial = body.Serial,
                Name = body.Name,
                Description = body.Description,
                Shape = body.Shape,
            });
        }
    }

    #endregion

    #region delete
    private sealed class DeleteWidget : IWidgetsDeleteHandler
    {
        public Task<WidgetsDeleteResponse> HandleAsync(WidgetsDeleteRequest request, CancellationToken cancellationToken) =>
            Task.FromResult<WidgetsDeleteResponse>(request.Id == Missing
                ? new WidgetsDeleteResponse.NotFound(Error("WIDGET_NOT_FOUND", request.Id))
                : new WidgetsDeleteResponse.NoContent());
    }

    #endregion

    #region sse
    /// <summary>
    /// A finite stream: a ping (no id), then <c>widget.changed</c> (id 1) and <c>widget.removed</c>
    /// (id 2). <c>type</c> keeps only the named events (an unknown name is a 400 before streaming);
    /// <c>Last-Event-ID</c> skips ids up to it.
    /// </summary>
    private sealed class StreamWidgetEvents : IWidgetEventsStreamHandler
    {
        public static readonly Guid WidgetId = Guid.Parse("0b6f7f0e-4a52-4d0e-9d7a-5b1f0f6c2a11");

        public Task<WidgetEventsStreamResponse> HandleAsync(WidgetEventsStreamRequest request, CancellationToken cancellationToken)
        {
            if (request.Type?.FirstOrDefault(t => !WidgetEvent.EventTypes.Contains(t)) is { } unknown)
            {
                return Task.FromResult<WidgetEventsStreamResponse>(
                    new WidgetEventsStreamResponse.BadRequest(FixtureError.Create("UNKNOWN_EVENT", $"Unknown event type '{unknown}'.")));
            }

            var after = long.TryParse(request.LastEventId, out var id) ? id : 0;
            var events = Events()
                .Where(e => request.Type is null || request.Type.Contains(e.Data.EventType))
                .Where(e => e.Id is null || long.Parse(e.Id, System.Globalization.CultureInfo.InvariantCulture) > after);
            return Task.FromResult<WidgetEventsStreamResponse>(new WidgetEventsStreamResponse.Ok(events.ToAsyncEnumerable()));
        }

        private static IEnumerable<ServerSentEvent<WidgetEvent>> Events()
        {
            yield return new(new Ping { At = DateTimeOffset.UnixEpoch });
            yield return new(new WidgetChanged { At = DateTimeOffset.UnixEpoch, WidgetId = WidgetId, Name = "sample", Shape = new Square { Side = 2 } }, "1");
            yield return new(new WidgetRemoved { At = DateTimeOffset.UnixEpoch, WidgetId = WidgetId }, "2");
        }
    }

    #endregion

    #region parameters
    private sealed class EchoTags : ITagsEchoHandler
    {
        public Task<TagsEchoResponse> HandleAsync(TagsEchoRequest request, CancellationToken cancellationToken) =>
            Task.FromResult<TagsEchoResponse>(new TagsEchoResponse.Ok(request.Names));
    }

    #endregion

    #region files
    /// <summary>Streams <see cref="FileContent"/> from a non-seekable stream, so nothing sets the length but the handler.</summary>
    private sealed class GetFile : IFilesGetHandler
    {
        public Task<FilesGetResponse> HandleAsync(FilesGetRequest request, CancellationToken cancellationToken)
        {
            if (request.Path.EndsWith(Missing, StringComparison.Ordinal))
            {
                return Task.FromResult<FilesGetResponse>(new FilesGetResponse.NotFound(Error("FILE_NOT_FOUND", request.Path)));
            }

            var content = FileContent(request.Path);
            return Task.FromResult<FilesGetResponse>(new FilesGetResponse.Ok(
                new ForwardOnlyStream(content), FileContentType, FileRevision, content.Length,
                $"attachment; filename=\"{Path.GetFileName(request.Path)}\""));
        }
    }

    private sealed class HeadFile : IFilesHeadHandler
    {
        public Task<FilesHeadResponse> HandleAsync(FilesHeadRequest request, CancellationToken cancellationToken) =>
            Task.FromResult<FilesHeadResponse>(request.Path.EndsWith(Missing, StringComparison.Ordinal)
                ? new FilesHeadResponse.NotFound()
                : new FilesHeadResponse.Ok(FileContent(request.Path).Length));
    }

    private sealed class ForwardOnlyStream(byte[] content) : MemoryStream(content)
    {
        public override bool CanSeek => false;
    }
    #endregion

    #region text
    private sealed class GetNotice : INoticesGetHandler
    {
        public Task<NoticesGetResponse> HandleAsync(NoticesGetRequest request, CancellationToken cancellationToken) =>
            Task.FromResult<NoticesGetResponse>(request.Busy == true
                ? new NoticesGetResponse.ServiceUnavailable("busy, try later\n", 30)
                : new NoticesGetResponse.Ok("hello \"world\"\n"));
    }
    #endregion

    #region raw-body
    /// <summary>Reads the raw body up to <see cref="MaxDeliveryBytes"/> and answers its SHA-256.</summary>
    private sealed class ReceiveDelivery : IDeliveriesReceiveHandler
    {
        public async Task<DeliveriesReceiveResponse> HandleAsync(DeliveriesReceiveRequest request, CancellationToken cancellationToken)
        {
            using var buffer = new MemoryStream();
            var chunk = new byte[256];
            int read;
            while ((read = await request.Body.ReadAsync(chunk, cancellationToken)) > 0)
            {
                if (buffer.Length + read > MaxDeliveryBytes)
                {
                    return new DeliveriesReceiveResponse.PayloadTooLarge();
                }

                buffer.Write(chunk, 0, read);
            }

            return new Digest
            {
                Sha256 = Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(buffer.ToArray())),
                Length = buffer.Length,
                Signature = request.Signature,
            };
        }
    }
    #endregion

    #region shapes
    /// <summary>A union in the request: the handler switches on the variant records.</summary>
    private sealed class ShapeArea : IShapesAreaHandler
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
    private sealed class GetShape : IShapesGetHandler
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
    #endregion
}
