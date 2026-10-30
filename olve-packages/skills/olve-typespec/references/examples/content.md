# Files, text, headers and raw bodies

Generated from `src/typespec-csharp/test/fixtures/widgets.tsp` by the emitter's tests; do not edit.

The content type decides how a body is written. `bytes` with a non-JSON content type is a `System.IO.Stream` the runtime copies to the response as it reads it (never buffered) and then disposes; a `text/*` `string` is written as is. A literal content type is a constant; for `bytes`, `@header contentType: string` leaves it to the handler (a `ContentType` member). Each `@header` of a response is a member of its variant, optional ones last and defaulting to null (unset = not sent); a string literal header is a constant. `{+path}` maps to a catch-all route (`a%2Fb`, as generated clients send it, binds as `a/b`; a `.` or `..` segment is a binding failure), and `@head` operations answer headers only. A `bytes` request body with a non-JSON content type (`*/*` for any) is the raw request stream, unread: the handler reads it, with its own size limit, and a signature over the exact bytes still verifies.

## Contract

```typespec
/** A stored file, streamed as it is read. */
model FileContent {
  @header contentType: string;
  @header contentLength?: int64;
  @header contentDisposition?: string;
  @header("X-Revision") revision: int32;
  @body body: bytes;
}

/** A stored file's type and size, without its content. */
model FileHead {
  @header contentType: "application/octet-stream";
  @header contentLength: int64;
}

model NoFile {
  @statusCode status: 404;
}

@route("/files")
interface Files {
  // OpenAPI has no reserved expansion: clients send `a%2Fb`, which the generated binding accepts as `a/b`.
  #suppress "@typespec/openapi3/path-reserved-expansion" "Served as a catch-all"
  /** Streams a file; its path may span segments (`{+path}`). */
  @get @route("{+path}") get(@path path: string): FileContent | NotFound;

  #suppress "@typespec/openapi3/path-reserved-expansion" "Served as a catch-all"
  /** A file's headers, without its content. */
  @head @route("{+path}") head(@path path: string): FileHead | NoFile;
}
```

```typespec
model Notice {
  @header contentType: "text/plain";
  @body text: string;
}

model NoticeUnavailable {
  @statusCode status: 503;
  @header("Retry-After") retryAfter: int32;
  @header contentType: "text/plain";
  @body text: string;
}

@route("/notice")
interface Notices {
  /** Plain text, written as is. */
  @get get(@query busy?: boolean): Notice | NoticeUnavailable;
}
```

```typespec
/** What the server received. */
model Digest {
  sha256: string;
  length: int64;
  signature?: string;
}

model TooLarge {
  @statusCode status: 413;
}

@route("/deliveries")
interface Deliveries {
  /** Takes the body exactly as sent, whatever its content type (signatures cover the raw bytes). */
  @post receive(
    @header contentType: "*/*",
    @header("X-Signature") signature?: string,
    @body body: bytes,
  ): Digest | TooLarge;
}
```

## Generated

```csharp
/// <summary>The responses <c>Files_get</c> declares; its handler returns one.</summary>
public abstract record FilesGetResponse : IOperationResponse
{
    private FilesGetResponse()
    {
    }

    /// <inheritdoc />
    public abstract int Status { get; }

    /// <inheritdoc />
    public abstract IResult ToHttpResult();

    /// <summary>200.</summary>
    public sealed record Ok(System.IO.Stream Body, string ContentType, int Revision, long? ContentLength = null, string? ContentDisposition = null) : FilesGetResponse
    {
        /// <inheritdoc />
        public override int Status => 200;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new ResponseHeaders(new StreamResponse(Body, ContentType, 200), [new("content-length", ContentLength?.ToString(System.Globalization.CultureInfo.InvariantCulture)), new("content-disposition", ContentDisposition), new("X-Revision", Revision.ToString(System.Globalization.CultureInfo.InvariantCulture))]);
    }

    /// <summary>404.</summary>
    public sealed record NotFound(FixtureErrorEnvelope Body) : FilesGetResponse
    {
        /// <inheritdoc />
        public override int Status => 404;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new JsonResponse<FixtureErrorEnvelope>(Body, 404);
    }
}

/// <summary>The responses <c>Files_head</c> declares; its handler returns one.</summary>
public abstract record FilesHeadResponse : IOperationResponse
{
    private FilesHeadResponse()
    {
    }

    /// <inheritdoc />
    public abstract int Status { get; }

    /// <inheritdoc />
    public abstract IResult ToHttpResult();

    /// <summary>200.</summary>
    public sealed record Ok(long ContentLength) : FilesHeadResponse
    {
        /// <inheritdoc />
        public override int Status => 200;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new ResponseHeaders(TypedResults.StatusCode(200), [new("content-type", "application/octet-stream"), new("content-length", ContentLength.ToString(System.Globalization.CultureInfo.InvariantCulture))]);
    }

    /// <summary>404.</summary>
    public sealed record NotFound() : FilesHeadResponse
    {
        /// <inheritdoc />
        public override int Status => 404;

        /// <inheritdoc />
        public override IResult ToHttpResult() => TypedResults.StatusCode(404);
    }
}

/// <summary>The responses <c>Notices_get</c> declares; its handler returns one.</summary>
public abstract record NoticesGetResponse : IOperationResponse
{
    private NoticesGetResponse()
    {
    }

    /// <inheritdoc />
    public abstract int Status { get; }

    /// <inheritdoc />
    public abstract IResult ToHttpResult();

    /// <summary>200.</summary>
    public sealed record Ok(string Body) : NoticesGetResponse
    {
        /// <inheritdoc />
        public override int Status => 200;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new TextResponse(Body, "text/plain", 200);
    }

    /// <summary>503.</summary>
    public sealed record ServiceUnavailable(string Body, int RetryAfter) : NoticesGetResponse
    {
        /// <inheritdoc />
        public override int Status => 503;

        /// <inheritdoc />
        public override IResult ToHttpResult() => new ResponseHeaders(new TextResponse(Body, "text/plain", 503), [new("Retry-After", RetryAfter.ToString(System.Globalization.CultureInfo.InvariantCulture))]);
    }
}

/// <summary>Input of <c>Deliveries_receive</c> (POST /api/deliveries).</summary>
public sealed record DeliveriesReceiveRequest(string? Signature, System.IO.Stream Body);
```

```csharp
FilesGet = app.MapGet("/api/files/{**path}", async (
        [FromServices] IFilesGetHandler handler,
        [FromRoute(Name = "path")] string path,
        CancellationToken ct) =>
        (await handler.HandleAsync(new FilesGetRequest(PathParameters.CatchAll(path)), ct)).ToHttpResult())
    .WithName("Files_get")
    .WithMetadata(FixtureOperations.FilesGet)
    .Produces(200, null, "application/octet-stream")
    .Produces<FixtureErrorEnvelope>(404),

DeliveriesReceive = app.MapPost("/api/deliveries", async (
        [FromServices] IDeliveriesReceiveHandler handler,
        [FromHeader(Name = "X-Signature")] string? signature,
        System.IO.Stream body,
        CancellationToken ct) =>
        (await handler.HandleAsync(new DeliveriesReceiveRequest(signature, body), ct)).ToHttpResult())
    .WithName("Deliveries_receive")
    .WithMetadata(FixtureOperations.DeliveriesReceive)
    .Produces<Digest>(200)
    .Produces(413),
```

## Hand-written handlers

```csharp
/// <summary>Streams <see cref="FileContent"/> from a non-seekable stream, so nothing sets the length but the handler.</summary>
[OperationHandler]
internal sealed class GetFile : IFilesGetHandler
{
    public Task<FilesGetResponse> HandleAsync(FilesGetRequest request, CancellationToken cancellationToken)
    {
        if (request.Path.EndsWith(Missing, StringComparison.Ordinal))
        {
            return Task.FromResult<FilesGetResponse>(new FilesGetResponse.NotFound(Error("FILE_NOT_FOUND", request.Path)));
        }

        // SetHttpFileName writes an ASCII `filename` and an encoded `filename*`, so any name is a valid header.
        var disposition = new Microsoft.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        disposition.SetHttpFileName(Path.GetFileName(request.Path));
        var content = FileContent(request.Path);
        return Task.FromResult<FilesGetResponse>(new FilesGetResponse.Ok(
            new ForwardOnlyStream(content), FileContentType, FileRevision, content.Length, disposition.ToString()));
    }
}

[OperationHandler]
internal sealed class HeadFile : IFilesHeadHandler
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
```

```csharp
[OperationHandler]
internal sealed class GetNotice : INoticesGetHandler
{
    public Task<NoticesGetResponse> HandleAsync(NoticesGetRequest request, CancellationToken cancellationToken) =>
        Task.FromResult<NoticesGetResponse>(request.Busy == true
            ? new NoticesGetResponse.ServiceUnavailable("busy, try later\n", 30)
            : new NoticesGetResponse.Ok("hello \"world\"\n"));
}
```

```csharp
/// <summary>Reads the raw body up to <see cref="MaxDeliveryBytes"/> and answers its SHA-256.</summary>
[OperationHandler]
internal sealed class ReceiveDelivery : IDeliveriesReceiveHandler
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
```